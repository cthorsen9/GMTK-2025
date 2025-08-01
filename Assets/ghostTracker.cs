using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using System.IO;


public class ghostTracker : MonoBehaviour
{

    bool onlineGhost = true;

    // Wrapper class for JSON serialization of Vector3 list
    [System.Serializable]
    private class PositionData
    {
        public List<Vector3> positions = new List<Vector3>();
    }


    public bool useBestGhostData = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    List<Vector3> posList = new List<Vector3>();

    [SerializeField]
    float captureEveryXseconds = .2f;


    private void Start()
    {
        Debug.Log(PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name));
        
    }

    public void StartTracking()
    {
        StartCoroutine(positionCapture());
    }

    IEnumerator positionCapture()
    {
        Debug.Log("should be tracking");

        while (GameManager.singleton.timePlayer)
        {
            Debug.Log("Tracking data");
            posList.Add(transform.position);
            yield return new WaitForSeconds(captureEveryXseconds);
        }
       // Debug.Log(PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name));

        Debug.Log(posList.Count);


        if (useBestGhostData)
        {

            if (PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name) == 0) SaveToJSON();

            else if (GameManager.singleton.timer < PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name)) SaveToJSON();
        }

        else SaveToJSON();
      
    }

    //thanks jason
    void SaveToJSON()
    {

        PositionData data = new PositionData();
        data.positions = posList;


        PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name, GameManager.singleton.timer);

        string jsonData = JsonUtility.ToJson(data);

        Debug.Log(jsonData);

        PlayerPrefs.SetString(SceneManager.GetActiveScene().name + "json", jsonData);

        PlayerPrefs.Save();
        onlineGhost = PlayerPrefs.GetInt("onlineGhost") == 0 ? false : true;
        if (onlineGhost)
        {
            LeaderboardsSample.singleton.AddScoreWithMetadata(GameManager.singleton.leaderBoardId, SceneManager.GetActiveScene().name, GameManager.singleton.username, jsonData, GameManager.singleton.timer);
        }
    }
}
