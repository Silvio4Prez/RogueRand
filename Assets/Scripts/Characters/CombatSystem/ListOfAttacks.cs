using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListOfAttacks
{
    public string name;
    public Attacks attack;
    public float damage;
    public float atkRate;

    [Range(0,10)]
    public int maxChance;

}
