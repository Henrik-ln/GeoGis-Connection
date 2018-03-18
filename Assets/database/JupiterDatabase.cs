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
        List<Borehole> boreholes = null;
        SelectResult selectResult = null;
        try
        {
            String sql = "SELECT BOREHOLENO, LATITUDE, LONGITUDE, ELEVATION, ZDVR90, PURPOSE, LOCATQUALI, LOCATMETHO FROM BOREHOLE" +
            " WHERE LATITUDE >= " + (currentLocation.X - radius).ToString(CultureInfo.InvariantCulture) + " AND LATITUDE <= " + (currentLocation.X + radius).ToString(CultureInfo.InvariantCulture)
            + " AND LONGITUDE >= " + (currentLocation.Y - radius).ToString(CultureInfo.InvariantCulture) + " AND LONGITUDE <= " + (currentLocation.Y + radius).ToString(CultureInfo.InvariantCulture);
            Debug.Log("Jupiter SQL: " + sql);
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
                                        borehole.BoreholeNo = Convert.ToString(row[i]);
                                        break;
                                    case "LATITUDE":
                                        latitude = Convert.ToSingle(row[i]);
                                        break;
                                    case "LONGITUDE":
                                        longitude = Convert.ToSingle(row[i]);
                                        break;
                                    case "ELEVATION":
                                        borehole.ReferencePoint = Convert.ToSingle(row[i]);
                                        break;
                                    case "ZDVR90":
                                        borehole.ReferencePointKote = Convert.ToSingle(row[i]);
                                        break;
                                    case "PURPOSE":
                                        borehole.BoreholeType = MediatorDatabase.getBoreholeType(Convert.ToString(row[i]));
                                        break;
                                    case "LOCATQUALI":
                                        locationQuality_Code = Convert.ToString(row[i]);
                                        break;
                                    case "LOCATMETHO":
                                        locationMethod_Code = Convert.ToString(row[i]);
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
                                //borehole.Location.LocationQuality = MediatorDatabase.getLocationQuality(locationQuality_Code);
                            }
                            if (locationMethod_Code != null)
                            {
                                //borehole.Location.LocationMethod = MediatorDatabase.getLocationMethod(locationMethod_Code);
                            }
                        }
                        if (borehole.BoreholeNo != null)
                        {
                            //borehole.Screens = getScreens(borehole.BoreholeNo);
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
    /*
    public List<Screen> getScreens(String boreholeNo)
    {
        List<Screen> screens = null;
        JupiterDatabaseReference.selectResult selectResult = null;
        try
        {
            String sql = "SELECT SCREENNO, INTAKENO, TOP, BOTTOM, DIAMETER, UNIT FROM SCREEN WHERE BOREHOLENO = '" + boreholeNo + "'";
            selectResult = jupiterClient.select(sql);
        }
        catch (System.ServiceModel.CommunicationException e)
        {
            Console.Out.WriteLine("Too many results");
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
                    screens = new List<Screen>();
                    foreach (JupiterDatabaseReference.dataRecord dataRecord in dataRecords)
                    {
                        object[] row = dataRecord.data;
                        Screen screen = new Screen();
                        int? intakeNo = null;
                        for (int i = 0; i < row.Length; i++)
                        {
                            switch (columnNames[i])
                            {
                                case "SCREENNO":
                                    screen.ScreenNo = Convert.ToInt32(row[i]);
                                    break;
                                case "INTAKENO":
                                    intakeNo = Convert.ToInt32(row[i]);
                                    break;
                                case "TOP":
                                    screen.Top = Convert.ToSingle(row[i]);
                                    break;
                                case "BOTTOM":
                                    screen.Bottom = Convert.ToSingle(row[i]);
                                    break;
                                case "DIAMETER":
                                    screen.Diameter = Convert.ToSingle(row[i]);
                                    break;
                                case "UNIT":
                                    screen.DiameterUnit = Convert.ToString(row[i]);
                                    break;
                                default:
                                    Console.WriteLine("Unidentified Column!");
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
                else
                {
                    Console.Out.WriteLine("DataRecords are null");
                }
            }

            if (selectResult.error != null)
            {
                JupiterDatabaseReference.elementError error = selectResult.error;
                Console.Out.WriteLine("Error getting Screens");
                Console.Out.WriteLine("Error Element Name: " + error.elementName);
                Console.Out.WriteLine("Error Message: " + error.errorMesage);
            }
        }
        else
        {
            Console.Out.WriteLine("SelectResult is null");
        }
        return screens;
    }
    */
    /*
    public List<WaterLevel> getWaterLevels(String boreholeNo, int intakeNo)
    {
        List<WaterLevel> waterLevels = null;
        JupiterDatabaseReference.selectResult selectResult = null;
        try
        {
            String sql = "SELECT TIMEOFMEAS, WATERLEVEL, REFPOINT FROM WATLEVEL WHERE BOREHOLENO = '" + boreholeNo + "' AND INTAKENO =" + intakeNo;
            selectResult = jupiterClient.select(sql);
        }
        catch (System.ServiceModel.CommunicationException e)
        {
            Console.Out.WriteLine("Too many results");
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
                    waterLevels = new List<WaterLevel>();
                    foreach (JupiterDatabaseReference.dataRecord dataRecord in dataRecords)
                    {
                        object[] row = dataRecord.data;
                        WaterLevel waterLevel = new WaterLevel();
                        for (int i = 0; i < row.Length; i++)
                        {
                            switch (columnNames[i])
                            {
                                case "TIMEOFMEAS":
                                    waterLevel.MeasurementDate = Convert.ToDateTime(row[i]);
                                    break;
                                case "WATERLEVEL":
                                    waterLevel.Measurement = Convert.ToSingle(row[i]);
                                    break;
                                case "REFPOINT":
                                    waterLevel.ReferencePoint = this.MediatorDatabase.getReferencePoint(Convert.ToString(row[i]));
                                    break;
                                default:
                                    Console.WriteLine("Unidentified Column!");
                                    break;
                            }
                        }
                        waterLevels.Add(waterLevel);
                    }
                }
                else
                {
                    Console.Out.WriteLine("DataRecords are null");
                }
            }

            if (selectResult.error != null)
            {
                JupiterDatabaseReference.elementError error = selectResult.error;
                Console.Out.WriteLine("Error getting Water Level");
                Console.Out.WriteLine("Error Element Name: " + error.elementName);
                Console.Out.WriteLine("Error Message: " + error.errorMesage);
            }
        }
        else
        {
            Console.Out.WriteLine("SelectResult is null");
        }
        return waterLevels;
    }
    */
    /*
    public List<BoreholeType> getBoreholeTypes()
    {
        List<BoreholeType> boreholeTypes = null;
        JupiterDatabaseReference.selectResult selectResult = null;

        try
        {
            String sql = "SELECT CODE, SHORTTEXT, LONGTEXT FROM CODE WHERE CODETYPE = 17";
            selectResult = jupiterClient.select(sql);
        }
        catch (System.ServiceModel.CommunicationException e)
        {
            Console.Out.WriteLine("Too many results");
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
                    boreholeTypes = new List<BoreholeType>();
                    foreach (JupiterDatabaseReference.dataRecord dataRecord in dataRecords)
                    {
                        object[] row = dataRecord.data;
                        BoreholeType boreholeType = new BoreholeType();
                        for (int i = 0; i < row.Length; i++)
                        {
                            switch (columnNames[i])
                            {
                                case "CODE":
                                    boreholeType.Code = Convert.ToString(row[i]);
                                    break;
                                case "SHORTTEXT":
                                    boreholeType.ShortDescription = Convert.ToString(row[i]);
                                    break;
                                case "LONGTEXT":
                                    boreholeType.Description = Convert.ToString(row[i]);
                                    break;
                                default:
                                    Console.WriteLine("Unidentified Column!");
                                    break;
                            }
                        }
                        boreholeTypes.Add(boreholeType);
                    }
                }
                else
                {
                    Console.Out.WriteLine("DataRecords are null");
                }
            }

            if (selectResult.error != null)
            {
                JupiterDatabaseReference.elementError error = selectResult.error;
                Console.Out.WriteLine("Error getting Borehole Types");
                Console.Out.WriteLine("Error Element Name: " + error.elementName);
                Console.Out.WriteLine("Error Message: " + error.errorMesage);
            }
        }
        else
        {
            Console.Out.WriteLine("SelectResult is null");
        }
        return boreholeTypes;
    }
    */
    /*
    public List<ReferencePoint> getReferencePoints()
    {
        List<ReferencePoint> referencePoints = null;
        JupiterDatabaseReference.selectResult selectResult = null;

        try
        {
            String sql = "SELECT CODE, LONGTEXT FROM CODE WHERE CODETYPE = 97";
            selectResult = jupiterClient.select(sql);
        }
        catch (System.ServiceModel.CommunicationException e)
        {
            Console.Out.WriteLine("Too many results");
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
                    referencePoints = new List<ReferencePoint>();
                    foreach (JupiterDatabaseReference.dataRecord dataRecord in dataRecords)
                    {
                        object[] row = dataRecord.data;
                        ReferencePoint referencePoint = new ReferencePoint();
                        for (int i = 0; i < row.Length; i++)
                        {
                            switch (columnNames[i])
                            {
                                case "CODE":
                                    referencePoint.Code = Convert.ToString(row[i]);
                                    break;
                                case "LONGTEXT":
                                    referencePoint.Description = Convert.ToString(row[i]);
                                    break;
                                default:
                                    Console.WriteLine("Unidentified Column!");
                                    break;
                            }
                        }
                        referencePoints.Add(referencePoint);
                    }
                }
                else
                {
                    Console.Out.WriteLine("DataRecords are null");
                }
            }

            if (selectResult.error != null)
            {
                JupiterDatabaseReference.elementError error = selectResult.error;
                Console.Out.WriteLine("Error getting ReferecePoints");
                Console.Out.WriteLine("Error Element Name: " + error.elementName);
                Console.Out.WriteLine("Error Message: " + error.errorMesage);
            }
        }
        else
        {
            Console.Out.WriteLine("SelectResult is null");
        }
        return referencePoints;
    }
    */
    /*
    public List<LocationQuality> getLocationQualities()
    {
        List<LocationQuality> locationQualities = null;
        JupiterDatabaseReference.selectResult selectResult = null;

        try
        {
            String sql = "SELECT CODE, LONGTEXT FROM CODE WHERE CODETYPE = 63";
            selectResult = jupiterClient.select(sql);
        }
        catch (System.ServiceModel.CommunicationException e)
        {
            Console.Out.WriteLine("Too many results");
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
                    locationQualities = new List<LocationQuality>();
                    foreach (JupiterDatabaseReference.dataRecord dataRecord in dataRecords)
                    {
                        object[] row = dataRecord.data;
                        LocationQuality locationQuality = new LocationQuality();
                        for (int i = 0; i < row.Length; i++)
                        {
                            switch (columnNames[i])
                            {
                                case "CODE":
                                    locationQuality.Code = Convert.ToString(row[i]);
                                    break;
                                case "LONGTEXT":
                                    locationQuality.Description = Convert.ToString(row[i]);
                                    break;
                                default:
                                    Console.WriteLine("Unidentified Column!");
                                    break;
                            }
                        }
                        locationQualities.Add(locationQuality);
                    }
                }
                else
                {
                    Console.Out.WriteLine("DataRecords are null");
                }
            }

            if (selectResult.error != null)
            {
                JupiterDatabaseReference.elementError error = selectResult.error;
                Console.Out.WriteLine("Error getting LocationQualities");
                Console.Out.WriteLine("Error Element Name: " + error.elementName);
                Console.Out.WriteLine("Error Message: " + error.errorMesage);
            }
        }
        else
        {
            Console.Out.WriteLine("SelectResult is null");
        }
        return locationQualities;
    }
    */
    /*
    public List<LocationMethod> getLocationMethods()
    {
        List<LocationMethod> locationMethods = null;
        JupiterDatabaseReference.selectResult selectResult = null;

        try
        {
            String sql = "SELECT CODE, LONGTEXT FROM CODE WHERE CODETYPE = 64";
            selectResult = jupiterClient.select(sql);
        }
        catch (System.ServiceModel.CommunicationException e)
        {
            Console.Out.WriteLine("Too many results");
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
                    locationMethods = new List<LocationMethod>();
                    foreach (JupiterDatabaseReference.dataRecord dataRecord in dataRecords)
                    {
                        object[] row = dataRecord.data;
                        LocationMethod locationMethod = new LocationMethod();
                        for (int i = 0; i < row.Length; i++)
                        {
                            switch (columnNames[i])
                            {
                                case "CODE":
                                    locationMethod.Code = Convert.ToString(row[i]);
                                    break;
                                case "LONGTEXT":
                                    locationMethod.Description = Convert.ToString(row[i]);
                                    break;
                                default:
                                    Console.WriteLine("Unidentified Column!");
                                    break;
                            }
                        }
                        locationMethods.Add(locationMethod);
                    }
                }
                else
                {
                    Console.Out.WriteLine("DataRecords are null");
                }
            }

            if (selectResult.error != null)
            {
                JupiterDatabaseReference.elementError error = selectResult.error;
                Console.Out.WriteLine("Error getting LocationQualities");
                Console.Out.WriteLine("Error Element Name: " + error.elementName);
                Console.Out.WriteLine("Error Message: " + error.errorMesage);
            }
        }
        else
        {
            Console.Out.WriteLine("SelectResult is null");
        }
        return locationMethods;
    }
    */

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
                    Console.Out.WriteLine("Too many results");
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
                                    Console.Out.WriteLine(columnNames[i] + " : " + columnTypes[i] + " = " + row[i]);
                                }
                                Console.Out.WriteLine();
                            }

                        }
                        else
                        {
                            Console.Out.WriteLine("DataRecords are null");
                        }
                    }

                    if (selectResult.error != null)
                    {
                        JupiterDatabaseReference.elementError error = selectResult.error;
                        Console.Out.WriteLine("Error Element Name: " + error.elementName);
                        Console.Out.WriteLine("Error Message: " + error.errorMesage);
                    }
                }
                else
                {
                    Console.Out.WriteLine("SelectResult is null");
                }


            }
            */
    /*
    public void readTables()
    {
        JupiterDatabaseReference.jupTable[] jupiterTables = jupiterClient.getJupTable();
        Console.Out.WriteLine("START");
        for (int i = 0; i < jupiterTables.Length; i++)
        {
            if(jupiterTables[i].corrtablna.Contains("SAMPL"))
            {
                //Console.Out.WriteLine("TableNo: " + jupiterTables[i].tableno);
                //Console.Out.WriteLine("TableNoSpecified: " + jupiterTables[i].tablenoSpecified);
                Console.Out.WriteLine("TableName: " + jupiterTables[i].tablename);
                //Console.Out.WriteLine("mastkyflds: " + jupiterTables[i].mastkyflds);
                //Console.Out.WriteLine("masttable: " + jupiterTables[i].masttable);
                Console.Out.WriteLine("corrtablna: " + jupiterTables[i].corrtablna);
                //Console.Out.WriteLine("TableType: " + jupiterTables[i].tabletype);
                //Console.Out.WriteLine("TableTypeSpecified: " + jupiterTables[i].tabletypeSpecified);
                Console.Out.WriteLine();
            }


        }

    }
    */
    /*
    public void readTableDescription(String tableName)
    {
        JupiterDatabaseReference.jupFieDe[] fieldDescriptions = jupiterClient.getJupFieDe(tableName);
        Console.Out.WriteLine("START");
        for (int i = 0; i < fieldDescriptions.Length; i++)
        {
            Console.Out.WriteLine("tablename: " + fieldDescriptions[i].tablename);
            Console.Out.WriteLine("columnname: " + fieldDescriptions[i].columnname);
            Console.Out.WriteLine("definition: " + fieldDescriptions[i].definition);
            Console.Out.WriteLine("description: " + fieldDescriptions[i].description);
            Console.Out.WriteLine();

        }
    }
    */
}
