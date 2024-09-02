using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField]
    private Button _changeNickNameButton;
    // [SerializeField]
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
            _changeNickNameButton.interactable = true;
            _changeNickNameButtonText.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            // Debug.Log("TUUU");
            _changeNickNameButtonText.color = new Color32(255, 255, 255, 100);
            _changeNickNameButton.interactable = false;

        }

    }
    // Start is called before the first frame update
    void Awake()
    {
        _changeNickNameButtonText = _changeNickNameButton.gameObject.GetComponentInChildren<TextMeshProUGUI>();
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


    public void ShowNicknameButton()
    {

    }

}
