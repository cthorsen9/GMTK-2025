using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// lowkenuinely confusing naming nbut this is for the UI setup, and levelEditorTools is for actual tool behavior
/// </summary>
public class LevelCreationTools : MonoBehaviour
{
    public static LevelCreationTools singleton;


    [Tooltip("the prop window/folder we have open")]
    public GameObject activePropSection;


    [SerializeField]
    [Tooltip("The game view window")]
    RectTransform window;


    [Header("Scene Init settings")]
    [SerializeField]
    List<GameObject> toEnableOnTest = new List<GameObject>();

    [SerializeField]
    List<GameObject> toDisableImmediately = new List<GameObject>();

    [SerializeField]
    List<GameObject> toDisableOnTest = new List<GameObject>();

    [SerializeField]
    RenderTexture gameText;

    [SerializeField]
    RenderTexture editorText;


    private void Start()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(singleton.gameObject);
        }
        singleton = this;


        foreach (GameObject go in toDisableImmediately)
        {
            go.SetActive(false);
        }
    }

    public void CreateObject(GameObject toSpawn)
    {

    }

    [Button]
    public void TestGame()
    {
        foreach(GameObject go in toDisableImmediately)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in toEnableOnTest)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in toDisableOnTest)
        {
            go.SetActive(false);
        }

        Camera.main.targetTexture = gameText;
    }

    [Button]
    public void StopTest()
    {
        if(Time.timeScale < 1) GameManager.singleton.PauseUnpause();

        foreach (GameObject go in toDisableImmediately)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in toEnableOnTest)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in toDisableOnTest)
        {
            go.SetActive(true);
        }

        Camera.main.targetTexture = editorText;

    }


    public void SwitchActivePropSection(GameObject newActiveWindow)
    {
        if (activePropSection == null) activePropSection = newActiveWindow;
        else
        {
            Debug.Log("trying to switch Windows");
            activePropSection.SetActive(false);
            newActiveWindow.SetActive(true);
            activePropSection = newActiveWindow;
        }
    }


    public bool InGameViewBounds()
    {
        Vector2 mPos = Input.mousePosition;
        return RectTransformUtility.RectangleContainsScreenPoint(window, mPos);
    }

}
