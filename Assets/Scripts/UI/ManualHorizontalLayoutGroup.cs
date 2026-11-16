using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ManualHorizontalLayoutGroup : UIBehaviour
{
    [SerializeField] private float spacing = 10.0f;
    [SerializeField] private float animationSpeed = 10.0f;

    private readonly List<RectTransform> children = new();
    private readonly Dictionary<RectTransform, float> targetX = new();

    protected override void Awake()
    {
        base.Awake();
        ApplyLayout();
    }

    private void OnTransformChildrenChanged()
    {
        ApplyLayout();
    }

    [ContextMenu("Apply Layout")]
    public void ApplyLayout()
    {
        children.Clear();

        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf) continue;

            RectTransform rt = child as RectTransform;
            children.Add(rt);
        }

        foreach (var stale in targetX.Keys.Except(children).ToList()) targetX.Remove(stale);

        RecalculateTargets();
    }

    private void RecalculateTargets()
    {
        if (children.Count == 0) return;

        float totalWidth = 0.0f;
        foreach (var child in children) totalWidth += child.rect.width;
        totalWidth += spacing * (children.Count - 1);

        float x = -totalWidth * 0.5f;

        foreach (var child in children)
        {
            float w = child.rect.width;
            float target = x + w * 0.5f;
            x += w + spacing;

            bool isNew = !targetX.ContainsKey(child);
            targetX[child] = target;
            if (isNew) child.anchoredPosition = new(target, child.anchoredPosition.y);
        }
    }

    private void Update()
    {
        float t = 1.0f - Mathf.Exp(-animationSpeed * Time.unscaledDeltaTime);

        foreach (var child in children)
        {
            float newX = Mathf.Lerp(child.anchoredPosition.x, targetX[child], t);
            child.anchoredPosition = new(newX, child.anchoredPosition.y);
        }
    }
}
