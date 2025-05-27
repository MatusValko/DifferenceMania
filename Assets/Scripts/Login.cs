using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class Login : MonoBehaviour
{
    [Header("Main Menu Canvas")]
    [SerializeField]
    private GameObject _mainMenuCanvas;

    [Header("Sprites")]
    [SerializeField]
    private Sprite _formInput;
    [SerializeField]
    private Sprite _formErrorInput;

    [SerializeField]
    private Sprite _openEye;
    [SerializeField]
    private Sprite _closedEye;

    [Header("Create Account Canvas")]
    [SerializeField]
    private GameObject _createAccountCanvas;
    [SerializeField]
    private TMP_InputField _email;
    private Image _emailFormInputImage;
    private const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    [SerializeField]
    private TMP_InputField _password;
    private Image _passwordFormInputImage;


    [SerializeField]
    private Image _eyeButton;
    private bool isPasswordVisible = false;




    [SerializeField]
    private TMP_InputField _nickname;
    private Image _nicknameFormInputImage;
    [SerializeField]
    private Toggle _checkbox;
    [SerializeField]
    private Button _createAccountButton;
    [SerializeField]
    private TextMeshProUGUI _createAccountButtonText;


    [SerializeField]
    private GameObject _emailErrorMessage;
    [SerializeField]
    private GameObject _passwordErrorMessage;
    [SerializeField]
    private GameObject _nicknameErrorMessage;


    [Header("Login To Account Canvas")]
    [SerializeField]
    private TMP_InputField _emailLogin;
    [SerializeField]
    private TMP_InputField _passwordLogin;
    [SerializeField]
    private Button _loginButton;
    [SerializeField]
    private TextMeshProUGUI _loginAccountButtonText;

    [SerializeField]
    private GameObject _emailLoginErrorMessage;
    [SerializeField]
    private GameObject _passwordLoginErrorMessage;

    private Image _emailLoginFormInputImage;
    private Image _passwordLoginFormInputImage;


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
    void Awake()
    {
        _emailFormInputImage = _email.gameObject.GetComponent<Image>();
        _passwordFormInputImage = _password.gameObject.GetComponent<Image>();
        _nicknameFormInputImage = _nickname.gameObject.GetComponent<Image>();
        _emailLoginFormInputImage = _emailLogin.gameObject.GetComponent<Image>();
        _passwordLoginFormInputImage = _passwordLogin.gameObject.GetComponent<Image>();

        //ONLY FOR TESTING
        GenerateRandomCredentials();
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
    //Vzdy ked sa zmeni pole vo formulari tak sa zavolá táto funkcia na validovanie vstupu

    //CLICK NA BUTTON
    public void CreateAccount()
    {
        _createAccountButton.interactable = false;
        StartCoroutine(UploadRegisterForm());
    }


    public void LoginToAccount()
    {
        _createAccountButton.interactable = false;
        StartCoroutine(UploadRegisterForm());
    }
    IEnumerator ShowError(string errorText)
    {
        _errorWindow.SetActive(true);
        _errorResponseText.text = errorText;
        yield return new WaitForSeconds(3);
        _errorWindow.SetActive(false);
        yield return null;
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
        GameManager.Instance.SetFreeNickName(false);
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
    IEnumerator UploadRegisterForm(bool randomAccount = false)
    {

        string email;
        string nickname;
        string password;
        string isEmailEnabled;
        if (randomAccount)
        {
            //generateGUID but not that long 
            // Use a shorter random string for anonymous accounts
            string randomGUID = Guid.NewGuid().ToString("N").Substring(0, 12);
            // string randomGUID = Guid.NewGuid().ToString();
            email = "anonymous_" + randomGUID + "@sharkrise.com";
            nickname = "anonymous_" + randomGUID;
            password = randomGUID;
            isEmailEnabled = "0";
            DebugLogger.Log("Registering new anonymous user with email: " + email);
            // form = new List<IMultipartFormSection>
            // {
            //     new MultipartFormDataSection("email", email),
            //     new MultipartFormDataSection("password", password),
            //     new MultipartFormDataSection("device_name", SystemInfo.deviceModel),
            //     new MultipartFormDataSection("nickname", nickname),
            //     new MultipartFormDataSection("is_email_enabled",  "0")
            // };
        }
        else
        {
            email = _email.text;
            nickname = _nickname.text;
            password = _password.text;
            isEmailEnabled = Convert.ToString(_checkbox.isOn ? 1 : 0);
            // form = new List<IMultipartFormSection>
            // {
            //     new MultipartFormDataSection("email", _email.text),
            //     new MultipartFormDataSection("password", _password.text),
            //     new MultipartFormDataSection("device_name", SystemInfo.deviceModel),
            //     new MultipartFormDataSection("nickname", _nickname.text),
            //     new MultipartFormDataSection("is_email_enabled", Convert.ToString(_checkbox.isOn ? 1 : 0))
            // };
        }

        List<IMultipartFormSection> form = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("email", email),
                new MultipartFormDataSection("password", password),
                new MultipartFormDataSection("device_name", SystemInfo.deviceModel),
                new MultipartFormDataSection("nickname", nickname),
                new MultipartFormDataSection("is_email_enabled",  isEmailEnabled)
            };

        using UnityWebRequest www = UnityWebRequest.Post(GameManager.API_REGISTER, form);
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            DebugLogger.LogError("WEB REQUEST ERROR:" + www.error);
            // StartCoroutine(ShowError(www.error));
            ShowErrorWindow(www.error);
        }
        else
        {
            DebugLogger.Log(www.result + " Form upload complete!");
            string responseText = www.downloadHandler.text;
            DebugLogger.Log("Response Text:" + responseText);

            CreateAccountResponse response;
            try
            {
                response = JsonUtility.FromJson<CreateAccountResponse>(responseText);
            }
            catch (Exception e)
            {
                // StartCoroutine(ShowError(e.Message));
                DebugLogger.LogError("TRY CATCH ERROR:" + e.Message);
                ShowErrorWindow(e.Message);
                throw;
            }

            DebugLogger.Log(response.token);
            //MOZEME SA POSUNUT DALEJ JEEJ
            //ULOZIT DATA O POUZIVATELOVI
            //POSLAT DATA DO DATABAZY
            GameManager.Instance.SetToken(response.token);
            GameManager.Instance.SetEmail(email);
            GameManager.Instance.SetNickname(nickname);
            GameManager.Instance.SetFreeNickName(true);
            GameManager.Instance.ISLOGGEDIN = true;

            StartCoroutine(_fetchLevelData());

            DataPersistenceManager.Instance.SaveGame();
            // _password.text

            //ZISKAT UDAJE ZO SERVERA
            _profileCanvas.SetActive(true);
            UpdateProfileMenu();
            _createAccountCanvas.SetActive(false);

        }
    }



    private IEnumerator _fetchLevelData()
    {
        yield return StartCoroutine(LevelLoader.GetProgressData());

    }
    // ON BUTTON, register default anonymous account user //OBSOLETE
    public void RegisterNewAnonymousUser()
    {
        //generate random uID
        StartCoroutine(UploadRegisterForm(true));

    }

    //TODO REWRITE 
    IEnumerator UploadNewAnonymousUser(string gUID)
    {
        string randomEmail = "anonymous_" + gUID + "@sharkrise.com";
        string nickaname = "anonymous_" + gUID;
        string password = gUID;
        DebugLogger.Log("Registering new anonymous user with email: " + randomEmail);


        List<IMultipartFormSection> form = new()
        {
            new MultipartFormDataSection("email", randomEmail),
            new MultipartFormDataSection("password", password),
            new MultipartFormDataSection("device_name", SystemInfo.deviceModel),
            new MultipartFormDataSection("nickname", nickaname),
            new MultipartFormDataSection("is_email_enabled", "1"  )
        };
        // Debug.Log(Convert.ToString(_checkbox.isOn ? 1 : 0));

        using UnityWebRequest www = UnityWebRequest.Post(GameConstants.API_REGISTER, form);
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
            GameManager.Instance.SetEmail(randomEmail);
            GameManager.Instance.SetNickname(nickaname);
            GameManager.Instance.SetFreeNickName(true);
            GameManager.Instance.ISLOGGEDIN = true;
            DataPersistenceManager.Instance.SaveGame(); //NOT SAVING DATA LOCALLY, JUST ON SERVER
            // _password.text

            //ZISKAT

            _profileCanvas.SetActive(true);
            UpdateProfileMenu();
            _createAccountCanvas.SetActive(false);

        }
    }

    void OnEnable()
    {
        CreateAccountValidation();
        LoginToAccountValidation();
        UpdateProfileMenu();
        CheckIfLoggedInAndChangeWindows();
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
    private void CheckIfLoggedInAndChangeWindows()
    {
        GameManager.Instance.CheckIfIsLoggedIn();

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
    public void LogoutUser()
    {
        GameManager.Instance.LogOut();
        CheckIfLoggedInAndChangeWindows();
    }



    #region VALIDATION

    public void LoginToAccountValidation()
    {
        if (GameManager.Instance.ISLOGGEDIN)
        {
            Debug.Log("NEVALIDUJEM LOGIN FORMULAR!");
            return;
        }
        // Debug.Log("VALIDUJEM, Logged in...");


        bool error = false;
        if (string.IsNullOrEmpty(_emailLogin.text))
        {
            Debug.LogWarning("login email.text is empty!");
            error = true;
        }
        if (string.IsNullOrEmpty(_passwordLogin.text))
        {
            Debug.LogWarning("Login password.text is empty!");
            error = true;
        }

        if (_validateEmailLogin())
        {
            error = true;
        }

        if (_validatePasswordLogin())
        {
            error = true;
        }

        ChangeButtonInteractibility(_loginButton, _loginAccountButtonText, !error);
    }
    //Vzdy ked sa zmeni pole vo formulari tak sa zavolá táto funkcia na validovanie vstupu
    public void CreateAccountValidation()
    {
        if (GameManager.Instance.ISLOGGEDIN)
        {
            Debug.Log("NEVALIDUJEM REGISTER FORMULAR!");
            return;
        }
        // Debug.Log("VALIDUJEM, Logged in...");


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

        if (_validateEmail())
        {
            error = true;
        }

        if (_validatePassword())
        {
            error = true;
        }

        //TODO CHECKBOX
        //MUSI BYT ZASKRTNUTY?

        ChangeButtonInteractibility(_createAccountButton, _createAccountButtonText, !error);
    }
    public void OnPasswordDeselect()
    {
        if (_validatePassword())
        {
            //PASSWORD MIN. 6 CHARACTERS
            //TODO DAJ CERVENY RAMIK
            _passwordErrorMessage.SetActive(true);
            _passwordFormInputImage.sprite = _formErrorInput;
        }
        else
        {
            _passwordErrorMessage.SetActive(false);
            _passwordFormInputImage.sprite = _formInput;
        }
    }
    private bool _validatePassword()
    {
        if (_password.text.Length < 6)
        {
            //PASSWORD MIN. 6 CHARACTERS
            // Debug.LogWarning("PASSWORD MIN. 6 CHARACTERS!");
            return true;
        }
        else
        {
            if (_passwordErrorMessage.activeSelf)
            {
                _passwordErrorMessage.SetActive(false);
                _passwordFormInputImage.sprite = _formInput;
                //TODO VYMENIT RAMIK
            }
            return false;
        }
    }

    public void OnEmailDeselect()
    {
        if (_validateEmail())
        {
            //DAJ CERVENY RAMIK
            _emailErrorMessage.SetActive(true);
            _emailFormInputImage.sprite = _formErrorInput;
        }
        else
        {
            _emailErrorMessage.SetActive(false);
            _emailFormInputImage.sprite = _formInput;
        }
    }
    private bool _validateEmail()
    {
        if (!Regex.IsMatch(_email.text, emailPattern))
        {
            //PASSWORD MIN. 6 CHARACTERS
            // Debug.LogWarning("INCORRECT EMAIL!");
            return true;
        }
        else
        {
            if (_emailErrorMessage.activeSelf)
            {
                _emailErrorMessage.SetActive(false);
                _emailFormInputImage.sprite = _formInput;
                //TODO VYMENIT RAMIK
            }
            return false;
        }
    }

    public void OnPasswordLoginDeselect()
    {
        if (_validatePasswordLogin())
        {
            //PASSWORD MIN. 6 CHARACTERS
            //TODO DAJ CERVENY RAMIK
            _passwordLoginErrorMessage.SetActive(true);
            _passwordLoginFormInputImage.sprite = _formErrorInput;
        }
        else
        {
            _passwordLoginErrorMessage.SetActive(false);
            _passwordLoginFormInputImage.sprite = _formInput;
        }
    }
    private bool _validatePasswordLogin()
    {
        if (_passwordLogin.text.Length < 6)
        {
            //PASSWORD MIN. 6 CHARACTERS
            // Debug.LogWarning("PASSWORD MIN. 6 CHARACTERS!");
            return true;
        }
        else
        {
            if (_passwordLoginErrorMessage.activeSelf)
            {
                _passwordLoginErrorMessage.SetActive(false);
                _passwordLoginFormInputImage.sprite = _formInput;
                //TODO VYMENIT RAMIK
            }
            return false;
        }
    }

    public void OnEmailLoginDeselect()
    {
        if (_validateEmailLogin())
        {
            //DAJ CERVENY RAMIK
            _emailLoginErrorMessage.SetActive(true);
            _emailLoginFormInputImage.sprite = _formErrorInput;
        }
        else
        {
            _emailLoginErrorMessage.SetActive(false);
            _emailLoginFormInputImage.sprite = _formInput;
        }
    }
    private bool _validateEmailLogin()
    {
        if (!Regex.IsMatch(_emailLogin.text, emailPattern))
        {
            //PASSWORD MIN. 6 CHARACTERS
            // Debug.LogWarning("INCORRECT EMAIL!");
            return true;
        }
        else
        {
            if (_emailLoginErrorMessage.activeSelf)
            {
                _emailLoginErrorMessage.SetActive(false);
                _emailLoginFormInputImage.sprite = _formInput;
                //TODO VYMENIT RAMIK
            }
            return false;
        }
    }

    #endregion VALIDATION

    public void ShowChangeNickNameWindow()
    {
        if (GameManager.Instance.GetCoins() < 50)
        {
            _refillWithCoinsButton.enabled = false;
            // TODO DOKONCIT
            return;
        }
    }





    public void TogglePasswordVisibility()
    {
        if (isPasswordVisible)
        {
            // Hide password: Set InputField to password mode
            _password.contentType = TMP_InputField.ContentType.Password;
            _eyeButton.sprite = _openEye;
            // toggleButton.GetComponentInChildren<Text>().text = "Show";
        }
        else
        {
            // Show password: Set InputField to standard text mode
            _password.contentType = TMP_InputField.ContentType.Standard;
            _eyeButton.sprite = _closedEye;
            // toggleButton.GetComponentInChildren<Text>().text = "Hide";
        }

        isPasswordVisible = !isPasswordVisible;
        // Apply the change immediately
        _password.ForceLabelUpdate();
    }
#if UNITY_EDITOR

    #region TESTING, CAN BE DELETED LATER
    private const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
    // private const string letters = "abcdefghijklmnopqrstuvwxyz";

    // Call this function to generate a random email, password, and nickname
    public void GenerateRandomCredentials()
    {
        string email = GenerateRandomString(5) + "@" + GenerateRandomString(3) + ".com";
        string password = GenerateRandomString(8); // Password longer than 6 characters
        string nickname = GenerateRandomString(6); // Nickname with 6 random characters

        _email.text = email;
        _password.text = password;
        _nickname.text = nickname;
        // return (email, password, nickname);
    }

    // Helper function to generate a random string of specified length
    private string GenerateRandomString(int length)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            result.Append(chars[UnityEngine.Random.Range(0, chars.Length)]);
        }
        return result.ToString();
    }
    #endregion

#endif
}


