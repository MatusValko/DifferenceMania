using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ProfileOneAvatarImage : MonoBehaviour
{
    [SerializeField] private int _avatarIndex; // Reference to the avatar GameObject
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _avatarImage;
    [SerializeField] private GameObject _ribbonCoins;
    [SerializeField] private TextMeshProUGUI _coinsPriceText;
    [SerializeField] private GameObject _ribbonAD;
    [SerializeField] private GameObject _selectedText;
    [SerializeField] private Color32 _selectedColor = new Color32(254, 210, 25, 255); // Text to show if the avatar is owned
    [SerializeField] private bool _selected = false; // Default color for the background image
    [SerializeField] private bool _buywCoins = false; // Default color for the background image
    [SerializeField] private bool _buywAD = false; // Default color for the background image
    [SerializeField] private bool _owned = false; // Default color for the background image
    public void SetAvatarImage(Sprite avatarSprite)
    {
        _avatarImage.sprite = avatarSprite;
    }
    //set avatar index
    public void SetAvatarIndex(int index)
    {
        _avatarIndex = index;
    }
    public bool IsSelected()
    {
        return _selected;
    }
    public bool IsBuyWithCoins()
    {
        return _buywCoins;
    }
    public bool IsBuyWithAD()
    {
        return _buywAD;
    }
    public bool IsOwned()
    {
        return _owned;
    }
    public int GetAvatarIndex()
    {
        return _avatarIndex;
    }
    public void SetAvatarBackgroundImage(Sprite backgroundSprite)
    {
        _backgroundImage.sprite = backgroundSprite;
    }
    public void SetAvatarToBeSelected()
    {
        _selectedText.SetActive(true);
        SetAvatarBackgroundImage(GameManager.Instance.GetProfileBackgroundSprite(-1)); // Set the background image to the selected sprite
        // _backgroundImage.color = _selectedColor;
        // _selected = true;
        _setState(0); // Set the state to selected
    }

    public void SetAvatarToOwned()
    {
        _ribbonCoins.SetActive(false);
        _ribbonAD.SetActive(false);
        _selectedText.SetActive(false);
        _setState(1); // Set the state to owned
        SetAvatarBackgroundImage(GameManager.Instance.GetProfileBackgroundSprite(_avatarIndex - 1));
    }

    public void SetAvatarBuyWCoins(int price)
    {
        _ribbonCoins.SetActive(true);
        _ribbonAD.SetActive(false);
        // Set the price text to the cost of the avatar
        _coinsPriceText.text = price.ToString() + " <sprite=0>";
        _setState(2); // Set the state to buy with coins
    }

    public void SetAvatarBuyWithAD()
    {
        _ribbonCoins.SetActive(false);
        _ribbonAD.SetActive(true);
        _setState(3); // Set the state to buy with AD
    }

    private void _setState(int state)
    {
        //selected state
        if (state == 0)
        {
            _selected = true;
            _buywCoins = false;
            _buywAD = false;
            _owned = false;
        }
        //owned state
        else if (state == 1)
        {
            _selected = false;
            _buywCoins = false;
            _buywAD = false;
            _owned = true;
        }
        //buy with coins state
        else if (state == 2)
        {
            _selected = false;
            _buywCoins = true;
            _buywAD = false;
            _owned = false;
        }
        //buy with AD state
        else if (state == 3)
        {
            _selected = false;
            _buywCoins = false;
            _buywAD = true;
            _owned = false;
        }
        else
        {
            Debug.LogError("Invalid state: " + state);
            return;
        }
    }



}
