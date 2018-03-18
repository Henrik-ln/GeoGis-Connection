using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcceptOptionsButtonScript : MonoBehaviour {

    public GameObject databaseComponentGO;
    public GameObject optionsPanelGO;

    void Start()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        Debug.Log("Refreshing List");
        ScreenLogger.Instance.clearText();
        DatabaseComponent databaseComponent = databaseComponentGO.GetComponent<DatabaseComponent>();
        StartCoroutine(databaseComponent.testDatabases());
        optionsPanelGO.SetActive(false);
    }
}
