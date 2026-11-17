using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    public float score = 0;
    public float timeSurvived = 0;

    [Header("Config")]
    [SerializeField] private int scorePerSecond;
    [SerializeField] private int scorePerKill;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        score = 0;
        timeSurvived = 0;

        Player.Instance.onKill += ScoreOnKill;
    }

    private void Update()
    {
        score += Time.deltaTime * scorePerSecond;
        timeSurvived += Time.deltaTime;
    }

    public void ScoreOnKill()
    {
        score += scorePerKill;
    }
}
