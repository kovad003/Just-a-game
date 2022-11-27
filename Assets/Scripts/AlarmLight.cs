using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmLight : MonoBehaviour
{
    [SerializeField] private float criticalIntensity = 0.8f;
    private Renderer _renderer;
    private Light _redLight;
    private Color _startColor;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    // Start is called before the first frame update
    void Start()
    {
        _renderer = gameObject.GetComponent<Renderer>();
        _redLight = FindObjectOfType<Light>();

        _startColor = _renderer.material.GetColor(EmissionColor);
    }

    // Update is called once per frame
    void Update()
    {
        
        _redLight.intensity = Mathf.PingPong(Time.time, 3);
        if (_redLight.intensity <= criticalIntensity)
        {
            _renderer.material.SetColor(EmissionColor, Color.black);
            // _renderer.material.SetColor("_EmissionColorUI", Color.black);
        }
        else
        {
            _renderer.material.SetColor(EmissionColor, _startColor);
            // _renderer.material.SetColor("_EmissionColorUI", _startColor);
        }
    }
}
