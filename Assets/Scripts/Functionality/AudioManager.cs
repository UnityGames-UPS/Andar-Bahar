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
    [Header("New Sound Effects")]
    [SerializeField] private AudioClip levelButtonClickClip;   
    [SerializeField] private AudioClip whooshClip;            
    [SerializeField] private AudioClip andarWinClip;      
    [SerializeField] private AudioClip baharWinClip;
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

    private float lastBetSoundTime = 0f;
    private const float BET_SOUND_COOLDOWN = 0.15f;

    internal void PlayWLAudio(string type)
    {
        int index = 0;
        bool useBetSource = true; // Use separate source for SFX to avoid interrupting timer/voice

        switch (type)
        {
            case "betDone":
                index = 0;
                audioBet_button.loop = true;
                break;
            case "betSelect":
                index = 1;
                audioBet_button.loop = false;
                break;
            case "numberchange":
                index = 2;
                audioBet_button.loop = false;
                break;
            case "coinSelect":
                index = 3;
                audioBet_button.loop = false;
                break;
            case "double": // Bet placement sound
                index = 4;
                audioBet_button.loop = false;
                // Add cooldown to prevent spam from multiple opponent bets
                if (Time.time - lastBetSoundTime < BET_SOUND_COOLDOWN) return;
                lastBetSoundTime = Time.time;
                break;
            case "cards":
                index = 5;
                audioBet_button.loop = false;
                break;
            case "midCard":
                index = 6;
                audioBet_button.loop = false;
                break;
        }

        if (useBetSource)
        {
            if (audioBet_button.loop)
            {
                audioBet_button.clip = clips[index];
                audioBet_button.Play();
            }
            else
            {
                audioBet_button.PlayOneShot(clips[index]);
            }
        }
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


    #region New Sound Effects

    /// <summary>
    /// Play level button click sound when user selects Casual/Novice/Expert/HighRoller
    /// </summary>
    internal void PlayLevelButtonClick()
    {
        if (levelButtonClickClip != null && audioPlayer_button != null)
        {
            // Use PlayOneShot to allow multiple clicks without cutting off previous sound
            audioPlayer_button.PlayOneShot(levelButtonClickClip);
        }
        else
        {
            Debug.LogWarning("Level button click clip or audio player is missing!");
        }
    }

    /// <summary>
    /// Play whoosh sound when loading screen appears after level selection
    /// </summary>
    internal void PlayWhooshSound()
    {
        if (whooshClip != null && audioPlayer_wl != null)
        {
            // Stop any current sound and play whoosh
            audioPlayer_wl.Stop();
            audioPlayer_wl.PlayOneShot(whooshClip);
        }
        else
        {
            Debug.LogWarning("Whoosh clip or audio player is missing!");
        }
    }

    /// <summary>
    /// Play win sound based on who won (andar or bahar)
    /// </summary>
    /// <param name="winner">String: "andar" or "bahar"</param>
    internal void PlayWinSound(string winner)
    {
        if (audioWin == null)
        {
            Debug.LogWarning("AudioWin source is missing!");
            return;
        }

        // Stop any previous win sound
        audioWin.Stop();

        // Play appropriate win sound based on winner
        string winnerLower = winner.ToLower();

        if (winnerLower == "andar" && andarWinClip != null)
        {
            audioWin.clip = andarWinClip;
            audioWin.Play();
            Debug.Log("Playing Andar win sound");
        }
        else if (winnerLower == "bahar" && baharWinClip != null)
        {
            audioWin.clip = baharWinClip;
            audioWin.Play();
            Debug.Log("Playing Bahar win sound");
        }
        else
        {
            Debug.LogWarning($"Win sound clip missing for winner: {winner}");
        }
    }

    #endregion
}