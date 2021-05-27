using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class MixerValueBlend
{
    public AudioMixer mixer;
    public string name;
    public AnimationCurve values = AnimationCurve.Linear(0, 0, 1, 1);
    public void Update(float T)
    {
        mixer.SetFloat(name, values.Evaluate(T));
    }
}
public class CustomAudioArea : MonoBehaviour
{
    public CustomVolumeGroup volume;
    public MixerValueBlend[] blends;
    // Start is called before the first frame update
    void Awake()
    {
        volume.Setup();
    }

    // Update is called once per frame
    void Update()
    {
        volume.UpdateVolumes();
        foreach (MixerValueBlend blend in blends)
        {
            blend.Update(volume.blendTimed);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        volume.DrawGizmos();
    }
#endif
}
