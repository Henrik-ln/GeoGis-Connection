
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using UnityEngine;

using UnityEngine.Networking;
using UnityEngine.UI;

public class DatabaseComponent : MonoBehaviour {

    //JupiterReadClient jupiterClient;
    // Use this for initialization
    private MediatorDatabase mediatorDatabase;

    void Start () {
        mediatorDatabase = new MediatorDatabase();
        StartCoroutine(initializeMediatorsLists());
        StartCoroutine(testDatabases());
        /*
        Debug.Log("Creating connection!");
        jupiterClient = new JupiterReadClient(new BasicHttpBinding(),
            new EndpointAddress("http://geusjuptest.geus.dk:80/jupiter.read.1.2.0/jupiterread.svc"));
        Debug.Log("Connection made!");
        
        String sql = "SELECT * FROM CODE WHERE CODETYPE = 47";
        jupiterClient.select(sql);
        //Console.Out.WriteLine(sql);
        //selectFromDatabase(sql);
        */
    }

    public IEnumerator initializeMediatorsLists()
    {
        ScreenLogger.Instance.addText("Henter Borings Typer fra Jupiter...");
        yield return new WaitForSeconds(0.1f);
        mediatorDatabase.BoreholeTypes = mediatorDatabase.JupiterDatabase.getBoreholeTypes();
        //Dem der er i GeoGis men ikke Jupiter
        ScreenLogger.Instance.addText("Henter Borings Typer fra GeoGis...");
        yield return new WaitForSeconds(0.1f);
        mediatorDatabase.addExtraGeoGisBoreholeTypes();
        ScreenLogger.Instance.addText("Henter Reference Punkter fra Jupiter...");
        yield return new WaitForSeconds(0.1f);
        mediatorDatabase.ReferencePoints = mediatorDatabase.JupiterDatabase.getReferencePoints();
        ScreenLogger.Instance.addText("Henter Lokation Kvaliteter fra Jupiter...");
        yield return new WaitForSeconds(0.1f);
        mediatorDatabase.LocationQualities = mediatorDatabase.JupiterDatabase.getLocationQualities();
        ScreenLogger.Instance.addText("Henter Lokation Metoder fra Jupiter...");
        yield return new WaitForSeconds(0.1f);
        mediatorDatabase.LocationMethods = mediatorDatabase.JupiterDatabase.getLocationMethods();
        yield break;
    }

    public IEnumerator testDatabases()
    {
        Debug.Log("Test Databases Called!");
        float radius = RadiusPicker.Instance.radius;
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
                Debug.Log("Waiting for GPS to initialize...");
                yield return new WaitForSeconds(0.1f);
            }
        }
        Debug.Log("Got User Location!");
        //Location currentLocation = new Location(55.871675f, 9.886150f); //VIA location
        Database.model.Location currentLocation = new Database.model.Location((float)latitude, (float)longitude);

        while(!mediatorDatabase.IsInitialized)
        {
            Debug.Log("Waiting for MediatorDatabase to initialize...");
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Medaitor database initialized!");

        //yieldsne nedenunder får den til at stoppe når CoRoutine bliver kaldt af ScreenLogger

        //ScreenLogger.Instance.addText("Henter GeoGis Boringer...");
        //yield return new WaitForSeconds(0.1f);
        mediatorDatabase.testGeoGis(currentLocation, radius);
        Debug.Log("Fik GeoGis!");

        //ScreenLogger.Instance.addText("Henter Jupiter Boringer...");
        //yield return new WaitForSeconds(0.1f);
        mediatorDatabase.testJupiter(currentLocation, radius);
        Debug.Log("Fik Jupiter!");
        
        yield break;
    }

    /*
    IEnumerator GetsoapText()//Get Request
    {
        string domain = "http://geusjuptest.geus.dk:80/jupiter.read.1.2.0/jupiterread.svc";
        using (UnityWebRequest www = UnityWebRequest.Get(domain))
        {
            string authorization = authenticate("tester", "Test1234");
            www.SetRequestHeader("AUTHORIZATION", authorization);
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.Send();

            if (www.isError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string soapText = www.downloadHandler.text;
                Debug.Log(soapText);
            }
        }
    }
    */




    /*
    public void selectFromDatabase(String sql)
    {
        selectResult selectResult = null;

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
                selectResultSet resultSet = selectResult.resultset;


                dataRecord[] dataRecords = resultSet.records;
                if (dataRecords != null)
                {
                    String[] columnNames = resultSet.columnNames;
                    String[] columnTypes = resultSet.columnTypes;
                    foreach (dataRecord dataRecord in dataRecords)
                    {
                        object[] row = dataRecord.data;

                        for (int i = 0; i < row.Length; i++)
                        {
                            Debug.Log(columnNames[i] + " : " + columnTypes[i] + " = " + row[i]);
                        }
                    }

                }
                else
                {
                    Debug.Log("DataRecords are null");
                }
            }

            if (selectResult.error != null)
            {
                elementError error = selectResult.error;
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
}
