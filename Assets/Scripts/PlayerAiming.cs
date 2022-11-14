using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class PlayerAiming : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 15.0f;
    [SerializeField] private float aimDuration = 0.3f;
    [SerializeField] private Rig aimLayer;
    [SerializeField] private GameObject rigLayers;
    [SerializeField] private GameObject holsteredPistol;
    private Camera _mainCamera;
    private RigBuilder _rigBuilder;
    private Animator _animator;
    /**************************************************************************************************************/
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _rigBuilder = GetComponent<RigBuilder>();
        _animator = GetComponent<Animator>();
    }
    
    // FixedUpdate() must be used bc player has physics and rigidbody.
    void FixedUpdate()
    {
        float playerCamera = _mainCamera.transform.rotation.eulerAngles.y;
        // Camera will blend in (on the y-axis) from current rotation towards the camera's rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, 
            Quaternion.Euler(0, playerCamera, 0), turnSpeed * Time.fixedDeltaTime );
    }

    // Update is enough for scanning user input.
    private void Update()
    {
        HolsterGun(KeyCode.H);
        MouseAim(Input.GetMouseButton(1));
    }
    /**************************************************************************************************************/
    private void MouseAim(bool mouseInput)
    {
        if (mouseInput)
        {
            aimLayer.weight += Time.deltaTime / aimDuration;
        }
        else
        {
            aimLayer.weight -= Time.deltaTime / aimDuration;
        }
    }
    
    private void HolsterGun(KeyCode key)
    {
        if (Input.GetKeyUp(key))
        {
            if (_rigBuilder.enabled)
            {
                // _rigBuilder.enabled = false;
                StartCoroutine(DisableRigBuilder());
                rigLayers.SetActive(false);
                _animator.SetTrigger("HolsterPistol");
                holsteredPistol.SetActive(true);
            }
            else
            {
                _animator.SetTrigger("UnholsterPistol");
                holsteredPistol.SetActive(false);
                rigLayers.SetActive(true);
                // _rigBuilder.enabled = true;
                StartCoroutine(EnableRigBuilder());
            }
        }
    }

    // The delay makes the transition better between animation states!
    // Events weren't working really well bc of the rig layers!
    IEnumerator EnableRigBuilder()
    {
        yield return new WaitForSeconds(0.6f);
        _rigBuilder.enabled = true;
    }
    IEnumerator DisableRigBuilder()
    {
        yield return new WaitForSeconds(0.1f);
        _rigBuilder.enabled = false;
    }
}
