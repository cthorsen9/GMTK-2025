using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public class LevelCreationTools : MonoBehaviour
{
    public static LevelCreationTools singleton;

    Transform camTrans;


    public GameObject activePropSection;


    [Header("Scene Init settings")]
    [SerializeField]
    List<GameObject> toEnableOnTest = new List<GameObject>();

    [SerializeField]
    List<GameObject> toDisableImmediately = new List<GameObject>();

    [SerializeField]
    List<GameObject> toDisableOnTest = new List<GameObject>();


    private void Start()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(singleton.gameObject);
        }
        singleton = this;

        camTrans = Camera.main.transform;

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

    }

    [Button]
    public void StopTest()
    {
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

}
