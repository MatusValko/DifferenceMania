using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public GameObject levelSelectWindow;
    public GameObject content;

    public RectTransform[] children; // Array of child RectTransforms


    void OnEnable()
    {
        // Debug.Log("GameObject enabled!");
        //GO TO TOP OF WINDOW
        content.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }

    public void GetChildrenAsArray()
    {
        int childCount = content.transform.childCount;
        children = new RectTransform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            children[i] = content.transform.GetChild(i).GetComponent<RectTransform>();
        }
    }

    public void GetHeight()
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
    public void ShowHideWindow()
    {
        if (levelSelectWindow.activeSelf)
        {
            levelSelectWindow.SetActive(false);
        }
        else
        {
            levelSelectWindow.SetActive(true);

            //GET CHILDREN OBJECT TO RESIZE WINDOW
            GetChildrenAsArray();
            GetHeight();
        }
    }


}
