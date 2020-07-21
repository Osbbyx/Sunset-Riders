using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam_trigger : MonoBehaviour {

    public bool minmax; //Min falso, Max verdadero

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            if(minmax)
            {
                transform.parent.GetComponent<camara_handler>().max_act = true;
            }
            else
            {
                transform.parent.GetComponent<camara_handler>().min_act = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if(minmax)
            {
                transform.parent.GetComponent<camara_handler>().max_act = false;
            }
            else
            {
                transform.parent.GetComponent<camara_handler>().min_act = false;
            }
        }
    }
}
