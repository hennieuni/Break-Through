using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
  

    public LayerMask whatStopsMove;
    public LayerMask whatSpawnsBreakThrough;
    public string playerPosFile;

    int nextSpawnCount;
    string canSpawnFile;

    


    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;

        playerPosFile = Application.dataPath + "/Saves/PlayerPos.txt";
        LoadPlayerPos();

        canSpawnFile = Application.dataPath + "/Saves/CanSpawn.txt";
       // LoadCanSpawnCount();
        nextSpawnCount = 4;

        
    }

    // Update is called once per frame
    void Update()
    {   
       if ((Physics2D.OverlapCircle(movePoint.position, 0.2f, whatSpawnsBreakThrough))){
            
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position , moveSpeed*Time.deltaTime);
            if (Vector3.Distance(transform.position, movePoint.position) == 0f){
                
                if (nextSpawnCount == 0){
                    SavePlayerPos();
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
                    
                    Debug.Log(""+nextSpawnCount);
                }
                
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

    void SavePlayerPos(){
        float playerx = transform.position.x; 
        float playery = transform.position.y;
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

    void LoadCanSpawnCount(){
       
        using (StreamReader reader = new StreamReader(canSpawnFile)){
            nextSpawnCount =  int.Parse(reader.ReadLine());
        }

    }
}
