using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DamageIndicator : MonoBehaviour
{
    public float intensity = 0f;

    PostProcessVolume postProcessVolume;
    Vignette vignette;

    private void Start()
    {
        postProcessVolume = GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings<Vignette>(out vignette);

    }
}
