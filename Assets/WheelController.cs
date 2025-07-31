using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WheelController : MonoBehaviour
{
    WheelCollider wCol;

    [SerializeField]
    Animator anim;

    Vector2 input;

    [SerializeField]
    float forwardForce = 10f;

    [SerializeField]
    float turnAmount = .5f;

    [SerializeField]    
    Transform wheelMesh;

    Vector3 startAngle;

    [Header("Jump variables")]

    Rigidbody rigid;
    
    //for coyote time
    bool canJump = false;

    //how long to charge our jump
    [SerializeField]
    float TimeForMaxPower = 3f;

    [SerializeField]
    float maxJumpPower = 10f;

    [SerializeField]
    float minJumpPower = 2f;

    //when we started holding jump
    float jumpStartTime = 0f;

    [SerializeField]
    float coyoteTime = .2f;

    //how long we give as grace for early input
    [SerializeField]
    float earlyJumpForgivenessTime = .2f;

    //holder for above seconds to tell our code to jump on grounding
    bool jumpedEarly = false;

    bool groundedLastFrame = false;


    Coroutine coyoteTimer_;

    Coroutine jumpForgiveness_;



    public void MoveInput(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        

    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpStartTime = Time.time;
            anim.SetTrigger("startJump");
        }
        if (context.canceled)
        {
            if (wCol.isGrounded || canJump) Jump();
            else
            {
                if(jumpForgiveness_ != null) StopCoroutine(jumpForgiveness_);
                jumpForgiveness_ = StartCoroutine(EarlyJumpTimer());
            }
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //anim = GetComponentInChildren<Animator>();  
        wCol = GetComponent<WheelCollider>();
        //wheelMesh = transform.GetChild(0);
        rigid = GetComponentInParent<Rigidbody>();  
        startAngle = wheelMesh.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        GroundedComp();

        WheelMover();

        MeshUpdater();


        groundedLastFrame = wCol.isGrounded;
    }


    //add some lean to the mesh when turning (sligth z rot) ------------------
    void WheelMover()
    {
        wCol.motorTorque = input.y * forwardForce;
        wCol.steerAngle += input.x * turnAmount * Time.deltaTime;
        
    }

    //compares grounded data to tell us when to jump
    void GroundedComp()
    {
        if(wCol.isGrounded && jumpedEarly)
        {
            jumpedEarly = false;
            StopCoroutine(jumpForgiveness_);
            Jump();
        }
        if (groundedLastFrame && !wCol.isGrounded)
        {
            if (coyoteTimer_ != null) StopCoroutine(coyoteTimer_);
            coyoteTimer_ = StartCoroutine(CoyoteTimer());
        }
    }

    void MeshUpdater()
    {
        wheelMesh.localEulerAngles = startAngle + wCol.steerAngle * Vector3.up + wCol.rotationSpeed * Time.time * Vector3.right;
    }
    IEnumerator CoyoteTimer()
    {
        canJump = true;
        yield return new WaitForSeconds(coyoteTime);
        canJump = false;
    }

    IEnumerator EarlyJumpTimer()
    {
        yield return new WaitForSeconds(earlyJumpForgivenessTime);
        anim.SetTrigger("endJump");

    }

    //I AM SO FUCKING SMARRTTTTT RAHHHHH
    //we clamp the value between 0 and the time to build power then get the ratio of that to the total then use that to lerp between our min and max FIRST FUCKING TRYYYYYYY
    public void Jump()
    {
        anim.SetTrigger("endJump");
        float jumpForce = Mathf.Clamp(Time.time - jumpStartTime, 0, TimeForMaxPower);

        jumpForce /= TimeForMaxPower;

        jumpForce = Mathf.Lerp(minJumpPower, maxJumpPower, jumpForce);

        Debug.Log(jumpForce);

        rigid.AddForce(Vector3.up * jumpForce);
    }
}
