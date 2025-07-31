using System.Collections;
using UnityEngine;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        part = GetComponent<ParticleSystem>();
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
            }
            else
            {
                velMag = Mathf.Clamp(rigid.linearVelocity.magnitude, 0, topSpeed);

                velMag /= topSpeed;


                emission.rateOverTime = Mathf.Lerp(0f, topEmit, velMag);
            }
                


            yield return new WaitForSeconds(loopFreq);
        }
    }
    
}
