using System.Collections;
using System.Collections.Generic;
using System.Globalization;

//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MusicMaster : MonoBehaviour
{

    public Rigidbody rb;

    public static MusicMaster singleton;

    public List<TrackParent> mc = new List<TrackParent>();

    int activeTrack = 0;

    [SerializeField]
    float topSpeedDrums = 90f;

    [SerializeField]
    float minSpeedDrums = 50f;

    [SerializeField]
    float topSpeedBass = 50f;


    float topVolume;



    float drumCalc;

    float bassCalc;

    bool fading = false;

    string scene;
    string prevScene;

    int sceneNum = 0;

    [SerializeField]
    float crossFadeSpeed = 1.5f;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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



        activeTrack = 0;


        //mc[activeTrack].gameObject.SetActive(true);
        topVolume = mc[activeTrack].ambience.volume;

        SceneManager.sceneLoaded += CrossFade;

    }

    

    void CrossFade(Scene scene, LoadSceneMode mode)
    {
        if (fading) return;
        int prevActiveTrack = activeTrack;
        if (scene.name[0] == 'l') sceneNum = int.Parse(scene.name.Remove(0, 5));
        else sceneNum = 0;
        Debug.Log(sceneNum);

        if (sceneNum < 5) activeTrack = 0;

        if (sceneNum > 4 && sceneNum < 8) activeTrack = 1;

        if(sceneNum >7) activeTrack = 2;

        if (prevActiveTrack == activeTrack) return;

        else StartCoroutine(CrossFadeLinear(prevActiveTrack));

    }

    IEnumerator CrossFadeLinear(int prevTrack)
    {
        fading = true;
        //mc[activeTrack].gameObject.SetActive(true);
        while (mc[prevTrack].ambience.volume != 0)
        {

            mc[prevTrack].ambience.volume -= Time.deltaTime * crossFadeSpeed;
            mc[prevTrack].bass.volume -= Time.deltaTime * crossFadeSpeed;
            mc[prevTrack].drums.volume -= Time.deltaTime * crossFadeSpeed;

            mc[activeTrack].ambience.volume += Time.deltaTime * crossFadeSpeed;
            mc[activeTrack].ambience.volume = Mathf.Clamp(mc[activeTrack].ambience.volume, 0, topVolume);




            yield return null;
        }
        //mc[prevTrack].gameObject.SetActive(false);

        fading = false;
    }

    
    private void FixedUpdate()
    {
        if (rb == null || fading) return;


        
        drumCalc = Mathf.Clamp(rb.linearVelocity.magnitude, minSpeedDrums, topSpeedDrums);

        bassCalc = Mathf.Clamp(rb.linearVelocity.magnitude, 0, topSpeedBass);

        drumCalc -= minSpeedDrums;

        drumCalc /= (topSpeedDrums - minSpeedDrums);

        bassCalc /= topSpeedBass;

        mc[activeTrack].drums.volume = drumCalc * topVolume;
        mc[activeTrack].bass.volume = bassCalc * topVolume;
        

    }
}
