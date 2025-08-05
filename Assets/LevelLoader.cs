using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    int levelsBeaten = 0;


    public void LoadBestLevel()
    {
        levelsBeaten = PlayerPrefs.GetInt("levelsBeaten");
        if (levelsBeaten == 10) SceneManager.LoadScene(11);
        else SceneManager.LoadScene(levelsBeaten+ 1);
    }
}
