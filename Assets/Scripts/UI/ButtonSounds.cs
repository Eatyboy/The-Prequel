using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSounds : MonoBehaviour, IPointerEnterHandler
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayClick);
    }

    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveListener(PlayClick);
    }

    private void PlayClick()
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.uiClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.IsInteractable()) return;
        AudioManager.instance.PlayOneShot(AudioManager.instance.uiHover);
    }
}
