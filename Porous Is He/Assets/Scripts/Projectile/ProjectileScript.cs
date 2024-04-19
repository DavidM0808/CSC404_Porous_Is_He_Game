using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject player;
    
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponent<Collider>());
        Destroy (gameObject, 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Fire"))
        {
            /*FireScript fs = other.gameObject.GetComponent<FireScript>();

            if (fs.IsOnFire() == true)
            {
                fs.Stop();

            }*/
            // Put out fire
            Destroy(other.gameObject);
            
        }
        Destroy(gameObject);
    }
}
