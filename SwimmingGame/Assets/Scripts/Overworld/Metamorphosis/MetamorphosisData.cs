using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Metamorphosis Data", menuName = "ScriptableObjects/Metamorphosis Data", order = 2)]
public class MetamorphosisData : ScriptableObject
{
    public Sprite[] sprites;
    public GameObject product;
}