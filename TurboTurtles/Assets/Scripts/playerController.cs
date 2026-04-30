using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] IncreaseRound _increaseRound;

    public int HP;
    public int speed;
    [SerializeField] float currentMoveSpeed;
    public float speedMultiplier = 1f;
    public int jumpMax;

    [Range(2, 5)][SerializeField] int sprintMod;
    [Range(5, 25)][SerializeField] int jumpSpeed;
    [Range(15, 50)][SerializeField] int gravity;

    public int bonusDamage = 0;
    [SerializeField] int spellDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] float emptyCooldownTime = 2f;
    public float damageReductionMultiplier = 1f;

    [SerializeField] Transform handPoint;

    [SerializeField] Volume postProcessVolume;
    [SerializeField] float pulseThreshold = 0.40f;
    [SerializeField] float pulseSpeedMin = 0.8f;
    [SerializeField] float pulseSpeedMax = 3.5f;
    [SerializeField] float pulseAmplitude = 0.18f;
    [SerializeField] float intensityLowHP = 0.65f;
    [SerializeField] float lerpSpeed = 4f;

    int jumpCount;
    public int HPOrig;
    public int speedOrig;

    float shootTimer;
    float emptyCooldown = 0f;
    float _vignetteIntensity;

    Vignette _vignette;

    Vector3 moveDir;
    Vector3 playerVel;

    List<SpellStats> spellList = new List<SpellStats>();
    List<GameObject> staffObjects = new List<GameObject>();
    int spellListPos = 0;

    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
        speedOrig = speed;
        
        if (postProcessVolume != null)
            postProcessVolume.profile.TryGet(out _vignette);
    }

    void Update()
    {
        movement();
        sprint();
        updateVignette();
        UpdateFinalDamage();
        CurrentSpeed();
    }

    void updateVignette()
    {
        if (_vignette == null)
        {
            if (postProcessVolume == null) return;
            if (!postProcessVolume.profile.TryGet(out _vignette)) return;
        }

        float healthPct = Mathf.Clamp01((float)HP / HPOrig);
        float baseIntensity = Mathf.Lerp(intensityLowHP, 0f, healthPct);
        float target = baseIntensity;

        if (healthPct <= pulseThreshold)
        {
            float danger = 1f - (healthPct / pulseThreshold);
            float pulseSpeed = Mathf.Lerp(pulseSpeedMin, pulseSpeedMax, danger);
            float pulse = Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f);
            target += pulse * pulseAmplitude * danger;
            _vignette.color.Override(Color.Lerp(Color.black, new Color(0.55f, 0f, 0f), danger));
        }
        else
        {
            _vignette.color.Override(Color.black);
        }

        _vignetteIntensity = Mathf.Lerp(_vignetteIntensity, target, Time.deltaTime * lerpSpeed);
        _vignette.intensity.Override(Mathf.Clamp(_vignetteIntensity, 0f, 1f));
    }

    void movement()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.yellow);

        shootTimer += Time.deltaTime;

        if (emptyCooldown > 0f)
            emptyCooldown -= Time.deltaTime;

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel.y = 0;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * speedMultiplier * Time.deltaTime);

        jump();
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (spellList.Count > 0)
        {
            float currentRate = spellList[spellListPos].castRate;

            if (Input.GetButton("Fire1") && shootTimer >= currentRate)
            {
                if (_increaseRound != null && !_increaseRound.CanShoot)
                {
                    // out of magic — lock shooting
                    emptyCooldown = emptyCooldownTime;
                    shootTimer = 0;
                }
                else if (emptyCooldown <= 0f)
                {
                    shoot();
                }
            }
        }

        selectStaff();
    }

    void sprint()
    {
        //if (_increaseRound == null) return;

        //if (_increaseRound.IsSprinting)
        //    speed = speedOrig * sprintMod;
        //else
        //    speed = speedOrig;
        if (_increaseRound.IsSprinting)
        {
            speedMultiplier = sprintMod;
        }
        else
        {
            speedMultiplier = 1f;
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpSpeed;
            jumpCount++;
        }
    }

    void shoot()
    {
        if (_increaseRound != null) _increaseRound.UseMagic();

        // if magic just ran out start cooldown
        if (_increaseRound != null && !_increaseRound.CanShoot)
            emptyCooldown = emptyCooldownTime;

        shootTimer = 0;

        SpellStats current = spellList[spellListPos];
        int finalDamage = current.damage + bonusDamage; 
        switch (current.spellType)
        {
            case SpellType.Fire:
            case SpellType.Ice:
                Vector3 spawnPos = handPoint.position + Vector3.up * 1.1f;
                Vector3 targetPoint = spawnPos + Camera.main.transform.forward * current.castDist;
                GameObject proj = Instantiate(current.projectilePrefab, spawnPos, Quaternion.identity);
                SpellProjectile sp = proj.GetComponent<SpellProjectile>();
                if (sp != null)
                    sp.Init(targetPoint, finalDamage, current.projectileSpeed, current.slowAmount, current.slowDuration); 
                break;

            case SpellType.Shock:
                ShockSpell.Fire(current, ignoreLayer);
                break;
        }
    }

    public void PickupSpell(SpellStats stats)
    {
        if (spellList.Contains(stats)) return;

        spellList.Add(stats);

        if (stats.staffPrefab != null && handPoint != null)
        {
            GameObject obj = Instantiate(stats.staffPrefab, handPoint.position, handPoint.rotation, handPoint);
            staffObjects.Add(obj);
        }
        else
        {
            staffObjects.Add(null);
        }

        spellListPos = spellList.Count - 1;
        equipStaff();
    }

    void equipStaff()
    {
        for (int i = 0; i < staffObjects.Count; i++)
        {
            if (staffObjects[i] != null)
                staffObjects[i].SetActive(i == spellListPos);
        }
    }
    void UpdateFinalDamage()
    {
        if (spellList.Count > 0)
        {
            SpellStats current = spellList[spellListPos];
            spellDamage = current.damage + bonusDamage;
        }
        else
        {
            spellDamage = bonusDamage;
        }
    }
    void CurrentSpeed()
    {
        currentMoveSpeed = speed * speedMultiplier;
    }
    void selectStaff()
    {
        if (spellList.Count <= 1) return;

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && spellListPos < spellList.Count - 1)
        {
            spellListPos++;
            equipStaff();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && spellListPos > 0)
        {
            spellListPos--;
            equipStaff();
        }
    }

    public void takeDamage(int amount)
    {
        amount = Mathf.RoundToInt(amount * damageReductionMultiplier);
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamage());

        if (HP <= 0)
            gamemanager.instance.HandlePlayerDeath();
    }

    public void Heal(int amount)
    {
        HP += amount;
        if (HP > HPOrig) HP = HPOrig;
        updatePlayerUI();
    }

    public void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    IEnumerator flashDamage()
    {
        gamemanager.instance.PlayerDamageFlashScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.PlayerDamageFlashScreen.SetActive(false);
    }
}