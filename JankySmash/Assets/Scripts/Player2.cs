using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player2 : MonoBehaviour
{
    public float healthPercent = 0f;
    public TMP_Text playerHealthText;
    public float knockbackValue = 0f;
    public float baseKnockbackValue = 1f;
    public float punchKnockbackValue = 0.1f;
    public float kickKnockbackValue = 3f;
    public float runKickKnockbackValue = 5f;
    public float moveSpeed = 8;
    public float jumpForce = 15;
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
            playerRigidBody.AddForce(Physics.gravity * (playerRigidBody.mass * playerRigidBody.mass));
        }

    }

    public void Move()
    {
        //         float horizontal = 0f;
        // if (Input.GetKey(KeyCode.A))
        // {
        //     horizontal = -(Time.deltaTime * moveSpeed);
        // }
        // else if (Input.GetKey(KeyCode.D))
        // {
        //     horizontal = Time.deltaTime * moveSpeed;
        // }
        float horizontal = Input.GetAxis("Horizontal 2") * Time.deltaTime * moveSpeed;
        bool isInAnimation = anim.GetCurrentAnimatorStateInfo(0).IsName("Kicking") || anim.GetCurrentAnimatorStateInfo(0).IsName("Punching") || anim.GetCurrentAnimatorStateInfo(0).IsName("Head Hit") || anim.GetCurrentAnimatorStateInfo(0).IsName("Block");
        bool isFlyingKick = anim.GetCurrentAnimatorStateInfo(0).IsName("Flying Kick");

        if (playerRigidBody.velocity.x < -5 || playerRigidBody.velocity.x > 5)
        {
            playerRigidBody.velocity = new Vector3(playerRigidBody.velocity.x / 1.1f, playerRigidBody.velocity.y, 0);
            return;
        }

        if (onGround)
        {
            if ((Input.GetKeyDown(KeyCode.Space)) && !isFlyingKick)
            {
                playerRigidBody.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
                onGround = false;
                StopBlocking();
                anim.SetTrigger("Jump");
                return;
            }

            if ((Input.GetKey(KeyCode.J)) && !isInAnimation && !isFlyingKick && !isBlocking)
            {
                StartCoroutine(StartBlockCoroutine());
            }
            if (isBlocking && (Input.GetKeyUp(KeyCode.J)))
            {
                StopBlocking();
                anim.SetTrigger("BlockToIdle");
                anim.speed = 1;
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

            if (!isInAnimation && !isFlyingKick)
            {
                if (horizontal > 0.01f)
                {
                    anim.SetBool("FaceLeft", false);
                    faceRight = true;

                    if (Input.GetKeyDown(KeyCode.L))
                    {
                        StartCoroutine(RunningKickCoroutine());
                        return;
                    }
                    else if (Input.GetKeyDown(KeyCode.K))
                    {
                        StartCoroutine(KickCoroutine());
                        return;
                    }
                }
                else if (horizontal < -0.01f)
                {
                    anim.SetBool("FaceLeft", true);
                    faceRight = false;

                    if (Input.GetKeyDown(KeyCode.L))
                    {
                        StartCoroutine(RunningKickCoroutine());
                        return;
                    }
                    else if (Input.GetKeyDown(KeyCode.K))
                    {
                        StartCoroutine(KickCoroutine());
                        return;
                    }
                }
                else
                {
                    if ((Input.GetKeyDown(KeyCode.K)))
                    {
                        StartCoroutine(PunchCoroutine());
                    }
                    else if (Input.GetKeyDown(KeyCode.L))
                    {
                        // do special move
                        Debug.Log("Special Move");
                    }
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

        if ((Input.GetKey(KeyCode.J) || Input.GetButton("Block 2")) && onGround)
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

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") return;
        // add knockback
        playerRigidBody.velocity = Vector3.zero;
        if (isBlocking)
        {
            healthPercent += 2;
            playerHealthText.text = healthPercent.ToString("F1");
            knockbackValue = 3f + (healthPercent * baseKnockbackValue * 0.1f);
            return;
        }

        if (other.gameObject.tag == "LeftPunch")
        {
            healthPercent += (2f + Random.Range(0.1f, 0.4f) * 3f);
            playerRigidBody.AddForce(new Vector3(-knockbackValue * punchKnockbackValue, Random.Range(1, 3), 0), ForceMode.Impulse);
        }
        else if (other.gameObject.tag == "RightPunch")
        {
            healthPercent += (2f + Random.Range(0.1f, 0.4f) * 3f);
            playerRigidBody.AddForce(new Vector3(knockbackValue * punchKnockbackValue, Random.Range(1, 3), 0), ForceMode.Impulse);
        }
        else if (other.gameObject.tag == "LeftKick")
        {
            healthPercent += (6f + Random.Range(0.1f, 0.5f) * 5f);
            playerRigidBody.AddForce(new Vector3(-knockbackValue * kickKnockbackValue, Random.Range(1.5f, 2.5f) * knockbackValue, 0), ForceMode.Impulse);
        }
        else if (other.gameObject.tag == "RightKick")
        {
            healthPercent += (6f + Random.Range(0.1f, 0.5f) * 5f);
            playerRigidBody.AddForce(new Vector3(knockbackValue * kickKnockbackValue, Random.Range(1.5f, 2.5f) * knockbackValue, 0), ForceMode.Impulse);
        }
        else if (other.gameObject.tag == "LeftRunKick")
        {
            healthPercent += (3f + Random.Range(0.1f, 0.7f) * 5f);
            playerRigidBody.AddForce(new Vector3(-knockbackValue * runKickKnockbackValue, Random.Range(10, 20), 0), ForceMode.Impulse);
        }
        else if (other.gameObject.tag == "RightRunKick")
        {
            healthPercent += (3f + Random.Range(0.1f, 0.7f) * 5f);
            playerRigidBody.AddForce(new Vector3(knockbackValue * runKickKnockbackValue, Random.Range(10, 20), 0), ForceMode.Impulse);
        }
        anim.Play("Head Hit");
        playerHealthText.text = healthPercent.ToString("F1");

        // recalculate knockback value
        knockbackValue = 3f + (healthPercent * baseKnockbackValue * 0.1f);
    }

    void DoKnockBack(string tag)
    {
        // apply knockback physics
        playerRigidBody.velocity = Vector3.zero;
        if (tag == "LeftPunch" || tag == "LeftKick" || tag == "LeftRunKick")
        {
            if (isBlocking)
            {
                playerRigidBody.AddForce(new Vector3(-3, 3, 0), ForceMode.Impulse);
            }
            else
            {
                playerRigidBody.AddForce(new Vector3(-knockbackValue, 6, 0), ForceMode.Impulse);
            }
        }
        else if (tag == "RightPunch" || tag == "RightKick" || tag == "RightRunKick")
        {
            if (isBlocking)
            {
                playerRigidBody.AddForce(new Vector3(3, 3, 0), ForceMode.Impulse);
            }
            else
            {
                playerRigidBody.AddForce(new Vector3(knockbackValue, 6, 0), ForceMode.Impulse);
            }
        }

        // recalculate knockback value
        knockbackValue = 3f + (healthPercent * baseKnockbackValue * 0.1f);
    }
}
