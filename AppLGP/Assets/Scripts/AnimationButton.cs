using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationButton : MonoBehaviour
{
    public Manager manager;
    
    public void PlaySign()
    {
        manager.PlaySign(name);
    }
}
