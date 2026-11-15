using UnityEngine;

public class HeadsUpDisplay : Singleton<HeadsUpDisplay>
{
    // Config
    public Vector2 Offset = Vector2.up;

    // Vars
    private Transform interactObject;

    // Refs
    [SerializeField] private RectTransform interactPrompt;

    private void Update()
    {
        if (interactObject == null) return;
        Vector2 interactObjectScreenPos = Camera.main.WorldToScreenPoint(interactObject.position + (Vector3)Offset);
        interactPrompt.position = interactObjectScreenPos;
    }

    public void Interact(Transform gameObject)
    {
        if (gameObject == null)
        {
            interactPrompt.gameObject.SetActive(false);
            return;
        }
        interactPrompt.gameObject.SetActive(true);
        interactObject = gameObject;
    }
}
