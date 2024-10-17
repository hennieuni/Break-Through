using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakthrough : MonoBehaviour
{
    public string nameBT;
    public int levelBT;
    public int currentHP;
    public int maxHP;
    public int resistance;
    public int damage;
    public int speed;
    public string moveIDs;
    public string ability;
    public string spawnLocations;
    public string school;
    public int evolveLvl;
    public string discription;
    public string spriteLocation;

    public bool TakeDamage(int dmg){
        currentHP-= dmg;

        if (currentHP <=0){
            return true;
        }else{
            return false;
        }
    } 
}
