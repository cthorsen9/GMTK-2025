using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class GhostMovement : MonoBehaviour
{

    string jsonData;

    List<Vector3> ghostPosData = new List<Vector3>();   

    float updateFreq = .2f;

    float bestTime;

    int indexer = 0;

    PositionData data;


    // Wrapper class for JSON serialization of Vector3 list
    [System.Serializable]
    private class PositionData
    {
        public List<Vector3> positions = new List<Vector3>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckForData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckForData()
    {
        jsonData = PlayerPrefs.GetString(SceneManager.GetActiveScene().name + "json");

        if (jsonData == "{}") Debug.Log("no ghost found");
        else
        {
            data = JsonUtility.FromJson<PositionData>(jsonData);

            ghostPosData = data.positions;  
            Debug.Log(ghostPosData.Count);

        }

    }

    public void StartGhost()
    {
        StartCoroutine(GhostMover());
        Debug.Log("ghost Should move");
    }

    IEnumerator GhostMover()
    {
        bestTime = GameManager.singleton.levelBest;
        Debug.Log(bestTime);

        int indexer = 0;

        if (ghostPosData.Count == 0 || bestTime <= 0) yield break;

        Debug.Log("ghost passed the count and best time check");


        while (indexer < ghostPosData.Count - 1 && GameManager.singleton.timer < bestTime)
        {
            // Calculate distance to next point
            float distance = Vector3.Distance(ghostPosData[indexer], ghostPosData[indexer + 1]);

            // Speed = Distance / Time (to cover the distance in 0.2 seconds)
            float speed = distance / 0.05f;

            // Move towards the next point at the calculated speed
            transform.position = Vector3.MoveTowards(
                transform.position,
                ghostPosData[indexer + 1],
                speed * Time.deltaTime
            );

            // Advance to the next point when reached
            if (Vector3.Distance(transform.position, ghostPosData[indexer + 1]) < 0.001f)
            {
                indexer++;
            }

            yield return null;
        }
    }

}
