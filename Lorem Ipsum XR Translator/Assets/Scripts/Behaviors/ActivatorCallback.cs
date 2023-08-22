using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calls an object to be activated or deactivated
/// </summary>
public class ActivatorCallback : MonoBehaviour
{
    public GameObject gameObject;

    private void Activate()
    {
        gameObject.SetActive(true);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
