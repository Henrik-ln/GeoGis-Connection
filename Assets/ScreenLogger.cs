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
        scrollableText.text = scrollableText.text + "\n" + "\n" + text;
    }
    
	
}
