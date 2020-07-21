using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy1_handler : MonoBehaviour {

    Vector2 velocidad;
    bool is_grounded = false;
    bool plataforma = false;
    bool cooldown = true;
    public float vel_desp;
    public float vel_salto;
    public GameObject bala;
    public GameObject spawns;
    public GameObject laser_spawn;
    public int puntos; //Puntos que otorgara el enemigo al morir
    private Vector2 pos_min;
    private Vector2 pos_max;
    public enum estados { idle, walking, jump, shooting, dead }
    public estados estado_actual = estados.walking;
    enum direcciones { derecha, izquierda, arriba, abajo, derarr, derab, izqarr, izqab }
    direcciones direccion = direcciones.izquierda;

    // Use this for initialization
    void Start () {
        pos_min = GameObject.Find("min").transform.position;
        pos_max = GameObject.Find("max").transform.position;
        GetComponent<Animator>().SetInteger("estado", 7);
        Invoke("disparar", 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (estado_actual != estados.dead) //Si no esta muerto entonces chequeo teclas, etc
        {
            if(estado_actual == estados.walking && is_grounded)
            {
                if(!GetComponent<SpriteRenderer>().flipX)
                {
                    velocidad.x = -vel_desp;
                }
                else
                {
                    velocidad.x = vel_desp;
                }
            }
        }
    }

    public void muerte()
    {
        if (estado_actual != estados.dead) //Si no estaba muerto, muere
        {
            velocidad.x = 0;
            GetComponent<Animator>().SetInteger("estado", 6); //Cambio estado 1 (animacion caminar)
            estado_actual = estados.dead;
            Destroy(gameObject, 1.0f);
        }
    }

    private void FixedUpdate()
    {
        if (!is_grounded)
        {
            velocidad += Physics2D.gravity * Time.deltaTime; //Multiplicamos gravedad * tiempo para obtener velocidad (v = a*t)
        }

        GetComponent<Rigidbody2D>().position += velocidad * Time.deltaTime;
        check_limites();
    }

    void check_limites()
    {
        if (GetComponent<Rigidbody2D>().position.x > pos_max.x)
        {
            GetComponent<Rigidbody2D>().position = new Vector2(pos_max.x, GetComponent<Rigidbody2D>().position.y);
        }
        else if (GetComponent<Rigidbody2D>().position.x < pos_min.x)
        {
            GetComponent<Rigidbody2D>().position = new Vector2(pos_min.x, GetComponent<Rigidbody2D>().position.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Suelo" || (collision.gameObject.tag == "Plataforma" && velocidad.y < 0))
        {
            if (!is_grounded)
            {
                is_grounded = true;
                velocidad.y = 0;

                GetComponent<Animator>().SetInteger("estado", 0); //Cambio estado 1 (animacion caminar)

                if (collision.gameObject.tag == "Plataforma")
                    plataforma = true;
                else
                    plataforma = false;

                if (estado_actual == estados.dead)
                    velocidad.x = 0;
            }
        }
        else if (collision.gameObject.tag == "Plataforma" && velocidad.y > 0)
        {
            GetComponent<Animator>().SetInteger("estado", 5); //Cambio estado trepar
            velocidad.y += vel_salto * 1.0f;
        }

        if (collision.gameObject.tag == "Obstaculo") //Si colisiona un obstaculo
        {
            velocidad.x = 0; //Velocidad en X es 0 (no avanza)
            RaycastHit2D col = Physics2D.Raycast(new Vector2(laser_spawn.transform.position.x, laser_spawn.transform.position.y), new Vector2(0, 1)); //Creamos Raycast (suerte de laser) para detectar plataforma sup
            
            if (col != null && col.collider != null) //Si colisiono con algo y no estoy tratando de detectar la nada misma
            {
                if (col.collider.gameObject.tag == "Plataforma")
                {
                    if(is_grounded)
                        saltar();
                }
            }

            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX; //Hago el volteado contrario (opuesto)
        }

        if(estado_actual != estados.dead && collision.gameObject.tag == "Bala_J") //Si colisiono con bala de jugador
        {
            GameObject.Find("GameHandler").GetComponent<game_handler>().asignar_puntos(collision.gameObject.GetComponent<bala>().n_jugador, puntos); //Asignamos puntos al jugador que lo mato
            muerte();
        }
    }

    private void OnCollisionExit2D(Collision2D collision) //Al salir de colision
    {
        if ((collision.gameObject.tag == "Plataforma" || collision.gameObject.tag == "Obstaculo") && velocidad.y <= 0) //Chequeo si el objeto del cual sali es una plataforma u obstaculo
        {
            if (collision.transform.position.y < transform.position.y)
            {
                is_grounded = false; //Habilito la caida
                GetComponent<Animator>().SetInteger("estado", 7); //Estado cayendo (animacion)
            }
        }
    }

    void saltar()
    {
        velocidad.x = 0;
        GetComponent<Animator>().SetInteger("estado", 7); //Cambio estado 2 (animacion jump)
        velocidad.y += vel_salto;
        is_grounded = false;
    }


    void disparar()
    {
        if(is_grounded && estado_actual != estados.dead)
        {
            int resultado = Random.Range(0, 11); //Recordar que al ser int el maximo no sera inclusivo (en este caso es de 0 a 10)
            if(resultado > 5)
            {
                estado_actual = estados.shooting;
                velocidad.x = 0;
                GameObject jugador = GameObject.FindGameObjectWithTag("Player").transform.Find("Corazon").gameObject;
                Vector2 distancia = new Vector2(jugador.transform.position.x - Mathf.Abs(transform.position.x), (jugador.transform.position.y - transform.position.y)); //Distancia entre enemigo y jugador (Vector)
                if(distancia.x > 0)
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                float angulo = Mathf.Atan2(distancia.y, distancia.x); //Utilizo Pitagoras (Tang Ang = Op / Ady) para obtener el angulo al que tendria que apuntar
                angulo *= Mathf.Rad2Deg;
                if(angulo < 0f)
                    angulo += 360;

                angulo = Mathf.Round(angulo / 45); //Esto es para que solo pueda rotar cada 45°
                angulo *= 45;

                Vector3 spawn_bala = determinar_posicion(angulo); //Determinare el punto de spawn de la bala

                GameObject newBala = Instantiate(bala, spawn_bala, Quaternion.identity);
                newBala.GetComponent<bala>().asignar_velocidad(angulo);
                newBala.tag = "Bala_E"; //Le pongo etiqueta de Bala_E (bala enemigo)
                Invoke("cooldown_reincorporarse", 1.0f); //Reincorporo para correr
            }
        }

        Invoke("disparar", 1.5f);
    }

    void cooldown_reincorporarse()
    {
        estado_actual = estados.walking;
        GetComponent<Animator>().SetInteger("estado", 0);
    }

    Vector3 determinar_posicion(float angulo)
    {
        if(angulo == 0)
        {
            GetComponent<Animator>().SetInteger("estado", 3);
            return spawns.transform.Find("Spawn_D").transform.position;
        }
        else if(angulo == 90)
        {
            GetComponent<Animator>().SetInteger("estado", 1);
            return spawns.transform.Find("Spawn_AR").transform.position;
        }
        else if(angulo == 180)
        {
            GetComponent<Animator>().SetInteger("estado", 3);
            return spawns.transform.Find("Spawn_I").transform.position;
        }
        else if(angulo == 270)
        {
            GetComponent<Animator>().SetInteger("estado", 4);
            return spawns.transform.Find("Spawn_AB").transform.position;
        }
        else if(angulo == 45)
        {
            GetComponent<Animator>().SetInteger("estado", 2);
            return spawns.transform.Find("Spawn_DARR").transform.position;
        }
        else if(angulo == 135)
        {
            GetComponent<Animator>().SetInteger("estado", 2);
            return spawns.transform.Find("Spawn_IARR").transform.position;
        }     
        else if(angulo == 225)
        {
            GetComponent<Animator>().SetInteger("estado", 4);
            return spawns.transform.Find("Spawn_IAB").transform.position;
        }
        else if(angulo == 315)
        {
            GetComponent<Animator>().SetInteger("estado", 4);
            return spawns.transform.Find("Spawn_DAB").transform.position;
        }
            

        return new Vector3(0,0,0);
    }


    void habilitar_cooldown()
    {
        cooldown = true;
       GetComponent<Animator>().SetInteger("estado", 0);
    }


    public void check_direccion(Vector3 posicion)
    {
        GameObject newBala = Instantiate(bala, posicion, Quaternion.identity);

        switch (direccion)
        {
            case direcciones.derecha:
                newBala.GetComponent<bala>().asignar_velocidad(0);
                break;

            case direcciones.izquierda:
                newBala.GetComponent<bala>().asignar_velocidad(180);
                break;

            case direcciones.arriba:
                newBala.GetComponent<bala>().asignar_velocidad(90);
                break;

            case direcciones.abajo:
                newBala.GetComponent<bala>().asignar_velocidad(270);
                break;

            case direcciones.derab:
                newBala.GetComponent<bala>().asignar_velocidad(315);
                break;

            case direcciones.derarr:
                newBala.GetComponent<bala>().asignar_velocidad(45);
                break;

            case direcciones.izqab:
                newBala.GetComponent<bala>().asignar_velocidad(225);
                break;

            case direcciones.izqarr:
                newBala.GetComponent<bala>().asignar_velocidad(135);
                break;
        }
    }

    private void OnBecameInvisible() //Al salir de la camara
    {
        Destroy(gameObject); //Destruye el objeto
    }
}
