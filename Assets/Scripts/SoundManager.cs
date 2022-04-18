using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioSource musicAudioSource = null;
    [SerializeField] private AudioSource soundEffectsAudioSource = null;
    [SerializeField] private AudioClip backgroundMusic;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        musicAudioSource.loop = true;
        musicAudioSource.Stop();
    }

    public void PlaySoundEffectAtPosition(AudioClip audioClip, Vector3 pos, float volumeScale = 1f)
    {
        soundEffectsAudioSource.transform.position = pos;
        soundEffectsAudioSource.PlayOneShot(audioClip, volumeScale);
    }
}
