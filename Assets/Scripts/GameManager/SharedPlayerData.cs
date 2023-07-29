using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SharedPlayerData : NetworkBehaviour
{

    [SyncVar] public List<GameObject> controllableEnemies;
    public static SharedPlayerData instance;
    [SyncVar] public int currentPlayerControlledEnemy = 1;
    [SyncVar] public int numberOfConnectedPlayers = 0;
    string currentSceneName;
    // Start is called before the first frame update
    void Start()
    {
        if (SharedPlayerData.instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        currentPlayerControlledEnemy = 1;
    }




    [ClientRpc]
    public void RpcAddEnemyToList(GameObject obj)
    {
        controllableEnemies.Add(obj);
    }

    

    [Server]
    public void RpcRemoveEnemyFromList(GameObject obj)
    {
        controllableEnemies.Remove(obj);
    }

    [Server]
    public void CmdChangeScene(string sceneName)
    {
        controllableEnemies.Clear();
        currentSceneName = sceneName;
        NetworkManager.singleton.ServerChangeScene("NewPlayer");
        Invoke("MoveBackToScene", 5.0f);

    }


    [Server]
    private void MoveBackToScene()
    {
        NetworkManager.singleton.ServerChangeScene(currentSceneName);
    }

    [Server]
    public void CmdUpdatePlayerNumber()
    {
        if(currentPlayerControlledEnemy >= NetworkManager.singleton.numPlayers)
        {
            currentPlayerControlledEnemy = 1;
        }
        else
        {
            currentPlayerControlledEnemy++;
        }
    }

}
