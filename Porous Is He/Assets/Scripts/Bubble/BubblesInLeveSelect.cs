using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BubblesInLevelSelect : MonoBehaviour
{

    public static BubblesInLevelSelect control;
    public bool sceneIsSwitched = false;

    public string Level1Bubbles = "";
    public string Level2Bubbles = "";
    public string Level3Bubbles = "";

    public void Start()
    {
        if (control != null)
        {
            Destroy(this.gameObject);
            return;
        }

        control = this;
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

}
