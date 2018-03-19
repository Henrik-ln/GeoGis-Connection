using Database.model;
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
        radiusSlider.value = START_RADIUS*10;
        StartCoroutine(setText());
        radiusSlider.onValueChanged.AddListener(delegate { SetRadius(); });
    }

    public IEnumerator setText()
    {
        float? latitude = null;
        float? longitude = null;
        while (latitude == null || longitude == null)
        {
            if (GPS.Instance.latitude != null && GPS.Instance.longitude != null)
            {
                latitude = GPS.Instance.latitude;
                longitude = GPS.Instance.longitude;
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }
        //Location currentLocation = new Location(55.871675f, 9.886150f); //VIA location
        Location currentLocation = new Location((float)latitude, (float)longitude);
        Location outerRadiusLocation = new Location((float)latitude - radius, (float)longitude);

        double radiusInMeters = Utility.getDistanceBetweenLocations(currentLocation, outerRadiusLocation);
        radiusText.text = "Radius: " + radiusInMeters.ToString("0.0") + " m";
        yield break;
    }

    public void SetRadius()
    {
        this.radius = radiusSlider.value/10;
        StartCoroutine(setText());
    }

}
