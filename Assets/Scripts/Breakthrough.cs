using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakthrough : MonoBehaviour
{
    public int btID;
    public string nameBT;
    public int levelBT;
    public int currentHP;
    public int maxHP;
    public int resistance;
    public int damage;
    public int speed;
    public Move[] moveIDs = new Move[4];
    public string ability;
    public string spawnLocations;
    public string school;
    public int evolveLvl;
    public string discription;
    public string spriteLocation;
    public string question;
    public string option1, option2, option3, option4;
    public int correctOption;

     // Copy constructor to create a deep copy of Breakthrough
    public Breakthrough(Breakthrough original) {
        this.btID = original.btID;
        this.nameBT = original.nameBT;
        this.levelBT = original.levelBT;
        this.currentHP = original.levelBT;
        this.maxHP = original.maxHP;
        this.resistance = original.resistance;
        this.damage = original.damage;
        this.speed = original.speed;
        this.moveIDs = original.moveIDs;
        this.ability = original.ability;
        this.spawnLocations = original.spawnLocations;
        this.school = original.school; 
        this.evolveLvl = original.evolveLvl; 
        this.discription = original.discription;
        this.spriteLocation = original.spriteLocation;  
        this.question = original.question; 
        this.option1 = original.option1; 
        this.option2 = original.option2; 
        this.option3 = original.option3;
        this.option4 = original.option4; 
        this.correctOption = original.correctOption;  


        // Copy other fields as necessary
    }
   
    public Breakthrough() {}
    public bool TakeDamage(int dmg){
         
        currentHP-= dmg/resistance;

        if (currentHP <=0){
            return true;
        }else{
            return false;
        }
    } 
}
