using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField]
    float boostAmount = 10000f;
    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) return;

        WheelController.singleton.rigid.AddForce(transform.forward * boostAmount);
    }
}
