using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAiming : MonoBehaviour
{
    public float turnSpeed = 15.0f;
    public float aimDuration = 0.3f;
    private Camera _mainCamera;
    [SerializeField] private Rig aimLayer;
    
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    // FixedUpdate() must be used bc player has physics and rigidbody.
    void FixedUpdate()
    {
        float playerCamera = _mainCamera.transform.rotation.eulerAngles.y;
        // Camera will blend in (on the y-axis) from current rotation towards the camera's rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, playerCamera, 0), turnSpeed * Time.fixedDeltaTime );
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            aimLayer.weight += Time.deltaTime / aimDuration;
        }
        else
        {
            aimLayer.weight -= Time.deltaTime / aimDuration;
        }
    }
}
