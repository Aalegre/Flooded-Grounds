using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(FirstPersonAIO))]
[DisallowMultipleComponent]
public class FirstPerson_AIO_Helper : MonoBehaviour
{
    [System.Serializable]
    public class AudioSurfaceGroup
    {
        [System.Serializable]
        public class AudioClipCustom
        {
            public AudioClip audio;
            [Range(0, 3)] public float scale = 1;
        }
        public PhysicMaterial[] mats;
        public AudioClipCustom[] audios;
        [Range(0, 1)]
        public float AudioScale = 1;
    }
    public enum WORLDMODE { NORMAL, LIMBO, HELL, COUNT}
    public WORLDMODE worldmode = WORLDMODE.NORMAL;
    WORLDMODE worldmodeLast = WORLDMODE.NORMAL;
    public AudioMixerSnapshot Normal;
    public ParticleSystem NormalParticles;
    ParticleSystem.EmissionModule NormalParticlesEmission;
    public AudioMixerSnapshot Limbo;
    public PostProcessVolume LimboVolume;
    public ParticleSystem LimboParticles;
    ParticleSystem.EmissionModule LimboParticlesEmission;
    public AudioMixerSnapshot Hell;
    public PostProcessVolume HellVolume;
    public ParticleSystem HellParticles;
    ParticleSystem.EmissionModule HellParticlesEmission;
    public FirstPersonAIO fp;
    public AudioController ac;
    public UnityEvent Footstep;
    [Range(0, 1)]
    public float FootstepVolume = .5f;
    public UnityEvent Jump;
    [Range(0, 1)]
    public float JumpVolume = .75f;
    public UnityEvent Land;
    [Range(0, 1)]
    public float LandVolume = 1f;
    public AudioSurfaceGroup[] surfaces;
    public AudioSurfaceGroup.AudioClipCustom[] fallbackaudios;
    RaycastHit hit;
    bool hitted;

    public Light spotlight;
    public AudioSurfaceGroup.AudioClipCustom lightToggle;
    private void Awake()
    {
        if (!fp)
            fp = GetComponent<FirstPersonAIO>();
        fp.Footstep = Footstep;
        fp.Footstep.AddListener(delegate { this.FootstepSound(); });
        fp.Jump = Jump;
        fp.Jump.AddListener(delegate { this.JumpSound(); });
        fp.Land = Land;
        fp.Land.AddListener(delegate { this.LandSound(); });
        spotlight.enabled = false;
        if (NormalParticles)
            NormalParticlesEmission = NormalParticles.emission;
        if (LimboParticles)
            LimboParticlesEmission = LimboParticles.emission;
        if (HellParticles)
            HellParticlesEmission = HellParticles.emission;
    }
    private void FixedUpdate()
    {
        hit = new RaycastHit();
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            hitted = true;
        }
        else
        {
            hitted = false;
        }
    }
    private void Update()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            spotlight.enabled = !spotlight.enabled;
            ac.audios = new AudioClip[] { lightToggle.audio };
            ac.Instantiate(transform, lightToggle.scale);
        }
        if (Input.GetButtonUp("Fire2"))
        {
            worldmode = (WORLDMODE)((((int)worldmode) + 1) % (int)WORLDMODE.COUNT);
        }
        if(worldmode != worldmodeLast)
        {
            switch (worldmode)
            {
                case WORLDMODE.NORMAL:
                    Normal.TransitionTo(1);
                    break;
                case WORLDMODE.LIMBO:
                    Limbo.TransitionTo(1);
                    break;
                case WORLDMODE.HELL:
                    Hell.TransitionTo(1);
                    break;
            }
            worldmodeLast = worldmode;
        }
        switch (worldmode)
        {
            case WORLDMODE.NORMAL:
                NormalParticlesEmission.enabled = true;
                LimboParticlesEmission.enabled = false;
                HellParticlesEmission.enabled = false;
                LimboVolume.weight -= Time.deltaTime;
                HellVolume.weight -= Time.deltaTime;
                break;
            case WORLDMODE.LIMBO:
                NormalParticlesEmission.enabled = false;
                LimboParticlesEmission.enabled = true;
                HellParticlesEmission.enabled = false;
                LimboVolume.weight += Time.deltaTime;
                HellVolume.weight -= Time.deltaTime;
                break;
            case WORLDMODE.HELL:
                NormalParticlesEmission.enabled = false;
                LimboParticlesEmission.enabled = false;
                HellParticlesEmission.enabled = true;
                LimboVolume.weight -= Time.deltaTime;
                HellVolume.weight += Time.deltaTime;
                break;
        }
        LimboVolume.weight = Mathf.Clamp01(LimboVolume.weight);
        HellVolume.weight = Mathf.Clamp01(HellVolume.weight);
    }
    public void JumpSound()
    {
        SurfaceSound(JumpVolume);
    }
    public void LandSound()
    {
        SurfaceSound(LandVolume);
    }
    public void FootstepSound()
    {
        SurfaceSound(FootstepVolume);
    }
    public void SurfaceSound(float volume = 1)
    {
        if (hitted)
        {
            foreach (var surface in surfaces)
            {
                if (surface.mats.Contains(hit.collider.sharedMaterial))
                {
                    int temp = Random.Range(0, surface.audios.Length);
                    ac.audios = new AudioClip[] { surface.audios[temp].audio };
                    ac.Instantiate(transform, volume * surface.AudioScale * surface.audios[temp].scale);
                    return;
                }
            }
            if (fallbackaudios.Length > 0)
            {
                int temp = Random.Range(0, fallbackaudios.Length);
                ac.audios = new AudioClip[] { fallbackaudios[temp].audio };
                ac.Instantiate(transform, volume * fallbackaudios[temp].scale);
            }
        }
    }
}
