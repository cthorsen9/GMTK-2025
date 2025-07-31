using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using MoreMountains.Tools;

public class cameraIntro : MonoBehaviour
{
    [SerializeField]
    CinemachineCamera dollyCamera;

    [SerializeField]
    CinemachineCamera mainCamera;

    [SerializeField]
    CinemachineSplineDolly camDolly;

    [SerializeField]
    CinemachineSplineCart cart;

    [SerializeField]
    MMFader fader;


    [SerializeField]
    float timeForIntro = 4f;

    [SerializeField]
    float fadeTime = 1f;

    float timer = 0f;

    bool done = false;

    public void FixCamera()
    {
        mainCamera.Priority = 1000;

    }


    public void Startintro()
    {
        StartCoroutine(CamSequencer());
    }

    IEnumerator CamSequencer()
    {
        while(camDolly.CameraPosition < 1)
        {
            timer += Time.deltaTime;

            camDolly.CameraPosition = timer / timeForIntro;
            cart.SplinePosition = camDolly.CameraPosition;

            if (camDolly.CameraPosition > .9 && !done) mainCamera.Priority = 1000;


            yield return null;
        }
        //mainCamera.Priority = 1000;


        yield return new WaitForSeconds(fadeTime);
        //fader.FadeOut(fadeTime, MMTweenType.DefaultEaseInCubic, true);

        GameManager.singleton.LevelCountdown();





    }

    void FadeAndTrans()
    {
        fader.FadeIn(fadeTime, MMTweenType.DefaultEaseInCubic);
        done = true;
    }
}
