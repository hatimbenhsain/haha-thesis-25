using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rustlable Data", menuName = "ScriptableObjects", order = 2)]
public class SpriteCollection: ScriptableObject
{
    public Sprite[] sprites;
}
