using UnityEngine;

public class Coffin : MonoBehaviour
{
    [SerializeField] private Interactable interactable;
    [SerializeField] private Animator animator;

    public PlayerAbility unlocks;
    public bool isOpened = false;

    private static readonly int Closed = Animator.StringToHash("Closed");
    private static readonly int Opening = Animator.StringToHash("Opening");
    private static readonly int OpenWithStake = Animator.StringToHash("OpenWithStake");
    private static readonly int OpenWithoutStake = Animator.StringToHash("OpenWithoutStake");

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
        isOpened = true;
        Player.Instance.OpenCoffin();

        animator.Play(Opening, 0, 0);
    }

    public async void OnCoffinOpen()
    {
        animator.Play(OpenWithoutStake, 0, 0);

        await Player.Instance.GainPower(unlocks);
    }
}
