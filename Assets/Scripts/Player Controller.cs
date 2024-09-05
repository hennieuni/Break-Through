using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
  

    public LayerMask whatStopsMove;
    public LayerMask whatSpawnsBreakThrough;

    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
        
    }

    // Update is called once per frame
    void Update()
    {   
       
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position , moveSpeed*Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f){
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f){
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3 (Input.GetAxisRaw("Horizontal"), 0f, 0f) , 0.2f, whatStopsMove) ){
                    movePoint.position += new Vector3 (Input.GetAxisRaw("Horizontal"), 0f, 0f); 
                }
                
            } 

            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f){

                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3 ( 0f, Input.GetAxisRaw("Vertical"), 0f) , 0.2f, whatStopsMove) ){
                    movePoint.position += new Vector3 ( 0f, Input.GetAxisRaw("Vertical"), 0f); 
                }
                
            } 
        }

        if (Physics2D.OverlapCircle(transform.position, 0.2f, whatSpawnsBreakThrough) ){
            SceneManager.LoadScene(2);
        }
    }
}
