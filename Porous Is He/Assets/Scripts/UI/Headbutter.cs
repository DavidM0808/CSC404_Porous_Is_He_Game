using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Headbutter : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private List<GameObject> triggersEntered = new List<GameObject>();

    [SerializeField] private Animator playerAnimator;

    private float lastHeadbutt = -1;
    private float headbuttCD = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Headbutt.started += Headbutt;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DoHeadbuttCollide() {

        bool hit = false;
        foreach (GameObject obj in triggersEntered)
        {
            if (obj.CompareTag("Collider"))
            {
                if (obj.GetComponent<HeavyCollider>())
                {
                    hit = true;
                    transform.parent.gameObject.GetComponent<PoSoundManager>().PlaySound("Headbutt_Hit");
                    obj.GetComponent<HeavyCollider>().push();
                }

                if (obj.GetComponent<Headbuttable>())
                {
                    hit = true;
                    transform.parent.gameObject.GetComponent<PoSoundManager>().PlaySound("Headbutt_Hit");
                    obj.GetComponent<Headbuttable>().push();
                }
            }
        }

        if (hit == false)
        {
            transform.parent.gameObject.GetComponent<PoSoundManager>().PlaySound("Headbutt_Miss");
        }

        lastHeadbutt = Time.time;
    }


    private void Headbutt(InputAction.CallbackContext context)
    {
        if (Time.time - lastHeadbutt > headbuttCD)
        {
            playerAnimator.Play("Headbutt");
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Collider"))
        {
            triggersEntered.Remove(other.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collider"))
        {
            triggersEntered.Add(other.gameObject);
        }
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Disable();
    }
}
