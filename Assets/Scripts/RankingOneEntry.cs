using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingOneEntry : MonoBehaviour
{
    [Header("Data")]
    [SerializeField]
    private TextMeshProUGUI _numberText;
    [SerializeField]
    private TextMeshProUGUI _nickNameText;
    [SerializeField]
    private TextMeshProUGUI _scoreText;
    [SerializeField]
    private Image _profilePicture;


    public void SetUpEntry(string number, string nickaname, string score)
    {
        _nickNameText.text = nickaname;
        _scoreText.text = score;
        _numberText.text = number;
        _profilePicture.sprite = null;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
