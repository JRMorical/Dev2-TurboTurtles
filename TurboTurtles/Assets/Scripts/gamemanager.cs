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
    [SerializeField] TMP_Text gameGoalCountText;

    [SerializeField] TMP_Text collectibleText;
    [SerializeField] TMP_Text scoreText;

    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text pointsText;

    [SerializeField] GameObject menuUpgrades;
    [SerializeField] GameObject menuSkillTree;

    public Image playerHPBar;
    public GameObject PlayerDamageFlashScreen;

    [SerializeField] int maxLevel = 20;
    [SerializeField] double nextLevel = 5;

    public int exp;
    public int level = 1;
    public int points;

    public bool isPaused;
    public GameObject player;
    public playerController playerScript;

    public System.Action<GameObject, GameObject> OnSuccessfulHit;

    float timeScaleOrig;

    int gameGoalCount;

    int collectiblesCurrent;
    int collectiblesNeeded;

    int score;
    int pointsPerKill = 100;

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
            else if (menuActive == menuPause || menuActive == menuUpgrades || menuActive == menuSkillTree)
            {
                stateUnpause();
            }
        }

        if (levelText != null)
            levelText.text = "Level: " + level + "/" + maxLevel;

        if (pointsText != null)
            pointsText.text = "Points: " + points;

        if (Input.GetKeyDown(KeyCode.U) && menuActive == null)
        {
            openUpgradeMenu();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuSkillTree;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuSkillTree)
            {
                stateUnpause();
            }
        }

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

        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
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
        gameGoalCountText.text = gameGoalCount.ToString("F0");

        if (gameGoalCount <= 0)
        {
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