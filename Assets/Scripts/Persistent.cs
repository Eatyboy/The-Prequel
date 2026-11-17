using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Persistent : MonoBehaviour
{
    public static Persistent instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static async void GoToTitleScreen()
    {
        await ScreenFader.FadeOut();
        AudioManager.instance.StopMusic();
        await SceneManager.LoadSceneAsync("Menu");
        await ScreenFader.FadeIn();
    }

    public static async void GoToGame()
    {
        await ScreenFader.FadeOut();
        await SceneManager.LoadSceneAsync("Main");
        AudioManager.instance.PlayMusic(AudioManager.instance.mainTheme);
        await ScreenFader.FadeIn();
    }
}
