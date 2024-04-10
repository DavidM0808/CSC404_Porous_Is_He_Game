using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SocialPlatforms.Impl;

public class ArsonistScript : MonoBehaviour
{

    private bool gameStarted = false;
    public float gameTime = 0;
    public float lastFire = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted == true) {

            gameTime += Time.deltaTime;

            float fireDelay = 5.0f;

            if (gameTime - lastFire > fireDelay)
            {
                //Debug.Log("Starting Fire");
                lastFire = gameTime;
                LightFire(2);
            }
        }
    }

    int CountFires()
    {
        GameObject Building = GameObject.Find("Building");
        int count = 0;
        for (int i = 0; i < Building.transform.childCount; i++)
        {
            Transform fire = Building.transform.GetChild(i);
            FireScript fireScript = fire.gameObject.GetComponent<FireScript>();
            if (fireScript.IsOnFire())
            {
                count++;
            }
        }

        return count;
    }

    void LightFire(int retries)
    {
        GameObject Building = GameObject.Find("BurningBuilding");
        Transform[] fires = gameObject.GetComponentsInChildren<Transform>(Building);
        int random = Random.Range(0, Building.transform.childCount);


        GameObject randomFire = Building.transform.GetChild(random).gameObject;
        FireScript fireScript = randomFire.GetComponent<FireScript>();
        if (fireScript.IsOnFire() && retries > 0)
        {
            retries--;
            LightFire(retries);
        }
        else
        {
            fireScript.Begin();
            //if (CountFires() >= 7)
            //{
            //    Stop();
            //}
        }
    }

    public void Begin()
    {
        gameStarted = true;
        gameTime = 0;
    }
    public void Stop()
    {
        gameStarted = false;

        GameObject GameInfo = GameObject.Find("GameInfo");
        GameScript gameScript = GameInfo.GetComponent<GameScript>();
        gameScript.Stop();
    }
}
