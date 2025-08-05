using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SolumeSlider : MonoBehaviour
{
   AudioListener listener;

    [SerializeField]
    Slider slider;

    private void Start()
    {
        setupVolume();
    }

    void setupVolume()
    {
        if (PlayerPrefs.GetFloat("level1") == 0 && SceneManager.GetActiveScene().buildIndex ==0) { PlayerPrefs.SetFloat("volumeLevel", .8f); PlayerPrefs.Save(); AudioListener.volume = .8f; }


        slider.value = PlayerPrefs.GetFloat("volumeLevel");
        
    }


    public void ChangedVolume(Slider s)
    {
        if(listener == null) listener =  Camera.main.GetComponent<AudioListener>();
        
        AudioListener.volume = s.value;

        PlayerPrefs.SetFloat("volumeLevel", s.value);
        PlayerPrefs.Save();
    }
}
