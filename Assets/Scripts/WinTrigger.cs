using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    /* EXPOSED FIELDS: */
    [Header("CANVAS: ")]
    [SerializeField] private Canvas winCanvas;

    /* METHODS: */
    private void Start()
    {
        winCanvas.enabled = false;
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        winCanvas.enabled = true;
        Time.timeScale = 0;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }
}
