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
    private List<ReferencePoint> referencePoints;
    private List<LocationQuality> locationQualities;
    private List<LocationMethod> locationMethods;

    public MediatorDatabase() 
    {
        this.dbCount = 0;
        this.geoGisDatabase = new GeoGisDatabase(this);
        this.jupiterDatabase = new JupiterDatabase(this);
    }

    public List<BoreholeType> BoreholeTypes
    {
        get
        {
            return boreholeTypes;
        }

        set
        {
            boreholeTypes = value;
        }
    }

    public List<ReferencePoint> ReferencePoints
    {
        get
        {
            return referencePoints;
        }

        set
        {
            referencePoints = value;
        }
    }

    public List<LocationQuality> LocationQualities
    {
        get
        {
            return locationQualities;
        }

        set
        {
            locationQualities = value;
        }
    }

    public List<LocationMethod> LocationMethods
    {
        get
        {
            return locationMethods;
        }

        set
        {
            locationMethods = value;
        }
    }

    public GeoGisDatabase GeoGisDatabase
    {
        get
        {
            return geoGisDatabase;
        }
    }

    public JupiterDatabase JupiterDatabase
    {
        get
        {
            return jupiterDatabase;
        }
    }

    public bool IsInitialized
    {
        get
        {
            return boreholeTypes != null && referencePoints != null && locationQualities != null && locationMethods != null;
        }
    }

    public void addExtraGeoGisBoreholeTypes()
    {
        List<BoreholeType> extraBoreholeTypes = geoGisDatabase.getBoreholeTypes();
        for (int i = 0; i < extraBoreholeTypes.Count; i++)
        {
            BoreholeType currentExtraBoreholeType = extraBoreholeTypes[i];
            bool extraBoreholeTypeFound = false;
            for (int j = 0; j < boreholeTypes.Count; j++)
            {
                BoreholeType currentBoreholeType = boreholeTypes[j];
                if (currentBoreholeType.Code.Equals(currentExtraBoreholeType.Code))
                {
                    extraBoreholeTypeFound = true;
                    break;
                }
            }
            if (!extraBoreholeTypeFound)
            {
                //Console.WriteLine("Adding: " + currentExtraBoreholeType.ToString());
                boreholeTypes.Add(currentExtraBoreholeType);
            }
        }
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

    public ReferencePoint getReferencePoint(String code)
    {
        if (code != null)
        {
            for (int i = 0; i < referencePoints.Count; i++)
            {
                if (referencePoints[i].Code.Equals(code))
                {
                    return referencePoints[i];
                }
            }
        }
        return null;
    }

    public LocationQuality getLocationQuality(String code)
    {
        if (code != null)
        {
            for (int i = 0; i < locationQualities.Count; i++)
            {
                if (locationQualities[i].Code.Equals(code))
                {
                    return locationQualities[i];
                }
            }
        }
        return null;
    }

    public LocationMethod getLocationMethod(String code)
    {
        if (code != null)
        {
            for (int i = 0; i < locationMethods.Count; i++)
            {
                if (locationMethods[i].Code.Equals(code))
                {
                    return locationMethods[i];
                }
            }
        }
        return null;
    }

    public void testGeoGis(Location location, float radius)
    {
        Debug.Log("Tester GeoGis");
        List<Borehole> boreholes = geoGisDatabase.getBoreholes(location, radius);
        if (boreholes != null)
        {
            if(boreholes.Count > 0)
            {
                for (int i = 0; i < boreholes.Count; i++)
                {

                    ScreenLogger.Instance.addText("GeoGis - " + boreholes[i].ToString());
                    //Debug.Log("GeoGIS - " + boreholes[i].ToString());
                    //Debug.Log("Distance: " + Utility.getDistanceBetweenLocations(location, boreholes[i].Location) / 10000 + " m");
                }
            }
            else
            {
                ScreenLogger.Instance.addText("Ingen GeoGis Boringer inden for valgte Radius");
            }
            
        }
        else
        {
            ScreenLogger.Instance.addText("Fejl ved hentning af GeoGis Boringer...");
        }
    }

    public void testJupiter(Location location, float radius)
    {
        Debug.Log("Tester Jupiter");
        List<Borehole> boreholes = jupiterDatabase.getBoreholes(location, radius);
        if (boreholes != null)
        {
            if (boreholes.Count > 0)
            {
                for (int i = 0; i < boreholes.Count; i++)
                {
                    ScreenLogger.Instance.addText("Jupiter - " + boreholes[i].ToString());
                    //Debug.Log("Jupiter - " + boreholes[i].ToString());
                    //Debug.Log("Distance: " + Utility.getDistanceBetweenLocations(location, boreholes[i].Location) + " m");
                }
            }
            else
            {
                ScreenLogger.Instance.addText("Ingen GeoGis Boringer inden for valgte Radius");
            }
        }
        else
        {
            ScreenLogger.Instance.addText("Fejl ved hentning af Jupiter Boringer...");
        }
    }
}
