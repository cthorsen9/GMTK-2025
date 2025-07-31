using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    Transform spawnPt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPt = transform.GetChild(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.singleton.CheckpointEntered(spawnPt);
    }
}
