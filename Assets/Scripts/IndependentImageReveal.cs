using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IndependentImageReveal : MonoBehaviour
{
    private Material materialInstance;

    // [Range(0f, 1f)]
    // public float progress = 0f; // Progress (0 = none, 1 = full)

    //three material variables for the shader
    [SerializeField] private Material material_0f;
    [SerializeField] private Material material_03f;
    [SerializeField] private Material material_06f;
    [SerializeField] private Material material_1f;
    [SerializeField] private Image _unlockedImage;

    void OnEnable()
    {
        _pickRandomMaterial();
    }

    void Awake()
    {

    }

    private void _pickRandomMaterial()
    {
        // Pick a random material for the shader
        int randomMaterialIndex = Random.Range(0, 4);
        switch (randomMaterialIndex)
        {
            case 0:
                materialInstance = new Material(material_0f);
                break;
            case 1:
                materialInstance = new Material(material_03f);
                break;
            case 2:
                materialInstance = new Material(material_06f);
                break;
            case 3:
                materialInstance = new Material(material_1f);
                break;
        }

        _unlockedImage.material = materialInstance;
    }

    void Update()
    {
        // Set the progress value for this object
        // propertyBlock.SetFloat("_Progress", progress);
        // materialInstance.SetFloat("_Progress", progress);
        // Apply the property block to the object's renderer
        // objectRenderer.SetPropertyBlock(propertyBlock);
    }
}