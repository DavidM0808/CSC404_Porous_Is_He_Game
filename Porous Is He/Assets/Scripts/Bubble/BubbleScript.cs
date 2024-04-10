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


    private AudioSource _audio;
    private Renderer _renderer;
    private Collider _collider;
    private BubbleCountingScript bubbleCountingScript;

    // Start is called before the first frame update
    public void Start()
    {
        _audio = GetComponent<AudioSource> ();
        _renderer = GetComponent<Renderer> ();
        _collider = GetComponent<Collider> ();

        _renderer.enabled = true;
        _collider.enabled = true;
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

            _audio.Play();
            _renderer.enabled = false;
            _collider.enabled = false;

            bubbleCountingScript = other.gameObject.GetComponent<BubbleCountingScript>();
            bubbleCountingScript.bubbles++;
            bubbleCountingScript.bubbleText.text = bubbleCountingScript.bubbles.ToString();

            popped = true;

        }
    }
}
