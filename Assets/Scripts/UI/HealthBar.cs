using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Image fill;
    [SerializeField] private Image laggedFill;

    [Header("Config")]
    [SerializeField] private float fillLagDuration = 0.3f;
    [SerializeField] private AnimationCurve fillLagCurve;

    private CancellationTokenSource cts;

    private void Start()
    {
        fill.fillAmount = 1;
        laggedFill.fillAmount = 1;
    }

    public void Init(float proportion)
    {
        fill.fillAmount = proportion;
        laggedFill.fillAmount = proportion;
    }

    public async void SetHealth(float proportion)
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = new();

        float oldFillAmount = fill.fillAmount;
        fill.fillAmount = proportion;

        float timer = 0.0f;
        while (timer < fillLagDuration)
        {
            timer += Time.deltaTime;

            float t = fillLagCurve.Evaluate(Mathf.Clamp01(timer / fillLagDuration));
            laggedFill.fillAmount = Mathf.Lerp(oldFillAmount, fill.fillAmount, t);

            // This is so we don't get an error when it gets cancelled
            try
            {
                await UniTask.NextFrame(cts.Token);
            }
            catch 
            { 
                return; 
            }
        }

        laggedFill.fillAmount = fill.fillAmount;
    }

    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}
