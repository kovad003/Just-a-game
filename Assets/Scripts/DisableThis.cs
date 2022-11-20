using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// AUTHOR: @Daniel K.
///
/// The game object this script is being attached to will be set as inactive upon start.
/// </summary>
public class DisableThis : MonoBehaviour
{
    private void Start()
    {
        SetObjectAsInactive();
    }

    private void SetObjectAsInactive()
    {
        gameObject.SetActive(false);
    }
}
