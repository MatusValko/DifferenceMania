using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [Header("Main Menu Canvas")]
    [SerializeField]
    private GameObject _mainMenuCanvas;

    [Header("Create Account Canvas")]
    [SerializeField]
    private GameObject _createAccountCanvas;
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


    [Header("Error Window")]
    [SerializeField]
    private GameObject _errorWindow;

    [SerializeField]
    private TextMeshProUGUI _errorResponseText;

    [Header("Profile Canvas")]
    [SerializeField]
    private GameObject _profileCanvas;
    [SerializeField]
    private TextMeshProUGUI _nicknameText;
    [SerializeField]
    private TextMeshProUGUI _emailText;
    [SerializeField]
    private Image _profilePicture;

    [Header("Edit Nickname Canvas")]
    [SerializeField]
    private GameObject _freeText;
    [SerializeField]
    private GameObject _cost50Text;
    [SerializeField]
    private Button _freeButton;
    [SerializeField]
    private Button _refillWithCoinsButton;
    [SerializeField]
    private Button _changeNickNameButton;
    [SerializeField]
    private TextMeshProUGUI _changeNickNameButtonText;
    [SerializeField]
    private TMP_InputField _newNickNameText;
    [SerializeField]
    private TextMeshProUGUI _popUpNewNickNameText;


    [Serializable]
    struct CreateAccountResponse
    {
        public bool succes;
        public string message;
        public string token;
        // public string nickname;
        // public bool checkbox;

    }

    public void CheckInputField()
    {
        string text = _newNickNameText.text;  // here "TextMeshProText" is 'TMP_InputField'
        // Debug.Log("Text is:" + text);
        if (!string.IsNullOrEmpty(text))
        {
            _popUpNewNickNameText.text = text;
            ChangeButtonInteractibility(_changeNickNameButton, _changeNickNameButtonText, true);
        }
        else
        {
            ChangeButtonInteractibility(_changeNickNameButton, _changeNickNameButtonText, false);
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
        if (GameManager.Instance.ISLOGGEDIN)
        {
            Debug.Log("NEVALIDUJEM");
            return;
        }
        Debug.Log("VALIDUJEM, Logged in...");


        bool error = false;
        if (string.IsNullOrEmpty(_email.text))
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
    }
    //CLICK NA BUTTON
    public void CreateAccount()
    {
        _createAccountButton.interactable = false;
        StartCoroutine(Upload());
    }

    IEnumerator ShowError(string errorText)
    {
        _errorWindow.SetActive(true);
        _errorResponseText.text = errorText;
        yield return new WaitForSeconds(3);
        _errorWindow.SetActive(false);
        yield return null;
    }

    public void CheckIfFreeOrCost()
    {
        if (GameManager.Instance.GetFreeNickName())
        {
            _freeText.SetActive(true);
            _cost50Text.SetActive(false);
            _freeButton.gameObject.SetActive(true);
            _refillWithCoinsButton.gameObject.SetActive(false);
        }
        else
        {
            _freeText.SetActive(false);
            _cost50Text.SetActive(true);
            _freeButton.gameObject.SetActive(false);
            _refillWithCoinsButton.gameObject.SetActive(true);
        }
    }
    public void CheckIfFreeOrCostNew()
    {
        bool free = GameManager.Instance.GetFreeNickName();
        _freeText.SetActive(free);
        _cost50Text.SetActive(!free);
        _freeButton.gameObject.SetActive(free);
        _refillWithCoinsButton.gameObject.SetActive(!free);
    }

    public void ShowErrorWindow(string errorText)
    {
        _errorWindow.SetActive(true);
        _errorResponseText.text = errorText;
    }
    public void ChangeNickname()
    {
        //poslat poziadavku na server
        GameManager.Instance.SetNickname(_newNickNameText.text);
        GameManager.Instance.SetFreeNickNameToFalse();
        DataPersistenceManager.Instance.SaveGame();
        UpdateProfileMenu();
    }
    public void ChangeNicknameForCoins()
    {
        //najskor pozri na clientovi ci ma 50 coins
        if (GameManager.Instance.GetCoins() < 50)
        {
            _errorWindow.SetActive(true);
            _errorResponseText.text = $"Not Enough Coins! ({GameManager.Instance.GetCoins()})";
            return;
        }
        // TODO: hore 
        // poslat poziadavku na server ci ma 50 coins


        //AK ma poslat na server poziadavku na zmenenie nickaname
        GameManager.Instance.AddCoins(-50);
        ChangeNickname();

    }
    IEnumerator Upload()
    {
        Debug.Log(SystemInfo.deviceModel);

        List<IMultipartFormSection> form = new()
        {
            new MultipartFormDataSection("email", _email.text),
            new MultipartFormDataSection("password", _password.text),
            new MultipartFormDataSection("device_name", SystemInfo.deviceModel),

            // new MultipartFormDataSection("nickname", _nickname.text),
            // new MultipartFormDataSection("checkbox", _checkbox.isOn.ToString())
        };

        Debug.Log(form);

        using UnityWebRequest www = UnityWebRequest.Post(GameManager.API_REGISTER, form);
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("WEB REQUEST ERROR:" + www.error);
            // StartCoroutine(ShowError(www.error));
            ShowErrorWindow(www.error);
        }
        else
        {
            Debug.Log(www.result + " Form upload complete!");
            string responseText = www.downloadHandler.text;
            Debug.Log("Response Text:" + responseText);

            CreateAccountResponse response;
            try
            {
                response = JsonUtility.FromJson<CreateAccountResponse>(responseText);
            }
            catch (Exception e)
            {
                // StartCoroutine(ShowError(e.Message));
                Debug.LogError("TRY CATCH ERROR:" + e.Message);
                ShowErrorWindow(e.Message);
                throw;
            }
            // Debug.Log("Response: " + response);

            Debug.Log(response.token);
            //MOZEME SA POSUNUT DALEJ JEEJ
            //ULOZIT DATA O POUZIVATELOVI
            //POSLAT DATA DO DATABAZY

            GameManager.Instance.SetToken(response.token);
            GameManager.Instance.SetEmail(_email.text);
            GameManager.Instance.SetNickname(_nickname.text);
            GameManager.Instance.ISLOGGEDIN = true;
            DataPersistenceManager.Instance.SaveGame();
            // _password.text

            //ZISKAT

            _profileCanvas.SetActive(true);
            _createAccountCanvas.SetActive(false);



        }
    }

    void OnEnable()
    {
        CreateAccountValidation();
        UpdateProfileMenu();
        CheckIfLoggedIn();
    }


    // Start is called before the first frame update
    void Awake()
    {
        //prehodid do enable

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



    private void UpdateProfileMenu()
    {
        _nicknameText.text = GameManager.Instance.GetNickname();
        _emailText.text = GameManager.Instance.GetEmail();
        // _emailText.text = GameManager.Instance.GetprofilePicture();
    }
    private void CheckIfLoggedIn()
    {
        if (GameManager.Instance.ISLOGGEDIN)
        {
            Debug.Log("Is logged in");
            _mainMenuCanvas.SetActive(false);
            _profileCanvas.SetActive(true);
        }
        else
        {
            Debug.Log("Is NOT logged in");
            _mainMenuCanvas.SetActive(true);
            _profileCanvas.SetActive(false);
        }
    }
}