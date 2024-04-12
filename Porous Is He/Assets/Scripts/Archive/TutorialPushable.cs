using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPushable : MonoBehaviour
{

    [SerializeField] private GameObject obj;
    [SerializeField] private bool startPush = false;
    private Headbuttable pushable;

    void Start()
    {
        pushable = obj.GetComponent<Headbuttable>();
        pushable.SetAllow(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if ((startPush && !pushable.GetIsPushed()) || (!startPush && pushable.GetIsPushed()))
            {
                pushable.SetAllow(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pushable.SetAllow(false);
        }
    }




}
