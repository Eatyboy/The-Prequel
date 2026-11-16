using UnityEngine;

public class CoffinIndicator : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Coffin coffin;
    [SerializeField] private GameObject visuals;
    [SerializeField] private Transform arrow;

    [Header("Config")]
    [SerializeField] private Vector2 offsetFromBorder;
    [SerializeField] private Vector2 offsetFromCoffin;
    [SerializeField] private float baseArrowDistance;
    [SerializeField] private float arrowBobDistance;
    [SerializeField] private float arrowBobSpeed;

    private void Update()
    {
        if (Player.Instance.abilities.unlockedAbilities.HasFlag(coffin.unlocks))
        {
            visuals.SetActive(false);
        }
        else
        {
            visuals.SetActive(true);

            Vector2 coffinViewportPos = Camera.main.WorldToViewportPoint(coffin.transform.position);
            bool isCoffinVisible = coffinViewportPos.x > 0.0f && coffinViewportPos.x < 1.0f
                && coffinViewportPos.y > 0.0f && coffinViewportPos.y < 1.0f;

            if (isCoffinVisible)
            {
                transform.position = (Vector2)Camera.main.ViewportToScreenPoint(coffinViewportPos) + offsetFromCoffin 
                    + arrowBobDistance * Mathf.Sin(Time.time * arrowBobSpeed) * Vector2.up;
                arrow.localPosition = baseArrowDistance * Vector2.down;
                arrow.rotation = Quaternion.Euler(0, 0, -90);
            }
            else
            {
                Vector2 fromCenter = coffinViewportPos - new Vector2(0.5f, 0.5f);
                float scale = 0.5f / Mathf.Max(Mathf.Abs(fromCenter.x), Mathf.Abs(fromCenter.y));
                Vector2 viewportEdge = new Vector2(0.5f, 0.5f) + scale * fromCenter;
                Vector2 screenPos = Camera.main.ViewportToScreenPoint(viewportEdge);

                Vector2 boundedScreenPos = new(
                    Mathf.Clamp(screenPos.x, offsetFromBorder.x, Screen.width - offsetFromBorder.x),
                    Mathf.Clamp(screenPos.y, offsetFromBorder.y, Screen.height - offsetFromBorder.y)
                );
                transform.position = boundedScreenPos;

                Vector2 dir = new Vector2(fromCenter.x * Screen.width, fromCenter.y * Screen.height).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                arrow.SetLocalPositionAndRotation(baseArrowDistance * dir, Quaternion.Euler(0, 0, angle));
            }
        }
    }
}
