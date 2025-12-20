using UnityEngine;
using UnityEngine.InputSystem;

public class LevelCreatorController : MonoBehaviour
{

    bool rightClickHeld = false;



    Vector2 horiData = new Vector2();

    Vector2 horiCopy;

    Vector3 moveVect;

    float vertMove = 0;

    [SerializeField]
    float moveSpeed = 1f;

    [SerializeField]
    float vertSpeed = 1f;

    [SerializeField]
    float lookSens = 1f;

    [Space]
    [Header("Acceleration settings")]
    [SerializeField]
    float accelerationRatePerSec = 1.1f;

    [SerializeField]
    float maxVelocity = 50f;

    [SerializeField]
    float accelStartBoost = 3f;

    float accelerator = 1f;

    float accelTimer = 0f;

    bool shouldAccelerate;
    

    //look settings
    Vector2 lookInput;

    float yaw;

    float pitch;



    [SerializeField]
    Camera cam;

    Vector3 initalCamPos;

    Quaternion initalRot;

    private void Start()
    {
        initalCamPos = cam.transform.position;
        initalRot = cam.transform.rotation; 
    }

    private void OnEnable()
    {
        cam.transform.SetPositionAndRotation(initalCamPos, initalRot);  
    }



    public void SpeedUpInput(InputAction.CallbackContext context)
    {
        if (!rightClickHeld) return;

        if (context.started)
        {
            accelerator = 1f;
            shouldAccelerate = true;


        }
        if (context.canceled)
        {
            shouldAccelerate = false;
            accelerator = 1f;
            accelTimer = 0f;
        }
    }

    public void HorizontalMove(InputAction.CallbackContext context)
    {
        if (!rightClickHeld) return;

        
        horiData = context.ReadValue<Vector2>();
            //Debug.Log(horiData);


        horiData.Normalize();
        

    }

    public void LookInput(InputAction.CallbackContext context)
    {
        if (!rightClickHeld) return;


        lookInput = context.ReadValue<Vector2>();
    }

    public void VerticalMove(InputAction.CallbackContext context)
    {
        if (!rightClickHeld) return;
        Debug.Log("readingInput QE");


        vertMove = context.ReadValue<float>();
        Debug.Log(vertMove);

    }


    //determines if we can move;
    public void RightClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
                        
            if (LevelCreationTools.singleton.InGameViewBounds())
            {
                //first reset our movement stuff, cuz it never got canceled
                yaw = cam.transform.localRotation.eulerAngles.y;
                pitch = cam.transform.localRotation.eulerAngles.x;

                vertMove = 0f;
                horiData = Vector2.zero;

                shouldAccelerate = false;
                accelerator = 1f;
                accelTimer = 0f;

                lookInput = Vector2.zero;


                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                rightClickHeld = true;

            }

        }
        if (context.canceled)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            rightClickHeld = false;

        }
    }

    

    void Update()
    {
        if (!rightClickHeld) return;

        Debug.Log(horiData + " " + vertMove);

        Look();

        if(shouldAccelerate) SpeedUp();

        Move();
       
    }

    //include WASD and QE move here
    void Move()
    {
        
        horiCopy = horiData * moveSpeed * Time.deltaTime * accelerator;

        moveVect = cam.transform.InverseTransformDirection(moveVect);

        moveVect = new Vector3(horiCopy.x, vertMove * vertSpeed * Time.deltaTime * accelerator, horiCopy.y);


        cam.transform.Translate(moveVect);
    }

    //mouse look if rclick held
    void Look()
    {
        yaw += lookInput.x * lookSens * Time.deltaTime;
        pitch -= lookInput.y * lookSens * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, -89f, 89f);

        cam.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    public void SpeedUp()
    {
        accelTimer += Time.deltaTime;   
        accelerator = accelerationRatePerSec * accelTimer + accelStartBoost;
        accelerator = Mathf.Clamp(accelerator, 0f, maxVelocity);
    }

}
