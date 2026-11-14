using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Coffin : MonoBehaviour
{
    public PlayerAbility unlocks;
    public bool isOpened = false;

    private void Start()
    {
        isOpened = false;
    }

    private void Awake()
    {
        GetComponent<Interactable>().OnInteract += OpenCoffin;
    }

    private async void OpenCoffin()
    {
        if (isOpened)
        {
            Debug.Log($"You already unlocked {unlocks}");
            return;
        }

        Player.Instance.abilities.Unlock(unlocks);
        isOpened = true;
        Debug.Log($"Unlocked {unlocks}!");
    }
}
