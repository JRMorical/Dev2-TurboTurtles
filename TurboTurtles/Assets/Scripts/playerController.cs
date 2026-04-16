using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] IncreaseRound _increaseRound;

    public int HP;
    public int speed;
    public int jumpMax;

    [Range(2, 5)][SerializeField] int sprintMod;
    [Range(5, 25)][SerializeField] int jumpSpeed;
    [Range(15, 50)][SerializeField] int gravity;

    public int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    public float damageReductionMultiplier = 1f;

    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform firePoint;

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
    float _vignetteIntensity;

    Vignette _vignette;

    Vector3 moveDir;
    Vector3 playerVel;

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

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel.y = 0;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            shoot();
        }
    }

    void sprint()
    {
        if (_increaseRound == null) return;

        if (_increaseRound.IsSprinting)
            speed = speedOrig * sprintMod;
        else
            speed = speedOrig;
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
        if (_increaseRound != null && !_increaseRound.CanShoot) return;
        if (_increaseRound != null) _increaseRound.UseMagic();

        shootTimer = 0;

        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
            targetPoint = hit.point;
        else
            targetPoint = Camera.main.transform.position + Camera.main.transform.forward * shootDist;

        if (fireballPrefab != null && firePoint != null)
        {
            GameObject fb = Instantiate(fireballPrefab, firePoint.position, Camera.main.transform.rotation);
            fb.GetComponent<Fireball>().targetPoint = targetPoint;
        }
    }

    public void takeDamage(int amount)
    {
        amount = Mathf.RoundToInt(amount * damageReductionMultiplier);
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            gamemanager.instance.youLose();
        }
    }

    public void Heal(int amount)
    {
        HP += amount;

        if (HP > HPOrig)
            HP = HPOrig;

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