using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIndicator : MonoBehaviour
{
    public PlayerAbility ability;

    [Header("Refs")]
    [SerializeField] private Image cooldownIndicator;
    [SerializeField] private Image keybindIconDisabledOverlay;
    [SerializeField] private Image abilityIcon;

    [Header("Config")]
    [SerializeField] private Color disabledIconColor;
    [SerializeField] private Vector2 startOffset; 
    [SerializeField] private float startAnimationDuration; 
    [SerializeField] private float startAnimationDip; 
    [SerializeField] private float startAnimationImpact; 

    private float AnimationEase(float t)
    {
        if (t < startAnimationImpact)
        {
            float u = t / startAnimationImpact;
            return u * u * u;
        }

        float v = (t - startAnimationImpact) / (1.0f - startAnimationImpact);
        return 1.0f + startAnimationDip * Mathf.Exp(-5.0f * v) * Mathf.Sin(v * Mathf.PI * 3.0f);
    }

    public async void DoStartAnimation()
    {
        float endY = transform.localPosition.y;

        float timer = 0.0f;
        while (timer < startAnimationDuration)
        {
            float t = AnimationEase(Mathf.Clamp01(timer / startAnimationDuration));
            float y = endY + startOffset.y * (1.0f - t);
            transform.localPosition = new(transform.localPosition.x, y);

            await UniTask.NextFrame();
            timer += Time.deltaTime;
        }
        transform.localPosition = new(transform.localPosition.x, endY);
    }

    public void Update()
    {
        if (Player.Instance.abilities.abilities.TryGetValue(ability, out var abilityState)) {
            if (abilityState.isUnlocked)
            {
                cooldownIndicator.fillAmount = 1.0f - Mathf.Clamp01(abilityState.cooldownProportion);
                keybindIconDisabledOverlay.gameObject.SetActive(abilityState.onCooldown);
                abilityIcon.color = abilityState.onCooldown ? disabledIconColor : Color.white;
            }
        }
    }
}
