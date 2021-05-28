using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceSoundManager : MonoBehaviour
{
	public AudioListener audioListener;

	[System.Serializable]
	public class SoundtrackGroup
	{
		[Tooltip("Should be 2D")]
		public AudioSource source = null;

		public AudioClip clip = null;

		[Tooltip("Music pitch value.")]
		[Range(-3f, 3f)]
		public float pitch = 1f;

		[Tooltip("Music volume value.")]
		[Range(0, 1f)]
		public float maxVolume = 1f;

		[Tooltip("Music fade in time value.")]
		[Min(0)]
		public float fadeInTime = 1f;

		public void SetUp()
		{
			source.pitch = pitch;
			source.volume = maxVolume;
			source.clip = clip;
			source.loop = true;
		}
		public void Play()
		{
			if (!source.isPlaying)
			{
				source.pitch = pitch;
				source.Play();
			}
		}
		public void Pause()
		{
			if (source.isPlaying)
				source.Pause();
		}
		public void Stop()
		{
			if (source.isPlaying)
				source.Stop();
		}
	}

	[Header("Soundtrack")]
	public SoundtrackGroup[] soundtracks;

	[System.Serializable]
	public class AmbienceGroup
    {
		[Tooltip("Should be 2D")]
		public AudioSource source = null;

		[Tooltip("Ambience clips to play in random time")]
		public List<AudioClip> clips = null;

		[Tooltip("Minimum time value (in seconds).")]
		[Min(0f)]
		public float minimumTime = 0.5f;
		[Tooltip("Maximum time value (in seconds).")]
		[Min(0f)]
		public float maximumTime = 1.0f;

		[Tooltip("Minimum volume value.")]
		[Range(0f, 1f)]
		public float minimumVolume = 0.5f;
		[Tooltip("Maximum volume value.")]
		[Range(0f, 1f)]
		public float maximumVolume = 1.0f;

		[Tooltip("Minimum pitch value.")]
		[Range(-3f, 3f)]
		public float lowPitchRange = 0.85f;
		[Tooltip("Maximum pitch value.")]
		[Range(-3f, 3f)]
		public float highPitchRange = 1.05f;

		[Range(0, 1)]
		public float randomPanning = 0.5f;

		[HideInInspector]
		public Coroutine coroutine = null;
		[HideInInspector]
		public float randomTime = 0.0f;

		public void Validate()
		{
			if (maximumVolume < minimumVolume)
			{
				maximumVolume = minimumVolume + 0.01f;
			}

			if (highPitchRange < lowPitchRange)
			{
				highPitchRange = lowPitchRange + 0.01f;
			}
		}
	}

	[Header("Ambience")]
	public AmbienceGroup[] ambiences;

	public static AmbienceSoundManager instance = null;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}

		else if (instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
		if (!audioListener)
			audioListener = FindObjectOfType<AudioListener>();
	}

	void Start()
    {
		// default music clip
		SetUpMusic();
		PlayEffects();
    }

    #region Music
	public void SetUpMusic()
    {
        foreach (SoundtrackGroup soundtrack in soundtracks)
        {
			soundtrack.SetUp();
			soundtrack.Play();
			AudioSourceFade(soundtrack.source, 0, soundtrack.maxVolume, soundtrack.fadeInTime);
        }
	}
	public void PauseMusic()
	{
		foreach (SoundtrackGroup soundtrack in soundtracks)
		{
			soundtrack.Pause();
		}
	}
	public void StopMusic()
	{
		foreach (SoundtrackGroup soundtrack in soundtracks)
		{
			soundtrack.Stop();
		}
	}
	public void FadeInMusic(float time = 1f, float wait = 0f)
	{
		foreach (SoundtrackGroup soundtrack in soundtracks)
		{
			soundtrack.Play();
			AudioSourceFade(soundtrack.source, 0, soundtrack.maxVolume, time, wait);
		}
	}
	public void FadeOutMusic(float time = 1f, float wait = 0f, bool stopAfter = true)
	{
		foreach (SoundtrackGroup soundtrack in soundtracks)
		{
			AudioSourceFade(soundtrack.source, soundtrack.source.volume, 0, time, wait, stopAfter);
		}
	}

	#endregion

	#region Ambience effects
	public void PlayEffects()
	{
		foreach (AmbienceGroup ambience in ambiences)
		{
			ambience.coroutine = StartCoroutine(RandomPlay(ambience));
		}
	}

    public void StopEffects()
	{
		foreach (AmbienceGroup ambience in ambiences)
		{
			StopCoroutine(ambience.coroutine);
			ambience.source.Stop();
		}
	}

	private IEnumerator RandomPlay(AmbienceGroup ambience)
	{
		while (true)
		{
			ambience.source.panStereo = Random.Range(-ambience.randomPanning, ambience.randomPanning);
			ambience.randomTime = Random.Range(ambience.minimumTime, ambience.maximumTime);

			ambience.source.clip = ambience.clips[Random.Range(0, ambience.clips.Count)];

			ambience.source.pitch = Random.Range(ambience.lowPitchRange, ambience.highPitchRange);
			ambience.source.volume = Random.Range(ambience.minimumVolume, ambience.maximumVolume);

			ambience.source.Play();
			yield return new WaitForSeconds(ambience.randomTime);
		}
	}
    #endregion
	public void AudioSourceFade(AudioSource source, float from, float to, float time = 1, float wait = 0, bool stopAfter = false)
    {
		StartCoroutine(AudioSourceFade_coroutine(source, from, to, time, wait, stopAfter));
    }
	IEnumerator AudioSourceFade_coroutine(AudioSource source, float from, float to, float time = 1, float wait = 0, bool stopAfter = false)
    {
		source.volume = from;
		float counter = wait;
		while(counter > 0)
        {
			counter -= Time.deltaTime;
			yield return null;
        }
		counter = 0;
		while(counter < 1)
        {
			counter += Time.deltaTime / time;
			source.volume = Mathf.Lerp(from, to, counter);
        }
		source.volume = to;
		if (stopAfter)
			source.Stop();
    }

    private void OnValidate()
    {
        foreach (AmbienceGroup ambience in ambiences)
        {
			ambience.Validate();
        }
	}
}
