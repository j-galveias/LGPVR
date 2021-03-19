using UnityEngine;
using UnityEngine.UI;

public class SwitchToggle : MonoBehaviour {
   public RectTransform uiHandleRectTransform ;
//    public Color backgroundActiveColor ;
   public Color backgroundActiveColor ;

   RawImage backgroundImage, handleImage ;

   Color backgroundDefaultColor, handleDefaultColor ;

   Toggle toggle ;

   Vector2 handlePosition ;

   void Awake ( ) {
      toggle = GetComponent <Toggle> ( ) ;

      handlePosition = uiHandleRectTransform.anchoredPosition;

      backgroundImage = uiHandleRectTransform.parent.GetComponent <RawImage> ( ) ;
    //   handleImage = uiHandleRectTransform.GetComponent <RawImage> ( ) ;

      backgroundDefaultColor = backgroundImage.color ;
    //   handleDefaultColor = handleImage.color ;
    //   Debug.Log(handleDefaultColor);

      toggle.onValueChanged.AddListener (OnSwitch) ;

      if (toggle.isOn)
         OnSwitch (true) ;
   }

   void OnSwitch (bool on) {
      //uiHandleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition ; // no anim
      uiHandleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition;

      backgroundImage.color = on ? backgroundActiveColor : backgroundDefaultColor;


    //   handleImage.color = on ? handleActiveColor : handleDefaultColor ; // no anim
   }

   void OnDestroy ( ) {
      toggle.onValueChanged.RemoveListener(OnSwitch) ;
   }
}