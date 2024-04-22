using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialModes : MonoBehaviour
{
    public GameObject SpecialModesWindow;
    public GameObject Content;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ShowHideWindow()
    {
        if (SpecialModesWindow.activeSelf)
        {
            SpecialModesWindow.SetActive(false);
        }
        else
        {
            SpecialModesWindow.SetActive(true);
            //GO TO TOP OF WINDOW
            Content.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

            //GET CHILDREN OBJECT TO RESIZE WINDOW
            //GetChildrenAsArray();
            //GetHeight();
        }
    }
}
