using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTest : MonoBehaviour
{
    public GameObject Caption;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            Vector3 origin = Camera.main.transform.position;
            Vector3 direction = ScreenToWorldRay(Input.mousePosition, Screen.width, Screen.height, Camera.main.cameraToWorldMatrix, Camera.main.projectionMatrix.inverse, origin);
            Ray ray = new Ray(origin, direction);
            Debug.DrawRay(origin, direction);
            Physics.Raycast(ray, out hit);
            Debug.Log(hit.transform.gameObject.name);
            Caption.transform.position = hit.point;

        }        
    }
    public Vector3 ScreenToWorldRay(Vector2 screenPosition, float screenWidth, float screenHeight, 
        Matrix4x4 cameraToWorld, Matrix4x4 cameraProjectionInverse, Vector3 cameraOrigin)
    {
        screenPosition = new Vector2(screenPosition.x, screenHeight - screenPosition.y);

        Vector4 clipSpace = new Vector4(((screenPosition.x * 2.0f) / screenWidth) - 1.0f, (1.0f - (2.0f * screenPosition.y) / screenHeight), 0.0f, 1.0f);

        Vector4 viewSpace = cameraProjectionInverse * clipSpace;
        viewSpace /= viewSpace.w;

        Vector4 worldSpace = cameraToWorld * viewSpace;

        Vector4 modifiedWorldSpace = worldSpace;
        modifiedWorldSpace.x -= cameraOrigin.x;
        modifiedWorldSpace.y -= cameraOrigin.y;
        modifiedWorldSpace.z -= cameraOrigin.z;

        Vector3 worldDirection = Vector3.Normalize(modifiedWorldSpace);

        return worldDirection;
    }

}
