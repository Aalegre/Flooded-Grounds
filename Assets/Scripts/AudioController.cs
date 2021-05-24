using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    public AudioClip[] audios;
    public AudioSource audioSource;
    public float minVolume = .5f;
    public float maxVolume = 1f;
    public float minPitch = .5f;
    public float maxPitch = 1.5f;
    public bool autoPlay = false;
    public bool is2D = true;

    private AudioListener listener;

    float currentMaxVolume;
    float currentVolume;
    float scaleVolume = 1;

    bool isFadingOut;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        listener = FindObjectOfType<AudioListener>();
        SetAudio();
    }

    void Update()
    {
        if (is2D && IsPlaying())
        {
            transform.position = new Vector3(transform.parent.transform.position.x, transform.parent.transform.position.y, listener.transform.position.z);
        }
        audioSource.volume = currentVolume * scaleVolume;
    }

    public void SetAudio()
    {
        audioSource.playOnAwake = autoPlay;
        if (audios.Length == 1)
            audioSource.clip = audios[0];
        else if(audios.Length > 1)
            audioSource.clip = audios[Random.Range(0, audios.Length - 1)];
        currentVolume = Random.Range(minVolume, maxVolume);
        currentMaxVolume = currentVolume;
        audioSource.volume = 0;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        Pause();
        if (autoPlay)
            Play();
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying || isFadingOut;
    }

    public bool IsFadingOut()
    {
        return isFadingOut;
    }

    public void SetVolumeScale(float volume_)
    {
        volume_ = Mathf.Clamp(volume_, 0, 1);
        scaleVolume = volume_;
    }
    public float GetVolumeScale()
    {
        return scaleVolume;
    }

    public void Play()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
        currentVolume = currentMaxVolume;
    }
    public void Pause()
    {
        if (audioSource.isPlaying)
            audioSource.Pause();
        currentVolume = 0;
    }
    public void Stop()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
        currentVolume = 0;
    }

    public void FadeIn(float time_ = 1)
    {
        StartCoroutine(FadeInCoroutine(time_));
    }
    public void FadeOut(float time_ = 1)
    {
        StartCoroutine(FadeOutCoroutine(time_));
    }

    IEnumerator FadeInCoroutine(float time_)
    {
        Play();
        currentVolume = 0;
        time_ = 1 / time_;
        while (currentVolume < currentMaxVolume)
        {
            currentVolume += Time.deltaTime * time_;
            yield return null;
        }
        currentVolume = currentMaxVolume;
        yield break;
    }
    IEnumerator FadeOutCoroutine(float time_)
    {
        isFadingOut = true;
        Play();
        time_ = 1 / time_;
        while (currentVolume > 0)
        {
            currentVolume -= Time.deltaTime * time_;
            yield return null;
        }
        currentVolume = 0;
        Pause();
        isFadingOut = false;
        yield break;
    }

    public void Instantiate(Transform parent = null, float vol = 1)
    {
        SetVolumeScale(vol);
        GameObject temp = new GameObject("Audio Instance");
        temp.transform.position = this.transform.position;
        if (parent)
            temp.transform.SetParent(parent);
        AudioSource tempAs = temp.AddComponent<AudioSource>();

        currentVolume = Random.Range(minVolume, maxVolume);
        currentMaxVolume = currentVolume;
        tempAs.volume = currentVolume * scaleVolume;

        if (audios.Length == 1)
            tempAs.clip = audios[0];
        else if (audios.Length > 1)
            tempAs.clip = audios[Random.Range(0, audios.Length - 1)];

        tempAs.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        tempAs.spatialBlend = audioSource.spatialBlend;
        tempAs.pitch = Random.Range(minPitch, maxPitch);
        tempAs.Play();
        Destroy(temp, tempAs.clip.length + 0.1f);
    }
}
