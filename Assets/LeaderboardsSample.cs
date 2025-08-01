using Newtonsoft.Json;
using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardsSample : MonoBehaviour
{


   public static LeaderboardsSample singleton;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        if (singleton == null)
        {
            // First instance - become the singleton
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (singleton != this)
        {
            // Subsequent instance - destroy yourself
            Destroy(gameObject);
            return; // Important to prevent further execution
        }

    }

    [Serializable]
    public class ScoreMetadata
    {

        public string jsonGhostData;
        public string level;
        public string username;
    }

    public async void AddScoreWithMetadata(string leaderboardId, string level, string user, string jsonData, float  time)
    {
        var scoreMetadata = new ScoreMetadata { jsonGhostData = jsonData, level = SceneManager.GetActiveScene().name, username = user };
        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(
                leaderboardId,
                time,
                new AddPlayerScoreOptions { Metadata = scoreMetadata }
            );
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }


    public async void GetScores(string leaderboardId)
    {
        var scoresResponse = await LeaderboardsService.Instance
            .GetScoresAsync(leaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

}


