using MoreMountains.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

public class LeaderboardsSample : MonoBehaviour
{


   public static LeaderboardsSample singleton;

   // static BuildCompression builder;

    public string lastScoreRequest;

    public float lastScoreTime = -100f;

    //public TMP_Text debugText;


    //once signed in
    public bool wereIn = false;


    private async void Awake()
    {
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

        await UnityServices.InitializeAsync();
        await SignInAnonymously();

       

    }

        async Task SignInAnonymously()
        {
            AuthenticationService.Instance.SignedIn += () =>
            {
                wereIn = true;
                Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
                //debugText.text = AuthenticationService.Instance.PlayerId;
            };
            AuthenticationService.Instance.SignInFailed += s =>
            {
                // Take some action here...
                //debugText.text = "couldnt log in?";
                Debug.Log(s);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerId;
        public string playerName;
        public float score;  // This is the "time" value
        public string updatedTime;
        public string metadata; // Contains nested JSON
    }

    [System.Serializable]
    public class LeaderboardResult
    {
        public int limit;
        public int total;
        public List<LeaderboardEntry> results; // List of all players
    }

   [System.Serializable]
    public class ScoreMetadata
    {
        public string username;
    }

    


    public async void AddScoreWithMetadata(string leaderboardId, string user, string jsonData, float  time)
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(user).ContinueWith(task =>
        {
            if (task.IsFaulted)
                Debug.LogError("Failed to update player name: " + task.Exception);
        });

        int utf16ByteCount = Encoding.Unicode.GetByteCount(jsonData);
        Debug.Log(utf16ByteCount);
        var scoreMetadata = new ScoreMetadata {  username = user };
        //string metadataJson = JsonConvert.SerializeObject(scoreMetadata);

        // 3. Debug check
        //Debug.Log($"Metadata JSON: {metadataJson} | Size: {Encoding.UTF8.GetByteCount(metadataJson)} bytes");

        // 4. Submit with explicit metadata
        var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(
            leaderboardId,
            time,
            new AddPlayerScoreOptions
            {
                Metadata = scoreMetadata // Pass the pre-serialized JSON string
            }
        );
        //debugText.text = JsonConvert.SerializeObject(playerEntry);

        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }


    public async void GetScores(string leaderboardId)
    {
        var scoresResponse = await LeaderboardsService.Instance
            .GetScoresAsync(leaderboardId);
        //Debug.Log(JsonConvert.SerializeObject(scoresResponse));
        lastScoreRequest = JsonConvert.SerializeObject(scoresResponse);
        lastScoreTime = Time.time;
    }

    public string TopPlayerUserAndSpeed()
    {
        LeaderboardEntry best = GetBestPlayer(lastScoreRequest);

        if (best == null) return "no record!";
        Debug.Log(best.playerName + " " + best.score);

        //ScoreMetadata metadata = JsonConvert.DeserializeObject<ScoreMetadata>(best.metadata);


        return best.playerName + ",  " + ((int)best.score / 60).ToString("00") + ":" + (best.score % 60).ToString("00.00");

    }


    public LeaderboardEntry GetBestPlayer(string jsonData)
    {
        try
        {
            // Deserialize the full leaderboard data
            LeaderboardResult leaderboard = JsonConvert.DeserializeObject<LeaderboardResult>(jsonData);

            // Return the top-ranked player (rank 0)
            if (leaderboard.results.Count > 0)
            {
                return leaderboard.results[0]; // First item is always best (rank 0)
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to parse leaderboard: {e.Message}");
        }

        return null; // Fallback if no players exist
    }


    /*void ParseLeaderboardEntry(string jsonString)
    {
        try
        {
            // Step 1: Deserialize the main JSON
            LeaderboardEntry entry = JsonConvert.DeserializeObject<LeaderboardEntry>(jsonString);

            // Step 2: Deserialize the nested metadata
            ScoreMetadata metadata = JsonConvert.DeserializeObject<ScoreMetadata>(entry.metadata);

            // Extract values
            float playerTime = entry.score; // 16.74677...
            string username = metadata.username; // "poopy"

            Debug.Log($"Username: {username} | Time: {playerTime}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to parse JSON: {e.Message}");
        }
    }*/

}


