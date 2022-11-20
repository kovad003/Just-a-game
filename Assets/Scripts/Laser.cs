using UnityEngine;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class Laser : MonoBehaviour
{
    private LineRenderer _laser;
    
    private void Start()
    {
        _laser = GetComponent<LineRenderer>();
    }

    // Line renderer works better in the LateUpdate.
    private void LateUpdate()
    {
        GenerateLaserBeam();
    }

    // Method Generates laser a beam for aiming.
    private void GenerateLaserBeam()
    {
        _laser.SetPosition(0, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider)
            {
                _laser.SetPosition(1, hit.point);
            }
        }
        else
        {
            _laser.SetPosition(1, transform.forward * 5000);
        }
    }
}