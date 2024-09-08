using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro; 

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

    void Start()
    {
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
        dialogText.text = playerBreakThrough.nameBT + " delt " + playerBreakThrough.power + " damage.";

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
        }else{
            dialogText.text = "You where defeated :(";
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
}
