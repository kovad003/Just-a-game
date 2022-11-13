using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAiming : MonoBehaviour
{
    public float turnSpeed = 15;
    private Camera _mainCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
        Cursor.visible = false;
    }
    
    // FixedUpdate() must be used bc player has physics and rigidbody.
    void FixedUpdate()
    {
        float playerCamera = _mainCamera.transform.rotation.eulerAngles.y;
        // Camera will blend in (on the y-axis) from current rotation towards the camera's rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, playerCamera, 0), turnSpeed * Time.fixedDeltaTime );
    }
}
