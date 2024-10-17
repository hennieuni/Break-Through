using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using UnityEditor.PackageManager;

public enum BattleState {START, PLAYERTURN, ENEMYTURN, WON, LOST, Between }


public class BattleSystem : MonoBehaviour
{
    public BattleState state;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattlestation;
    public Transform enemyBattlestation;
    Breakthrough playerBreakThrough;
    Breakthrough enemyBreakThrough;

    public TMP_Text dialogText;

    public InfoPanel playerInfoPanel;
    public InfoPanel enemyInfoPanel;

    string canSpawnFile;

    Breakthrough[] BreakthroughList;

    Breakthrough[] PlayerParty;

    public UnityEngine.TextAsset textAssetData;
    public UnityEngine.TextAsset playerPartyData;


    void loadListBT(){
       string[] data =  textAssetData.text.Split(new string[]{","}, StringSplitOptions.None);
    }

    void Start()
    {
        
        canSpawnFile = Application.dataPath + "/Saves/CanSpawn.txt";

        state = BattleState.START; 
        StartCoroutine(SetupBattle());

    }

    void ReadCSV(){
        string[] btString = textAssetData.text.Split(new string[] {"\n"}, StringSplitOptions.None);

        int leng = btString.Length;

        BreakthroughList = new Breakthrough[leng];
       

        for (int i =1; i< leng-1; i++){
            
            string[] stats = btString[i].Split(new string[] {","}, StringSplitOptions.None);

            BreakthroughList[i] = new Breakthrough();

            BreakthroughList[i].nameBT = stats[1];      
            BreakthroughList[i].maxHP = int.Parse(stats[2]);         
            BreakthroughList[i].resistance = int.Parse(stats[3]);       
            BreakthroughList[i].damage = int.Parse(stats[4]);          
            BreakthroughList[i].speed = int.Parse(stats[5]);         
            BreakthroughList[i].moveIDs = stats[6];          
            BreakthroughList[i].ability = stats[7];          
            BreakthroughList[i].spawnLocations = stats[8];      
            BreakthroughList[i].school = stats[9];
            BreakthroughList[i].evolveLvl = int.Parse(stats[10]);
            BreakthroughList[i].discription = stats[11];
            
            //add sprite location somehow her

        }     
    }

    void ReadPlayerBTCSV(){
        string[] btString = playerPartyData.text.Split(new string[] {"\n"}, StringSplitOptions.None);

        int leng = btString.Length;

        PlayerParty = new Breakthrough[leng];

        

        for (int i =1; i< leng-1; i++){
           
            
            string[] stats = btString[i].Split(new string[] {","}, StringSplitOptions.None);
             
            PlayerParty[i] = new Breakthrough();
             
            PlayerParty[i] = levelAdjusted(BreakthroughList[int.Parse(stats[0])], int.Parse(stats[2]));
             
            PlayerParty[i].currentHP = int.Parse(stats[1]);
             
            //add sprite location somehow her

        }
       
    }

    Breakthrough levelAdjusted(Breakthrough bt, int level){

        //(Level 100 stat / 4) + ( ¾ level 100 stat * level/100  )

        Breakthrough adjustedBT = new Breakthrough();

        adjustedBT = bt;
        adjustedBT.maxHP = adjustToLevel(bt.maxHP, level);
        adjustedBT.resistance = adjustToLevel(bt.resistance, level);
        adjustedBT.damage = adjustToLevel(bt.damage, level);
        adjustedBT.speed = adjustToLevel(bt.resistance, level);
        adjustedBT.resistance = adjustToLevel(bt.resistance, level);

        return adjustedBT;
    }

    int adjustToLevel(int val, int level){
        float corrected = (val/4)+(3/4*val*(level/100));
        //Debug.Log((int)Math.Floor(corrected));
        return (int)Math.Floor(corrected);
    }

    IEnumerator SetupBattle(){
        ReadCSV();
        ReadPlayerBTCSV();
        
        GameObject playerGO = Instantiate (playerPrefab, playerBattlestation);
        //playerBreakThrough = playerGO.GetComponent<Breakthrough>();
        playerBreakThrough = PlayerParty[1];
        
         Debug.Log("here");


        
        System.Random random = new System.Random();
        int randomNumber = random.Next(1, 2); 
        //Debug.Log(randomNumber);

        GameObject enemyGO = Instantiate (enemyPrefab, enemyBattlestation);

        int enemyLvl = 1;
        enemyBreakThrough = levelAdjusted(BreakthroughList[randomNumber], enemyLvl);
        enemyBreakThrough.currentHP =  BreakthroughList[randomNumber].maxHP;
        enemyBreakThrough.levelBT = enemyLvl;
        
        
        
        


        dialogText.text = "You descovered the " + enemyBreakThrough.nameBT + "!";

        playerInfoPanel.SetHud(playerBreakThrough);
        enemyInfoPanel.SetHud(enemyBreakThrough);

        yield return new WaitForSeconds(1.5f);


        //add check for speed to determine if its enemy or player turn first
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn(){
        dialogText.text = "What will you do?";
    }

    public void OnAttackBtn(){
        

        if (state != BattleState.PLAYERTURN){
            return;
        }
        state = BattleState.Between;
        StartCoroutine(PlayerAttack());
    }

    IEnumerator PlayerAttack(){
        
        bool isDead = enemyBreakThrough.TakeDamage(playerBreakThrough.damage);
        
        enemyInfoPanel.SetHP(enemyBreakThrough, enemyBreakThrough.currentHP);
        dialogText.text = playerBreakThrough.nameBT + " dealt " + playerBreakThrough.damage + " damage.";

        yield return new WaitForSeconds(2f);

        if (isDead){
           state = BattleState.WON; 
           EndBattle();
        }else{
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

    }

    void EndBattle(){
        if (state == BattleState.WON){
            dialogText.text = "You defeated " + enemyBreakThrough.nameBT + "!";
            SceneManager.LoadScene(1); 
        }else{
            dialogText.text = "You were defeated :(";
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator EnemyTurn(){
        dialogText.text = enemyBreakThrough.nameBT + "'s turn to attack.";

        yield return new WaitForSeconds(1f);

        bool isDead = playerBreakThrough.TakeDamage(enemyBreakThrough.damage);
        playerInfoPanel.SetHP(playerBreakThrough, playerBreakThrough.currentHP);
        dialogText.text = "You took " + enemyBreakThrough.damage + " damage!"; 

        yield return new WaitForSeconds(1f);

        if (isDead){
            state = BattleState.LOST; 
            EndBattle();
        }else{
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

    }
}
