using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mfimeNetworkManager : NetworkManager
{

    public override void OnClientConnect()
    {
        base.OnClientConnect();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Player player = conn.identity.GetComponent<Player>();

        player.SetPlayerNumber(numPlayers);


        
    }
}
