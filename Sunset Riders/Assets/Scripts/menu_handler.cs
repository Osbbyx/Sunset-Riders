using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //Libreria de UI y Funciones de Interfaz
using UnityEngine.SceneManagement; //Libreria para administrar escenas
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;

public class menu_handler : MonoBehaviour {

    bool opc_act = true; //True es 1 player, false es 2 player
    public GameObject pistola1;
    public GameObject pistola2;
    public datos data_g = new datos(); //Creamos un objeto del tipo datos (serializable)

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Z)) //Tecla select
        {
            opc_act = !opc_act; //Siempre va a la opcion contraria (el opuesto)
            if(opc_act) //Si es player 1 (cambiar grafico pistola a player 1)
            {
                data_g.players = 1;
                pistola1.GetComponent<Image>().enabled = true;
                pistola2.GetComponent<Image>().enabled = false;
            }
            else //Si es player 2 (cambiar grafico pistola a player 2)
            {
                data_g.players = 2;
                pistola2.GetComponent<Image>().enabled = true;
                pistola1.GetComponent<Image>().enabled = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.X)) //Start
        {
            save_data();
            SceneManager.LoadScene(1); //Cambio a la escena 1 (escena del juego)
        }
            
    }


    void save_data()
    {
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/gamedata.dat", FileMode.Create);
        bf.Serialize(file, data_g.players); //Guardo este dato
        //Si tuviera mas datos los guardo aca, importante el orden porque se leeran en mismo orden
        file.Close();

    }

    [Serializable]
    public class datos
    {
        public int players = 1;
    }

}
