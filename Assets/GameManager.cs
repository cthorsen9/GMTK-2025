using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    public GameObject player;

    Vector3 startPos;

    Quaternion startRot;

    public WheelController controller;

    public Rigidbody pRigid;

    public CinemachineInputAxisController cineCont;

    public GameObject winUi;

    public GameObject pauseUI;

    Transform checkpoint;


    //timer stuf--------------------
    bool timePlayer = false;

    float timer;

    public float countdownTime = 3;

    public TMP_Text countdown;

    public TMP_Text timerText;

    public bool playerFinsihed = false;

    string stTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //DontDestroyOnLoad(gameObject);

        if (singleton != null && singleton != this.gameObject)
        {
            Destroy(singleton.gameObject);
        }
        else if (singleton == null) singleton = this;

        startPos = player.transform.position;

        startRot = player.transform.rotation;       
        
        LevelCountdown();

    }

    // Update is called once per frame
    void Update()
    {
        
        if(timePlayer) UpdateTimer();
        

    }

    //gets player start and positions them
    void SetupLevel()
    {
        if (SceneInfoHolder.singleton.playedIntro)//just start the level
        {
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


    }

    //counts down player start and frees them, unlocking rigid and giving control
    void LevelCountdown()
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

        

        winUi.SetActive(true);
    }


    public void NextLevel()
    {
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
}
