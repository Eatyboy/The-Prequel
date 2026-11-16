using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour
{
    private Material mat;
    private Health health;

    private float hp;
    [SerializeField] private float smoothingSpeed = 1f;

    private void Awake()
    {
        mat = GetComponent<Image>().material;
    }

    private void Start()
    {
        GetComponent<Image>().enabled = true;
        health = Player.Instance.GetComponent<Health>();
        hp = health.Percent;
    }

    private void Update()
    {
        float diff = Mathf.Pow(Mathf.Abs(hp - health.Percent) + 1, 3);

        if (hp < health.Percent)
        {
            hp = Mathf.Min(hp + smoothingSpeed * diff * Time.deltaTime, health.Percent);
        }
        else if (hp > health.Percent)
        {
            hp = Mathf.Max(hp - smoothingSpeed * diff * Time.deltaTime, health.Percent);
        }

        mat.SetFloat("_NoiseSmoothness", Mathf.Lerp(4, 0, hp));
        mat.SetFloat("_VignetteSmoothness", Mathf.Lerp(2, 0, hp));
    }
}
