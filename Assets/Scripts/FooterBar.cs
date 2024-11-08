using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FooterBar : MonoBehaviour
{
    // Start is called before the first frame update

    public Image premium;
    public Image home;
    public Image gallery;

    private Image[] buttons = new Image[3];

    public Sprite pressedButtonSprite;

    public Sprite[] defaultButtons = new Sprite[3];


    [SerializeField]
    public int SelectedScreen;

    public GameObject premiumWindow;
    public GameObject galleryWindow;

    public GameObject topCanvas;
    void Start()
    {
        buttons[0] = premium;
        buttons[1] = home;
        buttons[2] = gallery;

        // SELECT HOME
        SelectButton(1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectButton(int index)
    {
        if (SelectedScreen == index)
        {
            return;
        }
        //DAJ SELECTNUTY BUTTON NA DEFAULT - NESTLACENE
        buttons[SelectedScreen].sprite = defaultButtons[SelectedScreen];

        SetActiveOff(index);
        //ZMEN STLACENE TLACIDLO SPRITE
        buttons[index].sprite = pressedButtonSprite;
        SelectedScreen = index;
    }

    private void SetActiveOff(int index)
    {
        if (index == 0)
        {
            premiumWindow.SetActive(true);
            galleryWindow.SetActive(false);
            topCanvas.SetActive(false);
        }
        else if (index == 1)
        {
            premiumWindow.SetActive(false);
            galleryWindow.SetActive(false);
            topCanvas.SetActive(true);

        }
        else
        {
            premiumWindow.SetActive(false);
            galleryWindow.SetActive(true);
            topCanvas.SetActive(false);
        }
    }
}
