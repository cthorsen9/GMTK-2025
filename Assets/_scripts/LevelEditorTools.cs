using Unity.VisualScripting;
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

    public RenderingLayerMask rLM;

    public RenderingLayerMask initialRLM;

    //did we click on a gizmo
    bool onGizmo;

    //save where we clicked from to draw a box!
    Vector2 initalClickPos;

    //out fabled minTime to know if its a click or hold, also in input settings, so kind a double set here
    [SerializeField]
    float minTime = .2f;


    float timer = 0f;


    //gizmo stuff!


    activeGizmo currentGizmo;

    [SerializeField]
    GameObject currentActiveGizmoParent;

    LEGizmo gizmoScript;

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
            initalClickPos = Input.mousePosition;

            //need an inital check to see if our inital click was on a gizmo or not!, if not on a gizmo we draw a box.
            TryGetWorldFromRenderTextureClick(out clickedObj);

            if (clickedObj != null) {
                if (clickedObj.transform.parent != null)
                {
                    if (clickedObj.transform.parent.TryGetComponent<LEGizmo>(out gizmoScript))
                    {
                        InteractWithGizmo();
                        return;
                    }
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

    private void LateUpdate()
    {
        ScaleGizmo();
    }

    //if canceled before mindays
    void SingleClick()
    {
        Debug.Log("Single click");

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
        //fuhhhhhh we need to do a double raycast, one taht just shoots on the gizmo layer :OOOOOOOOOOOO
        Debug.Log("touching gizmo!");
    }

    void ScaleGizmo()
    {
        float distance = Vector3.Distance(cam.transform.position, currentActiveGizmoParent.transform.position);

        float scale = distance * scaleFactor;

        currentActiveGizmoParent.transform.localScale = Vector3.one * scale;
    }

    bool TryGetWorldFromRenderTextureClick(out GameObject hitObj)
    {
        Debug.Log("Running cast");
        hitObj = default;

        if (!RectTransformUtility.RectangleContainsScreenPoint(raw.rectTransform,initalClickPos))
            return false;

        Debug.Log("Made it past the rect contains screenpt");


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
            return true;
        }

        return false;
    }



}
