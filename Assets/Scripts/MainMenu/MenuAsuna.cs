using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAsuna : MonoBehaviour
{
    private MenuWeapon _menuWeapon;
    // Start is called before the first frame update
    void Start()
    {
        _menuWeapon = FindObjectOfType<MenuWeapon>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnFireEvent(AnimationEvent animationEvent)
    {
        _menuWeapon.Invoke("Shoot", 0);
    }

}
