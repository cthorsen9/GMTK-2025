using System.Collections;
using UnityEngine;

public class musicScript : MonoBehaviour
{
    
    [SerializeField]
    AudioClip looper;

    AudioSource source;

    float refTime = 1f;


    void Start()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine(Timer());
        source.PlayScheduled(1f);
        
    }

   IEnumerator Timer()
    {
        while (true)
        {
            refTime += source.clip.length;
            //source.PlayScheduled(refTime);
            yield return new WaitForSecondsRealtime(source.clip.length);
            
            //source.clip = looper;
            
        }

    }
}
