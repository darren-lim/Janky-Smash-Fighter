using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 8;
    public float jumpForce = 10;
    public bool onGround = true;
    public bool faceRight = true;
    public Animator anim;
    Rigidbody playerRigidBody;
    public GameObject punchHitbox;
    public GameObject kickHitbox;
    public GameObject runningKickHitbox;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        onGround = true;
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
            }
            if (horizontal > 0.03f && !isFlyingKick && !isInAnimation)
            {
                faceRight = true;
                anim.SetBool("FaceLeft", false);

                if ((Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.L)))
                {
                    StartCoroutine(RunningKickCoroutine());
                }
            }
            else if (horizontal < -0.03f && !isFlyingKick && !isInAnimation)
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
                if (!isInAnimation)
                {
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
                if (horizontal <= 0.03f && horizontal >= -0.03f)
                {
                    transform.Translate(0, 0, 0);
                }
                else
                {
                    transform.Translate(horizontal, 0, 0);
                }
            }
        }
        else
        {
            // code flying back state when hit
            transform.Translate(horizontal, 0, 0);
        }


        // Animation
        if (horizontal < -0.03f || horizontal > 0.03f)
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
        // show hitboxes
        anim.SetTrigger("Punch");

        yield return new WaitForSeconds(0.5f);

        //remove hitboxes;
    }

    IEnumerator KickCoroutine()
    {
        // show hitboxes
        anim.SetTrigger("Kick");

        yield return new WaitForSeconds(0.5f);

        //remove hitboxes;
    }

    IEnumerator RunningKickCoroutine()
    {
        // show hitboxes
        anim.SetTrigger("RunKick");

        yield return new WaitForSeconds(0.4f);

        //remove hitboxes;
    }

    IEnumerator StartBlockCoroutine()
    {
        anim.SetBool("Block", true);
        anim.Play("Block", 0, 0.2f);
        yield return new WaitForSeconds(0.2f);

        if (Input.GetKey(KeyCode.J))
        {
            anim.speed = 0;
        }
        else
        {
            anim.SetBool("Block", false);
        }

    }

    IEnumerator OnHurtCoroutine()
    {
        anim.Play("Head Hit");

        yield return new WaitForSeconds(0.4f);


    }
}
