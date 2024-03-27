using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FooterBar : MonoBehaviour
{
    // Start is called before the first frame update

    public Image premium;
    public Image home;
    public Image quests;

    private Image[] buttons = new Image[3];

    public Sprite pressedButtonSprite;

    public Sprite[] defaultButtons = new Sprite[3];


    [SerializeField]
    public int selected;

    public GameObject premiumWindow;
    public GameObject questsWindow;

    public GameObject topCanvas;
    void Start()
    {
        buttons[0] = premium;
        buttons[1] = home;
        buttons[2] = quests;
        selected = 0;
        // SELECT HOME
        SelectButton(1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectButton(int index)
    {
        if (selected == index)
        {
            return;
        }
        //DAJ SELECTNUTY BUTTON NA DEFAULT - NESTLACENE
        buttons[selected].sprite = defaultButtons[selected];


        SetActiveOff(index);
        //ZMEN STLACENE TLACIDLO SPRITE
        buttons[index].sprite = pressedButtonSprite;
        selected = index;


    }

    private void SetActiveOff(int index)
    {
        if (index == 0)
        {
            premiumWindow.SetActive(true);
            questsWindow.SetActive(false);
            topCanvas.SetActive(false);
        }
        else if (index == 1)
        {
            premiumWindow.SetActive(false);
            questsWindow.SetActive(false);
            topCanvas.SetActive(true);

        }
        else
        {
            premiumWindow.SetActive(false);
            questsWindow.SetActive(true);
            topCanvas.SetActive(false);

        }

    }

}
