using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    //Movement directions will be based on numpad notation: https://www.dustloop.com/w/Notation

    public Vector3 moveDirection = Vector3.zero;
    public Vector3 playerRotation = Vector3.up;

    public bool isRunning = false;
    Animator anim;

    int orientationNum = 8;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!LevelManager.isGameOver)
        {
            //check whether player is moving
            if (moveDirection == Vector3.zero)
            {
                anim.SetBool("isMoving", false);
            }
            else 
            {
                anim.SetBool("isMoving", true);
            }

            //check whether player is running
            anim.SetBool("isRunning", isRunning);

            //compare angle between movement direction and player rotation
            float dot = Vector3.Dot(moveDirection, playerRotation);
            //Debug.Log(moveDirection + "; " + playerRotation + "; " + dot);
            if (dot > 0)
            {
                orientationNum = 8;
            }
            else if (dot < 0)
            {
                orientationNum = 2;
            }
            else 
            {
                orientationNum = 5; //meaningless
            }
            //set movement direction
            anim.SetInteger("moveDirection", orientationNum);
        }
        else 
        {
            //if game is over, player cannot be moving
            anim.SetBool("isMoving", false);
        }
        

    }

    public void SetMoveDirection(Vector3 newMovement)
    {
        moveDirection = newMovement;
    }

    public void SetPlayerRotation(Vector3 newRotation)
    {
        playerRotation = newRotation;
    }
}
