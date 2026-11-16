using Unity.Tutorials.Editor;
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
                Vector2 displacement = coffin.transform.position - Player.Instance.transform.position;
                Vector2 pointOnCamRectCircumscribedCircle = 0.5f * Mathf.Sqrt(2) * displacement.normalized;
                Vector2 normalizedViewportPosition = new(
                    0.5f + Mathf.Clamp(pointOnCamRectCircumscribedCircle.x, -0.5f, 0.5f),
                    0.5f + Mathf.Clamp(pointOnCamRectCircumscribedCircle.y, -0.5f, 0.5f)
                );
                Vector2 screenPos = Camera.main.ViewportToScreenPoint(normalizedViewportPosition);

                Vector2 boundedScreenPos = new(
                    screenPos.x - ((Mathf.Sign(displacement.x) < 0) ? -offsetFromBorder.x : offsetFromBorder.x),
                    screenPos.y - ((Mathf.Sign(displacement.y) < 0) ? -offsetFromBorder.y : offsetFromBorder.y)
                );
                transform.position = boundedScreenPos;

                ;
                float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
                arrow.SetLocalPositionAndRotation(arrow.localPosition = baseArrowDistance * displacement.normalized, Quaternion.Euler(0, 0, angle));
            }
        }
    }
}
