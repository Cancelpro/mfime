using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeet : NetworkBehaviour
{

    [Server]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>())
        {
            transform.parent.GetComponent<Rigidbody2D>().velocity=
                new Vector2(transform.parent.GetComponent<Rigidbody2D>().velocity.x, 1);
        }
    }
}

