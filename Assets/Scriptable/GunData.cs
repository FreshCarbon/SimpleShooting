using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableData/GunData", fileName = "Gun Data")]
public class GunData : ScriptableObject
{
    public AudioClip shootClip;
    public AudioClip reloadClip;

    public float damage = 25;

    //public int startAmmoRemain = 100;
    //public int maxCapacity = 100;
    public int magCapacity = 25;

    public float timeBetFire = 0.12f;

    public float reloadTime = 1.8f;
}
