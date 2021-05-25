using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
            [Range(0, 1)] public float scale = 1;
        }
        public PhysicMaterial[] mats;
        public AudioClipCustom[] audios;
        [Range(0, 1)]
        public float AudioScale = 1;
    }
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
