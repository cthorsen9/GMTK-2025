using UnityEngine;

public class UISOunds : MonoBehaviour
{

    AudioSource audio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        audio.Play();   
    }

}
