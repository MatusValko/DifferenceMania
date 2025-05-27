using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Profile : MonoBehaviour
{
    [SerializeField]
    private GameObject _connectAccount;
    [SerializeField]
    private GameObject _accountConnected;

    void OnEnable()
    {
        if (GameManager.Instance.ISLOGGEDIN)
        {
            _accountConnected.SetActive(true);
            _connectAccount.SetActive(false);
        }
        else
        {
            _accountConnected.SetActive(false);
            _connectAccount.SetActive(true);
        }
    }


    public void LoadLogin()
    {
        //SCENE 2 LOGIN
        SceneManager.LoadScene(2);
    }
}
