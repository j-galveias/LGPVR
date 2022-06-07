using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class HandButtonController : FeatureButtonController, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    protected override IEnumerator SelectButton()
    {
        yield return new WaitForSeconds(selectDelay);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
