using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum activeGizmo { pos, rot, scale };

public class LevelEditorTools : MonoBehaviour
{


    //if greater than minTime
    bool clickHeld;

    //if less than minTime
    bool clickOnce;

    Vector2 mouseDelta;

    public RenderingLayerMask rLM;

    public RenderingLayerMask initialRLM;

    public LayerMask gizmoMask;

    //did we click on a gizmo
    public bool onGizmo;

    //save where we clicked from to draw a box!
    Vector2 initalClickPos;

    //out fabled minTime to know if its a click or hold, also in input settings, so kind a double set here
    [SerializeField]
    float minTime = .2f;

    

    float timer = 0f;


    //gizmo stuff!


    activeGizmo currentGizmo = activeGizmo.pos;

    [SerializeField]
    GameObject currentActiveGizmoParent;

    [SerializeField]
    float gizmoPositionSens = .1f;

    [SerializeField]
    float gizmoRotationSens = 1;

    Vector3 startRot;

    public Space currentGizmoSpace;

    [SerializeField]
    LEGizmo gizmoScript;

    Vector3 gizmoDirection;

    //the obj we just cliked, to converted to our selected object
    GameObject clickedObj;

    //out obj we have selected
    GameObject selectedObj;

    [SerializeField]
    float scaleFactor = 2f;

    [SerializeField]
    Camera cam;

    [SerializeField]
    RenderTexture rt;

    [SerializeField]
    RawImage raw;


    [SerializeField]
    List<LEGizmo> posRotScale = new List<LEGizmo>();


    //need to know if click has been held as well!

    /// <summary>
    /// we'll start a timer after a click, and if we're less than min time we'll count it as a hold.
    /// also should check if in the rect.
    /// </summary>
    /// <param name="context"></param>
    /// 
    public void Click(InputAction.CallbackContext context)
    {
        //if we click outside of the window and aren't still figureing out our state, don't do nun
        if (!LevelCreationTools.singleton.InGameViewBounds() && !clickHeld) return;

        //started runs each click
        if (context.started)
        {
            if(gizmoScript!=null) gizmoScript.target = null;
            initalClickPos = Input.mousePosition;

            //need an inital check to see if our inital click was on a gizmo or not!, if not on a gizmo we draw a box.
            if (TryGetWorldFromRenderTextureClick(out clickedObj)) TryGetWorldFromRenderTextureClick(out clickedObj, true);
            
            if (onGizmo) {

                if (clickedObj.transform.parent.TryGetComponent<LEGizmo>(out gizmoScript))
                {

                    Debug.Log("gotta da parente");
                    gizmoDirection = gizmoScript.GetDirection(clickedObj.transform);
                    gizmoScript.target = selectedObj.transform;
                    clickHeld = true;
                    //create our ctrl + z list here of what we change? -------- we need to know the previous state of what we selected,

                    return;
                }
                
            }

            clickOnce = true;

            timer = 0f;

            Debug.Log("started");

        }

        if (context.canceled)
        {

            if (!clickHeld && clickOnce)
            {
                SingleClick();
            }
            else //Debug.Log("Finished multi-select");

            clickHeld = false;
            clickOnce= false;
            onGizmo = false;
            startRot = Vector3.one * 1000f;
        }
        

        //if we cancel in the time before the hold time, we didn't hold
        //if(context.canceled) Debug.Log("canceled");

    }

    public void MouseDelta(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
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

    private void LateUpdate()
    {
        //this scales the gizmo to maintain size, not to be confused with the gizmo used to control the prefab scale
        ScaleGizmo();
    }




    //if canceled before mindays
    void SingleClick()
    {
        //Debug.Log("Single click");

        if(clickedObj == selectedObj) return;

        if (clickedObj) { clickedObj.GetComponent<Renderer>().renderingLayerMask = rLM; Debug.Log(clickedObj.name); }
        if(selectedObj) selectedObj.GetComponent<Renderer>().renderingLayerMask = initialRLM;

        if (clickedObj != null) 
        {
            selectedObj = clickedObj;

            currentActiveGizmoParent.transform.position = selectedObj.transform.position;
         }
    }

    //If held and not initally a gizmo
    void DrawBox()
    {

    }

    //need a seperate system here for gizmo interaction, may be easy?
    void InteractWithGizmo()
    {
        if(currentGizmo == activeGizmo.pos)
        {
            if (gizmoDirection.magnitude > 1f) Drag(true);
            else Drag();
        }
        else if(currentGizmo == activeGizmo.rot)
        {
            //Debug.Log("rotating");
            Rotate();
        }
        else
        {
            Scale();
        }
        

    }

    void Drag()
    {
        Debug.Log("touching 1D gizmo! " + gizmoDirection);

        if (currentGizmoSpace == Space.Self)
            gizmoDirection = clickedObj.transform.parent.TransformDirection(gizmoDirection);

        Vector3 worldPos = clickedObj.transform.parent.position;

        Vector2 screenOrigin = cam.WorldToScreenPoint(worldPos);
        Vector2 screenDir = ((Vector2)cam.WorldToScreenPoint(worldPos + gizmoDirection) - screenOrigin).normalized;

        

        float amount = Vector2.Dot(mouseDelta, screenDir);

        clickedObj.transform.parent.position += gizmoDirection * amount * gizmoPositionSens;
    }
    void Drag(bool planar)
    {
        Vector3 axisA = Vector3.zero;
        Vector3 axisB = Vector3.zero;

        if (gizmoDirection.x != 0)
            (axisA == Vector3.zero ? ref axisA : ref axisB) = Vector3.right;

        if (gizmoDirection.y != 0)
            (axisA == Vector3.zero ? ref axisA : ref axisB) = Vector3.up;

        if (gizmoDirection.z != 0)
            (axisA == Vector3.zero ? ref axisA : ref axisB) = Vector3.forward;

        if (axisA == Vector3.zero || axisB == Vector3.zero)
            return; // not actually a plane

        if (currentGizmoSpace == Space.Self)
        {
            axisA = clickedObj.transform.parent.TransformDirection(axisA);
            axisB = clickedObj.transform.parent.TransformDirection(axisB);
        }

        Vector3 worldPos = clickedObj.transform.parent.position;
        Vector2 screenOrigin = cam.WorldToScreenPoint(worldPos);

        Vector2 screenDirA = ((Vector2)cam.WorldToScreenPoint(worldPos + axisA) - screenOrigin).normalized;

        Vector2 screenDirB = ((Vector2)cam.WorldToScreenPoint(worldPos + axisB) - screenOrigin).normalized;


        float moveA = Vector2.Dot(mouseDelta, screenDirA);
        float moveB = Vector2.Dot(mouseDelta, screenDirB);

        clickedObj.transform.parent.position += axisA * moveA * gizmoPositionSens + axisB * moveB * gizmoPositionSens;
    }

    //WIP edit to actually rotate not move
    void Rotate()
    {
        if(startRot == Vector3.one * 1000f) startRot = clickedObj.transform.localEulerAngles;
        Debug.Log("touching 1D gizmo! " + gizmoDirection);

        if (currentGizmoSpace == Space.Self)
            gizmoDirection = clickedObj.transform.parent.TransformDirection(gizmoDirection);

        Vector3 worldPos = clickedObj.transform.parent.position;

        Vector2 screenOrigin = cam.WorldToScreenPoint(worldPos);
        Vector2 screenDir = ((Vector2)cam.WorldToScreenPoint(worldPos + gizmoDirection) - screenOrigin).normalized;


        float amount = Vector2.Dot(mouseDelta, screenDir);
        //Debug.Log(amount + "<- amount, aadded amount -> " + (gizmoDirection * amount * gizmoRotationSens));

        clickedObj.transform.parent.Rotate(startRot + gizmoDirection * amount * gizmoRotationSens);
        
    }


    //TODO - MAKE SCALE NOT MOVE
    void Scale()
    {
        Debug.Log("touching 1D gizmo! " + gizmoDirection);

        if (currentGizmoSpace == Space.Self)
            gizmoDirection = clickedObj.transform.parent.TransformDirection(gizmoDirection);

        Vector3 worldPos = clickedObj.transform.parent.position;

        Vector2 screenOrigin = cam.WorldToScreenPoint(worldPos);
        Vector2 screenDir = ((Vector2)cam.WorldToScreenPoint(worldPos + gizmoDirection) - screenOrigin).normalized;


        float amount = Vector2.Dot(mouseDelta, screenDir);

        clickedObj.transform.parent.position += gizmoDirection * amount * gizmoPositionSens;
    }

    void ScaleGizmo()
    {
        float distance = Vector3.Distance(cam.transform.position, currentActiveGizmoParent.transform.position);

        float scale = distance * scaleFactor;

        currentActiveGizmoParent.transform.localScale = Vector3.one * scale;
    }

    bool TryGetWorldFromRenderTextureClick(out GameObject hitObj, bool gizcast = false)
    {
        Debug.Log("Running cast");
        hitObj = default;

        if (!RectTransformUtility.RectangleContainsScreenPoint(raw.rectTransform,initalClickPos))
            return false;

        //Debug.Log("Made it past the rect contains screenpt");


        RectTransformUtility.ScreenPointToLocalPointInRectangle(raw.rectTransform, initalClickPos, null,out Vector2 local);

        Rect r = raw.rectTransform.rect;

        Vector2 uv = new Vector2((local.x - r.x) / r.width,(local.y - r.y) / r.height);

        Vector2 pixel = new Vector2( uv.x * rt.width,uv.y * rt.height);

        Ray ray = cam.ScreenPointToRay(new Vector3(pixel.x, pixel.y, 0));

        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        //Debug.Break();



        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hitObj = hit.collider.gameObject;
            onGizmo = false;
            if(!gizcast) return true;
        }
        if(gizcast && Physics.Raycast(ray, out RaycastHit hitta, 4000f, gizmoMask))
        {
            hitObj = hitta.collider.gameObject;
            onGizmo = true;
            return true;
        }
        onGizmo = false;
        //if this code is running as gizcast, we already hit an obj, so if we didnt hit a agizmo default to our old obj
        if(!gizcast) return false;
        else
        {
            hitObj = clickedObj;
            return false;
        }
    }




    //Button functions
    public void ChangeGizmo(string toUse)
    {
        activeGizmo gizmo;

        LEGizmo gizmoSc;

        if (toUse == "pos")
        {
            Debug.Log("switch to pos");
            gizmoSc = posRotScale[0];
            gizmo = activeGizmo.pos;
            if (currentGizmo == gizmo) return;

        }
        else if (toUse == "rot")
        {
            gizmoSc = posRotScale[1];
            gizmo = activeGizmo.rot;
            if (currentGizmo == gizmo) return;

        }
        else
        {
            gizmoSc = posRotScale[2];
            gizmo = activeGizmo.scale;
            if (currentGizmo == gizmo) return;

        }

        gizmoScript.gameObject.SetActive(false);

        gizmoScript = gizmoSc;

        if (selectedObj != null)
        {
            Debug.Log("selected obj not null");
            gizmoScript.transform.position = selectedObj.transform.position;

            gizmoScript.target = selectedObj.transform;
        }



        currentGizmo = gizmo;
    }
}
