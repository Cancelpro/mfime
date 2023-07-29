using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraFollowHero : MonoBehaviour
{
    Transform hero;
    CinemachineVirtualCamera cam;
    Vector3 heroPos;
    private void Start()
    {
        try
        {
            hero = GameObject.Find("Hero").transform;
        }
        catch (Exception e)
        {
            
        }
        //m = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        
        if(hero == null)
        {
            try
            {
                hero = GameObject.Find("Hero").transform;
                heroPos = new Vector3(hero.position.x, hero.position.y, -10);

            } catch(Exception e)
            {
                
            }
        }


        if (hero == null) return;
        FollowHero();
        transform.position = Vector3.Lerp(transform.position, heroPos, 1 * Time.deltaTime);
    } 

    private void FollowHero()
    {

        if(hero.position.x - transform.position.x >= 1 || hero.position.x - transform.position.x <= -1)
        {
            heroPos = new Vector3(hero.position.x, hero.position.y, -10);
            
        }
        
    }
}
