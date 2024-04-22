using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RefillLives : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI _livesAmount;
    // Start is called before the first frame update


    void OnEnable()
    {
        _livesAmount.text = $"{UI_Manager.Instance.GetLives()}";
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
