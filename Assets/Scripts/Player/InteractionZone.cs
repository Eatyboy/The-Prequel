using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractionZone : MonoBehaviour
{
    public static int InteractionLayer => LayerMask.NameToLayer("Interaction");

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TryGetInteractable(collision, out var interactable))
        {
            Player.Instance.AddInteractable(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (TryGetInteractable(collision, out var interactable))
        {
            Player.Instance.RemoveInteractable(interactable);
        }
    }

    private static bool TryGetInteractable(Collider2D collision, out Interactable interactable)
    {
        interactable = null;
        if (collision.gameObject.layer != InteractionLayer) return false;
        return collision.TryGetComponent(out interactable);
    }
}
