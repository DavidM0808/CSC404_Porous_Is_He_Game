using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleDialogueScript : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered)
        {
            PoMessenger poMessenger = GameObject.Find("Player").GetComponent<PoMessenger>();
            PoMessage[] msg = { 
                new PoMessage("I think these bubbles are leading me somewhere...", 3)};
            //StartCoroutine(messenger.SendMessage(msg));
            poMessenger.AddReplayableMessage(msg);
        }

        isTriggered = true;
        
    }
}
