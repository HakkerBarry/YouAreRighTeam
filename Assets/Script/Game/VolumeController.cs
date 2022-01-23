using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.LookDev;
using UnityEngine.Rendering;
using UnityEngine;
using DG.Tweening;

public class VolumeController : MonoBehaviour
{
    [SerializeField, Range(0.01f, 0.2f)] float sceneSwitchSpeed = 0.05f;
    private Volume mVolume;
    FilmGrain grain;
    ChromaticAberration aberration;
    ColorAdjustments adjust;
    private bool transform = false;
    
    private void Start()
    {
        mVolume = this.GetComponent<Volume>();
        mVolume.profile.TryGet<FilmGrain>(out grain);
        mVolume.profile.TryGet<ChromaticAberration>(out aberration);
        mVolume.profile.TryGet<ColorAdjustments>(out adjust);
    }
    private void Update()
    {
        if (transform)
        {
            float lerpValue = grain.intensity.value;
            grain.intensity.value = lerpValue < 0.71f ? lerpValue + sceneSwitchSpeed : lerpValue;
            lerpValue = aberration.intensity.value;
            aberration.intensity.value = lerpValue < 0.55f ? lerpValue + sceneSwitchSpeed : lerpValue;
        }
        else
        {
            float lerpValue = grain.intensity.value;
            grain.intensity.value = lerpValue > 0.0f ? lerpValue - sceneSwitchSpeed : lerpValue;
            lerpValue = aberration.intensity.value;
            aberration.intensity.value = lerpValue > 0.0f ? lerpValue - sceneSwitchSpeed : lerpValue;
        }
    }
    public void Transform()
    {
        transform = !transform;
    }
    
}
