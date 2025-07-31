using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        WheelController.singleton.trackTarget.SetParent(transform);

        GameManager.singleton.LevelComplete();

    }
}
