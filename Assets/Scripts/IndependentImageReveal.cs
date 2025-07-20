using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IndependentImageReveal : MonoBehaviour
{
    private Material materialInstance;
    [SerializeField] private const int MAXPARTSINCOLLECTION = 4; // Number of materials to choose from
    [SerializeField] private int obtainedParts = 0; // Number of parts obtained
    [SerializeField] private Image _unlockedImage;
    [SerializeField] private Image _lockedImage;
    [SerializeField] private GameObject _showHowManyParts;
    [SerializeField] private Button _showHowManyPartsButton;

    //start
    private void Awake()
    {
        _showHowManyPartsButton.onClick.AddListener(() => _showHowManyPartsFunction());
    }

    private void _showHowManyPartsFunction()
    {
        if (_showHowManyParts.activeSelf)
        {
            return;
        }
        _showHowManyParts.SetActive(true);
    }

    void OnEnable()
    {
        _pickRandomMaterial();
    }
    private void _pickRandomMaterial()
    {
        // Pick a random material for the shader
        int randomMaterialIndex = Random.Range(0, 4);
        _unlockedImage.material = materialInstance;
    }



    public void AddPart(int parts)
    {
        obtainedParts += parts;
        if (obtainedParts > MAXPARTSINCOLLECTION)
        {
            obtainedParts = MAXPARTSINCOLLECTION; // Cap at max parts
        }
        UpdateText(); // Update the text display
        // Update the material based on the number of parts obtained
    }

    //set material for the image
    public void SetMaterial(Material material)
    {
        materialInstance = material;
    }
    public void UpdateText()
    {
        string icon = "<sprite=\"puzzle\" index=0>";
        _showHowManyParts.GetComponentInChildren<TextMeshProUGUI>().text = $"{obtainedParts}/{MAXPARTSINCOLLECTION}{icon}";
    }
    public int GetParts()
    {
        return obtainedParts;
    }

    public void SetImage(Sprite unlocked, Sprite locked)
    {
        _lockedImage.sprite = locked;
        _unlockedImage.sprite = unlocked;
        _lockedImage.SetNativeSize();
    }
}
