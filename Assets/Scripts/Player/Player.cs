using Mirror;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{


    public GameObject controlledEnemy = null;
    private int controlledEnemyPosition = 0;
    float speed = 5.0f;
    private Rigidbody2D feet;
    private bool canMove = true;


    [SyncVar]
    private int playerNumber = 0;
    // Start is called before the first frame update
    [Client]
    void Start()
    {
        DontDestroyOnLoad(this);
        //controlledEnemy = new GameObject();
        
        
        
    }


    // Update is called once per frame
    [Client]
    void Update()
    {
        if (!isLocalPlayer || SharedPlayerData.instance == null) return;



        if (SharedPlayerData.instance.currentPlayerControlledEnemy == playerNumber
                && controlledEnemy == null && GameObject.Find("Hero"))
        {
            // Debug.Log(playerNumber);
            CmdTakeHeroControl();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            CmdLoadNewScene();
        }

        Debug.Log(playerNumber);




        if (controlledEnemy != null && controlledEnemy.GetComponent<Rigidbody2D>())
        {
            if (canMove)
            {
                Move();
                Jump();
            }
        }

            if (SharedPlayerData.instance.currentPlayerControlledEnemy == playerNumber
                ) return;

            
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                CmdGetEnemy();
            }

        

    }
    [Command]
    private void CmdLoadNewScene()
    {
        SharedPlayerData.instance.controllableEnemies.Clear();
        NetworkManager.singleton.ServerChangeScene("Level1");
    }

    [Client]
    private void Move()
    {
        Debug.Log("hi");
        int MoveX = 0;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveX = 1;
        }  else if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveX = -1;

            
        }
        
        

        CmdMove(MoveX * speed);
    }
    [Client]
    private void Jump()
    {

        bool feetOnGround = controlledEnemy.transform.GetChild(0).GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Ground"));


        if (feetOnGround)
        {
            controlledEnemy.GetComponent<Enemy>().canJump = true;

        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            CmdJump();
        }
    }

    [Server]
    public void SetPlayerNumber(int num)
    {
        playerNumber = (num);
    }


    [Client]   
    private void CmdMove(float move)
    {
        controlledEnemy.GetComponent<Rigidbody2D>().velocity = new Vector2(move,
            controlledEnemy.GetComponent<Rigidbody2D>().velocity.y);
    }

    [Client]
    private void CmdJump()
    {
        Enemy enemyData = controlledEnemy.GetComponent<Enemy>();
        bool feetOnGround = controlledEnemy.transform.GetChild(0).GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Ground"));
        bool handsOnGround = controlledEnemy.transform.GetChild(1).GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Ground"));
         if(feetOnGround && !handsOnGround)
        {
            enemyData.canWallJump = true;
        }

        if (enemyData.canWallJump && handsOnGround && !feetOnGround)
        {
            Rigidbody2D rigid = controlledEnemy.GetComponent<Rigidbody2D>();
            rigid.velocity = Vector2.zero;
            canMove = false;
            Invoke("CanJumpAgain", 0.5f);
            controlledEnemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(enemyData.jumpHeight * -controlledEnemy.GetComponent<CharacterAnimator>().scale,
                enemyData.jumpHeight), ForceMode2D.Impulse);
            Debug.Log("huh");
        }


        if (enemyData.canJump)
        {
            Rigidbody2D rigid = controlledEnemy.GetComponent<Rigidbody2D>();
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            controlledEnemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 
                enemyData.jumpHeight), ForceMode2D.Impulse);
            enemyData.canJump = false;

        }
    }

    private void CanJumpAgain()
    {
        canMove = true;
    }


    [Command]
    private void CmdGetEnemy()
    {
        if (SharedPlayerData.instance.controllableEnemies == null) return;
        if(controlledEnemy != null)
        {
            controlledEnemy.GetComponent<NetworkIdentity>().RemoveClientAuthority();
            controlledEnemy = null;
        }
        if(controlledEnemyPosition >= SharedPlayerData.instance.controllableEnemies.Count)
        {
            controlledEnemyPosition = 0;
        }



        for(int i = controlledEnemyPosition; 
            i < SharedPlayerData.instance.controllableEnemies.Count; i++)
        {
            if (!SharedPlayerData.instance.controllableEnemies[i].GetComponent<NetworkIdentity>().isOwned)
            {
                if(controlledEnemy != null)
                {
                        controlledEnemy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                        controlledEnemy.GetComponent<NetworkIdentity>().RemoveClientAuthority();
                }

                

                    controlledEnemy = SharedPlayerData.instance.controllableEnemies[i];
                    controlledEnemy.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
                    controlledEnemy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;


                    TargetReturnGameObject(connectionToClient, controlledEnemy);
            }
        }
    }



    [Command]
    private void CmdTakeHeroControl()
    {
        Debug.Log("take control");
        if(controlledEnemy != null)
        {
            controlledEnemy.GetComponent<NetworkIdentity>().RemoveClientAuthority();
        }

        
        controlledEnemy = GameObject.Find("Hero");

        if (controlledEnemy.GetComponent<NetworkIdentity>().isOwned)
        {
            controlledEnemy.GetComponent<NetworkIdentity>().RemoveClientAuthority();
        }

        controlledEnemy.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        controlledEnemy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        //if (connectionToClient.identity.GetComponent<Player>().playerNumber == 1) return;
        TargetReturnGameObject(connectionToClient, controlledEnemy);
    }

    [TargetRpc]
    private void TargetReturnGameObject(NetworkConnectionToClient target, GameObject obj)
    {
        controlledEnemy = obj;
        controlledEnemy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }




}
