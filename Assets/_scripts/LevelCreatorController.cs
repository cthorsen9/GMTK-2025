using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class LevelCreatorController : MonoBehaviour
{

    bool rightClickHeld = false;

    [SerializeField]
    [Tooltip("The game view window")]
    RectTransform window;


    Vector2 horiData = new Vector2();

    Vector2 horiCopy;

    Vector3 moveVect;

    float vertMove = 0;

    [SerializeField]
    float moveSpeed = 1f;

    [SerializeField]
    float vertSpeed = 1f;


    [SerializeField]
    Camera cam;

    public void HorizontalMove(InputAction.CallbackContext context)
    {
        if (!rightClickHeld) return;

        
        horiData = context.ReadValue<Vector2>();
            //Debug.Log(horiData);


        horiData.Normalize();
        

        

    }

    public void VerticalMove(InputAction.CallbackContext context)
    {
        if (!rightClickHeld) return;
        Debug.Log("readingInput QE");


        vertMove = context.ReadValue<float>();
        Debug.Log(vertMove);

    }


    public void RightClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 mPos = Input.mousePosition;
            
            if (RectTransformUtility.RectangleContainsScreenPoint(window, mPos))
            {
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
        Move();
       
    }

    //include WASD and QE move here
    void Move()
    {
        
        horiCopy = horiData * moveSpeed * Time.deltaTime;
        moveVect = new Vector3(horiCopy.x, vertMove * vertSpeed * Time.deltaTime, horiCopy.y);

        

        cam.transform.Translate(moveVect);
    }

    //mouse look if rclick held
    void Look()
    {

    }


}
