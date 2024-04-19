using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{

    bool gameStarted = false;
    int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        Begin();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Begin()
    {
        gameStarted = true;
        score = 0;

        this.gameObject.GetComponent<ArsonistScript>().Begin();
    }
    public void Stop()
    {
        gameStarted = false;
        ReturnToMenu();
    }
    public bool IsPlaying()
    {
        return gameStarted;
    }
    public void AddScore(int score)
    {
        score += score;
    }
    public int GetScore()
    {
        return score;
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("GameScene");
    }
}
