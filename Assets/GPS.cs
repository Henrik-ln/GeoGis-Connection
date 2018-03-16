using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPS : MonoBehaviour {

    public static GPS Instance { set; get; }

    public float? latitude;
    public float? longitude;
    public float? accuracy;
    public DateTime? lastUpdated;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartCoroutine(StartLocationService());
    }

    private IEnumerator StartLocationService()
    {
        ScreenLogger.Instance.addText("Location Finder started");
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            ScreenLogger.Instance.addText("Location service not enabled by user");
            emulateVIALocation();
            yield break;
        }

        // Start service before querying location
        Input.location.Start();
        //Input.location.Start(10, 0.1f); // accuracy, every 0.1m
        
        // Wait until service initializes
        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            ScreenLogger.Instance.addText("Timed out");
            emulateVIALocation();
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            ScreenLogger.Instance.addText("LocationService Failed");
            emulateVIALocation();
            yield break;
        }
        else if (Input.location.status == LocationServiceStatus.Stopped)
        {
            ScreenLogger.Instance.addText("LocationService never started");
            emulateVIALocation();
            yield break;
        }
        else if (Input.location.status == LocationServiceStatus.Initializing)
        {
            ScreenLogger.Instance.addText("LocationService still initializing?!");
            emulateVIALocation();
            yield break;
        }
        else if (Input.location.status == LocationServiceStatus.Running)
        {
            // Access granted and location value could be retrieved
            while (true)
            {
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
                dateTime = dateTime.AddSeconds(Input.location.lastData.timestamp);
                ScreenLogger.Instance.addText("Location: Lat: " + Input.location.lastData.latitude + ", Lon: " + Input.location.lastData.longitude + ", Alti: " + Input.location.lastData.altitude + ", Accuracy: " + Input.location.lastData.horizontalAccuracy + ", Tid: " + dateTime.ToString());
                this.latitude = Input.location.lastData.latitude;
                this.longitude = Input.location.lastData.longitude;
                this.accuracy = Input.location.lastData.horizontalAccuracy;
                this.lastUpdated = dateTime;
                yield return new WaitForSeconds(5);
            }

            // Stop service if there is no need to query location updates continuously
            //Input.location.Stop(); 
        }
        else
        {
            ScreenLogger.Instance.addText("LocationService Status Unknown");
            yield break;
        }
    }

    private void emulateVIALocation()
    {
        ScreenLogger.Instance.addText("Emulating VIA location");
        this.latitude = 55.871675f;
        this.longitude = 9.886150f;
        this.accuracy = 0;
        this.lastUpdated = DateTime.Now;
    }
}
