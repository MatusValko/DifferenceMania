using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Premium : MonoBehaviour
{
    public GameObject PremiumWindow;
    public GameObject Content;
    // Start is called before the first frame update

    void OnEnable()
    {
        // Debug.Log("GameObject enabled!");
        //GO TO TOP OF WINDOW
        //content.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

        //GO TO TOP OF WINDOW
        Content.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
