using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class emitWithSpeed : MonoBehaviour
{

    [SerializeField]
    float loopFreq = .1f;

    ParticleSystem part;

    [SerializeField]
    float topSpeed = 20f;

    [SerializeField]
    float topEmit = 80f;

    [SerializeField]
    Rigidbody rigid;

    float velMag;

    [SerializeField]
    WheelCollider wCol;


    [SerializeField]
    Volume pp;

    ChromaticAberration chrom;

    AudioSource rollin;

    float rollinMaxVol = .8f;

    float minRollVol = .6f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        part = GetComponent<ParticleSystem>();

        pp.profile.TryGet<ChromaticAberration>(out chrom);

        rollin = GetComponent<AudioSource>();

       // rollinMaxVol = rollin.volume;

        StartCoroutine(FakeUpdate());

        
    }


    IEnumerator FakeUpdate()
    {
        while (true)
        {
           
            var emission = part.emission;

            if (!wCol.isGrounded)
            {
                emission.rateOverTime = 0;
                rollin.volume = 0;
            }
            else
            {
                velMag = Mathf.Clamp(rigid.linearVelocity.magnitude, 0, topSpeed);

                velMag /= topSpeed;

                rollin.volume = rollinMaxVol * velMag;


                emission.rateOverTime = Mathf.Lerp(0f, topEmit, velMag);
            }

            chrom.intensity.Override(velMag);


            yield return new WaitForSeconds(loopFreq);
        }
    }
    
}
