using UnityEngine;
using UnityEngine.Pool;

public class PopupManager : Singleton<PopupManager>
{
    private ObjectPool<DamagePopup> damagePopupPool;
    [SerializeField] private Transform popupContainer;
    [SerializeField] private DamagePopup damagePopupPrefab;

    public override void Awake()
    {
        base.Awake();

        damagePopupPool = new(
            createFunc: () => Instantiate(damagePopupPrefab, popupContainer),
            actionOnGet: obj => { obj.gameObject.SetActive(true); },
            actionOnRelease: obj => { obj.gameObject.SetActive(false); },
            actionOnDestroy: OnDestroyDamagePopup,
            collectionCheck: true,
            defaultCapacity: 16,
            maxSize: 512
        );
    }

    public DamagePopup SpawnDamagePopup(Vector2 position, float damage)
    {
        var newPopup = damagePopupPool.Get();
        newPopup.Init(position, damage);
        return newPopup;
    }

    public void DespawnDamagePopup(DamagePopup damagePopup)
    {
        damagePopupPool.Release(damagePopup);
    }

    private void OnDestroyDamagePopup(DamagePopup damagePopup)
    {
        if (damagePopup == null) return;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            DestroyImmediate(damagePopup.gameObject);
            return;
        }
#endif

        Destroy(damagePopup.gameObject);
    }
}
