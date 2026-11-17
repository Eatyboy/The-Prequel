using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class Coffin : MonoBehaviour
{
    [SerializeField] private Interactable interactable;
    [SerializeField] private Animator animator;
    [SerializeField] private AbilityTutorial tutorial;

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
        tutorial.gameObject.SetActive(false);
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

        tutorial.gameObject.SetActive(true);
        tutorial.Init(() =>
        {
            Player.Instance.UnlockEverything();
            tutorial.gameObject.SetActive(false);
        });
    }
}
