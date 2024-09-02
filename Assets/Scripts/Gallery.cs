using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gallery : MonoBehaviour
{
    public GameObject GalleryWindow;
    public GameObject Content;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnEnable()
    {
        Content.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

    }

    // public void ShowHideWindow()
    // {
    //     if (GalleryWindow.activeSelf)
    //     {
    //         GalleryWindow.SetActive(false);
    //     }
    //     else
    //     {
    //         GalleryWindow.SetActive(true);
    //         //GO TO TOP OF WINDOW
    //         Content.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

    //         //GET CHILDREN OBJECT TO RESIZE WINDOW
    //         //GetChildrenAsArray();
    //         //GetHeight();
    //     }
    // }
}
