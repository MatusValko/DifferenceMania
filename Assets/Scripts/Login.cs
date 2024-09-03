using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [Header("Create Account Canvas")]
    [SerializeField]
    private TMP_InputField _email;
    [SerializeField]
    private TMP_InputField _password;
    [SerializeField]
    private TMP_InputField _nickname;
    [SerializeField]
    private Toggle _checkbox;
    [SerializeField]
    private Button _createAccountButton;
    [SerializeField]
    private TextMeshProUGUI _createAccountButtonText;

    [Header("Edit Nickname Canvas")]
    [SerializeField]
    private Button _changeNickNameButton;
    [SerializeField]
    private TextMeshProUGUI _changeNickNameButtonText;
    [SerializeField]
    private TMP_InputField _newNickNameText;
    [SerializeField]
    private TextMeshProUGUI _popUpNewNickNameText;


    public void CheckInputField()
    {
        string text = _newNickNameText.text;  // here "TextMeshProText" is 'TMP_InputField'
        // Debug.Log("Text is:" + text);
        if (!string.IsNullOrEmpty(text))
        {
            _popUpNewNickNameText.text = text;
            ChangeButtonInteractibility(_changeNickNameButton, _changeNickNameButtonText, true);
            // _changeNickNameButton.interactable = true;
            // _changeNickNameButtonText.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            ChangeButtonInteractibility(_changeNickNameButton, _changeNickNameButtonText, false);
            // Debug.Log("TUUU");
            // _changeNickNameButtonText.color = new Color32(255, 255, 255, 100);
            // _changeNickNameButton.interactable = false;
        }
    }
    //MAKES BUTON INTERACTABLE AND CHANGES BUTTON TEXT ACCORDINGLY
    private void ChangeButtonInteractibility(Button button, TextMeshProUGUI textMeshProUGUI, bool boolean)
    {
        if (boolean)
        {
            button.interactable = true;
            textMeshProUGUI.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            button.interactable = false;
            textMeshProUGUI.color = new Color32(255, 255, 255, 100);
        }
    }

    public void CreateAccountValidation()
    {
        Debug.Log("VALIDUJEM");
        string text = _email.text;

        Debug.Log(text);

        bool error = false;
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("email.text is empty!");
            error = true;
        }
        if (string.IsNullOrEmpty(_password.text))
        {
            Debug.LogWarning("password.text is empty!");
            error = true;
        }
        if (string.IsNullOrEmpty(_nickname.text))
        {
            Debug.LogWarning("_nickname.text is empty!");
            error = true;
        }

        if (error)
        {
            ChangeButtonInteractibility(_createAccountButton, _createAccountButtonText, !error);
        }
        else
        {
            ChangeButtonInteractibility(_createAccountButton, _createAccountButtonText, !error);
        }
        // VALIDATE INPUTS
        // Debug.Log(_email);
        // Debug.Log(_password);
        // Debug.Log(_nickname);
        // Debug.Log(_checkbox);



    }
    void OnEnable()
    {
        CreateAccountValidation();
    }


    // Start is called before the first frame update
    void Awake()
    {
        // _changeNickNameButtonText = _changeNickNameButton.gameObject.GetComponentInChildren<TextMeshProUGUI>();
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
        SceneManager.LoadScene(1);
    }




}
