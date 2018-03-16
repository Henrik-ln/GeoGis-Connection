using Database.model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class MediatorDatabase
{
    private DatabaseComponent databaseComponent;
    private int dbCount;

    private GeoGisDatabase geoGisDatabase;

    private List<BoreholeType> boreholeTypes;

    public MediatorDatabase()
    {
        this.dbCount = 0;
        this.geoGisDatabase = new GeoGisDatabase(this);

        this.boreholeTypes = new List<BoreholeType>();
        //this.boreholeTypes = geoGisDatabase.getBoreholeTypes();

    }

    public int getNewId()
    {
        return dbCount++;
    }

    public BoreholeType getBoreholeType(String code)
    {
        if (code != null)
        {
            for (int i = 0; i < boreholeTypes.Count; i++)
            {
                if (boreholeTypes[i].Code.Equals(code))
                {
                    return boreholeTypes[i];
                }
            }
        }
        return null;
    }

    public IEnumerator testGeoGis()
    {
        float radius = 0.01f;
        float? latitude = null;
        float? longitude = null;
        while(latitude == null || longitude == null)
        {
            if(GPS.Instance.latitude != null && GPS.Instance.longitude != null)
            {
                latitude = GPS.Instance.latitude;
                longitude = GPS.Instance.longitude;
                ScreenLogger.Instance.addText("Database Status: GPS Set");
            }
            else
            {
                ScreenLogger.Instance.addText("Database Status: No GPS yet");
                yield return new WaitForSeconds(1);
            }
        }
        //Location currentLocation = new Location(55.871675f, 9.886150f); //VIA location
        Location currentLocation = new Location((float)latitude, (float)longitude);
        List<Borehole> boreholes = geoGisDatabase.getBoreholes(currentLocation, radius);
        //String sql = "select PointNo, X1, Y1 from points";
        //geoGisDatabase.getDS("Goegis_TEST_db", sql);

        if (boreholes != null)
        {
            for (int i = 0; i < boreholes.Count; i++)
            {
                ScreenLogger.Instance.addText(boreholes[i].ToString());
                Debug.Log("GeoGIS - " + boreholes[i].ToString());
                //Debug.Log("Distance: " + Utility.getDistanceBetweenLocations(currentLocation, boreholes[i].Location) + " m");
            }
        }
        yield break;
    }
}
