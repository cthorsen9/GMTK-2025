using UnityEngine;
using UnityEngine.SceneManagement;


public class killFloor : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collsiion enterd to rest Scen");
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
