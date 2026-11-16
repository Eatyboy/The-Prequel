using UnityEngine;

public class Coffin : MonoBehaviour
{
    [SerializeField] private Interactable interactable;

    public PlayerAbility unlocks;
    public bool isOpened = false;

    private void Start()
    {
        isOpened = false;
    }

    private void Awake()
    {
        if (interactable == null) interactable = GetComponentInChildren<Interactable>();
        interactable.OnInteract += OpenCoffin;
    }

    private async void OpenCoffin()
    {
        if (isOpened)
        {
            Debug.Log($"You already unlocked {unlocks}");
            return;
        }

        interactable.Enable(false);
        Player.Instance.abilities.Unlock(unlocks);
        isOpened = true;
        Debug.Log($"Unlocked {unlocks}!");
    }
}
