using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenLogger : MonoBehaviour {
    public static ScreenLogger Instance { set; get; }
    public Text scrollableText;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    public void addText(string text)
    {
        //Debug.Log("Future length: " + (scrollableText.text.Length + text.Length));
        if (scrollableText.text.Length + text.Length > 16000)
        {
            //Debug.Log("Gonna exceed 65000 chars. No go");
        }
        else
        {
            scrollableText.text = scrollableText.text + "\n" + "\n" + text;
        }
        
    }

    public void clearText()
    {
        scrollableText.text = "";
    }
}
