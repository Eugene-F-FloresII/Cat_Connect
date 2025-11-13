using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    // Called when the player clicks "Start Game"
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    // Called when the player clicks "Options"
    public void OpenOptions()
    {
        Debug.Log("Options Menu Opened");
        // You can later show an options panel here
    }

    // Called when the player clicks "Quit Game"
    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
