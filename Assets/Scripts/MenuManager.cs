using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    string buttonHoverSound = "ButtonHover";

    [SerializeField]
    string buttonPressSound = "ButtonPress";

    AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.instance;
        if(audioManager == null)
        {
            Debug.LogError("No AudioManager found!");
        }
    }

    public void StartGame()
    {
        audioManager.PlaySound(buttonPressSound);
        SceneManager.LoadScene("Level1");
    }

    public void QuitGame()
    {
        audioManager.PlaySound(buttonPressSound);
        Debug.Log("Quitting the Game.");
        Application.Quit();
    }

    public void OnMouseOver()
    {
        audioManager.PlaySound(buttonHoverSound);
    }
}
