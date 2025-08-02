using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;


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


        activeTrack = Random.Range(0, mc.Count);


        mc[activeTrack].gameObject.SetActive(true);
    }

    
    private void FixedUpdate()
    {
        if (rb == null) return;


        
        drumCalc = Mathf.Clamp(rb.linearVelocity.magnitude, minSpeedDrums, topSpeedDrums);

        bassCalc = Mathf.Clamp(rb.linearVelocity.magnitude, 0, topSpeedBass);

        drumCalc -= minSpeedDrums;

        drumCalc /= (topSpeedDrums - minSpeedDrums);

        bassCalc /= topSpeedBass;

        mc[activeTrack].drums.volume = drumCalc;
        mc[activeTrack].bass.volume = bassCalc;
        

    }
}
