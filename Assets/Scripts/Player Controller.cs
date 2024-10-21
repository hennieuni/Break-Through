using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
//using System.Numerics;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public Transform movePoint;
  

    public LayerMask whatStopsMove;
    public LayerMask whatSpawnsBreakThrough;
    public LayerMask whatLoadsTown1;
    public LayerMask whatLoadsHealingRoom;
    public LayerMask whatLoadsRoute1;
    public LayerMask whatLoadsHealingScreen;
    public string playerPosFile;


    int nextSpawnCount;
    string canSpawnFile;

    


    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;

        playerPosFile = Application.dataPath + "/Saves/PlayerPos.txt";
        LoadPlayerPos();

        nextSpawnCount = 4;
        
    }

    // Update is called once per frame
    void Update()
    {   
        
       if ((Physics2D.OverlapCircle(movePoint.position, 0.2f, whatSpawnsBreakThrough))){
            
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed*Time.deltaTime );
            if (Vector3.Distance(transform.position, movePoint.position) == 0f){
                
                if (nextSpawnCount == 0){
                    SavePlayerPos(new Vector3(0,0,0));
                    SceneManager.LoadScene(2);
                }else{
                    if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f){
                        if (!Physics2D.OverlapCircle(movePoint.position + new Vector3 (Input.GetAxisRaw("Horizontal"), 0f, 0f) , 0.2f, whatStopsMove) ){
                            movePoint.position += new Vector3 (Input.GetAxisRaw("Horizontal"), 0f, 0f); 
                            nextSpawnCount--;
                        }
                
                    } else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f){

                        if (!Physics2D.OverlapCircle(movePoint.position + new Vector3 ( 0f, Input.GetAxisRaw("Vertical"), 0f) , 0.2f, whatStopsMove) ){
                            movePoint.position += new Vector3 ( 0f, Input.GetAxisRaw("Vertical"), 0f);
                            nextSpawnCount--;
                        }
                
                    } 
                    
                }
                
            }
            
        }else if(Physics2D.OverlapCircle(movePoint.position, 0.2f, whatLoadsTown1)){  //loads town 1
            //add if from rout 1 and if from healing room to change posAFterLoad
            Vector3 posAfterLoad = new Vector3(1.5f,-1.5f,0);

            Scene currentScene = SceneManager.GetActiveScene ();
            string sceneNeme = currentScene.name;

            if (sceneNeme.Equals("HealingBuidling")){    //of loading town form healing building
                posAfterLoad = new Vector3(16.5f,-10.5f,0);
            }else if(sceneNeme.Equals("Route1")){           //if loading town form route1
                posAfterLoad = new Vector3(-17.5f,1.5f,0);
            }
            LoadTown1(posAfterLoad);
        }else if(Physics2D.OverlapCircle(movePoint.position, 0.2f, whatLoadsHealingRoom)){  //loads healing room
            Vector3 posAfterLoad = new Vector3(1.5f,-9.5f,0);
            LoadHealingRoom(posAfterLoad);

        }else if(Physics2D.OverlapCircle(movePoint.position, 0.2f, whatLoadsRoute1)){  //loads route 1
            Vector3 posAfterLoad = new Vector3(26.5f,-5.5f,0);
            LoadRoute1(posAfterLoad);
        }else if(Physics2D.OverlapCircle(movePoint.position, 0.2f, whatLoadsHealingScreen)){  //loads healinghud
            SavePlayerPos(new Vector3(0,-1,0));
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position , moveSpeed*Time.deltaTime);
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f){
                SceneManager.LoadScene(5);        
            }

        }else{
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position , moveSpeed*Time.deltaTime);

            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f){
                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f){
                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3 (Input.GetAxisRaw("Horizontal"), 0f, 0f) , 0.2f, whatStopsMove) ){
                        movePoint.position += new Vector3 (Input.GetAxisRaw("Horizontal"), 0f, 0f); 
                       
                    }
                
                } else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f){

                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3 ( 0f, Input.GetAxisRaw("Vertical"), 0f) , 0.2f, whatStopsMove) ){
                        movePoint.position += new Vector3 ( 0f, Input.GetAxisRaw("Vertical"), 0f); 
                        
                    }
                
                } 
            }
        }
         
    }

   

    void LoadTown1(Vector3 posAfterLoad){
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position , moveSpeed*Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f){
            ChangePlayerPos(posAfterLoad);
            SceneManager.LoadScene(3);        
        }
    }

    void LoadHealingRoom(Vector3 posAfterLoad){
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position , moveSpeed*Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f){
            ChangePlayerPos(posAfterLoad);
            SceneManager.LoadScene(4);        
        }
    }

    void LoadRoute1(Vector3 posAfterLoad){
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position , moveSpeed*Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f){
            ChangePlayerPos(posAfterLoad);
            SceneManager.LoadScene(1);        
        }
    }
    void SavePlayerPos(Vector3 stagger){
        float playerx = transform.position.x + stagger.x; 
        float playery = transform.position.y + stagger.y;
        string playerxy =  playerx + "," + playery;

        if (!File.Exists(playerPosFile)){
            File.WriteAllText(playerPosFile, playerxy);
        }else{
            using (StreamWriter writer = new StreamWriter(playerPosFile,false)){
            writer.WriteLine(playerxy);
            }
        }
        
    }

    void ChangePlayerPos(Vector3 newPos){
        float playerx = newPos.x; 
        float playery = newPos.y;
        string playerxy =  playerx + "," + playery;

        if (!File.Exists(playerPosFile)){
            File.WriteAllText(playerPosFile, playerxy);
        }else{
            using (StreamWriter writer = new StreamWriter(playerPosFile,false)){
            writer.WriteLine(playerxy);
            }
        }
    }

    void LoadPlayerPos(){
        string playerxy;
        using (StreamReader reader = new StreamReader(playerPosFile)){
            playerxy = reader.ReadLine();
        }

        string[] values = playerxy.Split(',');

        float playerx = float.Parse(values[0]);
        float playery = float.Parse(values[1]);

        transform.position = new Vector3(playerx,playery,0); 
        movePoint.position = transform.position;
    }

}
