using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class DeathScreen : Singleton<DeathScreen>
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private float fadeDuration = 1.5f;

    private float timer;

    public void OpenDeathScreen()
    {
        Pause.PauseGame(true);

        scoreText.text = $"Score: {Mathf.RoundToInt(ScoreManager.Instance.score)}";
        TimeSpan duration = TimeSpan.FromSeconds(ScoreManager.Instance.timeSurvived);
        timeText.text = $"Time Survived: {duration:mm\\:ss}";
        canvasGroup.alpha = 0.0f;

        FadeIn();
    }

    public void Retry()
    {
        Pause.PauseGame(false);
        Persistent.GoToGame();
    }

    public void End()
    {
        Pause.PauseGame(false);
        Persistent.GoToTitleScreen();
    }
    
    private async void FadeIn()
    {
        timer = 0.0f;
        try
        {
            while (timer < fadeDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, Mathf.Clamp01(timer / fadeDuration));
                timer += Time.unscaledDeltaTime;
                await UniTask.NextFrame(cancellationToken: this.GetCancellationTokenOnDestroy());
            }
        } catch {
        }
        canvasGroup.alpha = 1.0f;
        timer = 0.0f;
    }
}
