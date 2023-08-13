using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScrollContent : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] private float speedTranslate = 0.1f;
    [SerializeField] [Range(0f, 1f)] private float delayOpenTranslate = 0.1f;

    private Image image;
    private float translateSlider = 0;
    private float slider = 0;

    private void Start() => image = GetComponent<Image>();

    private void FixedUpdate()
    {
        var num = Mathf.Abs(slider - translateSlider);
        if (num >= speedTranslate)
            slider = ClearLerp(slider, translateSlider, speedTranslate);
        else
            slider = translateSlider;

        image.fillAmount = Mathf.Clamp01(slider);
    }

    private float ClearLerp(float a, float b, float t)
    {
        if (a > b)
            return a - t;
        if (a < b)
            return a + t;

        if (Mathf.Abs(a - b) <= t)
            return b;

        return a;
    }

    public void OpenScroll()
    {
        slider = -delayOpenTranslate;
        translateSlider = 1f;
    }

    public void CloseScroll()
    {
        translateSlider = 0f;
    }
}
