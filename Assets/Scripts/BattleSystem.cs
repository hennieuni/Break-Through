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
    BattleState state;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    

    public Transform playerBattlestation;
    public Transform enemyBattlestation;
    Breakthrough playerBreakThrough;
    Breakthrough enemyBreakThrough;

    public TMP_Text dialogText;
    public TMP_Text moveText;
    public TMP_Text questionText;
    public TMP_Text btSelectText;

    public InfoPanel playerInfoPanel;
    public InfoPanel enemyInfoPanel;

    //string canSpawnFile;

    Breakthrough[] BreakthroughList;

    Breakthrough[] PlayerParty;
    Move[] moveList;
    public GameObject attackOptions;
    public GameObject battleOptions;
    public GameObject swapOptions;
    public GameObject replaceOptions;
    public GameObject questionOptions;

    public TMP_Text move1BtnT, move2BtnT, move3BtnT, move4BtnT;
    public TMP_Text option1BtnT, option2BtnT, option3BtnT, option4BtnT;
    public TMP_Text bt1, bt2, bt3, bt4, bt5, bt6;
    public TMP_Text Rbt1, Rbt2, Rbt3, Rbt4, Rbt5, Rbt6;

    public UnityEngine.TextAsset textAssetData;
    //public UnityEngine.TextAsset playerPartyData;
    public UnityEngine.TextAsset movesData;
    int enemyID ;

    string playerBTfile;

    int totalPartySize;
    string spriteLocation1, spriteLocation2;
    public Sprite sunSprite, neutronSprite, blackSprite, mitoSprite;
    public GameObject playerSprite, enemySprite; 

    void Start()
    {
        playerBTfile = Application.dataPath + "/CSV/PlayerBT.csv";

        spriteLocation1 = Application.dataPath + "/BTSprites/" ;
        spriteLocation2 = "BT.png";
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
            BreakthroughList[i].correctOption = int.Parse(stats[17]);
            
            //add sprite location somehow here
        }  
          
    }

    void ReadPlayerBTCSV(){
        
        string[] btString = File.ReadAllLines(playerBTfile);
        //string[] btString = playerPartyData.text.Split(new string[] {"\n"}, StringSplitOptions.None);
        int leng = btString.Length;
        
        PlayerParty = new Breakthrough[7];
        totalPartySize = leng-1;

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

        adjustedBT.maxHP = adjustToLevel(bt.maxHP, level);
        adjustedBT.resistance = adjustToLevel(bt.resistance, level);
        adjustedBT.damage = adjustToLevel(bt.damage, level);
        adjustedBT.speed = adjustToLevel(bt.resistance, level);
        
        return adjustedBT;
    }

    int adjustToLevel(int val, int level){
        float corrected = (val/4f)+(3f/4f*val*(level/100f));
        return (int)Math.Floor(corrected);
    }

    IEnumerator SetupBattle(){

        ReadMoves();
        ReadCSV();
        ReadPlayerBTCSV();
        
        //GameObject playerGO = Instantiate (playerPrefab, playerBattlestation);
        //playerBreakThrough = playerGO.GetComponent<Breakthrough>();
        int l = 1;
        while ( l <PlayerParty.Length ){
            if (PlayerParty[l].currentHP > 0){
                playerBreakThrough = PlayerParty[l];
                SetPlayerSprite(playerBreakThrough);
                l=PlayerParty.Length;
            }
            l++;
        }
       
      

        //playerGO.GetComponent<SpriteRenderer>().sprite = sunSprite;

        System.Random random = new System.Random();
        enemyID = random.Next(1, 5); 

        //GameObject enemyGO = Instantiate (enemyPrefab, enemyBattlestation);

        int enemyLvl = playerBreakThrough.levelBT;
        enemyBreakThrough = levelAdjusted(BreakthroughList[enemyID], enemyLvl);
        enemyBreakThrough.currentHP =  enemyBreakThrough.maxHP;
        enemyBreakThrough.levelBT = enemyLvl;

        SetEnemySprite(enemyBreakThrough);
    

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
    public void OnSwapBtn(){
        if (state != BattleState.PLAYERTURN){
            return;
        }

        battleOptions.SetActive(false);
        swapOptions.SetActive(true);
        
        bt1.text = PlayerParty[1].nameBT;
        
        if (totalPartySize >=2){
            bt2.text = PlayerParty[2].nameBT;
        }
        if (totalPartySize >=3){
            bt3.text = PlayerParty[3].nameBT;
        }
        if (totalPartySize >=4){
           bt4.text = PlayerParty[4].nameBT; 
        }
        if (totalPartySize >=5){
           bt5.text = PlayerParty[5].nameBT;  
        }
        if (totalPartySize >=6){
           bt6.text = PlayerParty[6].nameBT; 
        }  
        
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
    public void onReplaceBt1(){
        PlayerParty[1] = enemyBreakThrough;
        StartCoroutine(playerWrite());
    }
    public void onReplaceBt2(){
        PlayerParty[2] = enemyBreakThrough;
        StartCoroutine(playerWrite());
    }
    public void onReplaceBt3(){
        PlayerParty[3] = enemyBreakThrough;
        StartCoroutine(playerWrite());
    }
    public void onReplaceBt4(){
        PlayerParty[4] = enemyBreakThrough;
        StartCoroutine(playerWrite());
    }
    public void onReplaceBt5(){
        PlayerParty[5] = enemyBreakThrough;
        StartCoroutine(playerWrite());
    }
    public void onReplaceBt6(){
        PlayerParty[6] = enemyBreakThrough;
        StartCoroutine(playerWrite());
    }
    public void onReplaceBtNo(){
        StartCoroutine(playerWrite());
    }
    void SetPlayerSprite( Breakthrough breakT){
        if (breakT.btID == 1){
            playerSprite.GetComponent<SpriteRenderer>().sprite = sunSprite;
        }else if(breakT.btID == 2){
            playerSprite.GetComponent<SpriteRenderer>().sprite = neutronSprite;
        }else if(breakT.btID == 3){
            playerSprite.GetComponent<SpriteRenderer>().sprite = blackSprite;
        }else{
            playerSprite.GetComponent<SpriteRenderer>().sprite = mitoSprite;
        }
        playerInfoPanel.SetHud(breakT);
    }

    void SetEnemySprite(Breakthrough e){
        if (e.btID == 1){
            enemySprite.GetComponent<SpriteRenderer>().sprite = sunSprite;
        }else if(e.btID == 2){
            enemySprite.GetComponent<SpriteRenderer>().sprite = neutronSprite;
        }else if(e.btID == 3){
            enemySprite.GetComponent<SpriteRenderer>().sprite = blackSprite;
        }else{
            enemySprite.GetComponent<SpriteRenderer>().sprite = mitoSprite;
        }
    }
    
    public void OnBT1Select(){
        if (PlayerParty[1].currentHP > 0){
            playerBreakThrough = PlayerParty[1];
            playerInfoPanel.SetHud(playerBreakThrough); 
            battleOptions.SetActive(true);
            swapOptions.SetActive(false);
            SetPlayerSprite(playerBreakThrough);
        }else{
            btSelectText.text = "That BreakThrough is out of HP";  
        }
        
    }
    public void OnBT2Select(){
        if (totalPartySize >= 2){
            if (PlayerParty[2].currentHP > 0){
                playerBreakThrough = PlayerParty[2]; 
                playerInfoPanel.SetHud(playerBreakThrough);
                battleOptions.SetActive(true);
                swapOptions.SetActive(false);
                SetPlayerSprite(playerBreakThrough);
            }else{
              btSelectText.text = "That BreakThrough is out of HP";  
            }
          
        }else{
            btSelectText.text = "Dont have that many please pick a lower one";
        }
       
    }

    public void OnBT3Select(){
        if(totalPartySize >= 3){
            if (PlayerParty[3].currentHP > 0){
                playerBreakThrough = PlayerParty[3];
                playerInfoPanel.SetHud(playerBreakThrough); 
                battleOptions.SetActive(true);
                swapOptions.SetActive(false);
                SetPlayerSprite(playerBreakThrough);
            }else{
                btSelectText.text = "That BreakThrough is out of HP";  
            }
            
        }else{
            btSelectText.text = "Dont have that many please pick a lower one";
        }
        
    }

    public void OnBT4Select(){
        if(totalPartySize >= 4){
            if (PlayerParty[4].currentHP > 0){
                playerBreakThrough = PlayerParty[4];
                playerInfoPanel.SetHud(playerBreakThrough); 
                battleOptions.SetActive(true);
                swapOptions.SetActive(false); 
                SetPlayerSprite(playerBreakThrough);  
            }else{
              btSelectText.text = "That BreakThrough is out of HP";  
            }
            
        }else{
            btSelectText.text = "Dont have that many please pick a lower one";
        }
       
    }

    public void OnBT5Select(){
        if(totalPartySize >= 5){
            if (PlayerParty[5].currentHP > 0){
                playerBreakThrough = PlayerParty[5];
                playerInfoPanel.SetHud(playerBreakThrough); 
                battleOptions.SetActive(true);
                swapOptions.SetActive(false);
                SetPlayerSprite(playerBreakThrough);
            }else{
              btSelectText.text = "That BreakThrough is out of HP";  
            }
            
        }else{
            btSelectText.text = "Dont have that many please pick a lower one";
        }
        
    }
    public void OnBT6Select(){
        if(totalPartySize >= 6){
            if (PlayerParty[6].currentHP > 0){
                playerBreakThrough = PlayerParty[6];
                playerInfoPanel.SetHud(playerBreakThrough); 
                battleOptions.SetActive(true);
                swapOptions.SetActive(false);
                SetPlayerSprite(playerBreakThrough);
            }else{
              btSelectText.text = "That BreakThrough is out of HP";  
            }
            
        }else{
            btSelectText.text = "Dont have that many please pick a lower one";
        }
        
    }
    
    
    
    IEnumerator checkOption(int option){

        if (enemyBreakThrough.correctOption == option){
            questionText.text = "Correct! You caught "+ enemyBreakThrough.nameBT; 
            yield return new WaitForSeconds(1.5f);   
            if (PlayerParty.Length > 7){

                battleOptions.SetActive(false);
                replaceOptions.SetActive(true);

                Rbt1.text = PlayerParty[1].nameBT;
                Rbt2.text = PlayerParty[2].nameBT;
                Rbt3.text = PlayerParty[3].nameBT;
                Rbt4.text = PlayerParty[4].nameBT;
                Rbt5.text = PlayerParty[5].nameBT;
                Rbt6.text = PlayerParty[6].nameBT;
               
            }else{
                
                PlayerParty[PlayerParty.Length-1] = enemyBreakThrough;
                Debug.Log("Here");
                StartCoroutine(playerWrite());
            }
        }else{
            questionText.text = "Inorrect :( "+ enemyBreakThrough.nameBT + " got away";
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(playerWrite());
        }
        //update current HP after battle
        
    }

    IEnumerator playerWrite(){
       
        File.WriteAllText(playerBTfile, string.Empty);  //clears file
        
        using (StreamWriter writer = new StreamWriter(playerBTfile,true)){
            
            writer.WriteLine("ID,currentHP,levelBT,expNextLevel");
            

            for(int i=1;i<PlayerParty.Length; i++ ){  //writes from data
                writer.WriteLine(PlayerParty[i].btID + "," +PlayerParty[i].currentHP + "," + PlayerParty[i].levelBT+",0" );
            }
                
        }
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(1); 
    }


    IEnumerator PlayerAttack(int moveNumber){
        
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
           playerBreakThrough.levelBT = playerBreakThrough.levelBT+1;
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
            dialogText.text = "Returning to the library to heal";
            SceneManager.LoadScene(5);
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
                state = BattleState.LOST; 
                StartCoroutine(EndBattle());
            } else{
                dialogText.text = "Your " + playerBreakThrough.nameBT + " fainted!";
                yield return new WaitForSeconds(1f);

                i=1;
                while ( i <PlayerParty.Length ){
                if (PlayerParty[i].currentHP > 0){
                    playerBreakThrough = PlayerParty[i];
                    SetPlayerSprite(playerBreakThrough);
                    
                    i=PlayerParty.Length;
                }
                i++;
            }
                dialogText.text = playerBreakThrough.nameBT + " Swapped in";
                yield return new WaitForSeconds(1f);
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
            
        }else{
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

    }
}
