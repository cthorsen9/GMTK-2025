using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WheelController : MonoBehaviour
{
    public static WheelController singleton;

    public Transform trackTarget;

    public WheelCollider wCol;

    [SerializeField]
    Animator anim;

    [SerializeField]
    float rotModifier = .1f;

    Vector2 input;

    [SerializeField]
    float forwardForce = 10f;

    [SerializeField]
    float turnAmount = .5f;

    [SerializeField]    
    Transform wheelMesh;

    

    Vector3 startAngle;

    [Header("Lean Settings")]
    public Transform leanParent; // Assign the bike/vehicle body in Inspector

    [SerializeField]
    Transform slopeParent;

    [SerializeField] 
    float maxLeanAngle = 15f;

    [SerializeField] 
    float leanSpeed = 5f;

    [SerializeField] 
    float leanResetSpeed = 3f;

    [SerializeField] 
    float minSpeedForLean = 2f; // Avoid lean when almost stopped

    [SerializeField]
    LayerMask mask;

    //for matching slope
    RaycastHit hit;

    float additionalZLean = 0f;

    //jump settings-----------------------------------------------
    public Rigidbody rigid;
    
    //for coyote time
    bool canJump = false;

    [Header("Slope Matching")]
    [SerializeField] 
    float slopeMatchSpeed = 5f;  // How quickly to align with slopes

    [SerializeField] 
    float maxSlopeAngle = 45f;  // Maximum slope angle to attempt matching


    [Header("Jump variables")]

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
        if (!GameManager.singleton.timePlayer) return;

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

    public void PauseInput(InputAction.CallbackContext context)
    {
        if(context.started) GameManager.singleton.PauseUnpause();


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(singleton != null && singleton != this.gameObject)
        {
            Destroy(singleton.gameObject);
        }
        else if(singleton == null) singleton = this;


            //anim = GetComponentInChildren<Animator>();  
            wCol = GetComponent<WheelCollider>();
        //wheelMesh = transform.GetChild(0);
        rigid = GetComponentInParent<Rigidbody>();  
        startAngle = wheelMesh.localEulerAngles;
    }


    private void FixedUpdate()
    {
        Physics.Raycast(transform.position, -transform.up, out hit, 1.5f, mask);

        
        
    }
    // Update is called once per frame
    void Update()
    {
        GroundedComp();

        WheelMover();

        MeshUpdater();

        UpdateLean();

        UpdateSlopeMatching();

        groundedLastFrame = wCol.isGrounded;
    }


    //add some lean to the mesh when turning (sligth z rot) ------------------
    void WheelMover()
    {
        
        if(wCol.isGrounded) wCol.motorTorque = input.y * forwardForce;
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
        wheelMesh.localEulerAngles = startAngle + wCol.steerAngle * Vector3.up + wCol.rotationSpeed * Time.time * rotModifier * Vector3.right;
    }

    void UpdateLean()
    {
        if (!leanParent) return;

        float currentSpeed = Mathf.Abs(wCol.rpm * wCol.radius);
        float targetLeanZ = 0f;

        // Only lean if moving and turning
        if (currentSpeed > minSpeedForLean && Mathf.Abs(input.x) > 0.1f)
        {
            targetLeanZ = -input.x * maxLeanAngle; // Negative for natural tilt
        }

        // Smoothly interpolate the parent's Z rotation
        Quaternion currentRot = leanParent.localRotation;
        Quaternion targetRot = Quaternion.Euler(
            currentRot.eulerAngles.x,
            currentRot.eulerAngles.y,
            targetLeanZ
        );

        leanParent.localRotation = Quaternion.Slerp(
            currentRot,
            targetRot,
            (targetLeanZ == 0f ? leanResetSpeed : leanSpeed) * Time.deltaTime
        );
    }

    void UpdateSlopeMatching()
    {
        if (!slopeParent || hit.collider == null) return;

        // Only match slopes within reasonable angles
        if (Vector3.Angle(Vector3.up, hit.normal) <= maxSlopeAngle)
        {
            // Calculate target rotation based on normal
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            // Smoothly interpolate to match the slope
            slopeParent.rotation = Quaternion.Slerp(
                slopeParent.rotation,
                targetRotation,
                slopeMatchSpeed * Time.deltaTime
            );
        }
        else
        {
            // Smoothly return to upright position if slope is too steep
            slopeParent.rotation = Quaternion.Slerp(
                slopeParent.rotation,
                Quaternion.identity,
                slopeMatchSpeed * Time.deltaTime
            );
        }
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
