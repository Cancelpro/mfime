using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : NetworkBehaviour
{
    public bool isHero = false;
    public float jumpHeight = 5.0f;
    public float moveSpeed = 5.0f;
    public bool canJump = true;

    public enum CharacterType
    {
        Hero,
        SplashyFish,
        HoppyPumpkin,
        FallingLog
    }

    public static CharacterType charType;


    [Server]
    private void OnBecameVisible()
    {
        Debug.Log("i am viewed");
        if (gameObject.name == "Hero") return;

        SharedPlayerData.instance.RpcAddEnemyToList(gameObject);

    }

    [Server]

    private void OnBecameInvisible()
    {
        if (gameObject.name == "Hero") return;

        SharedPlayerData.instance.RpcRemoveEnemyFromList(gameObject);
    }

    


    [Server]
    private void CmdDie()
    {
        if (isHero)
        {
            SharedPlayerData.instance.CmdUpdatePlayerNumber();

            SharedPlayerData.instance.CmdChangeScene(SceneManager.GetActiveScene().name);
            //NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
        }



        //GetComponent<NetworkIdentity>().RemoveClientAuthority();
        if(gameObject.name != "Hero")
        {
            Debug.Log("hiasdfas");
            SharedPlayerData.instance.RpcRemoveEnemyFromList(gameObject);
        }
        Destroy(gameObject);
    }

    [Server]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerFeet>() || collision.GetComponent<EnemyDamage>())
        {
            CmdDie();
        }
    }

    

}
