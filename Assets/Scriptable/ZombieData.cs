using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableData/Zombie Data", fileName = "Zombie Data")]
public class ZombieDatas : ScriptableObject
{
    public float health = 100f;
    public float damage = 20f;
    public float speed = 10f;
    public Color skinColor = Color.white;
}