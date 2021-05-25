using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class effectGroup
{
	[Range(0f, 10f)]
	public int priority = 0;
	public List<AudioClip> audioClips;
}

public class AmbienceSoundManager : MonoBehaviour
{
	public AudioListener audioListener;

	[Header("Ambience music")]
	[Tooltip("Should be 2D")]
	public AudioSource musicSource = null;

	public AudioClip currentMusicClip = null;

	[Tooltip("Music pitch value.")]
	[Range(-3f, 3f)]
	public float musicPitch = 1f;	
	
	[Tooltip("Music fade in time value.")]
	[Min(0)]
	public float musicFadeInTime = 1f;

	[Header("Ambience effects")]
	[Tooltip("Should be 2D")]
	public AudioSource ambienEffectSource = null;

	[Tooltip("Ambience clips to play in random time")]
	public List<effectGroup> effectGroups = null;
	private List<effectGroup> finalEffectGroups = new List<effectGroup>();

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

	[Range(0,1)]
	public float randomPanning = 0.5f;

	private Coroutine ambienceCoroutine = null;
	private float randomTime = 0.0f;

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
		musicSource.clip = currentMusicClip;
		musicSource.loop = true;
		FadeInMusic(musicFadeInTime);
	}
	public void PlayMusic()
    {
        if (!musicSource.isPlaying)
        {
			musicSource.volume = 1f;
			musicSource.pitch = musicPitch;
			musicSource.Play();
        }
    }

	public void PlayMusic(AudioClip clip)
    {
		if (!musicSource.isPlaying)
		{ 
			musicSource.clip = currentMusicClip = clip;
			musicSource.volume = 1f;
			musicSource.pitch = musicPitch;
			musicSource.Play();
        }
			
    }

	public void PauseMusic()
    {
		if(musicSource.isPlaying)
			musicSource.Pause();
    }

	public void StopMusic()
    {
		if(musicSource.isPlaying)
			musicSource.Stop();
    }

	public void FadeInMusic(float time = 1f)
	{
		StartCoroutine(FadeInCoroutine(time));
	}
	public void FadeOutMusic(float time = 1f)
	{
		StartCoroutine(FadeOutCoroutine(time));
	}

	IEnumerator FadeInCoroutine(float time)
	{
		PlayMusic();
		musicSource.volume = 0f;
		time = 1 / time;
		while (musicSource.volume < 1f)
		{
			musicSource.volume += Time.deltaTime * time;
			yield return null;
		}
		musicSource.volume = 1f; ;
		yield break;
	}

	IEnumerator FadeOutCoroutine(float  time)
	{
		time = 1 / time;
		while (musicSource.volume > 0f)
		{
			musicSource.volume -= Time.deltaTime * time;
			yield return null;
		}
		musicSource.volume = 0f;
		PauseMusic();
		yield break;
	}

	#endregion

	#region Ambience effects
	public void PlayEffects()
    {

		if(finalEffectGroups.Count < 1)
        {
			foreach(effectGroup group in effectGroups)
            {
				for (int i = 0; i < group.priority; i++)
                {
					finalEffectGroups.Add(group);
                }
            }
        }

		if(ambienEffectSource != null)
			ambienceCoroutine = StartCoroutine(RandomPlay());
	}

    public void StopEffects()
	{
		if (ambienEffectSource.isPlaying)
		{
			ambienEffectSource.Stop();
			ambienceCoroutine = null;
		}
	}

	private IEnumerator RandomPlay()
	{
		while (true)
		{
			ambienEffectSource.panStereo = Random.Range(-randomPanning, randomPanning);
			randomTime = Random.Range(minimumTime, maximumTime);


			effectGroup selectedGroup = finalEffectGroups[Random.Range(0, finalEffectGroups.Count)];
			ambienEffectSource.clip = selectedGroup.audioClips[Random.Range(0, selectedGroup.audioClips.Count)];

			ambienEffectSource.pitch = Random.Range(lowPitchRange, highPitchRange);
			ambienEffectSource.volume = Random.Range(minimumVolume, maximumVolume);

			ambienEffectSource.Play();
			yield return new WaitForSeconds(randomTime);
		}
	}
    #endregion

    private void OnValidate()
    {
		musicSource.pitch = musicPitch;

		if (maximumVolume < minimumVolume)
        {
			maximumVolume = minimumVolume + 0.1f;
		}

		if(highPitchRange < lowPitchRange)
		{
			highPitchRange = lowPitchRange + 0.1f;
		}
	}
}
