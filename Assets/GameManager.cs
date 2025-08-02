using System.Collections;
using TMPro;
using Unity.Cinemachine;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Text;
using System.Threading.Tasks;
using NaughtyAttributes;


public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    public GameObject player;

    [HideInInspector]
    public string leaderBoardId;

    //public  string username = "testUser";

    public ghostTracker ghostTracker_;

    public GhostMovement ghost;

    Vector3 startPos;

    Quaternion startRot;

    public WheelController controller;

    public Rigidbody pRigid;

    public CinemachineInputAxisController cineCont;

    public GameObject winUi;

    public GameObject loseUI;

    public TMP_Text lostByText;

    public GameObject pauseUI;

    Transform checkpoint;

    bool paused = false;

    [HideInInspector]
    public cameraIntro intro;

    //timer stuf--------------------
    public bool timePlayer = false;

    public float timer;

    public float countdownTime = 3;

    public TMP_Text countdown;

    public TMP_Text timerText;

    public TMP_Text recordText;

    public bool playerFinsihed = false;

    string stTime;

    public float levelBest;

    bool awaitingLogin = true;

    bool awaitingWR = true;



    //------------------
    public TMP_Text speedText;
    float prevFrameVelMag = 0f;
    public Animator velAnims;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //DontDestroyOnLoad(gameObject);

        if (singleton != null && singleton != this.gameObject)
        {
            Destroy(singleton.gameObject);
        }
        else if (singleton == null) singleton = this;

        leaderBoardId = SceneManager.GetActiveScene().name;

        startPos = player.transform.position;

        startRot = player.transform.rotation;

        intro = GetComponent<cameraIntro>();

        levelBest = PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name);

        cineCont.enabled = false;

        if(MusicMaster.singleton != null) MusicMaster.singleton.rb = pRigid;

        SetupLevel();

    }

    // Update is called once per frame
    void Update()
    {
        
        if(timePlayer) UpdateTimer();

        if (awaitingWR)
        {
            if(LeaderboardsSample.singleton.wereIn)
            {
                awaitingWR = false;
                LeaderboardsSample.singleton.GetScores(leaderBoardId);
                awaitingLogin = true;
            }
        }

        if (awaitingLogin)
        {
            if (Time.time - LeaderboardsSample.singleton.lastScoreTime < .5f)
            {
                awaitingLogin = false;
                Debug.Log(LeaderboardsSample.singleton.lastScoreRequest);
                if (levelBest > 0) recordText.text = "world record: " + LeaderboardsSample.singleton.TopPlayerUserAndSpeed() + "\npersonal best: " + ((int)levelBest / 60).ToString("00") + ":" + (levelBest % 60).ToString("00.00");
                else recordText.text = "world record: " + LeaderboardsSample.singleton.TopPlayerUserAndSpeed();

            }
        }

        

    }

    private void FixedUpdate()
    {
        if (timePlayer) speedText.text = pRigid.linearVelocity.magnitude.ToString("00.0");

        //if ((int)pRigid.linearVelocity.magnitude - (int)prevFrameVelMag >= 1) velAnims.SetTrigger("velUp");

        prevFrameVelMag = pRigid.linearVelocity.magnitude;

        
    }


    [Button]
    public void ResetPB()
    {
        PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name, 0);
        PlayerPrefs.Save();
    }
    

    //gets player start and positions them
    void SetupLevel()
    {
        if (SceneInfoHolder.singleton.playedIntro)//just start the level
        {
            intro.FixCamera();
            LevelCountdown();

        }
        else //play our flythrough
        {
            LevelIntro();


        }

    }


    //plays cinemachine fly through of level
    void LevelIntro()
    {
        SceneInfoHolder.singleton.playedIntro = true;

        intro.Startintro();
    }


    //counts down player start and frees them, unlocking rigid and giving control
    public void LevelCountdown()
    {
        playerFinsihed = false;
        StartCoroutine(CountDown());

    }

    IEnumerator CountDown()
    {
        cineCont.enabled = true;

        for (int i = 0; i < countdownTime; i++)
        {
            countdown.text = (countdownTime - i ).ToString();
            yield return new WaitForSeconds(1);
        }
        pRigid.isKinematic = false; 
        controller.enabled = true;

        StartTimer();

        countdown.text = "go.";
        yield return new WaitForSeconds(1);
        countdown.text = "";
     }


    void StartTimer()
    {
        timer = 0f;
        timePlayer = true; 
        
        ghostTracker_.StartTracking();
        ghost.StartGhost();
    }

    void UpdateTimer()
    {
        timer += Time.deltaTime;

        stTime = ((int)timer / 60).ToString("00") + ":" + (timer % 60).ToString("00.00");
        timerText.text = stTime;
    }

    public void CheckpointEntered(Transform cp)
    {
        checkpoint = cp;
    }

    //called externally to reset to cp
    public void ReturnToCheckPoint()
    {
        if (!timePlayer) return;

        if (checkpoint == null)
        {
            pRigid.Move(startPos,startRot);
            pRigid.linearVelocity = Vector3.zero;
            pRigid.angularVelocity = Vector3.zero;
            
        }
        else
        {
            pRigid.Move(checkpoint.position, checkpoint.rotation);
            pRigid.linearVelocity = Vector3.zero;
            pRigid.angularVelocity = Vector3.zero;
        }
        WheelController.singleton.wCol.rotationSpeed = 0;
        WheelController.singleton.wCol.steerAngle = 0;

    }

   //resets to start position and restarts timer
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void LevelComplete()
    {
        timePlayer = false;
        playerFinsihed = true;

        cineCont.enabled = false;

        Debug.Log(levelBest + " <level best, time to beat> " + SceneInfoHolder.singleton.TimeToBeat);

        if (levelBest == 0 || timer < levelBest) levelBest = timer;

        if (levelBest < SceneInfoHolder.singleton.TimeToBeat) winUi.SetActive(true);
        else
        {
            lostByText.text = "required time: " + SceneInfoHolder.singleton.TimeToBeat + " seconds" + "\ndiff: " + (SceneInfoHolder.singleton.TimeToBeat - timer).ToString("0.00");

            loseUI.SetActive(true);

        }
    }


    public void NextLevel()
    {
        Destroy(SceneInfoHolder.singleton.gameObject);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);

    }

    public void Retry()
    {
        RestartLevel();
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }


    public void PauseUnpause()
    {
        if (paused)
        {
            paused = false;
            Time.timeScale = 1;
            pauseUI.SetActive(false);
        }
        else
        {
            paused = true;
            Time.timeScale = 0;
            pauseUI.SetActive(true);

        }
    }

    public void UseOnlineGhost()
    {
        var flipGhost = PlayerPrefs.GetInt("onlineGhost") == 0 ? 1 : 0;
        PlayerPrefs.SetInt("onlineGhost", flipGhost);
    }
}
