using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PerisitentData : MonoBehaviour
{
    public static PerisitentData singleton;

    public string username;

   public TMP_InputField input;


    StringChecker checker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPrefs.SetFloat("volumeLevel", 1f);
        PlayerPrefs.Save();
        if (singleton == null)
        {
            // First instance - become the singleton
            singleton = this;
        }
        else if (singleton != this)
        {
            // Subsequent instance - destroy yourself
            Destroy(gameObject);
            return; // Important to prevent further execution
        }

        checker = GetComponent<StringChecker>();

        username = PlayerPrefs.GetString("username");
        if (username == null)
        {
            username = "Player";
            PlayerPrefs.SetString("username", username);
            PlayerPrefs.Save();
        }

        if(input != null && username != "username")
        {
            input.text = username;
        }

        

    }
    public void TrySetUserName(TMP_InputField inputter)
    {
        if( checker.CheckString(inputter.text))
        {
            username = inputter.text;
        }
        else
        {
            inputter.text = "PottyMouth";
            username = "PottyMouth";
        }
        PlayerPrefs.SetString("username", username);
        PlayerPrefs.Save();
    }
}
