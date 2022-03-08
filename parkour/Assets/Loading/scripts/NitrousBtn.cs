using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NitrousBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    
    public void OnPointerDown(PointerEventData eventData) {
        // if(NitrousIndicator.NitrousCount > 1)
        // {
            NitrousIndicator.Static.isNitrousOn = true;
        // }
    }


    public void OnPointerUp(PointerEventData eventData) {
        NitrousIndicator.Static.isNitrousOn = false;
        playerBIKEControl.isDoubleSpeed=1.0f;
    }
}
