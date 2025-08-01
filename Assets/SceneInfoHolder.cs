using UnityEngine;



//this exists as our gamemanager destroys on load as each has local level vars, so this holds data between runs
public class SceneInfoHolder : MonoBehaviour
{
    public static SceneInfoHolder singleton;

    public bool playedIntro =  false;

    public float TimeToBeat = 24f;

    //public GameObject PauseMenu

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (singleton == null)
        {
            // First instance - become the singleton
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (singleton != this)
        {
            // Subsequent instance - destroy yourself
            Destroy(gameObject);
            return; // Important to prevent further execution
        }

        PlayerPrefs.SetInt("onlineGhost", 1);

    }

}
