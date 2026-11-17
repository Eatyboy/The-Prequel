using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    private static ScreenFader instance;

    [SerializeField] private CanvasGroup fadeBackground;
    public const float DEFAULT_FADE_DURATION = 0.5f;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        DontDestroyOnLoad(gameObject);

        instance.fadeBackground.alpha = 0.0f;
        instance.fadeBackground.blocksRaycasts = false;
    }

    public static async UniTask FadeIn(float duration = DEFAULT_FADE_DURATION, CancellationToken ct = default)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            instance.fadeBackground.alpha = Mathf.Clamp01(1.0f - t);
            await UniTask.NextFrame(cancellationToken: ct);
        }
        instance.fadeBackground.alpha = 0.0f;
        instance.fadeBackground.blocksRaycasts = false;
    }

    public static async UniTask FadeOut(float duration = DEFAULT_FADE_DURATION, CancellationToken ct = default)
    {
        instance.fadeBackground.blocksRaycasts = true;
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            instance.fadeBackground.alpha = Mathf.Clamp01(t);
            await UniTask.NextFrame(cancellationToken: ct);
        }
        instance.fadeBackground.alpha = 1.0f;
    }
}
