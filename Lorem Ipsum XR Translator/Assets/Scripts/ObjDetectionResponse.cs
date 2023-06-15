using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Category {
    public string name;
    public float score;
}

[System.Serializable]
public class ObjDetectionResponse
{
    public Category[] categories;
    public string requestId;
    public int height;
    public int width;
    public string format;
    public string modelVersion;
}
