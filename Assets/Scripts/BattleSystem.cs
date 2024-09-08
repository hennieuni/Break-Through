using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;
using System.IO;

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

    void Start()
    {
        canSpawnFile = Application.dataPath + "/Saves/CanSpawn.txt";

        state = BattleState.START; 
        StartCoroutine(SetupBattle());

    }

    IEnumerator SetupBattle(){
        GameObject playerGO = Instantiate (playerPrefab, playerBattlestation);
        playerBreakThrough = playerGO.GetComponent<Breakthrough>();

        GameObject enemyGO = Instantiate (enemyPrefab, enemyBattlestation);
        enemyBreakThrough = enemyGO.GetComponent<Breakthrough>();

        dialogText.text = "You descovered the " + enemyBreakThrough.nameBT + "!";

        playerInfoPanel.SetHud(playerBreakThrough);
        enemyInfoPanel.SetHud(enemyBreakThrough);

        yield return new WaitForSeconds(2f);

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
        
        bool isDead = enemyBreakThrough.TakeDamage(playerBreakThrough.power);
        
        enemyInfoPanel.SetHP(enemyBreakThrough, enemyBreakThrough.currentHP);
        dialogText.text = playerBreakThrough.nameBT + " dealt " + playerBreakThrough.power + " damage.";

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
            SaveNextSpawn();
            SceneManager.LoadScene(1); 
        }else{
            dialogText.text = "You were defeated :(";
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator EnemyTurn(){
        dialogText.text = enemyBreakThrough.nameBT + "'s turn to attack.";

        yield return new WaitForSeconds(1f);

        bool isDead = playerBreakThrough.TakeDamage(enemyBreakThrough.power);
        playerInfoPanel.SetHP(playerBreakThrough, playerBreakThrough.currentHP);
        dialogText.text = "You took " + enemyBreakThrough.power + " damage!"; 

        yield return new WaitForSeconds(1f);

        if (isDead){
           state = BattleState.LOST; 
           EndBattle();
        }else{
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

    }

     void SaveNextSpawn(){

        int nNextSpawn = 4;

        if (!File.Exists(canSpawnFile)){
            File.WriteAllText(canSpawnFile, "" + nNextSpawn);
        }else{
            using (StreamWriter writer = new StreamWriter(canSpawnFile,false)){
                writer.WriteLine("" + nNextSpawn);
            }
        }
        
    }
}
