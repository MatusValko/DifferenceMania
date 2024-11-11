using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class DifferencesManager : MonoBehaviour
{



    [System.Serializable]
    private class Difference
    {
        public float x;
        public float y;
        public float width;
        public float height;
    }

    [System.Serializable]
    private class DifferencesData
    {
        public string image_id;
        public List<Difference> differences;
    }

    // Set the path to the JSON file/ ONLY FOR TESTING
    public string jsonFilePath = "/path/to/your/df.json";
    [SerializeField]
    private GameObject _parentObject;

    void Start()
    {
        // Load and parse the JSON data
        string jsonContent = File.ReadAllText(jsonFilePath);
        DifferencesData data = JsonUtility.FromJson<DifferencesData>(jsonContent);
        // Debug.Log(data.image_id);
        // Generate colliders
        foreach (var diff in data.differences)
        {
            CreateCollider(diff.x, diff.y, diff.width, diff.height);
        }
    }

    private void CreateCollider(float x, float y, float width, float height)
    {
        // Create a new GameObject
        GameObject colliderObject = new GameObject("Collider");

        if (_parentObject != null)
        {
            colliderObject.transform.SetParent(_parentObject.transform);
        }
        colliderObject.transform.localPosition = new Vector2(x, -y);
        colliderObject.transform.localScale = new Vector2(1, 1);


        // Add a BoxCollider2D component and set its size
        BoxCollider2D boxCollider = colliderObject.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(width, height);
        boxCollider.offset = new Vector2(width / 2, -height / 2);

    }


    // Update is called once per frame
    void Update()
    {

    }
}
