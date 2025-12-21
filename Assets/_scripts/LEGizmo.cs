using UnityEngine;

public class LEGizmo : MonoBehaviour
{
    public Transform x;
    public Transform y;
    public Transform z;
    public Transform xy;
    public Transform xz;
    public Transform yz;
    public Transform all;

    public activeGizmo currentGizmo;

    public Transform target;

    

    public Vector3 GetDirection(Transform toUse)
    {
        if (toUse == x) return Vector3.right;
        if (toUse == y) return Vector3.up;
        if (toUse == z) return Vector3.forward;

        if (toUse == xy) return Vector3.right + Vector3.up;
        if (toUse == xz) return Vector3.right + Vector3.forward;
        if (toUse == yz) return Vector3.up + Vector3.forward;
        if(toUse == all) return Vector3.one;

        return Vector3.zero;
    }

    private void Update()
    {

        if(target == null) return;  
        if (currentGizmo == activeGizmo.pos)
        {
            target.position = transform.position;
        }
        if(currentGizmo == activeGizmo.rot)
        {
            target.rotation = transform.rotation;
        }
        if(currentGizmo == activeGizmo.scale)
        {
            target.localScale = transform.localScale;
        }
    }


}
