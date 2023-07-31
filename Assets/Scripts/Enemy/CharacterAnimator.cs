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
    public int scale = 1;
    public bool isOnWall = false;
    private void Start()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = Time.time;
    }


    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer && !isServer) return;
        
        if (Time.time - timer <= 1f) return;
        
        if(characterType == CharacterType.Hero)
        {
            HeroAnim();
        }
    }


    private void HeroAnim()
    {
        string anim;

        

        if(playerRigid.velocity.x <= -0.1f)
        {
            scale = -1;
        } else if (playerRigid.velocity.x >= 0.1f)
        {
            scale = 1;
        }

        transform.localScale = Vector2.MoveTowards(transform.localScale,
            new Vector2(scale, 1), 10 * Time.deltaTime);
        

        if (playerRigid.velocity.y >= 0.1)
        {
            anim = "Hero_Jump";
        } else if(playerRigid.velocity.y <= -0.1f)
        {
            anim = "Hero_Fall";
        } else if(playerRigid.velocity.x >= 0.1f || playerRigid.velocity.x <= -0.1f)
        {
            anim = ("Hero_Run");
            
        }
        else
        {
            anim = ("Hero_Idle");
            
        }


        bool feetOnGround = transform.GetChild(0).GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Ground"));
        bool handsOnGround = transform.GetChild(1).GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Ground"));

        if (handsOnGround && !feetOnGround)
        {
            anim = "Hero_Wall";
        }
        




        if (anim != "" && anim != animator.GetCurrentAnimatorClipInfo(0)[0].clip.name){
            CmdHeroAdmin(anim, scale);
            animator.Play(anim);
        }
        
    }



    [Command]
    private void CmdHeroAdmin(string anim, int scale)
    {
        PlayerTheFuckingAnim(anim, scale);
    }

    [Server]
    private void PlayerTheFuckingAnim(string anim, int scale)
    {
        animator.Play(anim);
        if (scale == 0) return;
        transform.localScale = Vector2.MoveTowards(transform.localScale,
            new Vector2(scale, 1), 10 * Time.deltaTime);
    }


}
