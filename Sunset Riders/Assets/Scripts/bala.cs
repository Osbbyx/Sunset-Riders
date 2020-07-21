using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bala : MonoBehaviour {

    Vector2 velocidad;
    public float vel_desp;
    public int n_jugador; //Numero de jugador propietario de este proyectil


    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().position += velocidad * Time.deltaTime;
    }

    
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(transform.tag == "Bala_E" && collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<player_handler>().muerte();
        }
        else if(transform.tag == "Bala_J" && collision.gameObject.tag == "Enemigo")
        {
            collision.gameObject.GetComponent<enemy1_handler>().muerte();
            GameObject.Find("GameHandler").GetComponent<game_handler>().asignar_puntos(n_jugador, collision.gameObject.GetComponent<enemy1_handler>().puntos); //Asignamos puntos al jugador que lo mato
        }
    }

    public void asignar_velocidad(float angulo)
    {
        velocidad.x = vel_desp * Mathf.Cos(deg2rad(angulo)); //ADYACENTE = HIPOTENUSA * COS ANGULO
        velocidad.y = vel_desp * Mathf.Sin(deg2rad(angulo)); //OPUESTO = HIPOTENUSA * SIN ANGULO
    }

    public float deg2rad(float angulo)
    {
        return angulo * 3.14f / 180.0f;
    }

   
    
}
