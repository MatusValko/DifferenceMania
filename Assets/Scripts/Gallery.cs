using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gallery : MonoBehaviour
{
    public GameObject Content;
    [SerializeField] private Animator _collectionAnimator;
    [SerializeField] private IndependentImageReveal[] _collectionImages;

    [SerializeField] private Material material_0f;
    [SerializeField] private Material material_25f;
    [SerializeField] private Material material_5f;
    [SerializeField] private Material material_75f;
    [SerializeField] private Material material_1f;

    void OnEnable()
    {
        Content.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        SoundManager.PlaySound(SoundType.COLLECTION_START_OPEN_WINDOW);
        _collectionAnimator.SetTrigger("OpenCurtains");
        //play curtain sound
        _adjustImages();
    }

    private void _adjustImages()
    {
        Sprite[] _collectionItemSprites = GameManager.Instance.GetCollectionItemSprites();
        Sprite[] _collectionItemSpritesBlacknWhite = GameManager.Instance.GetCollectionItemSpritesBlacknWhite();

        int spriteLength = _collectionItemSprites.Length;
        if (spriteLength != _collectionImages.Length)
        {
            DebugLogger.LogError("Sprite Length is not same to GO in Collection shelves");
            return;
        }
        if (spriteLength != _collectionItemSpritesBlacknWhite.Length)
        {
            DebugLogger.LogError("Colorful sprites are not same length as blacknWhite sprites in Collection");
            return;
        }
        for (int i = 0; i < spriteLength; i++)
        {
            _collectionImages[i].SetImage(_collectionItemSprites[i], _collectionItemSpritesBlacknWhite[i]);

            //ASIGN RANDOM PARTS TO COLLECTION IMAGES, TODO REDO FROM BACKEND
            int modResult = i % 5;
            _collectionImages[i].AddPart(modResult);
            _setMaterialBasedOnParts(_collectionImages[i]);
            // _collectionImages[i].gameObject.name = $"OneGalleryImage_{i}";
        }
    }

    private void _setMaterialBasedOnParts(IndependentImageReveal image)
    {
        int parts = image.GetParts();
        DebugLogger.Log($"Setting material for image with parts: {parts}");
        switch (parts)
        {
            case 0:
                image.SetMaterial(material_0f);
                break;
            case 1:
                image.SetMaterial(material_25f);
                break;
            case 2:
                image.SetMaterial(material_5f);
                break;
            case 3:
                image.SetMaterial(material_75f);
                break;
            case 4:
                image.SetMaterial(material_1f);
                break;
            default:
                DebugLogger.LogError("Invalid parts value: " + parts);
                break;
        }
    }
}
