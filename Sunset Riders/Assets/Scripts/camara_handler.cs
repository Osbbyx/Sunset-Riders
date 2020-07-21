using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camara_handler : MonoBehaviour {

    public GameObject min;
    public GameObject max;
    public bool min_act = false;
    public bool max_act = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        
        if (max_act && !min_act)
        {
            if (max.transform.position.x < GameObject.FindGameObjectWithTag("Nivel").GetComponent<level_handler>().max.transform.position.x)
            {
                transform.position += new Vector3(0.02f, 0, 0); //Aumenta posicion de camara en 5X
            }
        }
	}

}
