using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Difference : MonoBehaviour
{
    public float x;
    public float y;
    public float width;
    public float height;

    public int id;

    // [SerializeField] private Camera mainCamera;

    //ASI MOZE BYT VOID KEDZE SA NEPOUZIVA NAVRATOVA HODNOTA
    public static Difference CreateDifference(float x, float y, float width, float height, int id, GameObject parent)
    {
        // Create a new GameObject
        GameObject differenceObject = new GameObject("Difference");

        // Add the Difference component
        Difference difference = differenceObject.AddComponent<Difference>();

        differenceObject.transform.SetParent(parent.transform);

        // Initialize properties
        difference.x = x;
        difference.y = y;
        difference.width = width;
        difference.height = height;
        difference.id = id;

        difference.transform.localPosition = new Vector3(x, -y, -5);
        difference.transform.localScale = new Vector2(1, 1);


        // Add a BoxCollider2D component and set its size
        BoxCollider2D boxCollider = differenceObject.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(width, height);
        boxCollider.offset = new Vector2(width / 2, -height / 2);

        //Add tag Difference
        differenceObject.tag = "Difference";

        return difference;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButtonDown(0)) // Detect left mouse click
        // {
        //     mainCamera = Camera.main;
        //     Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        //     if (Physics.Raycast(ray, out RaycastHit hit))
        //     {
        //         Debug.Log(hit.collider.gameObject.name + " was clicked!");
        //         // Additional actions on the clicked object
        //     }
        // }

        // if (Input.GetMouseButtonDown(0)) // Detect left mouse click or tap
        // {
        //     Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

        //     if (hitCollider == null)
        //     {
        //         DifferencesManager.Instance.TakeLife();
        //         Debug.LogWarning("NO DIFFERENCE CLICKED!");
        //     }

        //     DifferencesManager.Instance.Clicked(id);

        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);
        //     if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        //     {
        //         Debug.LogWarning("Hit object: " + hit.collider.name);
        //     }
        //     else
        //     {
        //         Debug.Log("NO Hit object");

        //     }
        // }
    }

    private void OnMouseDown()
    {
        // if (EventSystem.current.IsPointerOverGameObject())
        // {
        //     // Do nothing if over a UI element
        //     return;
        // }
        // if (EventSystem.current.IsPointerOverGameObject())
        // {
        //     Debug.Log("Click blocked by UI element.");
        //     return;
        // }
        // Clicked();
    }



    public void Clicked()
    {
        DebugLogger.Log($"{id} was clicked!");
        DifferencesManager.Instance.Clicked(id);
    }

}
