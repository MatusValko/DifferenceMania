using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gallery : MonoBehaviour
{
    public GameObject Content;
    [SerializeField] private Animator _collectionAnimator;
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
        SoundManager.PlaySound(SoundType.COLLECTION_START_OPEN_WINDOW);
        _collectionAnimator.SetTrigger("OpenCurtains");
        //play curtain sound

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
