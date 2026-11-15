using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TextMeshProUGUI tmp;

    [Header("Config")]
    [SerializeField] private Vector2 travel;
    [SerializeField] private Vector2 startOffset = Vector2.zero;
    [SerializeField] private float duration;
    [SerializeField] private Gradient startColor;
    [SerializeField] private Gradient endColor;
    [SerializeField] private Vector2 damageColorThresholds;
    [SerializeField] private Vector2 startScale;
    [SerializeField] private Vector2 endScale;
    [SerializeField] private AnimationCurve xCurve;
    [SerializeField] private AnimationCurve yCurve;
    [SerializeField] private AnimationCurve colorCurve;
    [SerializeField] private AnimationCurve opacityCurve;
    [SerializeField] private AnimationCurve scaleCurve;
    private float lifetime = 0;
    private float damage = 0;
    private Vector2 startPos;
    private bool initialized = false;

    private void Awake()
    {
        initialized = false;
        tmp.alpha = opacityCurve.Evaluate(0);
        transform.localScale = startScale;
    }

    public void Init(Vector2 startPos, float damage)
    {
        this.startPos = startPos + startOffset;
        tmp.text = damage.ToString();
        this.damage = damage;
        float normalizedDamage = Mathf.InverseLerp(damageColorThresholds.x, damageColorThresholds.y, damage);
        tmp.color = startColor.Evaluate(normalizedDamage);
        lifetime = 0.0f;
        initialized = true;
    }

    public void Update()
    {
        if (!initialized) return;

        lifetime += Time.deltaTime;
        if (lifetime < duration)
        {
            float t = lifetime / duration;
            float normalizeDamage = Mathf.InverseLerp(damageColorThresholds.x, damageColorThresholds.y, damage);
            transform.position = startPos + new Vector2(travel.x * xCurve.Evaluate(t), travel.y * yCurve.Evaluate(t));
            transform.localScale = Vector2.Lerp(startScale, endScale, scaleCurve.Evaluate(t));
            tmp.color = Color.Lerp(startColor.Evaluate(normalizeDamage), endColor.Evaluate(normalizeDamage), colorCurve.Evaluate(t));
            tmp.alpha = opacityCurve.Evaluate(t);
        }
        else
        {
            PopupManager.Instance.DespawnDamagePopup(this);
        }
    }
}
