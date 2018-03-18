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
    private JupiterDatabase jupiterDatabase;

    private List<BoreholeType> boreholeTypes;

    public MediatorDatabase()
    {
        this.dbCount = 0;
        this.geoGisDatabase = new GeoGisDatabase(this);
        this.jupiterDatabase = new JupiterDatabase(this);

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

    public void testGeoGis(Location location, float radius)
    {
        ScreenLogger.Instance.addText("Henter GeoGis Boringer...");
        List<Borehole> boreholes = geoGisDatabase.getBoreholes(location, radius);
        if (boreholes != null)
        {
            for (int i = 0; i < boreholes.Count; i++)
            {
                //Debug.Log("GeoGIS - " + boreholes[i].ToString());
                ScreenLogger.Instance.addText("GeoGis - " + boreholes[i].ToString());
                Debug.Log("Distance: " + Utility.getDistanceBetweenLocations(location, boreholes[i].Location) + " m");
            }
        }
        else
        {
            ScreenLogger.Instance.addText("Fejl ved hentning af GeoGis Boringer...");
        }
    }

    public void testJupiter(Location location, float radius)
    {
        ScreenLogger.Instance.addText("Henter Jupiter Boringer...");
        List<Borehole> boreholes = jupiterDatabase.getBoreholes(location, radius);
        if (boreholes != null)
        {
            for (int i = 0; i < boreholes.Count; i++)
            {
                ScreenLogger.Instance.addText("Jupiter - " + boreholes[i].ToString());
                //Debug.Log("Jupiter - " + boreholes[i].ToString());
                Debug.Log("Distance: " + Utility.getDistanceBetweenLocations(location, boreholes[i].Location) + " m");
            }
        }
        else
        {
            ScreenLogger.Instance.addText("Fejl ved hentning af Jupiter Boringer...");
        }
    }
}
