using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    public EventInstance musicEventInstance;
    public EventInstance ambienceEventInstance;

    [Header("Music")]
    public EventReference mainTheme;

    [Header("SFX")]
    public EventReference uiClick;
    public EventReference uiHover;
    public EventReference batSwarm;
    public EventReference clawSwipe;
    public EventReference openCoffin;
    public EventReference enemyHit;
    public EventReference heartbeat;
    public EventReference stakeStab;
    public EventReference unlockAbility;
    public EventReference slicingWhirl;
    public EventReference bloodSpike;
    public EventReference stunFlash;
    public EventReference reviveOffCooldown;
    public EventReference death;
    public EventReference deathRefusal;
    public EventReference crossbowFire;
    public EventReference chomp;
    public EventReference running;
    public EventReference playerHurt;

    [Header("Ambience")]

    [Header("Volume")]
    [Range(0, 1)] public float masterVolume { get; set; }
    [Range(0, 1)] public float sfxVolume { get; set; }
    [Range(0, 1)] public float musicVolume { get; set; }
    [Range(0, 1)] public float ambienceVolume { get; set; }

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;
    private Bus ambienceBus;

    [HideInInspector] public bool areBussesInitialized = false;
    private bool isMusicPlaying = false;

    private const float DEFAULT_VOLUME = 0.5f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);

        LoadBusses(this.GetCancellationTokenOnDestroy()).Forget();
    }

    void Update()
    {
        if (areBussesInitialized)
        {
            masterBus.setVolume(masterVolume);
            sfxBus.setVolume(sfxVolume);
            musicBus.setVolume(musicVolume);
            ambienceBus.setVolume(ambienceVolume);
        }
    }

    public async UniTask LoadBusses(CancellationToken ct = default)
    {
        await UniTask.WaitUntil(() => RuntimeManager.HaveAllBanksLoaded, cancellationToken: ct);

        masterBus = RuntimeManager.GetBus("bus:/");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");

        if (!areBussesInitialized)
        {
            masterVolume = DEFAULT_VOLUME;
            sfxVolume = DEFAULT_VOLUME;
            musicVolume = DEFAULT_VOLUME;
            ambienceVolume = DEFAULT_VOLUME;
        }

        masterBus.setVolume(masterVolume);
        sfxBus.setVolume(sfxVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);

        areBussesInitialized = true;
    }

    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance newInstance = RuntimeManager.CreateInstance(eventReference);
        return newInstance;
    }

    public void StopMusic()
    {
        musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        isMusicPlaying = false;
    }

    public void PauseUnpause()
    {
        musicEventInstance.getPaused(out bool isMusicPaused);
        musicEventInstance.setPaused(!isMusicPaused);
    }

    public void PlayMusic(EventReference music)
    {
        InitializeMusic(music, this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask InitializeMusic(EventReference music, CancellationToken ct = default)
    {
        await UniTask.WaitUntil(() => areBussesInitialized, cancellationToken: ct);

        if (isMusicPlaying) StopMusic();
        musicEventInstance = CreateEventInstance(music);
        musicEventInstance.start();
        isMusicPlaying = true;
    }
}
