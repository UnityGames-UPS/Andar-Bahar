using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages loading screen display with proper timing:
/// 1. Shows loading screen for 0.5 sec minimum
/// 2. Emits socket event
/// 3. Keeps loading until acknowledgment
/// </summary>
public class LoadingScreenManager : MonoBehaviour
{
    [Header("Loading Screen References")]
    [SerializeField] private GameObject loadingPage;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private ImageAnimation loadingAnimation;

    
    private Coroutine currentLoadingCoroutine;
    private bool isWaitingForAck = false;
    private float minimumLoadingTime = 0.5f;
    private bool hasLoadedHistoryPageOnce = false;

    public enum LoadingType
    {
        JoiningTable,
        LeavingTable,
        InitWaiting,
        LoadingHistory,
        Connecting,
    }

    /// <summary>
    /// Show loading screen with minimum 0.5 sec display time
    /// </summary>
    public void ShowLoading(LoadingType type, System.Action emitAction)
    {
        if (type == LoadingType.LoadingHistory && hasLoadedHistoryPageOnce)
        {
            emitAction?.Invoke();
            return;
        }
        if (currentLoadingCoroutine != null)
        {
            StopCoroutine(currentLoadingCoroutine);
        }

        currentLoadingCoroutine = StartCoroutine(LoadingSequence(type, emitAction));
    }

    /// <summary>
    /// Hide loading screen when acknowledgment is received
    /// </summary>
    public void HideLoading()
    {
        isWaitingForAck = false;
        
        // If we're still in the minimum display time, the coroutine will handle hiding
        // Otherwise, hide immediately
        if (currentLoadingCoroutine == null)
        {
            HideLoadingScreen();
        }
    }

    /// <summary>
    /// Force hide loading screen immediately (for errors/special cases)
    /// </summary>
    public void ForceHideLoading()
    {
        if (currentLoadingCoroutine != null)
        {
            StopCoroutine(currentLoadingCoroutine);
            currentLoadingCoroutine = null;
        }

        isWaitingForAck = false;
        HideLoadingScreen();
    }

    /// <summary>
    /// Reset history loading flag when history panel closes
    /// </summary>
    public void ResetHistoryLoadingFlag()
    {
        hasLoadedHistoryPageOnce = false;
    }
    private IEnumerator LoadingSequence(LoadingType type, System.Action emitAction)
    {
        // Step 1: Show loading screen with appropriate text
        ShowLoadingScreen(GetLoadingText(type));

        // Step 2: Wait minimum display time (0.5 sec)
        float startTime = Time.time;
        yield return new WaitForSeconds(minimumLoadingTime);

        // Step 3: Emit the socket event
        isWaitingForAck = true;
        emitAction?.Invoke();

        // Mark history as loaded after first time
        if (type == LoadingType.LoadingHistory)
        {
            hasLoadedHistoryPageOnce = true;
        }

        // Step 4: Keep loading screen visible until acknowledgment
        while (isWaitingForAck)
        {
            yield return null;
        }

        // Step 5: Hide loading screen
        HideLoadingScreen();
        currentLoadingCoroutine = null;
    }

    private void ShowLoadingScreen(string text)
    {
        if (loadingPage)
        {
            loadingPage.SetActive(true);
        }

        if (loadingText)
        {
            loadingText.text = text;
        }

        if (loadingAnimation)
        {
            loadingAnimation.StartAnimation();
        }

   
    }

    private void HideLoadingScreen()
    {
        if (loadingAnimation)
        {
            loadingAnimation.StopAnimation();
        }

        if (loadingPage)
        {
            loadingPage.SetActive(false);
        }
    }

    private string GetLoadingText(LoadingType type)
    {
        switch (type)
        {
            case LoadingType.JoiningTable:
                return "Joining a table";
            case LoadingType.LeavingTable:
                return "Leaving table";
            case LoadingType.InitWaiting:
                return "";
            case LoadingType.LoadingHistory:
                return "Loading history";
            case LoadingType.Connecting:
                return "Connecting";
            default:
                return "Loading...";
        }
    }

    /// <summary>
    /// Check if currently showing loading screen
    /// </summary>
    public bool IsLoading()
    {
        return loadingPage != null && loadingPage.activeSelf;
    }

    /// <summary>
    /// Update loading text while loading (useful for ping/connection attempts)
    /// </summary>
    public void UpdateLoadingText(string newText)
    {
        if (loadingText && IsLoading())
        {
            loadingText.text = newText;
        }
    }
}