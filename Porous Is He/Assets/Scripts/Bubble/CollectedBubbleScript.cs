using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CollectedBubbleScript : MonoBehaviour
{

    private GameObject scoreText;
    public static int score;

    public void Start()
    {
        scoreText = GameObject.Find("BubbleCounter");

        if (scoreText == null)
        {
            Debug.Log("ERROR ERROR ERROR, TEXT IS NULL");
        }
        else
        {
            Debug.Log("NO ERROR");
        }
    }

    // public void Update()
    // {
    //     scoreText.GetComponent<Text>().text = "Bubbles: " + score;
    // }
}
