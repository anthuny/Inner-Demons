using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaTransition : MonoBehaviour
{
    public bool canIncrease;
    private void Update()
    {
        if (canIncrease)
        {
            GetComponent<CanvasGroup>().alpha += Time.deltaTime * FindObjectOfType<DialogueManager>().alphaIncSpeed;
        }
    }
}
