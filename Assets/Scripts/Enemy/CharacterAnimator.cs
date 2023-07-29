using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : NetworkBehaviour
{

    public enum CharacterType
    {
        Hero,
        SplashyFish,
        HoppyPumpkin,
        FallingLog
    }

    CharacterType characterType;
    Rigidbody2D playerRigid;
    Animator animator;
    float timer = 0;
    private void Start()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = Time.time;
    }

    [Client]
    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        if (Time.time - timer <= 1f) return;
        
        if(characterType == CharacterType.Hero)
        {
            HeroAnim();
        }
    }

    [Client]
    private void HeroAnim()
    {
        string anim;
        if(playerRigid.velocity.x >= 0.1f || playerRigid.velocity.x <= -0.1f)
        {
            anim = ("Hero_Run");
            animator.Play(anim);
        }
        else
        {
            anim = ("Hero_Idle");
            animator.Play(anim);
        }
        try
        {
            RpcHeroAdmin(anim);

        }
        catch (Exception r) { }
        
    }

    [ClientRpc]
    private void RpcHeroAdmin(string anim)
    {
        animator.Play(anim);
    }


}
