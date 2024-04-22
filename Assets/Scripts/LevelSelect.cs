using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public GameObject LevelSelectWindow;
    public GameObject Content;

    public RectTransform[] children; // Array of child RectTransforms


    void OnEnable()
    {
        // Debug.Log("GameObject enabled!");
        //GO TO TOP OF WINDOW
        //content.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
    private void GetChildrenAsArray()
    {
        //PARENT HAS A LAYOUT GROUP AND CONTENT SIZE FITTER DOESNT WORK

        int childCount = Content.transform.childCount;
        children = new RectTransform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            children[i] = Content.transform.GetChild(i).GetComponent<RectTransform>();
        }
    }

    private void GetHeight()
    {
        foreach (RectTransform child in children)
        {
            float totalHeight = 0;
            // Debug.Log("CHILD: " + child);


            foreach (RectTransform minichild in child)
            {
                // Debug.Log(minichild.gameObject + " SIZE " + minichild.rect.height);

                totalHeight += minichild.rect.height;
            }
            child.sizeDelta = new Vector2(child.sizeDelta.x, totalHeight);
        }
    }
    //BUTTON CLICK FROM INSPECTOR
    public void ShowHideWindow()
    {
        if (LevelSelectWindow.activeSelf)
        {
            LevelSelectWindow.SetActive(false);
        }
        else
        {
            LevelSelectWindow.SetActive(true);
            //GO TO TOP OF WINDOW
            Content.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

            //GET CHILDREN OBJECT TO RESIZE WINDOW
            GetChildrenAsArray();
            GetHeight();
        }
    }


}
