using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ProfileOneAvatarImage : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _avatarImage;
    [SerializeField] private GameObject _ribbonCoins;
    [SerializeField] private TextMeshProUGUI _coinsPriceText;
    [SerializeField] private GameObject _ribbonAD;
    [SerializeField] private GameObject _selectedText;



    public void SetAvatarImage(Sprite avatarSprite)
    {
        _avatarImage.sprite = avatarSprite;
    }
    public void SetAvatarToBeSelected()
    {
        _selectedText.SetActive(true);
    }

    public void SetAvatarToOwned()
    {
        _ribbonCoins.SetActive(false);
        _ribbonAD.SetActive(false);
    }

    public void SetAvatarBuyWCoins(int price)
    {
        _ribbonCoins.SetActive(true);
        _ribbonAD.SetActive(false);
        // Set the price text to the cost of the avatar
        _coinsPriceText.text = price.ToString() + " <sprite=0>";
    }

    public void SetAvatarBuyWithAD()
    {
        _ribbonCoins.SetActive(false);
        _ribbonAD.SetActive(true);
    }
}
