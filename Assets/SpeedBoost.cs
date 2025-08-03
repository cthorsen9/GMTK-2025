using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField]
    float boostAmount = 10000f;

    AudioSource audio;
    private void Start()
    {
        audio = GetComponent<AudioSource>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) return;
        audio.Play();

        WheelController.singleton.rigid.AddForce(transform.forward * boostAmount);
    }
}
