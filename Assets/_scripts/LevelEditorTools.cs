using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelEditorTools : MonoBehaviour
{
    //if greater than minTime
    bool clickHeld;

    //if less than minTime
    bool clickOnce;

    //did we click on a gizmo
    bool onGizmo;

    //save where we clicked from to draw a box!
    Vector2 initalClickPos;

    //out fabled minTime to know if its a click or hold, also in input settings, so kind a double set here
    [SerializeField]
    float minTime = .2f;


    float timer = 0f;


    //need to know if click has been held as well!

    /// <summary>
    /// we'll start a timer after a click, and if we're less than min time we'll count it as a hold.
    /// also should check if in the rect.
    /// </summary>
    /// <param name="context"></param>
    public void Click(InputAction.CallbackContext context)
    {
        //if we click outside of the window and aren't still figureing out our state, don't do nun
        if (!LevelCreationTools.singleton.InGameViewBounds() && !clickOnce) return;

        //started runs each click
        if (context.started)
        {
            //need an inital check to see if our inital click was on a gizmo or not!, if not on a gizmo we draw a box.-----------------------------TODO
            initalClickPos = Input.mousePosition;
            clickOnce = true;

            timer = 0f;

            Debug.Log("started");

        }

        if (context.canceled)
        {

            if (!clickHeld)
            {
                SingleClick();
            }
            else Debug.Log("Finished multi-select");

            clickHeld = false;
            clickOnce= false;
        }
        

        //if we cancel in the time before the hold time, we didn't hold
        if(context.canceled) Debug.Log("canceled");

    }

    private void Update()
    {
        if (clickOnce)
        {
            timer += Time.deltaTime;

            if(timer > minTime) clickHeld = true;
        }

        
    }

    private void FixedUpdate()
    {
        if(clickHeld && !onGizmo) DrawBox();
        if(clickHeld && onGizmo) InteractWithGizmo();
    }

    //if canceled before mindays
    void SingleClick()
    {
        Debug.Log("selecting this object");
    }

    //If held and not initally a gizmo
    void DrawBox()
    {

    }

    //need a seperate system here for gizmo interaction, may be easy?
    void InteractWithGizmo()
    {

    }



}
