//zzambari : stereoko 2017

using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.UI.Extensions;
//using System.Collections;
//using System.Collections.Generic;

public class zCameraInspectorHelper : MonoBehaviour
{
    [Header("Attach this to cameras parent")]
    [Header("Rotate")]

    [Range(-1, 1)]
    public float lookLeftRight;
    [Range(-1, 1)]
    public float lookUpDown;
    [Header("Move")]
    [Range(-1, 1)]
    public float panX;
    [Range(-1, 1)]
    public float panY;
    [Range(-1, 1)]
    public float track;

    [Header("Track")]
    [Range(0, 6)]
    public float distance = 3;
    [Range(180,10)]
    public float zoom = -1;
    void OnValidate()
    {
        Camera cam = GetComponentInChildren<Camera>();
        if (zoom == -1)
            zoom = cam.fieldOfView;
        else
            cam.fieldOfView = zoom;
        Vector3 currentRotation = transform.localRotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(currentRotation + new Vector3(lookUpDown * 2, -lookLeftRight * 2, 0));
        transform.localPosition = transform.localPosition + transform.right * panX / 5 + transform.up * panY / 5 + transform.forward * track / 5;
        panX = 0;
        panY = 0;
        track = 0;
        lookLeftRight = 0;
        lookUpDown = 0;
        cam.transform.localRotation = Quaternion.identity;
        cam.transform.localPosition = new Vector3(0, 0, -distance);
    }
}
