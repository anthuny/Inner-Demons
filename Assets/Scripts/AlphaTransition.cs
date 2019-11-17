using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaTransition : MonoBehaviour
{
    public bool canIncrease;
    public bool canDecrease;
    private void Update()
    {
        FadeIn();
        FadeOut();
    }

    void FadeIn()
    {
        if (canIncrease)
        {
            GetComponent<CanvasGroup>().alpha += Time.deltaTime * FindObjectOfType<DialogueManager>().alphaIncSpeed;
        }
    }
    void FadeOut()
    {
        if (canDecrease)
        {
            GetComponent<CanvasGroup>().alpha -= Time.deltaTime * FindObjectOfType<DialogueManager>().alphaDecSpeed;
        }
    }
}
