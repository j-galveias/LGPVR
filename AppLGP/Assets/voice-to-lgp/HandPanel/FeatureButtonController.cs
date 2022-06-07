using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>	
/// </summary>
public abstract class FeatureButtonController : ScrollButton, IPointerEnterHandler, IPointerExitHandler
{
    protected float selectDelay = 0.02f; //When set to 0, it appears to provide a 1 frame delay
    protected IEnumerator selectCoroutine;
    public string folder = "";

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectCoroutine != null)
            StopCoroutine(selectCoroutine);

        selectCoroutine = SelectButton();
        StartCoroutine(selectCoroutine);
    }

    protected abstract IEnumerator SelectButton();

    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(selectCoroutine);
    }



    protected string GetName()
    {
        return (folder == "" ? "" : folder+"/") +  GetComponentInChildren<Button>().transform.name;
    }
}
