using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CustomAudioAreaSnapshot : MonoBehaviour
{
    public CustomVolumeGroup volume;
    public AudioMixerSnapshot inside;
    public AudioMixerSnapshot middle;
    public AudioMixerSnapshot outside;
    public float transitionTime = 0.25f;
    // Start is called before the first frame update
    void Awake()
    {
        volume.Setup();
    }

    // Update is called once per frame
    void Update()
    {
        volume.UpdateVolumes();
        if(volume.blend >= 1)
        {
            inside.TransitionTo(transitionTime);
        }
        else if(volume.blend > 0)
        {
            middle.TransitionTo(transitionTime);
        }
        else
        {
            outside.TransitionTo(transitionTime);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        volume.DrawGizmos();
    }
#endif
}
