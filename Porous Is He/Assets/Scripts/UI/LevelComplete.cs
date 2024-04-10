using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using static BubblesInLevelSelect;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] private GameObject LevelCompletePanel;
    [SerializeField] private GameObject LevelCompletePopup;
    [SerializeField] private GameObject selectFirst;
    private Animator animator;
    public string NextLevelScene;
    public TextMeshProUGUI BubblesCollected;
    private int allBubbles;
    public static bool LevelEnd;

    private GameManager gameManager;

    private BubbleCountingScript bubbleCounting;
    private BubblesInLevelSelect bubbleInLevelSelect;


    void Start()
    {
        gameManager = GameManager.Instance;

        LevelCompletePanel.SetActive(false);
        LevelEnd = false;
        animator = LevelCompletePopup.GetComponent<Animator>();
        allBubbles = GameObject.FindGameObjectsWithTag("Bubble").Length;

        bubbleCounting = GameObject.Find("Player").GetComponent<BubbleCountingScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.None;
            PauseMenu.isPaused = true;
            LevelCompletePanel.SetActive(true);
            BubblesCollected.text = "Bubbles Collected: " + bubbleCounting.bubbles.ToString() + "/" + allBubbles.ToString();
            EventSystem.current.SetSelectedGameObject(selectFirst);

            Scene scene = SceneManager.GetActiveScene();
            // Check the scene's name and set up the Bubbles GUI correctly
            GameObject selectBoard = GameObject.Find("SelectBoard");
            if (selectBoard)
            {
                bubbleInLevelSelect = selectBoard.GetComponent<BubblesInLevelSelect>();
                if (scene.name == "Level1")
                {
                    bubbleInLevelSelect.Level1Bubbles = bubbleCounting.bubbles.ToString() + "/" + allBubbles.ToString();
                }
                else if (scene.name == "Level2")
                {
                    bubbleInLevelSelect.Level2Bubbles = bubbleCounting.bubbles.ToString() + "/" + allBubbles.ToString();
                }
                else if (scene.name == "Level3")
                {
                    bubbleInLevelSelect.Level3Bubbles = bubbleCounting.bubbles.ToString() + "/" + allBubbles.ToString();
                }
            }
            

            if (animator != null && animator.isActiveAndEnabled)
            {
                animator.SetBool("open", true);
                StartCoroutine(WaitAnimationEnd());
            }
        }
    }

    IEnumerator WaitAnimationEnd()
    {
        while (!AnimatorFinished())
        {
            yield return null;
        }
        Time.timeScale = 0f;
        LevelEnd = true;
    }

    bool AnimatorFinished()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f;
    }

    public void NextLevel()
    {
        LevelEnd = false;
        if (NextLevelScene == "MainMenu")
            SceneManager.LoadScene(0);
        else
            SceneManager.LoadScene(NextLevelScene);
    }
    public void MainMenu()
    {
        LevelEnd = false;
        gameManager.MainMenu();
    }

}
