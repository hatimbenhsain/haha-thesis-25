using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    public PathNodeType type;
    public float pauseLength=0f;
}

public enum PathNodeType{
    Continue,
    Pause
}