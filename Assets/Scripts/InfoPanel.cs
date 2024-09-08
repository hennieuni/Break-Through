using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;


public class InfoPanel : MonoBehaviour
{

    public TMP_Text nameTxt;
    public TMP_Text levelTxt;
    public TMP_Text HPTxt;
    public Slider hpSlider;

    public void SetHud(Breakthrough breakthrough){
        nameTxt.text = breakthrough.nameBT;
        levelTxt.text = "Lvl " + breakthrough.levelBT; 
        hpSlider.maxValue = breakthrough.maxHP;
        hpSlider.value = breakthrough.currentHP;
        HPTxt.text = breakthrough.currentHP + "/" + breakthrough.maxHP;

    }

    public void SetHP(Breakthrough breakthrough, int newHp){
        breakthrough.currentHP = newHp;
        hpSlider.value = breakthrough.currentHP;
        HPTxt.text = breakthrough.currentHP + "/" + breakthrough.maxHP;
    }
   
}
