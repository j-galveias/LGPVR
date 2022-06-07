using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScalePercentage : MonoBehaviour
{
    public float ratio = 1 / 3;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
}

    void Update()
    {
        rectTransform.sizeDelta = new Vector2(Screen.width * ratio, rectTransform.sizeDelta.y);
    }
}
