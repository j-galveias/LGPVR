using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationButton : MonoBehaviour
{
    public Manager manager;
    public bool playAnimation = true;
    
    public void PlaySign()
    {
        if (playAnimation)
            StartCoroutine(manager.PlaySign(name));
    }
}
