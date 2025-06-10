using UnityEngine;
using UnityEngine.UI;


public class ProfileOneAvatarImage : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _avatarImage;
    [SerializeField] private GameObject _ribbonCoins;
    [SerializeField] private GameObject _ribbonAD;
    [SerializeField] private GameObject _selectedText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetAvatar(Sprite avatarSprite, bool isSelected, bool hasCoins, bool hasAD)
    {
        _avatarImage.sprite = avatarSprite;
        _selectedText.SetActive(isSelected);
        _ribbonCoins.SetActive(hasCoins);
        _ribbonAD.SetActive(hasAD);

        // Optionally, you can change the background color based on selection
        _backgroundImage.color = isSelected ? Color.yellow : Color.white; // Example color change
    }

    public void SetAvatarImage(Sprite avatarSprite)
    {
        _avatarImage.sprite = avatarSprite;
    }
}
