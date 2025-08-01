using UnityEngine;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class networktest : MonoBehaviour
{
    // Configuration
    private const string LEADERBOARD_KEY = "GlobalLeaderboard";
    private const int MAX_ENTRIES = 20;
    private const float UPDATE_INTERVAL = 300f; // 5 minutes in seconds

    // Runtime data
    private List<ScoreEntry> cachedLeaderboard = new List<ScoreEntry>();
    private float lastUpdateTime = -float.MaxValue;
    private bool isInitialized = false;

    void Start()
    {
        Initialize();
    }

    private async void Initialize()
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        await ForceRefreshLeaderboard();
        isInitialized = true;
    }

    public async Task<List<ScoreEntry>> GetLeaderboard()
    {
        if (!isInitialized) return new List<ScoreEntry>();

        // Refresh if enough time has passed
        if (Time.unscaledTime - lastUpdateTime > UPDATE_INTERVAL)
        {
            await RefreshLeaderboard();
        }

        return cachedLeaderboard;
    }

    public async Task<bool> TrySubmitScore(string playerName, float newScore, string jsonData)
    {
        if (!isInitialized) return false;

        try
        {
            // Get current state (may use cached version)
            var leaderboard = await GetLeaderboard();

            // Check if score qualifies
            bool qualifies = leaderboard.Count < MAX_ENTRIES ||
                           newScore > leaderboard[leaderboard.Count - 1].score;

            if (!qualifies) return false;

            // Add and sort
            leaderboard.Add(new ScoreEntry
            {
                playerName = playerName,
                score = newScore,
                jsonData = jsonData,
                timestamp = DateTime.UtcNow.Ticks
            });

            leaderboard.Sort((a, b) => b.score.CompareTo(a.score));
            if (leaderboard.Count > MAX_ENTRIES)
            {
                leaderboard.RemoveRange(MAX_ENTRIES, leaderboard.Count - MAX_ENTRIES);
            }

            // Save to cloud
            var saveData = new Dictionary<string, object> {
                { LEADERBOARD_KEY, JsonUtility.ToJson(new Wrapper { entries = leaderboard }) }
            };

            await CloudSaveService.Instance.Data.ForceSaveAsync(saveData);

            // Update cache
            cachedLeaderboard = leaderboard;
            lastUpdateTime = Time.unscaledTime;

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Score submission failed: {e.Message}");
            return false;
        }
    }

    private async Task RefreshLeaderboard()
    {
        // Only refresh if enough time has passed
        if (Time.unscaledTime - lastUpdateTime < UPDATE_INTERVAL)
            return;

        await ForceRefreshLeaderboard();
    }

    private async Task ForceRefreshLeaderboard()
    {
        try
        {
            var data = await CloudSaveService.Instance.Data.LoadAsync(
                new HashSet<string> { LEADERBOARD_KEY });

            if (data.ContainsKey(LEADERBOARD_KEY))
            {
                cachedLeaderboard = JsonUtility.FromJson<Wrapper>(
                    data[LEADERBOARD_KEY].ToString()).entries;
            }
            else
            {
                cachedLeaderboard = new List<ScoreEntry>();
            }

            lastUpdateTime = Time.unscaledTime;
        }
        catch (Exception e)
        {
            Debug.LogError($"Leaderboard refresh failed: {e.Message}");
            // Keep using old cached data if available
        }
    }

    [System.Serializable]
    private class Wrapper { public List<ScoreEntry> entries; }

    [System.Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public float score;
        public string jsonData;
        public long timestamp;
    }
}
