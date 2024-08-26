using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CoinShineMaterial : MonoBehaviour
{
    [SerializeField]
    private Material _mat;

    void Start()
    {
        // _mat = gameObject.GetComponent<Renderer>().material;
        _mat = GetComponent<Image>().material;
        XRay();
    }

    void XRay()
    {

        _mat.SetFloat("_ShineWidth", 0.2f); //This sets the Standard Shaders Rendering mode to transparent
        _mat.SetFloat("_ShineLocation", 0.5f);
        _mat.SetColor("_Tint", Color.white);
        Debug.Log("SET FLOAT AND COLOR");
    }
}
