using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuWin;

    [SerializeField] TMP_Text collectibleText;

    public bool isPaused;
    public GameObject player;
    public playerController playerScript;

    float timeScaleOrig;

    int gameGoalCount;

    int collectiblesCurrent;
    int collectiblesNeeded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
    }

    private void Start()
    {
        updateCollectibleUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;

        if (gameGoalCount <= 0)
        {
            // You Win!!!
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void addCollectible()
    {
        collectiblesCurrent++;
        updateCollectibleUI();

        if (collectiblesCurrent >= collectiblesNeeded)
        {
            Debug.Log("All collectibles gathered!");
        }
    }

    public void registerCollectible()
    {
        collectiblesNeeded++;
        updateCollectibleUI();
    }

    void updateCollectibleUI()
    {
        if (collectibleText != null)
        {
            collectibleText.text = "Collectibles: " + collectiblesCurrent + " / " + collectiblesNeeded;
        }
    }

    public void youWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }
}
