using UnityEngine;

public class ErrorWindow : MonoBehaviour
{

    //go to main manu scene
    public void GoToMainMenu()
    {
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
