using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealingParty : MonoBehaviour
{
    public TMP_Text dialogText;
    string playerBTfile;
    public UnityEngine.TextAsset textAssetData;
    Breakthrough[] BreakthroughList;
    Breakthrough[] PlayerParty;
    int totalPartySize;
    bool loaded = false;

    void Start()
    {

        playerBTfile = Application.dataPath + "/CSV/PlayerBT.csv";
        StartCoroutine(LoadFiles());

    }

    void ReadCSV(){
        string[] btString = textAssetData.text.Split(new string[] {"\n"}, StringSplitOptions.None);
        Debug.Log("here");
        int leng = btString.Length;
        
        BreakthroughList = new Breakthrough[leng];
       
       
        for (int i =1; i< leng-1; i++){
             
            string[] stats = btString[i].Split(new string[] {","}, StringSplitOptions.None);

            
            
            BreakthroughList[i] = new Breakthrough();

            BreakthroughList[i].btID = int.Parse(stats[0]);
            
            BreakthroughList[i].nameBT = stats[1];  
            BreakthroughList[i].maxHP = int.Parse(stats[2]);              
            
            
            //add sprite location somehow here
        }  
          
    }

    
    void ReadPlayerBTCSV(){
        
        string[] btString = File.ReadAllLines(playerBTfile);
        //string[] btString = playerPartyData.text.Split(new string[] {"\n"}, StringSplitOptions.None);
        int leng = btString.Length;
        
        PlayerParty = new Breakthrough[leng];
        totalPartySize = leng-1;
        //Debug.Log(totalPartySize);
        for (int i =1; i< leng; i++){
            string[] stats = btString[i].Split(new string[] {","}, StringSplitOptions.None);
             
            PlayerParty[i] = new Breakthrough();
             
            PlayerParty[i] = levelAdjusted(BreakthroughList[int.Parse(stats[0])], int.Parse(stats[2]));
            PlayerParty[i].currentHP = int.Parse(stats[1]);
            PlayerParty[i].levelBT = int.Parse(stats[2]);
             
            //add sprite location somehow her

        }
       
    }

    Breakthrough levelAdjusted(Breakthrough bt, int level){

        //(Level 100 stat / 4) + ( ¾ level 100 stat * level/100  )

        Breakthrough adjustedBT = new Breakthrough(bt);
        //Debug.Log(bt.maxHP);
        adjustedBT.maxHP = adjustToLevel(bt.maxHP, level);

        //Debug.Log("resistance " +adjustedBT.resistance);
        return adjustedBT;
    }

     int adjustToLevel(int val, int level){
        float corrected = (val/4f)+(3f/4f*val*(level/100f));
        //Debug.Log((int)Math.Floor(corrected));
        return (int)Math.Floor(corrected);
    }


    IEnumerator LoadFiles(){
        yield return new WaitForSeconds(0.1f);
        ReadCSV();
        
        ReadPlayerBTCSV();
        loaded = true;

        bool isOut = true;
        int i = 1;
            
        while ( i <PlayerParty.Length ){
            if (PlayerParty[i].currentHP > 0){
                isOut=false;
                i=PlayerParty.Length;
            }
            i++;
        }

        if(isOut){
            OnBtnYes();
        }
    }

    public void OnBtnYes(){
        if(loaded == false){
            return;
        }
        dialogText.text = "Healed to FUll";
        //update current HP after battle
        File.WriteAllText(playerBTfile, string.Empty);  //clears file
        
        using (StreamWriter writer = new StreamWriter(playerBTfile,true)){
            
            
            writer.WriteLine("ID,currentHP,levelBT,expNextLevel");
            for(int i=1;i<PlayerParty.Length; i++ ){  //writes from data
                writer.WriteLine(PlayerParty[i].btID + "," +PlayerParty[i].maxHP + "," + PlayerParty[i].levelBT+",0" );
            }
        } 

        StartCoroutine(LoadRoom());
    }
    public void OnBtnNo(){
        if(loaded == false){
            return;
        }
        dialogText.text = "Not Healed"; 
        StartCoroutine(LoadRoom());
    }

    IEnumerator LoadRoom(){
        yield return new WaitForSeconds(1.5f);
        string playerPosFile = Application.dataPath + "/Saves/PlayerPos.txt";
        if (!File.Exists(playerPosFile)){
            File.WriteAllText(playerPosFile, "-6.5,2.5");
        }else{
            using (StreamWriter writer = new StreamWriter(playerPosFile,false)){
            writer.WriteLine("-6.5,2.5");
            }
        }
        SceneManager.LoadScene(4);
    }

    
}
