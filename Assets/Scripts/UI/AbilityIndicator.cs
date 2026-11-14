using UnityEngine;
using UnityEngine.UI;

public class AbilityIndicator : MonoBehaviour
{
    public PlayerAbility ability;

    [Header("Refs")]
    [SerializeField] private Image cooldownIndicator;

    public void Update()
    {
        if (Player.Instance.abilities.abilities.TryGetValue(ability, out var abilityState)) {
            if (abilityState.isUnlocked)
            {
                cooldownIndicator.fillAmount = Mathf.Clamp01(abilityState.cooldownProportion);
            }
        }
    }
}
