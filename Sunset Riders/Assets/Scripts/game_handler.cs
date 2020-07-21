using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;

public class game_handler : MonoBehaviour {

    public List<GameObject> indice_objetos;
    public List<GameObject> spawns;
    public List<int> vidas_j;
    public List<int> puntos_j;
    public float offset_x_lifes;



    // Use this for initialization
    void Start() {
        load_data();
        

    }

    void spawn_j1()
    {
        for (int i = 0; i < vidas_j[0]; i++) //Recorro las vidas del Jugador
        {
            GameObject newVida = Instantiate(indice_objetos[4], GameObject.Find("corchete1").transform); //Instanciar una nueva vida y agregar como hijo al corchete correspondiente
            newVida.GetComponent<RectTransform>().position += new Vector3((i) * offset_x_lifes, 0, 0); //Posiciono vida segun el offset (elemento i)
            newVida.name = "Vida" + i;
        }
        GameObject newBilly = Instantiate(indice_objetos[0], spawns[0].transform.position, Quaternion.identity);
        newBilly.name = "Billy";
        newBilly.GetComponent<player_handler>().player_n = 1;
    }

    void spawn_j2()
    {
        for (int i = 0; i < vidas_j[0]; i++) //Recorro las vidas del Jugador
        {
            GameObject newVida = Instantiate(indice_objetos[4], GameObject.Find("corchete2").transform); //Instanciar una nueva vida y agregar como hijo al corchete correspondiente
            newVida.GetComponent<RectTransform>().position += new Vector3((i) * offset_x_lifes, 0, 0); //Posiciono vida segun el offset (elemento i)
            newVida.name = "Vida" + i;
        }
        GameObject newCormano = Instantiate(indice_objetos[1], spawns[1].transform.position, Quaternion.identity);
        newCormano.name = "Cormano";
        newCormano.GetComponent<player_handler>().player_n = 2;
    }

    void actualizar_puntos(int n_jugador)
    {
        string numero_corchete_jugador = "corchete" + n_jugador.ToString();
        GameObject.Find(numero_corchete_jugador).transform.Find("txt_puntos").GetComponent<Text>().text = "$" + puntos_j[n_jugador - 1].ToString();
    }

    public void set_vidas(int valor, int jugador_n)
    {
        Debug.Log(jugador_n);
        vidas_j[jugador_n] += valor; //Le incrementa x vidas al jugador_n
        if (valor < 0)
        {
            check_vidas(jugador_n); //Chequeamos si le quedan vidas para respawnear
            if(vidas_j[jugador_n] >= 0)
                eliminar_vida(jugador_n); //Actualizo GUI
        }
            

    }

    void check_vidas(int jugador_n)
    {
        if (vidas_j[jugador_n] >= 0) //Compruebo si el jugador tiene mas de 0 vidas
        {
                string nombre_jugador = "respawn_j" + (jugador_n+1);
                Invoke(nombre_jugador, 2.0f); //Respawneo al jugador dentro de 2 segundos
        }
        else //Se murio y no tiene mas vidas
        {
            for(int i = 0; i < vidas_j.Count; i++) //Recorro cuantos jugadores existan para ver si alguno tiene vidas, sino termino el juego
            {
                if(vidas_j[i] > 0) //Si algun jugador todavia tiene vidas o esta vivo con su ultima
                {
                    return; //No termino el juego, finalizo la funcion
                }
            }

            Invoke("back_menu", 3.0f); //Espero 3 segundos para volver al menu
        }
    }

    void back_menu()
    {
        SceneManager.LoadScene(0); //Carga el menu principal
    }

    void respawn_j1()
    {
        GameObject newBilly = Instantiate(indice_objetos[0], spawns[0].transform.position, Quaternion.identity);
        newBilly.name = "Billy";
        newBilly.GetComponent<player_handler>().player_n = 1;
    }

    void respawn_j2()
    {
        GameObject newCormano = Instantiate(indice_objetos[1], spawns[1].transform.position, Quaternion.identity);
        newCormano.name = "Cormano";
        newCormano.GetComponent<player_handler>().player_n = 2;
    }

    void eliminar_vida(int jugador_n) //Actualiza las vidas en la interfaz (GUI/HUD)
    {
            string nombre_corchete = "corchete" + (jugador_n+1);
            if(GameObject.Find(nombre_corchete).transform.Find("Vida" + vidas_j[jugador_n]))
            {
                Destroy(GameObject.Find(nombre_corchete).transform.Find("Vida" + vidas_j[jugador_n]).gameObject);
            }
    }

    public void asignar_puntos(int n_jugador, int puntos_n)
    {
        puntos_j[n_jugador] += puntos_n; //Le agrego puntos_n al jugador_n
        actualizar_puntos(n_jugador+1);
    }

	// Update is called once per frame
	void Update () {
		
	}

    void load_data()
    {
        int cant_players = 1;
        if (File.Exists(Application.persistentDataPath + "/gamedata.dat"))
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamedata.dat", FileMode.Open); //Generamos lectura del archivo
            cant_players = (int)bf.Deserialize(file);
            //Todas las lineas de lectura (importante orden)
            file.Close();

            for(int i = 0; i < cant_players; i++)
            {
                vidas_j[i] = 0; //Le asigno 2 vidas a la cantidad de players que se haya seleccionado
            }
        }

        spawn_j1(); //Siempre spawnea al menos al jugador 1

        if(cant_players > 1)
        {
            spawn_j2();
        }

        for(int i = 0; i < cant_players; i++)
        {
            actualizar_puntos(i+1);
        }

    }
}
