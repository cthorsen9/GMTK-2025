using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class ChangeColorWithSpeed : MonoBehaviour
{

    public float updateFreq = .1f;

    public Gradient gradient = new Gradient();

    [SerializeField]
    float topSpeed;

    float velMag;

    [SerializeField]
    TMP_Text text;

    [SerializeField]
    Rigidbody rigid;

    [SerializeField]    
    bool audio;

    AudioSource source;

    [SerializeField]
    float maxVolume = .6f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(audio) source = GetComponent<AudioSource>();
        StartCoroutine(fakeUpdate());
    }

    IEnumerator fakeUpdate()
    {
        while (true)
        {
            velMag = Mathf.Clamp(rigid.linearVelocity.magnitude, 0, topSpeed);

            velMag /= topSpeed;


            if (audio)
            {
                source.volume = velMag * maxVolume;
            }

            else text.color = gradient.Evaluate(velMag);

            yield return new WaitForSeconds(updateFreq);
        }

    }
}
