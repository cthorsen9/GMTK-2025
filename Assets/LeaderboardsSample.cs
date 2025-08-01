using MoreMountains.Tools;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardsSample : MonoBehaviour
{


   public static LeaderboardsSample singleton;

    static BuildCompression builder;

    public string lastScoreRequest;

    public float lastScoreTime = -100f;

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
            };
            AuthenticationService.Instance.SignInFailed += s =>
            {
                // Take some action here...
                Debug.Log(s);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }


        [Serializable]
    public class ScoreMetadata
    {

        public string username;
    }

    public async void AddScoreWithMetadata(string leaderboardId, string user, string jsonData, float  time)
    {

        int utf16ByteCount = Encoding.Unicode.GetByteCount(jsonData);
        Debug.Log(utf16ByteCount);
        var scoreMetadata = new ScoreMetadata {  username = user };
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
        //Debug.Log(JsonConvert.SerializeObject(scoresResponse));
        lastScoreRequest = JsonConvert.SerializeObject(scoresResponse);
        lastScoreTime = Time.time;
    }

}


