using UnityEngine;
using UnityEngine.UI;


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
