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
    List<GameObject> toEnable = new List<GameObject>();

    [SerializeField]
    List<GameObject> toDisable = new List<GameObject>();


    private void Start()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(singleton.gameObject);
        }
        singleton = this;

        camTrans = Camera.main.transform;

        foreach (GameObject go in toDisable)
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
        foreach(GameObject go in toDisable)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in toEnable)
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
