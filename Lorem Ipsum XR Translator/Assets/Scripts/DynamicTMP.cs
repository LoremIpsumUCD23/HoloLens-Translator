using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;

public class DynamicTMP : MonoBehaviour, IMixedRealityPointerHandler
{
    public GameObject textMeshProPrefab;

    public void Test()
    {
        Debug.Log("Click!");
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
    //    // Get the pointing position and direction
    //    Vector3 pointingPosition = eventData.Pointer.Result.Details.Point;
    //    Vector3 pointingDirection = eventData.Pointer.Result.Details.Normal;
    //
    //    GameObject textObject = Instantiate(textMeshProPrefab, pointingPosition, Quaternion.LookRotation(pointingDirection), transform);
    //    TextMeshPro textMeshPro = textObject.GetComponent<TextMeshPro>();
    //    textMeshPro.text = "Here!";
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }

    public void OnPointerClicked(MixedRealityPointerEventData eventData) { }
}
