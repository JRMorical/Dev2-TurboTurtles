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
    [SerializeField] TMP_Text scoreText;

    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text pointsText;

    [SerializeField] GameObject menuUpgrades;

    [SerializeField] int maxLevel = 20;
    [SerializeField] double nextLevel = 5;

    public int exp;
    public int level = 1;
    public int points;

    public bool isPaused;
    public GameObject player;
    public playerController playerScript;

    float timeScaleOrig;

    int gameGoalCount;

    int collectiblesCurrent;
    int collectiblesNeeded;

    int score;
    int pointsPerKill = 100;

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
        updateScoreUI();
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

        // Level UI
        if (levelText != null)
            levelText.text = "Level: " + level + "/" + maxLevel;

        if (pointsText != null)
            pointsText.text = "Points: " + points;

        // Open upgrade menu
        if (Input.GetKeyDown(KeyCode.U) && menuActive == null)
        {
            openUpgradeMenu();
        }
        else if (menuActive == menuUpgrades && Input.GetButtonDown("Cancel"))
        {
            stateUnpause();
        }

        // Level logic
        if (exp >= nextLevel && level < maxLevel)
        {
            level++;
            points += 3;

            nextLevel = (nextLevel * 1.3) + 2;
            exp = 0;

            openUpgradeMenu();
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

    public void AddScore()
    {
        score += pointsPerKill;
        updateScoreUI();
    }

    void updateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score " + score;
        }
    }

    public void openUpgradeMenu()
    {
        statePause();
        menuActive = menuUpgrades;
        menuActive.SetActive(true);
    }
}
