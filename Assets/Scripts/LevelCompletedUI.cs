using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletedUI : MonoBehaviour
{
    [SerializeField]
    string mouseHoverSoundName = "ButtonHover";

    [SerializeField]
    string buttonPressSoundName = "ButtonPress";

    AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No Audiomanager on the scene!");
        }
    }

    public void Quit()
    {
        audioManager.PlaySound(buttonPressSoundName);
        Debug.Log("APPLICATION QUIT!");
        Application.Quit();
    }

    public void Retry()
    {
        audioManager.PlaySound(buttonPressSoundName);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMouseOver()
    {
        audioManager.PlaySound(mouseHoverSoundName);
    }
}
