using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioController))]
public class AudioThumper : MonoBehaviour
{
    AudioController ac;
    private void Awake()
    {
        ac = GetComponent<AudioController>();
    }
    public void Play(float magnitude_ = 1f)
    {
        //Debug.Log("Playing collision magnitude: " + magnitude_);
        ac.SetVolumeScale(magnitude_);
        ac.SetAudio();
        ac.Play();
    }
    private void OnCollisionEnter(Collision collision_)
    {
        Play(collision_.relativeVelocity.magnitude);
    }
    private void OnCollisionEnter2D(Collision2D collision_)
    {
        Play(collision_.relativeVelocity.magnitude);
    }
}
