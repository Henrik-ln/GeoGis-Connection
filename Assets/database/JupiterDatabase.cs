using Database.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using UnityEngine;

class JupiterDatabase : ProxyDatabase
{
    private readonly string JUPITER_WEBSERVICE_URL = "http://85.129.124.47:1878/JupiterConnectionService/JupiterConnectionService/";
    private JupiterConnectionServiceClient jupiterClient;

    public JupiterDatabase(MediatorDatabase mediatorDatabase) : base(mediatorDatabase)
    {
        BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
        basicHttpBinding.MaxReceivedMessageSize = 99999999;
        basicHttpBinding.MaxBufferSize = 99999999;
        basicHttpBinding.MaxBufferPoolSize = 99999999;

        this.jupiterClient = new JupiterConnectionServiceClient(basicHttpBinding, new EndpointAddress(JUPITER_WEBSERVICE_URL));
    }

    public List<Borehole> getBoreholes(Location currentLocation, double radius)
    {
        Debug.Log("Jupiter - Henter boringer");
        List<Borehole> boreholes = null;
        SelectResult selectResult = null;
        try
        {
            String sql = "SELECT BOREHOLENO, LATITUDE, LONGITUDE, ELEVATION, ZDVR90, PURPOSE, LOCATQUALI, LOCATMETHO FROM BOREHOLE" +
                " WHERE LATITUDE >= " + (currentLocation.X - radius).ToString(CultureInfo.InvariantCulture) + 
                " AND LATITUDE <= " + (currentLocation.X + radius).ToString(CultureInfo.InvariantCulture) + 
                " AND LONGITUDE >= " + (currentLocation.Y - radius).ToString(CultureInfo.InvariantCulture) +
                " AND LONGITUDE <= " + (currentLocation.Y + radius).ToString(CultureInfo.InvariantCulture);
            Debug.Log("Jupiter Boringer SQL: " + sql);
            selectResult = jupiterClient.select(sql);
        }
        catch (System.ServiceModel.CommunicationException e)
        {
            Debug.Log("Exception: Too Many Results");
            return null;
        }
        catch(System.Net.WebException e)
        {
            Debug.Log("Exception: Jupiter Connection Service not available");
            return null;
        }
        catch(Exception e)
        {
            Debug.Log("Exception: Unknown");
        }

        if (selectResult != null)
        {
            if (selectResult.DataSetResult != null)
            {
                foreach (DataTable table in selectResult.DataSetResult.Tables)
                {
                    String[] columnNames = new String[table.Columns.Count];
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        columnNames[i] = table.Columns[i].ColumnName;
                    }

                    boreholes = new List<Borehole>();

                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        object[] itemArray = row.ItemArray;

                        Borehole borehole = new Borehole();
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
                                    case "BOREHOLENO":
                                        borehole.BoreholeNo = Convert.ToString(itemArray[i]);
                                        break;
                                    case "LATITUDE":
                                        latitude = Convert.ToSingle(itemArray[i], new CultureInfo("da-DK"));
                                        break;
                                    case "LONGITUDE":
                                        longitude = Convert.ToSingle(itemArray[i], new CultureInfo("da-DK"));
                                        break;
                                    case "ELEVATION":
                                        borehole.ReferencePoint = Convert.ToSingle(itemArray[i], new CultureInfo("da-DK"));
                                        break;
                                    case "ZDVR90":
                                        borehole.ReferencePointKote = Convert.ToSingle(itemArray[i], new CultureInfo("da-DK"));
                                        break;
                                    case "PURPOSE":
                                        borehole.BoreholeType = MediatorDatabase.getBoreholeType(Convert.ToString(itemArray[i]));
                                        break;
                                    case "LOCATQUALI":
                                        locationQuality_Code = Convert.ToString(itemArray[i]);
                                        break;
                                    case "LOCATMETHO":
                                        locationMethod_Code = Convert.ToString(itemArray[i]);
                                        break;
                                    default:
                                        Debug.Log("Unidentified Column!");
                                        break;
                                }
                            }
                        }

                        if (latitude != null && longitude != null)
                        {
                            borehole.Location = new Location((float)latitude, (float)longitude);
                            if (locationQuality_Code != null)
                            {
                                borehole.Location.LocationQuality = MediatorDatabase.getLocationQuality(locationQuality_Code);
                            }
                            if (locationMethod_Code != null)
                            {
                                borehole.Location.LocationMethod = MediatorDatabase.getLocationMethod(locationMethod_Code);
                            }
                        }
                        if (borehole.BoreholeNo != null)
                        {
                            borehole.Screens = getScreens(borehole.BoreholeNo);
                        }
                        boreholes.Add(borehole);
                    }
                }
            }
            else
            {
                Debug.Log("DataSetResult is null");
            }

            if (selectResult.Error != null)
            {
                Debug.Log("Error getting Boreholes. " + selectResult.Error);
            }
        }
        else
        {
            Debug.Log("SelectResult is null");
        }
        return boreholes;
    }
    
    public List<Database.model.Screen> getScreens(String boreholeNo)
    {
        List<Database.model.Screen> screens = null;
        SelectResult selectResult = null;
        try
        {
            String sql = "SELECT SCREENNO, INTAKENO, TOP, BOTTOM, DIAMETER, UNIT FROM SCREEN WHERE BOREHOLENO = '" + boreholeNo + "'";
            Debug.Log("Jupiter Filtrer SQL: " + sql);
            selectResult = jupiterClient.select(sql);
        }
        catch (System.ServiceModel.CommunicationException e)
        {
            Debug.Log("Too many results");
        }


        if (selectResult != null)
        {
            if (selectResult.DataSetResult != null)
            {
                foreach (DataTable table in selectResult.DataSetResult.Tables)
                {
                    String[] columnNames = new String[table.Columns.Count];
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        columnNames[i] = table.Columns[i].ColumnName;
                    }
                    screens = new List<Database.model.Screen>();

                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        object[] itemArray = row.ItemArray;
                        Database.model.Screen screen = new Database.model.Screen();
                        int? intakeNo = null;
                        for (int i = 0; i < itemArray.Length; i++)
                        {
                            switch (columnNames[i])
                            {
                                case "SCREENNO":
                                    screen.ScreenNo = Convert.ToInt32(itemArray[i]);
                                    break;
                                case "INTAKENO":
                                    intakeNo = Convert.ToInt32(itemArray[i]);
                                    break;
                                case "TOP":
                                    screen.Top = Convert.ToSingle(itemArray[i], new CultureInfo("da-DK"));
                                    break;
                                case "BOTTOM":
                                    screen.Bottom = Convert.ToSingle(itemArray[i], new CultureInfo("da-DK"));
                                    break;
                                case "DIAMETER":
                                    screen.Diameter = Convert.ToSingle(itemArray[i], new CultureInfo("da-DK"));
                                    break;
                                case "UNIT":
                                    screen.DiameterUnit = Convert.ToString(itemArray[i]);
                                    break;
                                default:
                                    Debug.Log("Unidentified Column!");
                                    break;
                            }
                        }
                        if (intakeNo != null)
                        {
                            screen.WaterLevels = getWaterLevels(boreholeNo, (int)intakeNo);
                        }
                        screens.Add(screen);
                    }
                }
            }
            else
            {
                Debug.Log("DataSetRecords are null");
            }

            if (selectResult.Error != null)
            {
                string error = selectResult.Error;
                Debug.Log("Error getting Screens: " + error);
            }
        }
        else
        {
            Debug.Log("SelectResult is null");
        }
        return screens;
    }
    
    
    public List<WaterLevel> getWaterLevels(String boreholeNo, int intakeNo)
    {
        List<WaterLevel> waterLevels = null;
        SelectResult selectResult = null;
        try
        {
            String sql = "SELECT TIMEOFMEAS, WATERLEVEL, REFPOINT FROM WATLEVEL WHERE BOREHOLENO = '" + boreholeNo + "' AND INTAKENO =" + intakeNo;
            Debug.Log("Jupiter Vand Måling SQL: " + sql);
            selectResult = jupiterClient.select(sql);
        }
        catch (System.ServiceModel.CommunicationException e)
        {
            Debug.Log("Too many results");
        }

        if (selectResult != null)
        {
            if (selectResult.DataSetResult != null)
            {
                foreach (DataTable table in selectResult.DataSetResult.Tables)
                {
                    String[] columnNames = new String[table.Columns.Count];
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        columnNames[i] = table.Columns[i].ColumnName;
                    }
                    waterLevels = new List<WaterLevel>();

                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        object[] itemArray = row.ItemArray;
                        WaterLevel waterLevel = new WaterLevel();
                        for (int i = 0; i < itemArray.Length; i++)
                        {
                            switch (columnNames[i])
                            {
                                case "TIMEOFMEAS":
                                    waterLevel.MeasurementDate = Convert.ToDateTime(itemArray[i]);
                                    break;
                                case "WATERLEVEL":
                                    waterLevel.Measurement = Convert.ToSingle(itemArray[i], new CultureInfo("da-DK"));
                                    break;
                                case "REFPOINT":
                                    //waterLevel.ReferencePoint = this.MediatorDatabase.getReferencePoint(Convert.ToString(itemArray[i]));
                                    break;
                                default:
                                    Debug.Log("Unidentified Column!");
                                    break;
                            }
                        }
                        waterLevels.Add(waterLevel);
                    }
                }

            }
            else
            {
                Debug.Log("SelectResult's DataSetResult is null");
            }

            if (selectResult.Error != null)
            {
                string error = selectResult.Error;
                Debug.Log("Error getting Water Level: " + error);
            }
        }
        else
        {
            Debug.Log("SelectResult is null");
        }
        return waterLevels;
    }
    
    
    public List<BoreholeType> getBoreholeTypes()
    {
        List<BoreholeType> boreholeTypes = new List<BoreholeType>();
        SelectResult selectResult = null;

        try
        {
            String sql = "SELECT CODE, SHORTTEXT, LONGTEXT FROM CODE WHERE CODETYPE = 17";
            Debug.Log("Jupiter Borings Typer SQL: " + sql);
            selectResult = jupiterClient.select(sql);
        }
        catch (System.ServiceModel.CommunicationException e)
        {
            Debug.Log("Too many results");
        }

        if (selectResult != null)
        {
            if (selectResult.DataSetResult != null)
            {
                foreach (DataTable table in selectResult.DataSetResult.Tables)
                {
                    String[] columnNames = new String[table.Columns.Count];
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        columnNames[i] = table.Columns[i].ColumnName;
                    }

                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        object[] itemArray = row.ItemArray;
                        BoreholeType boreholeType = new BoreholeType();
                        for (int i = 0; i < itemArray.Length; i++)
                        {
                            switch (columnNames[i])
                            {
                                case "CODE":
                                    boreholeType.Code = Convert.ToString(itemArray[i]);
                                    break;
                                case "SHORTTEXT":
                                    boreholeType.ShortDescription = Convert.ToString(itemArray[i]);
                                    break;
                                case "LONGTEXT":
                                    boreholeType.Description = Convert.ToString(itemArray[i]);
                                    break;
                                default:
                                    Debug.Log("Unidentified Column!");
                                    break;
                            }
                        }
                        boreholeTypes.Add(boreholeType);
                    }
                }
            }
            else
            {
                Debug.Log("SelectResult's DataSetResult is null");
            }

            if (selectResult.Error != null)
            {
                string error = selectResult.Error;
                Debug.Log("Error getting Borehole Types: " + error);
            }
        }
        else
        {
            Debug.Log("SelectResult is null");
        }
        return boreholeTypes;
    }
    
    
    public List<ReferencePoint> getReferencePoints()
    {
        List<ReferencePoint> referencePoints = new List<ReferencePoint>();
        SelectResult selectResult = null;

        try
        {
            String sql = "SELECT CODE, LONGTEXT FROM CODE WHERE CODETYPE = 97";
            Debug.Log("Jupiter Reference Punkter SQL: " + sql);
            selectResult = jupiterClient.select(sql);
        }
        catch (System.ServiceModel.CommunicationException e)
        {
            Debug.Log("Too many results");
        }

        if (selectResult != null)
        {
            if (selectResult.DataSetResult != null)
            {
                foreach (DataTable table in selectResult.DataSetResult.Tables)
                {
                    String[] columnNames = new String[table.Columns.Count];
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        columnNames[i] = table.Columns[i].ColumnName;
                    }

                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        object[] itemArray = row.ItemArray;
                        ReferencePoint referencePoint = new ReferencePoint();
                        for (int i = 0; i < itemArray.Length; i++)
                        {
                            switch (columnNames[i])
                            {
                                case "CODE":
                                    referencePoint.Code = Convert.ToString(itemArray[i]);
                                    break;
                                case "LONGTEXT":
                                    referencePoint.Description = Convert.ToString(itemArray[i]);
                                    break;
                                default:
                                    Debug.Log("Unidentified Column!");
                                    break;
                            }
                        }
                        referencePoints.Add(referencePoint);
                    }
                }
            }
            else
            {
                Debug.Log("SelectResult's DataSetResult is null");
            }

            if (selectResult.Error != null)
            {
                string error = selectResult.Error;
                Debug.Log("Error getting ReferecePoints: " + error);
            }
        }
        else
        {
            Debug.Log("SelectResult is null");
        }
        return referencePoints;
    }
    
    
    public List<LocationQuality> getLocationQualities()
    {
        List<LocationQuality> locationQualities = new List<LocationQuality>();
        SelectResult selectResult = null;

        try
        {
            String sql = "SELECT CODE, LONGTEXT FROM CODE WHERE CODETYPE = 63";
            Debug.Log("Jupiter Lokation Kvaliteter SQL: " + sql);
            selectResult = jupiterClient.select(sql);
        }
        catch (System.ServiceModel.CommunicationException e)
        {
            Debug.Log("Too many results");
        }

        if (selectResult != null)
        {
            if (selectResult.DataSetResult != null)
            {
                foreach (DataTable table in selectResult.DataSetResult.Tables)
                {
                    String[] columnNames = new String[table.Columns.Count];
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        columnNames[i] = table.Columns[i].ColumnName;
                    }

                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        object[] itemArray = row.ItemArray;
                        LocationQuality locationQuality = new LocationQuality();
                        for (int i = 0; i < itemArray.Length; i++)
                        {
                            switch (columnNames[i])
                            {
                                case "CODE":
                                    locationQuality.Code = Convert.ToString(itemArray[i]);
                                    break;
                                case "LONGTEXT":
                                    locationQuality.Description = Convert.ToString(itemArray[i]);
                                    break;
                                default:
                                    Debug.Log("Unidentified Column!");
                                    break;
                            }
                        }
                        locationQualities.Add(locationQuality);
                    }
                }
            }
            else
            {
                Debug.Log("SelectResult's DataSetResult is null");
            }

            if (selectResult.Error != null)
            {
                string error = selectResult.Error;
                Debug.Log("Error getting LocationQualities: " + error);
            }
        }
        else
        {
            Debug.Log("SelectResult is null");
        }
        return locationQualities;
    }
    
    
    public List<LocationMethod> getLocationMethods()
    {
        List<LocationMethod> locationMethods = new List<LocationMethod>();
        SelectResult selectResult = null;

        try
        {
            String sql = "SELECT CODE, LONGTEXT FROM CODE WHERE CODETYPE = 64";
            Debug.Log("Jupiter Lokation Metoder SQL: " + sql);
            selectResult = jupiterClient.select(sql);
        }
        catch (System.ServiceModel.CommunicationException e)
        {
            Debug.Log("Too many results");
        }

        if (selectResult != null)
        {
            if (selectResult.DataSetResult != null)
            {
                foreach (DataTable table in selectResult.DataSetResult.Tables)
                {
                    String[] columnNames = new String[table.Columns.Count];
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        columnNames[i] = table.Columns[i].ColumnName;
                    }

                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        object[] itemArray = row.ItemArray;
                        LocationMethod locationMethod = new LocationMethod();
                        for (int i = 0; i < itemArray.Length; i++)
                        {
                            switch (columnNames[i])
                            {
                                case "CODE":
                                    locationMethod.Code = Convert.ToString(itemArray[i]);
                                    break;
                                case "LONGTEXT":
                                    locationMethod.Description = Convert.ToString(itemArray[i]);
                                    break;
                                default:
                                    Debug.Log("Unidentified Column!");
                                    break;
                            }
                        }
                        locationMethods.Add(locationMethod);
                    }
                }
            }
            else
            {
                Debug.Log("SelectResult's DataSetResult is null");
            }

            if (selectResult.Error != null)
            {
                string error = selectResult.Error;
                Debug.Log("Error getting LocationQualities: " + error);
            }
        }
        else
        {
            Debug.Log("SelectResult is null");
        }
        return locationMethods;
    }
    

    /*
            public void selectFromDatabase(String sql)
            {
                JupiterDatabaseReference.selectResult selectResult = null;

                try
                {
                    selectResult = jupiterClient.select(sql);
                }
                catch (System.ServiceModel.CommunicationException e)
                {
                    Debug.Log("Too many results");
                }

                if (selectResult != null)
                {
                    if (selectResult.resultset != null)
                    {
                        JupiterDatabaseReference.selectResultSet resultSet = selectResult.resultset;


                        JupiterDatabaseReference.dataRecord[] dataRecords = resultSet.records;
                        if (dataRecords != null)
                        {
                            String[] columnNames = resultSet.columnNames;
                            String[] columnTypes = resultSet.columnTypes;
                            foreach (JupiterDatabaseReference.dataRecord dataRecord in dataRecords)
                            {
                                object[] row = dataRecord.data;

                                for (int i = 0; i < row.Length; i++)
                                {
                                    Debug.Log(columnNames[i] + " : " + columnTypes[i] + " = " + row[i]);
                                }
                                Debug.Log();
                            }

                        }
                        else
                        {
                            Debug.Log("DataRecords are null");
                        }
                    }

                    if (selectResult.error != null)
                    {
                        JupiterDatabaseReference.elementError error = selectResult.error;
                        Debug.Log("Error Element Name: " + error.elementName);
                        Debug.Log("Error Message: " + error.errorMesage);
                    }
                }
                else
                {
                    Debug.Log("SelectResult is null");
                }


            }
            */
    /*
    public void readTables()
    {
        JupiterDatabaseReference.jupTable[] jupiterTables = jupiterClient.getJupTable();
        Debug.Log("START");
        for (int i = 0; i < jupiterTables.Length; i++)
        {
            if(jupiterTables[i].corrtablna.Contains("SAMPL"))
            {
                //Debug.Log("TableNo: " + jupiterTables[i].tableno);
                //Debug.Log("TableNoSpecified: " + jupiterTables[i].tablenoSpecified);
                Debug.Log("TableName: " + jupiterTables[i].tablename);
                //Debug.Log("mastkyflds: " + jupiterTables[i].mastkyflds);
                //Debug.Log("masttable: " + jupiterTables[i].masttable);
                Debug.Log("corrtablna: " + jupiterTables[i].corrtablna);
                //Debug.Log("TableType: " + jupiterTables[i].tabletype);
                //Debug.Log("TableTypeSpecified: " + jupiterTables[i].tabletypeSpecified);
                Debug.Log();
            }


        }

    }
    */
    /*
    public void readTableDescription(String tableName)
    {
        JupiterDatabaseReference.jupFieDe[] fieldDescriptions = jupiterClient.getJupFieDe(tableName);
        Debug.Log("START");
        for (int i = 0; i < fieldDescriptions.Length; i++)
        {
            Debug.Log("tablename: " + fieldDescriptions[i].tablename);
            Debug.Log("columnname: " + fieldDescriptions[i].columnname);
            Debug.Log("definition: " + fieldDescriptions[i].definition);
            Debug.Log("description: " + fieldDescriptions[i].description);
            Debug.Log();

        }
    }
    */
}
