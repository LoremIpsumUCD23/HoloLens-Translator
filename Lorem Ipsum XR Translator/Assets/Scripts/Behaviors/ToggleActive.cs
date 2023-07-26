using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleActive : MonoBehaviour
{
    public GameObject gameobject;

    public void ToggleActiveState ()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
    
}
