using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioController))]
public class AudioThumper : MonoBehaviour
{
    public float MagnitudeScale = .1f;
    bool StartPlay = false;
    AudioController ac;
    Rigidbody rb;
    private void Awake()
    {
        ac = GetComponent<AudioController>();
        rb = GetComponent<Rigidbody>();
        if (!rb)
            StartPlay = true;
    }
    private void FixedUpdate()
    {
        if (!StartPlay && Time.time > 0.1f)
        {
            if(Mathf.Abs(rb.velocity.x) < 0.05f && Mathf.Abs(rb.velocity.y) < 0.05f && Mathf.Abs(rb.velocity.z) < 0.05f)
            {
                StartPlay = true;
            }
        }
    }
    public void Play(float magnitude_ = 1f)
    {
        if (StartPlay)
            ac.Instantiate(transform, magnitude_);
    }
    private void OnCollisionEnter(Collision collision_)
    {
        Play(collision_.relativeVelocity.magnitude * MagnitudeScale);
    }
    private void OnCollisionEnter2D(Collision2D collision_)
    {
        Play(collision_.relativeVelocity.magnitude * MagnitudeScale);
    }
}
