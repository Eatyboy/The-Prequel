using UnityEngine;
using UnityEngine.UI;

public class AbilityIndicator : MonoBehaviour
{
    public PlayerAbility ability;

    [Header("Refs")]
    [SerializeField] private Image cooldownIndicator;
    [SerializeField] private Image keybindIconDisabledOverlay;

    public void Update()
    {
        if (Player.Instance.abilities.abilities.TryGetValue(ability, out var abilityState)) {
            if (abilityState.isUnlocked)
            {
                cooldownIndicator.fillAmount = 1.0f - Mathf.Clamp01(abilityState.cooldownProportion);
                keybindIconDisabledOverlay.gameObject.SetActive(abilityState.onCooldown);
            }
        }
    }
}
