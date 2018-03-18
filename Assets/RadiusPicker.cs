using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadiusPicker : MonoBehaviour {

    public static RadiusPicker Instance { set; get; }

    public Text radiusText;
    public Slider radiusSlider;

    public float radius;
    public readonly float START_RADIUS = 0.005f;
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        radius = START_RADIUS;
    }

    // Use this for initialization
    void Start()
    {
        radiusSlider.value = START_RADIUS;
        radiusText.text = "Radius: " + START_RADIUS.ToString("0.000");
        radiusSlider.onValueChanged.AddListener(delegate { SetRadius(); });
    }

    public void SetRadius()
    {
        this.radius = radiusSlider.value;
        radiusText.text = "Radius: " + radius.ToString("0.000");
    }

}
