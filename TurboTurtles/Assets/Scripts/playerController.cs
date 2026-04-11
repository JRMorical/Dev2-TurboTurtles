using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] IncreaseRound _increaseRound;

    [Range(1, 10)][SerializeField] int HP;
    [Range(3, 7)][SerializeField] int speed;
    [Range(2, 5)][SerializeField] int sprintMod;
    [Range(5, 25)][SerializeField] int jumpSpeed;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(15, 50)][SerializeField] int gravity;

    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    int jumpCount;
    int HPOrig;
    int speedOrig;

    float shootTimer;

    Vector3 moveDir;    //WASD
    Vector3 playerVel;  //Player Velocity

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        speedOrig = speed;
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();
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
        //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); //This is world based movement. This works for Top-Down movement but not for any other type of movement.
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime); //Time.deltaTime helps keep pace between bad computers and good computers.

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

        if (_increaseRound != null)
            _increaseRound.UseMagic();
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            //Hey! I know this sucks but I am Dead...
            gamemanager.instance.youLose();
        }
    }
}
