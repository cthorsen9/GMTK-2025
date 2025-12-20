using UnityEngine;

public class LEGizmo : MonoBehaviour
{
    public Transform x;
    public Transform y;
    public Transform z;
    public Transform xy;
    public Transform xz;
    public Transform yz;

    public activeGizmo currentGizmo;

    public Vector3 GetDirection(Transform toUse)
    {
        if (toUse == x) return Vector3.right;
        if (toUse == y) return Vector3.up;
        if (toUse == z) return Vector3.forward;

        if (toUse == xy) return Vector3.right + Vector3.up;
        if (toUse == xz) return Vector3.right + Vector3.forward;
        if (toUse == yz) return Vector3.up + Vector3.forward;

        return Vector3.zero;
    }



}
