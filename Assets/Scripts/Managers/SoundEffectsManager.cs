using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager Instance { get; private set;}
    [SerializeField] private AudioSource sfxObject;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void PlaySFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(sfxObject, spawnTransform.position, quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLenght = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLenght);
    }

    public void PlayRandomSFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        int rand = Random.Range(0, audioClip.Length);
        AudioSource audioSource = Instantiate(sfxObject, spawnTransform.position, quaternion.identity);

        audioSource.clip = audioClip[rand];

        audioSource.volume = volume;

        audioSource.Play();

        float clipLenght = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLenght);
    }
}
