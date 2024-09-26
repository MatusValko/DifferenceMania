using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ranking : MonoBehaviour
{
    [Header("Images")]
    [SerializeField]
    private Sprite _active;
    [SerializeField]
    private Sprite _deactive;
    [SerializeField]
    private Sprite _greenBG;

    [Header("Buttons")]
    [SerializeField]
    private Image _globalImage;
    [SerializeField]
    private TextMeshProUGUI _globalText;
    [SerializeField]
    private Image _seasonImage;
    [SerializeField]
    private TextMeshProUGUI _seasonText;
    [Header("Entries")]

    [SerializeField]
    private Image _firstProfilePicture;
    [SerializeField]
    private TextMeshProUGUI _firstNickname;
    [SerializeField]
    private Image _secondProfilePicture;
    [SerializeField]
    private TextMeshProUGUI _secondNickname;
    [SerializeField]
    private Image _thirdProfilePicture;
    [SerializeField]
    private TextMeshProUGUI _thirdNickname;
    [SerializeField]
    private RankingOneEntry[] _entryList;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void LoadGlobal()
    {
        _globalImage.sprite = _active;
        _globalText.color = Color.black;

        _seasonImage.sprite = _deactive;
        _seasonText.color = Color.white;
        int index = 1;
        foreach (var item in _entryList)
        {
            string nickname = "Fero" + index;
            item.SetUpEntry(index.ToString(), nickname, "69");
            UpdateTopkars(index, nickname);

            index++;
        }
    }

    public void LoadSeason()
    {
        _seasonImage.sprite = _active;
        _seasonText.color = Color.black;

        _globalImage.sprite = _deactive;
        _globalText.color = Color.white;
        int index = 1;
        foreach (var item in _entryList)
        {
            string nickname = "Peto" + index;
            item.SetUpEntry(index.ToString(), nickname, "420");
            UpdateTopkars(index, nickname);

            index++;
        }
    }

    private void UpdateTopkars(int rank, string nickname, int PFP = 0)
    {
        if (rank == 1)
        {
            _firstNickname.text = nickname;
        }
        else if (rank == 2)
        {
            _secondNickname.text = nickname;
        }
        else if (rank == 3)
        {
            _thirdNickname.text = nickname;
        }
    }
    void OnEnable()
    {
        LoadGlobal();

    }
}
