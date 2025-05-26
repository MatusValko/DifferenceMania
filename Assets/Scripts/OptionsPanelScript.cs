using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class OptionsPanelScript : MonoBehaviour
{
  [Header("Email Settings")]
  public string emailAddress = "";
  public WWWForm form = new WWWForm();

  public TextMeshProUGUI emailInputField;

  [Header("UI Elements")]
  public Button sendEmailButton;

  void Start()
  {
    if (sendEmailButton != null)
      sendEmailButton.onClick.AddListener(SendEmail);
  }

  public void SendEmail()
  {
    if (string.IsNullOrEmpty(emailInputField.text) || !System.Text.RegularExpressions.Regex.IsMatch(emailInputField.text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
    {
      Debug.LogWarning("Invalid email address.");
      return;
    }
    form.AddField("email", emailInputField.text);
    using UnityWebRequest www = UnityWebRequest.Post(GameConstants.API_ADD_EMAIL, form);
    www.SetRequestHeader("Accept", "application/json");
    
  }
}