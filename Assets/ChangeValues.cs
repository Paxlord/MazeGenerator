using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeValues : MonoBehaviour {

    public Text text;
    Slider sl;

	// Use this for initialization
	void Start () {
        this.sl = GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeText()
    {
        text.text = sl.value.ToString();
    }
}
