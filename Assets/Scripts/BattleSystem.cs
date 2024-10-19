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

    public TMP_Text moveText;
    public TMP_Text questionText;

    public InfoPanel playerInfoPanel;
    public InfoPanel enemyInfoPanel;

    //string canSpawnFile;

    Breakthrough[] BreakthroughList;

    Breakthrough[] PlayerParty;
    Move[] moveList;
    public GameObject attackOptions;
    public GameObject battleOptions;
    public GameObject questionOptions;

    public TMP_Text move1BtnT, move2BtnT, move3BtnT, move4BtnT;
    public TMP_Text option1BtnT, option2BtnT, option3BtnT, option4BtnT;

    public UnityEngine.TextAsset textAssetData;
    //public UnityEngine.TextAsset playerPartyData;
    public UnityEngine.TextAsset movesData;
    int enemyID ;

    string playerBTfile;


    void loadListBT(){
       string[] data =  textAssetData.text.Split(new string[]{","}, StringSplitOptions.None);
    }

    void Start()
    {
        playerBTfile = Application.dataPath + "/CSV/PlayerBT.csv";


        //canSpawnFile = Application.dataPath + "/Saves/CanSpawn.txt";

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

            BreakthroughList[i].btID = int.Parse(stats[0]);
            
            BreakthroughList[i].nameBT = stats[1];  
            BreakthroughList[i].maxHP = int.Parse(stats[2]);        
            BreakthroughList[i].resistance = int.Parse(stats[3]);       
            BreakthroughList[i].damage = int.Parse(stats[4]);          
            BreakthroughList[i].speed = int.Parse(stats[5]);
            
            string[] knowMoves = stats[6].Split(new string[] {"-"}, StringSplitOptions.None);
            BreakthroughList[i].moveIDs[0] = moveList[int.Parse(knowMoves[0])];
            BreakthroughList[i].moveIDs[1] = moveList[int.Parse(knowMoves[1])];
            BreakthroughList[i].moveIDs[2] = moveList[int.Parse(knowMoves[2])];
            BreakthroughList[i].moveIDs[3] = moveList[int.Parse(knowMoves[3])];
            
            BreakthroughList[i].ability = stats[7];          
            BreakthroughList[i].spawnLocations = stats[8];      
            BreakthroughList[i].school = stats[9];
            BreakthroughList[i].evolveLvl = int.Parse(stats[10]);
            BreakthroughList[i].discription = stats[11];
            
            BreakthroughList[i].question = stats[12];
            BreakthroughList[i].option1 = stats[13];
            BreakthroughList[i].option2 = stats[14];
            BreakthroughList[i].option3 = stats[15];
            BreakthroughList[i].option4 = stats[16];
            //Debug.Log("here " +stats[17]); 
            BreakthroughList[i].correctOption = int.Parse(stats[17]);
            
            //add sprite location somehow here
        }  
          
    }

    void ReadPlayerBTCSV(){
        
        string[] btString = File.ReadAllLines(playerBTfile);
        //string[] btString = playerPartyData.text.Split(new string[] {"\n"}, StringSplitOptions.None);
        int leng = btString.Length;
        
        PlayerParty = new Breakthrough[leng];

        for (int i =1; i< leng; i++){
            string[] stats = btString[i].Split(new string[] {","}, StringSplitOptions.None);
             
            PlayerParty[i] = new Breakthrough();
             
            PlayerParty[i] = levelAdjusted(BreakthroughList[int.Parse(stats[0])], int.Parse(stats[2]));
            PlayerParty[i].currentHP = int.Parse(stats[1]);
            PlayerParty[i].levelBT = int.Parse(stats[2]);
             
            //add sprite location somehow her

        }
       
    }

    void ReadMoves(){
        string[] moveString = movesData.text.Split(new string[] {"\n"}, StringSplitOptions.None);

        int leng = moveString.Length;

        moveList = new Move[leng];

        for (int i =1; i< leng-1; i++){
            
            string[] stats = moveString[i].Split(new string[] {","}, StringSplitOptions.None);

            moveList[i] = new Move();

            moveList[i].nameM = stats[1];
            moveList[i].power =  int.Parse(stats[2]);
            moveList[i].type = stats[3];
            moveList[i].effect = stats[4];
            moveList[i].accuracy =  int.Parse(stats[5]);
            moveList[i].description = stats[6]; 
        }
    }

    

    Breakthrough levelAdjusted(Breakthrough bt, int level){

        //(Level 100 stat / 4) + ( ¾ level 100 stat * level/100  )

        Breakthrough adjustedBT = new Breakthrough(bt);
        Debug.Log(bt.maxHP);
        adjustedBT.maxHP = adjustToLevel(bt.maxHP, level);
        adjustedBT.resistance = adjustToLevel(bt.resistance, level);
        adjustedBT.damage = adjustToLevel(bt.damage, level);
        adjustedBT.speed = adjustToLevel(bt.resistance, level);
        

        //Debug.Log("resistance " +adjustedBT.resistance);
        return adjustedBT;
    }

    int adjustToLevel(int val, int level){
        float corrected = (val/4f)+(3f/4f*val*(level/100f));
        //Debug.Log((int)Math.Floor(corrected));
        return (int)Math.Floor(corrected);
    }

    IEnumerator SetupBattle(){
       

        ReadMoves();
        ReadCSV();
        ReadPlayerBTCSV();
        
        GameObject playerGO = Instantiate (playerPrefab, playerBattlestation);
        //playerBreakThrough = playerGO.GetComponent<Breakthrough>();
        playerBreakThrough = PlayerParty[2];
        


        
        System.Random random = new System.Random();
        enemyID = random.Next(1, 2); 
        //Debug.Log(randomNumber);

        GameObject enemyGO = Instantiate (enemyPrefab, enemyBattlestation);

        int enemyLvl = playerBreakThrough.levelBT;
        //Debug.Log(BreakthroughList[enemyID].maxHP);
        enemyBreakThrough = levelAdjusted(BreakthroughList[enemyID], enemyLvl);
        enemyBreakThrough.currentHP =  enemyBreakThrough.maxHP;
        //Debug.Log( BreakthroughList[enemyID].maxHP);
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
        attackOptions.SetActive(true);
        battleOptions.SetActive(false);

        move1BtnT.text = playerBreakThrough.moveIDs[0].nameM;
        move2BtnT.text = playerBreakThrough.moveIDs[1].nameM;
        move3BtnT.text = playerBreakThrough.moveIDs[2].nameM;
        move4BtnT.text = playerBreakThrough.moveIDs[3].nameM;
        
        //StartCoroutine(PlayerAttack());
    }

    public void OnMove1Btn(){
        if (state != BattleState.PLAYERTURN){
            return;
        }
        state = BattleState.Between;
        StartCoroutine(PlayerAttack(1));
    }

    public void OnMove2Btn(){
        if (state != BattleState.PLAYERTURN){
            return;
        }
        state = BattleState.Between;
        StartCoroutine(PlayerAttack(2));
    }

    public void OnMove3Btn(){
        if (state != BattleState.PLAYERTURN){
            return;
        }
        state = BattleState.Between;
        StartCoroutine(PlayerAttack(3));
    }

    public void OnMove4Btn(){
        if (state != BattleState.PLAYERTURN){
            return;
        }
        state = BattleState.Between;
        StartCoroutine(PlayerAttack(4));
    }

    public void OnOption1Btn(){
        StartCoroutine(checkOption(1));
    }
    public void OnOption2Btn(){
        StartCoroutine(checkOption(2));
    }
    public void OnOption3Btn(){
        StartCoroutine(checkOption(3));
    }
    public void OnOption4Btn(){
        StartCoroutine(checkOption(4));
    }

    IEnumerator checkOption(int option){

        
        //update current HP after battle
        File.WriteAllText(playerBTfile, string.Empty);  //clears file
        
        using (StreamWriter writer = new StreamWriter(playerBTfile,true)){
            
            
            writer.WriteLine("ID,currentHP,levelBT,expNextLevel");
            for(int i=1;i<PlayerParty.Length; i++ ){  //writes from data
                writer.WriteLine(PlayerParty[i].btID + "," +PlayerParty[i].currentHP + "," + PlayerParty[i].levelBT+",0" );
            }
            
            if (enemyBreakThrough.correctOption == option){
                
                writer.WriteLine(""+enemyID + ",0," + enemyBreakThrough.levelBT+",5");
                questionText.text = "Correct! You caught "+ enemyBreakThrough.nameBT; 
                
            }else{
                questionText.text = "Inorrect :( "+ enemyBreakThrough.nameBT + " got away";
            }
        }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(1); 
    }


    IEnumerator PlayerAttack(int moveNumber){
        
        //Debug.Log("move used " + playerBreakThrough.moveIDs[moveNumber-1].nameM);
        bool isDead = enemyBreakThrough.TakeDamage(playerBreakThrough.damage * playerBreakThrough.moveIDs[moveNumber-1].power);
        
        enemyInfoPanel.SetHP(enemyBreakThrough, enemyBreakThrough.currentHP);
        moveText.text = playerBreakThrough.nameBT + " used " + playerBreakThrough.moveIDs[moveNumber-1].nameM;
        yield return new WaitForSeconds(1f);

        attackOptions.SetActive(false);
        battleOptions.SetActive(true);
        
        dialogText.text = playerBreakThrough.nameBT + " dealt " + playerBreakThrough.damage*playerBreakThrough.moveIDs[moveNumber-1].power/enemyBreakThrough.resistance + " damage.";
        
        yield return new WaitForSeconds(1f);
        
        if (isDead){
           state = BattleState.WON;
           StartCoroutine(EndBattle());
        }else{
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        
        
        
    }

    IEnumerator EndBattle(){
         
        if (state == BattleState.WON){
            dialogText.text = "You defeated " + enemyBreakThrough.nameBT + "!";
            yield return new WaitForSeconds(1f);
            battleOptions.SetActive(false);
            questionOptions.SetActive(true);

            questionText.text = enemyBreakThrough.question;
            option1BtnT.text = enemyBreakThrough.option1;
            option2BtnT.text = enemyBreakThrough.option2;
            option3BtnT.text = enemyBreakThrough.option3;
            option4BtnT.text = enemyBreakThrough.option4;


            //SceneManager.LoadScene(1); 
        }else{
            dialogText.text = "You were defeated :(";
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator EnemyTurn(){
        dialogText.text = enemyBreakThrough.nameBT + "'s turn to attack.";

        yield return new WaitForSeconds(1f);

         System.Random randomMove = new System.Random();
        int randomNumber = randomMove.Next(0, 4); //random move 1-4

        bool isDead = playerBreakThrough.TakeDamage(enemyBreakThrough.damage * enemyBreakThrough.moveIDs[randomNumber].power); //enemy attacks for random 1 of 4 moves
        playerInfoPanel.SetHP(playerBreakThrough, playerBreakThrough.currentHP);
        dialogText.text = "You took " + enemyBreakThrough.damage* enemyBreakThrough.moveIDs[randomNumber].power/playerBreakThrough.resistance + " damage!"; 

        yield return new WaitForSeconds(1f);

        if (isDead){
            state = BattleState.LOST; 
            StartCoroutine(EndBattle());
        }else{
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

    }
}
