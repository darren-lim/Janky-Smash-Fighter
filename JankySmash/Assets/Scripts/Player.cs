using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float healthPercent = 0f;
    public float knockbackValue = 3f;
    public float moveSpeed = 7;
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
    public GameObject blockIndicator;


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
        blockIndicator.SetActive(false);
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
        float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        bool isInAnimation = anim.GetCurrentAnimatorStateInfo(0).IsName("Kicking") || anim.GetCurrentAnimatorStateInfo(0).IsName("Punching") || anim.GetCurrentAnimatorStateInfo(0).IsName("Head Hit") || anim.GetCurrentAnimatorStateInfo(0).IsName("Block");
        bool isFlyingKick = anim.GetCurrentAnimatorStateInfo(0).IsName("Flying Kick");

        if (onGround)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerRigidBody.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
                onGround = false;
                StopBlocking();
                anim.SetTrigger("Jump");
                return;
            }

            if (Input.GetKeyDown(KeyCode.K) && !isBlocking)
            {
                StartCoroutine(PunchCoroutine());
                return;
            }

            if (Input.GetKey(KeyCode.J) && !isInAnimation && !isFlyingKick && !isBlocking)
            {
                StartCoroutine(StartBlockCoroutine());
            }
            if (isBlocking && Input.GetKeyUp(KeyCode.J))
            {
                StopBlocking();
                anim.SetTrigger("BlockToIdle");
                anim.speed = 1;
            }

            if (!isInAnimation && !isFlyingKick)
            {
                if (horizontal > 0.01f)
                {
                    anim.SetBool("FaceLeft", false);
                    faceRight = true;

                    if (Input.GetKeyDown(KeyCode.L))
                    {
                        StartCoroutine(RunningKickCoroutine());
                    }
                }
                else if (horizontal < -0.01f)
                {
                    anim.SetBool("FaceLeft", true);
                    faceRight = false;

                    if (Input.GetKeyDown(KeyCode.L))
                    {
                        StartCoroutine(RunningKickCoroutine());
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.L))
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
        // add knockback
        if (isBlocking)
        {
            healthPercent += 2;
        }
        if (other.gameObject.tag == "Punch")
        {
            healthPercent += (3f + Random.Range(0.1f, 0.4f) * 3f);
            anim.Play("Head Hit");
        }
        else if (other.gameObject.tag == "Kick")
        {
            healthPercent += (7f + Random.Range(0.1f, 0.5f) * 5f);
            anim.Play("Head Hit");
        }
        else if (other.gameObject.tag == "RunKick")
        {
            healthPercent += (5f + Random.Range(0.1f, 0.7f) * 5f);
            anim.Play("Head Hit");
        }
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
        yield return new WaitForSeconds(0.15f);

        if (Input.GetKey(KeyCode.J) && onGround)
        {
            blockIndicator.SetActive(true);
            anim.speed = 0;
        }
        else
        {
            anim.SetBool("Block", false);
            isBlocking = false;
            blockIndicator.SetActive(false);
        }

    }

    void StopBlocking()
    {
        anim.speed = 1;
        anim.SetBool("Block", false);
        isBlocking = false;
        blockIndicator.SetActive(false);
    }
}
