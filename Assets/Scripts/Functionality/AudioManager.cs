using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource bg_adudio;
    [SerializeField] internal AudioSource audioPlayer_wl;
    [SerializeField] internal AudioSource audioPlayer_button;
    [SerializeField] internal AudioSource audioBet_button;
    [SerializeField] internal AudioSource audioWin;

    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioClip[] Voiceclips;

    // PlayerPrefs keys
    private const string PREF_MUSIC_MUTED = "AudioManager_MusicMuted";
    private const string PREF_SOUND_MUTED = "AudioManager_SoundMuted";

    // Track user's preference (not temporary focus state)
    private bool userMusicMuted = false;
    private bool userSoundMuted = false;

    // Track if we're currently in background
    private bool isInBackground = false;

    private void Awake()
    {
        // Load saved preferences BEFORE Start()
        LoadAudioPreferences();
    }

    private void Start()
    {
        // Set initial clips
        audioPlayer_button.clip = clips[7];
        audioBet_button.clip = clips[3];
        audioWin.clip = clips[4];

        // Apply loaded preferences to audio sources
        ApplyAudioPreferences();

        // Start background music if not muted
        if (bg_adudio && !userMusicMuted)
        {
            bg_adudio.Play();
        }
    }

    #region Audio Playback Methods

    internal void PlayWLAudio(string type)
    {
        audioPlayer_wl.loop = false;
        int index = 0;
        switch (type)
        {
            case "betDone":
                index = 0;
                audioPlayer_wl.loop = true;
                break;
            case "betSelect":
                index = 1;
                break;
            case "numberchange":
                index = 2;
                break;
            case "coinSelect":
                index = 3;
                break;
            case "double":
                index = 4;
                break;
            case "cards":
                index = 5;
                break;
            case "midCard":
                index = 6;
                break;
        }
        StopWLAaudio();
        audioPlayer_wl.clip = clips[index];
        audioPlayer_wl.Play();
    }

    internal void PlayButtonAudio()
    {
        audioPlayer_button.Play();
    }

    internal void PlayBetButtonAudio()
    {
        audioBet_button.Play();
    }

    internal void PlayGirlAudio(string type)
    {
        audioWin.loop = false;
        int index = 0;
        switch (type)
        {
            case "timeisrunning":
                index = 0;
                audioPlayer_wl.loop = true;
                break;
            case "betSelect":
                index = 1;
                break;
            case "placeyourbet":
                index = 2;
                break;
            case "nomorebets":
                index = 3;
                break;
            case "newround":
                index = 4;
                break;
            case "cards":
                index = 5;
                break;
            case "midCard":
                index = 6;
                break;
        }
        StopWLAaudio();
        audioPlayer_wl.clip = Voiceclips[index];
        audioPlayer_wl.Play();
    }

    internal void StopWLAaudio()
    {
        audioPlayer_wl.Stop();
        audioPlayer_wl.loop = false;
    }

    internal void StopBgAudio()
    {
        bg_adudio.Stop();
    }

    #endregion

    #region Mute/Unmute with Persistence

    /// <summary>
    /// Toggle mute state for specific audio type or all audio.
    /// This saves the preference to PlayerPrefs.
    /// </summary>
    internal void ToggleMute(bool mute, string type = "all")
    {
        switch (type)
        {
            case "bg":
                userMusicMuted = mute;
                bg_adudio.mute = mute;
                SaveMusicPreference();

                // ✅ FIX: If unmuting and music is paused/stopped, resume it
                if (!mute && bg_adudio && !bg_adudio.isPlaying)
                {
                    bg_adudio.Play();
                }
                break;

            case "button":
                audioPlayer_button.mute = mute;
                UpdateSoundMutedState();
                break;

            case "wl":
                audioPlayer_wl.mute = mute;
                UpdateSoundMutedState();
                break;

            case "win":
                audioWin.mute = mute;
                UpdateSoundMutedState();
                break;

            case "bet":
                audioBet_button.mute = mute;
                UpdateSoundMutedState();
                break;

            case "all":
                // This is for sound effects (not background music)
                userSoundMuted = mute;
                audioPlayer_wl.mute = mute;
                audioPlayer_button.mute = mute;
                audioBet_button.mute = mute;
                audioWin.mute = mute;
                SaveSoundPreference();
                break;
        }
    }

    /// <summary>
    /// Update the overall sound muted state based on individual sources.
    /// If all sound effects are muted, consider sound as muted.
    /// </summary>
    private void UpdateSoundMutedState()
    {
        // Check if all sound sources are muted
        bool allMuted = audioPlayer_button.mute &&
                       audioPlayer_wl.mute &&
                       audioWin.mute &&
                       audioBet_button.mute;

        userSoundMuted = allMuted;
        SaveSoundPreference();
    }

    /// <summary>
    /// Get current music mute state
    /// </summary>
    internal bool IsMusicMuted()
    {
        return userMusicMuted;
    }

    /// <summary>
    /// Get current sound mute state
    /// </summary>
    internal bool IsSoundMuted()
    {
        return userSoundMuted;
    }

    #endregion

    #region PlayerPrefs Save/Load

    private void LoadAudioPreferences()
    {
        // Load with default values (0 = not muted, 1 = muted)
        userMusicMuted = PlayerPrefs.GetInt(PREF_MUSIC_MUTED, 0) == 1;
        userSoundMuted = PlayerPrefs.GetInt(PREF_SOUND_MUTED, 0) == 1;

    }

    private void ApplyAudioPreferences()
    {
        // Apply music preference
        if (bg_adudio)
        {
            bg_adudio.mute = userMusicMuted;
        }

        // Apply sound preferences
        audioPlayer_button.mute = userSoundMuted;
        audioBet_button.mute = userSoundMuted;
        audioPlayer_wl.mute = userSoundMuted;
        audioWin.mute = userSoundMuted;
    }

    private void SaveMusicPreference()
    {
        PlayerPrefs.SetInt(PREF_MUSIC_MUTED, userMusicMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SaveSoundPreference()
    {
        PlayerPrefs.SetInt(PREF_SOUND_MUTED, userSoundMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    #endregion

    #region Application Focus Handling

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            // ✅ FIXED: Restore to user's preference, not force unmute
            OnApplicationGainedFocus();
        }
        else
        {
            // ✅ FIXED: Just pause, don't change mute state
            OnApplicationLostFocus();
        }
    }

    private void OnApplicationGainedFocus()
    {
        isInBackground = false;

        // Resume background music only if user hasn't muted it
        if (bg_adudio && !userMusicMuted)
        {
            if (!bg_adudio.isPlaying)
            {
                bg_adudio.Play();
            }
        }
        // ✅ FIX: If music is muted, ensure it stays paused
        else if (bg_adudio && userMusicMuted)
        {
            // Ensure it's paused/stopped when muted
            if (bg_adudio.isPlaying)
            {
                bg_adudio.Pause();
            }
        }

        // Restore sound effects to user's preference (not force unmute)
        audioPlayer_button.mute = userSoundMuted;
        audioBet_button.mute = userSoundMuted;
        audioPlayer_wl.mute = userSoundMuted;
        audioWin.mute = userSoundMuted;

    }

    private void OnApplicationLostFocus()
    {
        isInBackground = true;

        // Pause background music (don't stop, so it can resume)
        if (bg_adudio && bg_adudio.isPlaying)
        {
            bg_adudio.Pause();
        }

        // Mute all sound effects while in background
        // (This is temporary, will restore to user preference when focus returns)
        audioPlayer_button.mute = true;
        audioBet_button.mute = true;
        audioPlayer_wl.mute = true;
        audioWin.mute = true;

    }

    #endregion

    #region Optional: Clear Preferences (for testing)

    /// <summary>
    /// Call this from UIManager or debug menu to reset audio preferences
    /// </summary>
    internal void ResetAudioPreferences()
    {
        PlayerPrefs.DeleteKey(PREF_MUSIC_MUTED);
        PlayerPrefs.DeleteKey(PREF_SOUND_MUTED);
        PlayerPrefs.Save();

        userMusicMuted = false;
        userSoundMuted = false;
        ApplyAudioPreferences();

    }

    #endregion
}