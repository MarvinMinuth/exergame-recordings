using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaderScreenManager : MonoBehaviour
{
    public static FaderScreenManager Instance;

    public event EventHandler OnFadedOut;

    [SerializeField] private bool fadeInOnStart;
    [SerializeField] private Transform faderScreen;
    [SerializeField] private float fadeDuration;
    [SerializeField] private Color faderColor;

    private MeshRenderer faderScreenMeshRenderer;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        faderScreenMeshRenderer = faderScreen.GetComponent<MeshRenderer>();
        if (fadeInOnStart)
        {
            FadeIn();
        }
        else
        {
            faderColor.a = 0f;
            faderScreenMeshRenderer.material.color = faderColor;
        }

    }
    
    public void FadeIn()
    {
        Fade(1, 0);
    }

    public void FadeOut()
    {
        Fade(0, 1);
    }

    public void Fade(float alphaIn, float alphaOut)
    {
        StartCoroutine(FadeRoutine(alphaIn, alphaOut));
    }

    public IEnumerator FadeRoutine(float alphaIn, float alphaOut)
    {
        float timer = 0f;
        while(timer <= fadeDuration)
        {
            Color newColor = faderColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut, timer/fadeDuration);

            faderScreenMeshRenderer.material.color = newColor;

            timer += Time.deltaTime;
            yield return null;
        }

        Color newColor2 = faderColor;
        newColor2.a = alphaOut;
        faderScreenMeshRenderer.material.color = newColor2;

        if(alphaOut == 1f)
        {
            OnFadedOut?.Invoke(this, EventArgs.Empty);
        }
    }
}
