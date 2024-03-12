using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageComparer : MonoBehaviour
{
    // Start is called before the first frame update
    public Texture2D image1;
    public Texture2D image2;

    public Image imageinunity1;
    public Image imageinunity2;

    public float colorThreshold;

    void Start()
    {
    }

    float ColorDifference(Color color1, Color color2)
    {
        float deltaR = Mathf.Abs(color1.r - color2.r);
        float deltaG = Mathf.Abs(color1.g - color2.g);
        float deltaB = Mathf.Abs(color1.b - color2.b);
        float deltaA = Mathf.Abs(color1.a - color2.a);
        return deltaR + deltaG + deltaB + deltaA;
    }

    void firstComparison()
    {

        if (image1.width != image2.width || image1.height != image2.height)
        {
            Debug.LogError("Images must have the same dimensions!");
            return;
        }

        for (int y = 0; y < image1.height / 2; y++)
        {
            for (int x = 0; x < image1.width / 2; x++)
            {
                Color color1 = image1.GetPixel(x, y);
                Color color2 = image2.GetPixel(x, y);

                if (ColorDifference(color1, color2) > colorThreshold)
                {
                    image1.SetPixel(x, y, Color.red);
                    Debug.Log("SET PIXEL:" + x + y);
                    // GameObject differenceMarker = new GameObject("Difference_" + x + "_" + y);
                    // differenceMarker.transform.parent = transform;
                    // differenceMarker.transform.localPosition = new Vector3(x, y, 0);
                    // BoxCollider collider = differenceMarker.AddComponent<BoxCollider>();
                    // collider.size = new Vector3(0.1f, 0.1f, 0.1f); // Adjust size as needed
                }
                else
                {


                }
            }
        }
        Sprite sprite = Sprite.Create(image1, new Rect(0.0f, 0.0f, image1.width, image1.height), new Vector2(0.5f, 0.5f));
        imageinunity1.sprite = sprite;

    }


    void secondComparison()
    {
        // Načítanie obrázkov
        // var image1 = Resources.Load<Texture2D>("obrazok1");
        // var image2 = Resources.Load<Texture2D>("obrazok2");

        // Vytvorenie zoznamu rozdielov
        var differences = new List<Vector2>();

        // Porovnanie obrázkov
        for (int i = 0; i < image1.width; i++)
        {
            for (int j = 0; j < image1.height; j++)
            {
                // Ak sú pixely na oboch obrázkoch odlišné
                if (image1.GetPixel(i, j) != image2.GetPixel(i, j))
                {
                    // Pridajte rozdiel do zoznamu
                    differences.Add(new Vector2(i, j));
                }
            }
        }

        // Zobrazenie rozdielov
        for (int i = 0; i < differences.Count; i++)
        {
            // Nájdite pixel na obrázkoch
            var pixel1 = image1.GetPixel(Mathf.RoundToInt(differences[i].x), Mathf.RoundToInt(differences[i].y));
            // var pixel2 = image2.GetPixel(differences[i].x, differences[i].y);

            image2.SetPixel(Mathf.RoundToInt(differences[i].x), Mathf.RoundToInt(differences[i].y), pixel1);
            // Zobrazte rozdiel
            // Debug.DrawPixel(differences[i].x, differences[i].y, pixel1);

        };
        byte[] _bytes = image2.EncodeToPNG();
        var dirPath = Application.dataPath + "/SaveImages/";
        System.IO.File.WriteAllBytes(dirPath, _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + dirPath);

    }
    // Update is called once per frame
    void Update()
    {

    }
}
