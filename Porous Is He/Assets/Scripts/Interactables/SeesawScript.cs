using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeesawScript : MonoBehaviour
{

    public GameObject PositiveZone;
    public GameObject NegativeZone;
    public GameObject Platform;

    private float rotSpdFactor = 0.2f;
    private float maxRotation = 30.0f;

    private GameObject Player;
    private int lastDirection;
    private bool hasHit;

    void Start()
    {
        Player = GameObject.Find("Player");
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Player.GetComponent<MoverScript>().onMovingPlatform = true;
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Player.GetComponent<LiquidTracker>().IsHeavy())
            {
                float maxDist = (NegativeZone.transform.position - PositiveZone.transform.position).magnitude / 2;
                float negativeDist = (NegativeZone.transform.position - other.transform.position).magnitude;
                float positiveDist = (PositiveZone.transform.position - other.transform.position).magnitude;

                float rotationSpdRatio = 0.0f;

                if (negativeDist < positiveDist)
                {
                    rotationSpdRatio = -maxDist / negativeDist;
                    if (lastDirection>-1){
                        gameObject.GetComponent<AudioSource>().Play();
                    }
                    lastDirection=-1;
                }
                else
                {
                    rotationSpdRatio = maxDist / positiveDist;
                       if (lastDirection<1){
                        gameObject.GetComponent<AudioSource>().Play();
                    }
                    lastDirection=1;
                }
                float rotationSpd = rotSpdFactor * rotationSpdRatio;

                float rot = Platform.transform.localRotation.eulerAngles.x + rotationSpd;
                if ( rot > 180 ){
                    rot -= 360;
                }

                if (rot >= maxRotation)
                {
                    Quaternion newRotation = Quaternion.Euler(maxRotation, Platform.transform.localRotation.eulerAngles.y, Platform.transform.localRotation.eulerAngles.z);
                    Platform.transform.localRotation = newRotation;
                    if (!hasHit){
                        PositiveZone.GetComponent<AudioSource>().Play();
                    }
                    hasHit=true;
                }
                else if (rot <= -maxRotation)
                {
                    Quaternion newRotation = Quaternion.Euler(-maxRotation, Platform.transform.localRotation.eulerAngles.y, Platform.transform.localRotation.eulerAngles.z);
                    Platform.transform.localRotation = newRotation;
                     if (!hasHit){
                        NegativeZone.GetComponent<AudioSource>().Play();
                    }
                    hasHit=true;
                }
                else
                {
                    Platform.transform.Rotate(rotationSpd, 0, 0, Space.Self);
                    hasHit=false;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Player.GetComponent<MoverScript>().onMovingPlatform = false;
        gameObject.GetComponent<AudioSource>().Stop();
    }
}
