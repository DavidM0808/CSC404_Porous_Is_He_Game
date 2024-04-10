using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This script is in charge of the "Bubbles"
// The main thing is that it has a floating animation
// As well as disappearing when collided by the player
public class BubbleScript : MonoBehaviour
{
    // Variables for bobbing up & down animation
    public float amplitude = 0.25f;
    public float frequency = 1.5f;
    private Vector3 posOffset = new Vector3 ();
    private Vector3 tempPosition = new Vector3 ();
    public bool popped = false;

    // Start is called before the first frame update
    public void Start()
    {
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
        tempPosition = transform.position;
        posOffset = tempPosition;
    }

    public void Update()
    {

        // Float up/down with a Sin()
        tempPosition = posOffset;
        tempPosition.y += Mathf.Sin (Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPosition;
    }

    public void OnTriggerEnter(Collider other) {
        if (popped == false && other.transform.gameObject.CompareTag("Player"))
        {

            GetComponent<AudioSource>().Play();

            GetComponent<Renderer>().enabled = false;
            
            GetComponent<Collider>().enabled = false;
            
            other.gameObject.GetComponent<BubbleCountingScript>().bubbles++;

            other.gameObject.GetComponent<BubbleCountingScript>().bubbleText.text = 
            other.gameObject.GetComponent<BubbleCountingScript>().bubbles.ToString();

            popped = true;

        }

    }
}
