using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneProfile : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI _nicknameText;
    [SerializeField]
    private TextMeshProUGUI _emailText;

    [SerializeField]
    private Image _profilePicture;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        // _nicknameText.text = GameManager.Instance.GetNickname();
        // _emailText.text = GameManager.Instance.GetEmail();
    }
}
