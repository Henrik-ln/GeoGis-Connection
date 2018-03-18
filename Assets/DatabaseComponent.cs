
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
    void Start () {
        
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

    public IEnumerator testDatabases()
    {
        MediatorDatabase mediator = new MediatorDatabase();

        float radius = RadiusPicker.Instance.radius;
        //float radius = 0.005f;
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
        Database.model.Location currentLocation = new Database.model.Location((float)latitude, (float)longitude);

        mediator.testGeoGis(currentLocation, radius);
        mediator.testJupiter(currentLocation, radius);
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
