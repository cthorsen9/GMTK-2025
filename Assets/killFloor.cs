using UnityEngine;
using UnityEngine.SceneManagement;


public class killFloor : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collsiion enterd to rest Scen");
        if (GameManager.singleton.playerFinsihed) return;

        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.singleton.ReturnToCheckPoint();
        }
    }
}
