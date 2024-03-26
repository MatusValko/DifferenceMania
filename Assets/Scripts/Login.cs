using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {

    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
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
