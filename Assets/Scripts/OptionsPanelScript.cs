using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

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
    StartCoroutine(SendEmailCoroutine());
  }

  IEnumerator SendEmailCoroutine()
  {
    if (string.IsNullOrEmpty(emailInputField.text) || !System.Text.RegularExpressions.Regex.IsMatch(emailInputField.text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
    {
      Debug.LogWarning("Invalid email address.");
      yield break;
    }
    form = new WWWForm();
    form.AddField("email", emailInputField.text);
    using UnityWebRequest request = UnityWebRequest.Post(GameConstants.API_ADD_EMAIL, form);
    request.SetRequestHeader("Accept", "application/json");
    request.SetRequestHeader("Authorization", "Bearer " + GameManager.Instance.GetToken());
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    {
      Debug.LogError("Error sending email: " + request.error);
    }
    else
    {
      Debug.Log("Email sent successfully: " + request.downloadHandler.text);
      emailInputField.text = ""; // Clear the input field after sending
    }
  }
}