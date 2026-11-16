using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : Singleton<Pause>
{
    public bool IsPaused;

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PauseGame(!IsPaused);
        }
    }

    public void PauseGame(bool isPaused)
    {
        IsPaused = isPaused;
        if (isPaused)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }
}
