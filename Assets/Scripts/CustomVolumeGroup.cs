using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

[System.Serializable] public class CustomVolumeGroup
{
    [System.Serializable] public class CustomVolume
    {
        public Vector3 position;
        public float radius;
        public float sqrDistance;
    }
    public CustomVolume[] volumes;

    public Camera cam;
    public float blendDistance = 10;

    [Header("Readonly")]
    public bool inside = false;
    public float distance = 0;
    [Range(0, 1)] public float blend = 0;
    public int nearestVolume;

    public void Setup()
    {
        if (!cam)
            cam = Camera.main;
    }

    public void UpdateVolumes()
    {
        int tempNearestVolume = -1;
        for (int i = 0; i < volumes.Length; i++)
        {
            volumes[i].sqrDistance = (volumes[i].position - cam.transform.position).sqrMagnitude;
            if (tempNearestVolume < 0 || volumes[i].sqrDistance < volumes[tempNearestVolume].sqrDistance)
            {
                tempNearestVolume = i;
            }
        }
        nearestVolume = tempNearestVolume;
        if (nearestVolume >= 0)
        {
            distance = Vector3.Distance(volumes[nearestVolume].position, cam.transform.position) - volumes[nearestVolume].radius;
            blend = Mathf.InverseLerp(blendDistance, 0, distance);
            inside = distance <= 0;
        }
        else
        {
            distance = 0;
            inside = false;
        }
    }

#if UNITY_EDITOR
    public void DrawGizmos()
    {
        Camera tempcam = cam;
        cam = Camera.current;
        UpdateVolumes();
        cam = tempcam;
        CustomVolume[] tempVolumes = volumes.OrderBy(t => t.sqrDistance).ToArray();
        for (int i = tempVolumes.Length - 1; i >= 0; i--)
        {
            Gizmos.color = new Color(1, 1, 0, 1);
            Gizmos.DrawWireSphere(tempVolumes[i].position, tempVolumes[i].radius);
            Gizmos.color = new Color(1, 1, 0, .5f);
            Gizmos.DrawSphere(tempVolumes[i].position, tempVolumes[i].radius);
            Gizmos.DrawWireSphere(tempVolumes[i].position, tempVolumes[i].radius + blendDistance);
        }
    }
#endif
}
