using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GoTo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GoToLoginMenu()
    {
        SceneManager.LoadScene("Login");
    }
    public void GoToLoading()
    {
        SceneManager.LoadScene("Loading");
    }
}
