using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    // ==================================================================================================
    private Vector3 CameraTargetPosition;
    public float RotationSpeed = 4.0f;
    public float SmoothFactor = 0.25f;

    private Vector3 CameraOffset;

    private Vector3 SmoothedTargetPosition;

    public bool LockedMouse;

    private float LastMouseX;

    // ================================================================
    void Start()
    {
        CameraTargetPosition = Vector3.zero;

        SmoothedTargetPosition = CameraTargetPosition;
        CameraOffset = transform.position - SmoothedTargetPosition;

        LastMouseX = 0.0f;

        LockedMouse = true;
    } // Start
    // ================================================================
    // Update is called once per frame
    void Update()
    {

        /*// Check Camera View Lock
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (LockedMouse) LockedMouse = false;
            else LockedMouse = true;
        }
        // Otherwise Scroll Left/ Right on Mouse 
        if (!LockedMouse)
        {
            LastMouseX = Input.GetAxis("Mouse X");

            Quaternion CameraTurnAngle = Quaternion.AngleAxis(LastMouseX * RotationSpeed, Vector3.up);
            CameraOffset = CameraTurnAngle * CameraOffset;
        }
        */

        // =========================================================================================
        // Now Perform Camera Pan
        Vector3 NewCameraPosition = CameraTargetPosition + CameraOffset;

        transform.position = Vector3.Slerp(transform.position, NewCameraPosition, SmoothFactor);

        transform.LookAt(CameraTargetPosition);

        // Zoom Controls
        if ((Input.GetKeyDown(KeyCode.DownArrow)) && (CameraOffset.magnitude < 50.0f)) CameraOffset = CameraOffset * 1.5f;
        if ((Input.GetKeyDown(KeyCode.UpArrow)) && (CameraOffset.magnitude > 10.0f)) CameraOffset = CameraOffset * 0.6666f;
        if ((Input.mouseScrollDelta.y > 0.0f) && (CameraOffset.magnitude > 10.0f)) CameraOffset = CameraOffset * 0.95f;
        if ((Input.mouseScrollDelta.y < 0.0f) && (CameraOffset.magnitude < 50.0f)) CameraOffset = CameraOffset * 1.1f;

        // Pan Control in liue of using Mouse Pan
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Quaternion CameraTurnAngle = Quaternion.AngleAxis(-2.5f * RotationSpeed, Vector3.up);
            CameraOffset = CameraTurnAngle * CameraOffset;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Quaternion CameraTurnAngle = Quaternion.AngleAxis(2.5f * RotationSpeed, Vector3.up);
            CameraOffset = CameraTurnAngle * CameraOffset;
        }

    }  // Update
       // ================================================================
}
