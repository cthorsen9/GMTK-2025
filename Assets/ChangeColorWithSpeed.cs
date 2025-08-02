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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(fakeUpdate());
    }

    IEnumerator fakeUpdate()
    {
        while (true)
        {
            velMag = Mathf.Clamp(rigid.linearVelocity.magnitude, 0, topSpeed);

            velMag /= topSpeed;


            text.color = gradient.Evaluate(velMag);

            yield return new WaitForSeconds(updateFreq);
        }

    }
}
