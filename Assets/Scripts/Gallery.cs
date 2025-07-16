using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gallery : MonoBehaviour
{
    public GameObject Content;
    [SerializeField] private Animator _collectionAnimator;
    [SerializeField] private IndependentImageReveal[] _collectionImages;


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
            // _collectionImages[i].gameObject.name = $"OneGalleryImage_{i}";
        }
    }
}
