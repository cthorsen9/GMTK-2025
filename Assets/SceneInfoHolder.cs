using UnityEngine;

public class SceneInfoHolder : MonoBehaviour
{
    public static SceneInfoHolder singleton;

    public bool playedIntro =  false;

    //public GameObject PauseMenu

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (singleton != null && singleton != this.gameObject)
        {
            Destroy(singleton.gameObject);
        }
        else if (singleton == null) singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
