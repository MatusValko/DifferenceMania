using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public GameObject levelSelectWindow;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowHideWindow()
    {
        if (levelSelectWindow.activeSelf)
        {
            levelSelectWindow.SetActive(false);
        }
        else
        {
            levelSelectWindow.SetActive(true);
        }
    }


}
