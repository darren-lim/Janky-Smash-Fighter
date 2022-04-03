using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 8;
    public float jumpForce = 10;
    public bool onGround = true;
    public bool faceRight = true;
    public bool isBlocking = false;
    public Animator anim;
    Rigidbody playerRigidBody;
    public GameObject leftPunchHitbox;
    public GameObject leftKickHitbox;
    public GameObject leftRunningKickHitbox;
    public GameObject rightPunchHitbox;
    public GameObject rightKickHitbox;
    public GameObject rightRunningKickHitbox;


    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        onGround = true;
        leftPunchHitbox.SetActive(false);
        leftKickHitbox.SetActive(false);
        leftRunningKickHitbox.SetActive(false);
        rightPunchHitbox.SetActive(false);
        rightKickHitbox.SetActive(false);
        rightRunningKickHitbox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void FixedUpdate()
    {
        if (!onGround)
        {
            GetComponent<Rigidbody>().AddForce(Physics.gravity * 1.5f, ForceMode.Acceleration);
        }

    }

    public void Move()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(OnHurtCoroutine());
        }
        float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        bool isInAnimation = anim.GetCurrentAnimatorStateInfo(0).IsName("Kicking") || anim.GetCurrentAnimatorStateInfo(0).IsName("Punching") || anim.GetCurrentAnimatorStateInfo(0).IsName("Head Hit") || anim.GetCurrentAnimatorStateInfo(0).IsName("Block");
        bool isFlyingKick = anim.GetCurrentAnimatorStateInfo(0).IsName("Flying Kick");

        if (onGround)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerRigidBody.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
                onGround = false;
                anim.SetTrigger("Jump");
                return;
            }

            if (Input.GetKeyDown(KeyCode.J) && !isInAnimation && !isFlyingKick)
            {
                StartCoroutine(StartBlockCoroutine());
            }
            if (Input.GetKeyUp(KeyCode.J))
            {
                anim.speed = 1;
                anim.SetBool("Block", false);
                isBlocking = false;
            }

            if (!isInAnimation && !isFlyingKick)
            {
                if (horizontal > 0.03f)
                {
                    faceRight = true;
                    anim.SetBool("FaceLeft", false);

                    if ((Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.L)))
                    {
                        StartCoroutine(RunningKickCoroutine());
                    }
                }
                else if (horizontal < -0.03f)
                {
                    faceRight = false;
                    anim.SetBool("FaceLeft", true);

                    if ((Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.L)))
                    {
                        StartCoroutine(RunningKickCoroutine());
                    }
                }
                else
                {
                    // punch/kick
                    if (Input.GetKeyDown(KeyCode.K))
                    {
                        StartCoroutine(PunchCoroutine());
                    }
                    else if (Input.GetKeyDown(KeyCode.L))
                    {
                        StartCoroutine(KickCoroutine());
                    }
                }
            }

            if (isFlyingKick)
            {
                if (faceRight) transform.Translate(Time.deltaTime * (moveSpeed - 2), 0, 0);
                else transform.Translate(Time.deltaTime * -(moveSpeed - 2), 0, 0);
            }
            else if (!isInAnimation)
            {
                transform.Translate(horizontal, 0, 0);
            }
        }
        else
        {
            // code flying back state when hit
            transform.Translate(horizontal, 0, 0);
        }


        // Animation
        if (horizontal != 0f)
        {
            anim.SetBool("Run", true);
        }
        else
        {
            anim.SetBool("Run", false);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("We got hit");
    }

    IEnumerator PunchCoroutine()
    {
        anim.SetTrigger("Punch");

        yield return new WaitForSeconds(0.1f);
        if (faceRight)
        {
            rightPunchHitbox.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            rightPunchHitbox.SetActive(false);
        }
        else
        {
            leftPunchHitbox.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            leftPunchHitbox.SetActive(false);
        }
    }

    IEnumerator KickCoroutine()
    {
        // show hitboxes
        anim.SetTrigger("Kick");

        yield return new WaitForSeconds(0.3f);
        if (faceRight)
        {
            rightKickHitbox.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            rightKickHitbox.SetActive(false);
        }
        else
        {
            leftKickHitbox.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            leftKickHitbox.SetActive(false);
        }
    }

    IEnumerator RunningKickCoroutine()
    {
        // show hitboxes
        anim.SetTrigger("RunKick");


        yield return new WaitForSeconds(0.2f);
        if (faceRight)
        {
            rightRunningKickHitbox.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            rightRunningKickHitbox.SetActive(false);
        }
        else
        {
            leftRunningKickHitbox.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            leftRunningKickHitbox.SetActive(false);
        }
    }

    IEnumerator StartBlockCoroutine()
    {
        anim.SetBool("Block", true);
        anim.Play("Block", 0, 0.2f);
        isBlocking = true;
        yield return new WaitForSeconds(0.2f);

        if (Input.GetKey(KeyCode.J))
        {
            anim.speed = 0;
        }
        else
        {
            anim.SetBool("Block", false);
            isBlocking = false;
        }

    }

    IEnumerator OnHurtCoroutine()
    {
        anim.Play("Head Hit");

        yield return new WaitForSeconds(0.4f);


    }
}
