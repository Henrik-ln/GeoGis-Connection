using Database.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

class GeoGisDatabase
{
    private readonly string GEOGIS_DBNAME = "Goegis_TEST_db";
    private readonly string GEOGIS_USERNAME = "HenrikLodsgaardNielsen";
    private readonly string GEOGIS_PASSWORD = "HenrikLodsgaardNielsen1229";
    private readonly string GEOGIS_WEBSERVICE_URL = "http://geogis.rsyd.dk/service.asmx";

    private MediatorDatabase mediatorDatabase;
    private Service client;

    public GeoGisDatabase(MediatorDatabase mediatorDatabase)
    {
        this.mediatorDatabase = mediatorDatabase;
        client = new Service(GEOGIS_WEBSERVICE_URL);

    }

    public List<Borehole> getBoreholes(Location currentLocation, double radius)
    {
        List<Borehole> boreholes = null;
        /*
         * pointid = Id i database - Guid
         * pointno = punkt nummer - Guid
         * publicno = DGU nummer - String
         * PointType = punkttype - String - Table: PointTypes
         * Purpose = Formål - String - Table: PointPurposes
         * Projection1 = Primær Projektion: EPSG - Integer - Table: Projections
         * Projection2
         * X1 = Primær X koordinat : float
         * Y1 = Primær Y koordinat : float
         * Z1 = primær z koordinat - Reference Niveau : float
         * ElevationMethod1 = Primær kote metode  - String - Table: PointElevationMethods
         * CoordinateQuality1 = Primær koordinat kvalitet - String - Table: Pointcoordinatequalities
         * CoordinateMethod1 = Primær koordinat metode - String - Table: Pointcoordinatemethods
         * VerticalRefId1 - Højdesystem 1 - String - Table: VerticalRefs
         * ZDVR90 - DVR90 - Double
         * Top - Dybde til top af boring - float
         * Bottom - Dybde til bund af boring [m] - float
         * JupiterId - JupiterId - Int - Check om allerede har data?
        */
        radius = radius * 100000;
        LatLngUTMConverter latLngUTMConverter = new LatLngUTMConverter("EUREF89");
        LatLngUTMConverter.UTMResult utmResult = latLngUTMConverter.convertLatLngToUtm(currentLocation.X, currentLocation.Y);
        //Debug.Log(utmResult.ToString());
        //Debug.Log("Radius:" + radius);
        String sql = "select PointNo, PublicNo, Purpose, X1, Y1, CoordinateMethod1, CoordinateQuality1, Z1, ZDVR90 from points" +
            " WHERE X1 >= " + (utmResult.Easting - radius).ToString(CultureInfo.InvariantCulture) + 
            " AND X1 <= " + (utmResult.Easting + radius).ToString(CultureInfo.InvariantCulture) + 
            " AND Y1 >= " + (utmResult.Northing - radius).ToString(CultureInfo.InvariantCulture) + 
            " AND Y1 <= " + (utmResult.Northing + radius).ToString(CultureInfo.InvariantCulture);

        Debug.Log("GeoGis Boringer SQL: " + sql);

        String errMessage = null;
        DataSet dataset = client.GetDS(this.GEOGIS_DBNAME, this.GEOGIS_USERNAME, this.GEOGIS_PASSWORD, sql, ref errMessage);

        bool allGood = false;
        if (errMessage == null)
        {
            allGood = true;
        }
        else if (errMessage != null)
        {
            if(errMessage == "")
            {
                allGood = true;
            }
            else
            {
                Debug.Log("ErrMessage: " + errMessage);
            }
        }
        if(allGood)
        {
            boreholes = new List<Borehole>();
            foreach (DataTable table in dataset.Tables)
            {

                String[] columnNames = new String[table.Columns.Count];
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    columnNames[i] = table.Columns[i].ColumnName;
                    //Debug.Log(table.Columns[i].ColumnName);
                }

                foreach (System.Data.DataRow row in table.Rows)
                {
                    object[] itemArray = row.ItemArray;
                    Borehole borehole = new Borehole();
                    String pointNo = null;
                    String publicNo = null;
                    float? latitude = null;
                    float? longitude = null;
                    String locationQuality_Code = null;
                    String locationMethod_Code = null;
                    for (int i = 0; i < itemArray.Length; i++)
                    {
                        if (!(itemArray[i] is DBNull))
                        {
                            switch (columnNames[i])
                            {
                                case "PointNo":
                                    pointNo = Convert.ToString(itemArray[i]);
                                    break;
                                case "PublicNo":
                                    publicNo = Convert.ToString(itemArray[i]);
                                    break;
                                case "X1":
                                    latitude = Convert.ToSingle(itemArray[i]);
                                    break;
                                case "Y1":
                                    longitude = Convert.ToSingle(itemArray[i]);
                                    break;
                                case "Z1":
                                    borehole.ReferencePoint = Convert.ToSingle(itemArray[i]);
                                    break;
                                case "ZDVR90":
                                    borehole.ReferencePointKote = Convert.ToSingle(itemArray[i]);
                                    break;
                                case "CoordinateQuality1":
                                    locationQuality_Code = Convert.ToString(itemArray[i]);
                                    break;
                                case "CoordinateMethod1":
                                    locationMethod_Code = Convert.ToString(itemArray[i]);
                                    break;
                                case "Purpose":
                                    borehole.BoreholeType = mediatorDatabase.getBoreholeType(Convert.ToString(itemArray[i]));
                                    break;
                                default:
                                    Debug.Log("Unidentified Column!");
                                    break;
                            }
                        }
                    }

                    borehole.BoreholeNo = publicNo != null ? publicNo : pointNo;

                    if (latitude != null && longitude != null)
                    {
                        borehole.Location = new Location((float)latitude, (float)longitude);
                        
                        if (locationQuality_Code != null)
                        {
                            borehole.Location.LocationQuality = mediatorDatabase.getLocationQuality(locationQuality_Code);
                        }
                        if (locationMethod_Code != null)
                        {
                            borehole.Location.LocationMethod = mediatorDatabase.getLocationMethod(locationMethod_Code);
                        }
                        
                    }
                    boreholes.Add(borehole);
                }
            }
        }
        return boreholes;
    }
    
    public List<BoreholeType> getBoreholeTypes()
    {
        List<BoreholeType> boreholeTypes = null;
        String sql = "SELECT Purpose, Description FROM PointPurposes WHERE Setup = 'DK'";
        Debug.Log("GeoGis Borings Typer SQL: " + sql);
        String errMessage = null;
        DataSet dataset = client.GetDS(this.GEOGIS_DBNAME, this.GEOGIS_USERNAME, this.GEOGIS_PASSWORD, sql, ref errMessage);

        bool allGood = false;
        if (errMessage == null)
        {
            allGood = true;
        }
        else if (errMessage != null)
        {
            if (errMessage == "")
            {
                allGood = true;
            }
            else
            {
                Debug.Log("ErrMessage: " + errMessage);
            }
        }

        if(allGood)
        {
            boreholeTypes = new List<BoreholeType>();
            foreach (DataTable table in dataset.Tables)
            {
                String[] columnNames = new String[table.Columns.Count];
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    columnNames[i] = table.Columns[i].ColumnName;
                    //Debug.Log(table.Columns[i].ColumnName);
                }

                foreach (System.Data.DataRow row in table.Rows)
                {
                    object[] itemArray = row.ItemArray;
                    BoreholeType boreholeType = new BoreholeType();
                    for (int i = 0; i < itemArray.Length; i++)
                    {
                        if (!(itemArray[i] is DBNull))
                        {
                            switch (columnNames[i])
                            {
                                case "Purpose":
                                    boreholeType.Code = Convert.ToString(itemArray[i]);
                                    break;
                                case "Description":
                                    boreholeType.Description = Convert.ToString(itemArray[i]);
                                    break;
                                default:
                                    Debug.Log("Unidentified Column!");
                                    break;
                            }
                        }
                    }
                    boreholeTypes.Add(boreholeType);
                }
            }
        }
        
        return boreholeTypes;
    }

    public List<string> getDatabaseNames()
    {
        System.Data.DataSet dataSet = client.GetDBList();
        List<string> dbNames = new List<string>();
        foreach (System.Data.DataTable table in dataSet.Tables)
        {
            /*
           Debug.Log("TableName: " + table.TableName);
            foreach (System.Data.DataColumn column in table.Columns)
            {
                Debug.Log("ColumnName: " + column.ColumnName);
            }
            */
            foreach (System.Data.DataRow row in table.Rows)
            {
                object[] itemArray = row.ItemArray;
                dbNames.Add(itemArray[0].ToString());
                /*
                for (int i = 0; i < itemArray.Length; i++)
                {
                    Debug.Log("ItemArray " + i + ": " + itemArray[i].ToString());
                }
                */
            }
        }
        return dbNames;
    }

    public void getDS(string dbName, string sql)
    {
        String errMessage = "";
        DataSet dataset = client.GetDS(dbName, "HenrikLodsgaardNielsen", "HenrikLodsgaardNielsen1229", sql, ref errMessage);

        foreach (DataTable table in dataset.Tables)
        {

            Debug.Log("TableName: " + table.TableName);
            foreach (System.Data.DataColumn column in table.Columns)
            {
                Debug.Log("ColumnName: " + column.ColumnName);
            }

            foreach (System.Data.DataRow row in table.Rows)
            {
                object[] itemArray = row.ItemArray;

                for (int i = 0; i < itemArray.Length; i++)
                {


                    if (i == 0)
                    {
                        Debug.Log("PunktNr: " + itemArray[i].ToString());
                    }
                    else if (i == 1)
                    {
                        Debug.Log("X1: " + itemArray[i].ToString());
                    }
                    else if (i == 2)
                    {
                        Debug.Log("Y1: " + itemArray[i].ToString());
                    }
                    else
                    {
                        Debug.Log("ItemArray " + i + ": " + itemArray[i].ToString());
                    }

                }
            }
        }
    }



}
