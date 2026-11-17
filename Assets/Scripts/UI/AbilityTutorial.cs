using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class AbilityTutorial : MonoBehaviour
{
    public static bool IsTutorialActive;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private float animSpeed = 1.0f;
    [SerializeField] private float animScaleFactor = 1.2f;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float minTimeSinceEnabledToContinue = 2.0f;

    private IDisposable eventListener;
    private Vector3 initialScale;
    private float timer = 0.0f;
    private float timeSinceEnabled = 0.0f;
    private Action callback;
    private bool isFading = false;

    private void Awake()
    {
        initialScale = continueText.transform.localScale;
    }

    private void OnEnable()
    {
        eventListener = InputSystem.onAnyButtonPress.Call(ctrl => Close());
        timeSinceEnabled = 0.0f;
    }

    public void Init(Action callback)
    {
        this.callback = callback;
        IsTutorialActive = true;
        Pause.PauseGame(true);
        FadeIn();
        timer = 0.0f;
    }
    
    private async void FadeIn()
    {
        isFading = true;
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
            Close();
        }
        canvasGroup.alpha = 1.0f;
        timer = 0.0f;
        isFading = false;
    }

    private async void FadeOut()
    {
        isFading = true;
        timer = 0.0f;
        try
        {
            while (timer < fadeDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, Mathf.Clamp01(timer / fadeDuration));
                timer += Time.unscaledDeltaTime;
                await UniTask.NextFrame(cancellationToken: this.GetCancellationTokenOnDestroy());
            }
        } catch {
            return;
        }
        canvasGroup.alpha = 0.0f;
        timer = 0.0f;
        callback.Invoke();
        isFading = false;
    }

    private void OnDisable()
    {
        eventListener?.Dispose();
    }

    private void Close()
    {
        if (timeSinceEnabled < minTimeSinceEnabledToContinue || isFading) return;

        FadeOut();
        Pause.PauseGame(false);
        IsTutorialActive = false;
    }

    private void Update()
    {
        if (!isFading)
        {
            float t = Mathf.Sin(timer * animSpeed);
            continueText.transform.localScale = Vector3.Lerp(initialScale, animScaleFactor * initialScale, t * t);

            timer += Time.unscaledDeltaTime;
        }

        timeSinceEnabled += Time.unscaledDeltaTime;
    }
}
