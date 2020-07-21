using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class player_handler : MonoBehaviour {

    Vector2 velocidad;
    bool is_grounded = false;
    bool plataforma = false;
    bool cooldown = true;
    public int player_n;
    public float vel_desp;
    public float vel_salto;
    public GameObject spr1;
    public GameObject bala;
    public GameObject spr2;
    public GameObject spawns;
    public GameObject spr_sup;
    public GameObject spr_inf;
    public GameObject ref_colA;
    public GameObject ref_colI;
    public GameObject ref_posA;
    public GameObject ref_posI;
    public GameObject ref_flipV;
    public GameObject ref_flipF;
    Vector2 last_collider;
    public List<KeyCode> tecla;
    private Vector2 pos_min;
    private Vector2 pos_max;
    enum estados { idle, walking, jump, dead }
    estados estado_actual = estados.idle;
    bool[] teclas = { false, false, false, false }; //0 Arriba, 1 Abajo,2 Izquierda, 3 Derecha
    enum direcciones {derecha,izquierda,arriba,abajo,derarr,derab,izqarr,izqab }
    direcciones direccion = direcciones.derecha;

    void Start() { //Bloque
        pos_min = GameObject.Find("min").transform.position;
        pos_max = GameObject.Find("max").transform.position;
        GetComponent<Animator>().SetInteger("estado", 2);
        spr2.transform.position += new Vector3(-0.09f, 0, 0);
    }
	
	void Update () { //Actualizar

        if (estado_actual != estados.dead) //Si no esta muerto entonces chequeo teclas, etc
        {
            Vector3 posicion = transform.position; //Creamos una variable para copiar la posicion del transform provisoriamente
                                                   //SI presiono tal tecla hace tal cosa
            if (Input.GetKeyDown(tecla[1]) && GetComponent<Animator>().GetInteger("estado") != 3)
            {
                teclas[2] = true;
                velocidad.x = -vel_desp;
                if (!spr1.GetComponent<SpriteRenderer>().flipX)
                {
                    spr1.GetComponent<SpriteRenderer>().flipX = true;
                    spr2.GetComponent<SpriteRenderer>().flipX = true;
                    if (is_grounded)
                        spr2.transform.position += new Vector3(-0.07f, 0, 0);
                    else
                        spr2.transform.position += new Vector3(0.09f, 0, 0);
                }
                if (is_grounded)
                {
                    GetComponent<Animator>().SetInteger("estado", 1); //Cambio estado 1 (animacion walk)
                }
            }

            if (Input.GetKeyDown(tecla[3]) && GetComponent<Animator>().GetInteger("estado") != 3)
            {
                teclas[3] = true;
                velocidad.x = vel_desp;
                if (spr1.GetComponent<SpriteRenderer>().flipX)
                {
                    spr1.GetComponent<SpriteRenderer>().flipX = false;
                    spr2.GetComponent<SpriteRenderer>().flipX = false;
                    if (is_grounded)
                        spr2.transform.position += new Vector3(+0.07f, 0, 0);
                    else
                        spr2.transform.position += new Vector3(-0.09f, 0, 0);
                }

                if (is_grounded)
                    GetComponent<Animator>().SetInteger("estado", 1); //Cambio estado 1 (animacion walk)
            }

            if (((Input.GetKeyUp(tecla[1]) && velocidad.x < 0) || (Input.GetKeyUp(tecla[3]) && velocidad.x > 0)))
            {
                velocidad.x = 0.0f;
                if (is_grounded)
                    GetComponent<Animator>().SetInteger("estado", 0); //Cambio estado 1 (animacion walk)
            }

            if (Input.GetKeyDown(tecla[2]) && is_grounded)
            {
                velocidad.x = 0.0f;
                GetComponent<Animator>().SetInteger("estado", 3); //Agacha
                spr2.transform.position = ref_posA.transform.position;
                transform.position += new Vector3(0, -0.2f, 0);
                GetComponent<BoxCollider2D>().offset = ref_colA.GetComponent<BoxCollider2D>().offset;
                GetComponent<BoxCollider2D>().size = ref_colA.GetComponent<BoxCollider2D>().size;

                if (spr1.GetComponent<SpriteRenderer>().flipX)
                {
                    spr2.transform.position = new Vector2(ref_flipV.transform.position.x, spr2.transform.position.y);
                }
                else
                {
                    spr2.transform.position = new Vector2(ref_flipF.transform.position.x, spr2.transform.position.y);
                }
            }

            if (Input.GetKeyUp(tecla[2]) && (GetComponent<Animator>().GetInteger("estado") == 3 || GetComponent<Animator>().GetInteger("estado") == 0 || GetComponent<Animator>().GetInteger("estado") == 1))
            {
                GetComponent<Animator>().SetInteger("estado", 0); //Idle
                
                transform.position += new Vector3(0, 0.2f, 0);
                if(spr1.GetComponent<SpriteRenderer>().flipX)
                {
                    spr2.transform.position = ref_flipV.transform.position;
                }
                else
                {
                    spr2.transform.position = ref_flipF.transform.position;
                }
                GetComponent<BoxCollider2D>().offset = ref_colI.GetComponent<BoxCollider2D>().offset;
                GetComponent<BoxCollider2D>().size = ref_colI.GetComponent<BoxCollider2D>().size;
            }

            if (Input.GetKeyDown(tecla[0]))
                teclas[0] = true;

            if (Input.GetKeyDown(tecla[2]))
                teclas[1] = true;

            if (Input.GetKeyUp(tecla[1]))
                teclas[2] = false;

            if (Input.GetKeyUp(tecla[3]))
                teclas[3] = false;

            if (Input.GetKeyUp(tecla[0]))
                teclas[0] = false;

            if (Input.GetKeyUp(tecla[2]))
                teclas[1] = false;


            if (Input.GetKeyUp(tecla[5]) && is_grounded) //Salto
            {
                if (GetComponent<Animator>().GetInteger("estado") != 3)
                {
                    GetComponent<Animator>().SetInteger("estado", 2); //Cambio estado 2 (animacion jump)
                    velocidad.y += vel_salto;
                    is_grounded = false;

                    if (!spr1.GetComponent<SpriteRenderer>().flipX)
                    {
                        spr2.transform.position += new Vector3(-0.09f, 0, 0);
                    }
                    else
                    {
                        spr2.transform.position += new Vector3(0.09f, 0, 0);
                    }
                }
                else if (plataforma) //Si esta agachado voy a querer ver si esta en plataforma para bajarlo
                {
                    plataforma = false;
                    GetComponent<Animator>().SetInteger("estado", 2);
                    spr2.transform.position += new Vector3(0, -0.0911f, 0);
                    is_grounded = false;

                    if (!spr1.GetComponent<SpriteRenderer>().flipX)
                    {
                        spr2.transform.position += new Vector3(-0.09f, 0, 0);
                    }
                    else
                    {
                        spr2.transform.position += new Vector3(0.09f, 0, 0);
                    }
                }
            }

            if (Input.GetKeyDown(tecla[4]) && cooldown)
            {

                cooldown = false;
                chequear_teclas();
                Invoke("habilitar_cooldown", 0.5f);
            }
        }
	}


    public void muerte()
    {
        if (estado_actual != estados.dead) //Si no estaba muerto, entonces muere
        {
            GetComponent<Animator>().SetInteger("estado", 5); //Cambio estado 1 (animacion caminar)
            spr_sup.GetComponent<Animator>().SetInteger("estado", 7);
            estado_actual = estados.dead;
            GameObject.Find("GameHandler").GetComponent<game_handler>().set_vidas(-1, player_n-1); //Quitamos vida al jugador
            string nombre_spawn = "spawn_j" + player_n;
            GameObject.Find(nombre_spawn).transform.position = Camera.main.transform.position;
            Destroy(gameObject, 2.0f);
        }
    }

    private void FixedUpdate()
    {

        detectar_suelo();
        detectar_obstaculo();

        if (!is_grounded)
        {
            velocidad += Physics2D.gravity * Time.deltaTime; //Multiplicamos gravedad * tiempo para obtener velocidad (v = a*t)
        }
        
        GetComponent<Rigidbody2D>().position += velocidad * Time.deltaTime;
        check_limites();
    }


    void detectar_obstaculo()
    {
        RaycastHit2D col;
        if (!spr1.GetComponent<SpriteRenderer>().flipX)
            col = Physics2D.Raycast(new Vector2(transform.Find("Pies").transform.position.x, transform.Find("Pies").transform.position.y), new Vector2(1, 0));
        else
            col = Physics2D.Raycast(new Vector2(transform.Find("Pies").transform.position.x, transform.Find("Pies").transform.position.y), new Vector2(-1, 0));
        if (col && col.transform.tag == "Obstaculo") //Si colisiona un obstaculo
        {
            if (Mathf.Abs(col.point.x - transform.Find("Pies").transform.position.x) < 0.08f)
            {
                Debug.Log(col.point.x - transform.Find("Corazon").transform.position.x);
                velocidad.x = 0; //Velocidad en X es 0 (no avanza)
                if (GetComponent<Animator>().GetInteger("estado") == 1) //Si estaba caminando
                {
                    GetComponent<Animator>().SetInteger("estado", 0); //Lo pongo en reposo
                }

                if (!spr1.GetComponent<SpriteRenderer>().flipX)
                {
                    transform.position += new Vector3(-0.09f, 0, 0);
                }
                else
                {
                    transform.position += new Vector3(0.09f, 0, 0);
                }
            }
        }
    }

    void detectar_suelo()
    {
        RaycastHit2D col = Physics2D.Raycast(new Vector2(transform.Find("Corazon").transform.position.x, transform.Find("Corazon").transform.position.y), new Vector2(0, -10));
   
        if (col && GetComponent<Animator>().GetInteger("estado") != 3 && (col.collider.tag == "Suelo" || col.collider.tag == "Plataforma" || col.collider.tag == "Obstaculo"))
        {
            last_collider = col.point;
            if (Mathf.Abs(col.point.y - transform.Find("Pies").transform.position.y) > 0.05f && is_grounded) //Si la distancia entre pies y collider (suelo) es mayor a 10
            {
                cayendo();
            }
            else if(Mathf.Abs(col.point.y - transform.Find("Pies").transform.position.y) <= 0.03f)
            {
              
                transform.position = new Vector3(transform.position.x, col.point.y + (transform.position.y - transform.Find("Pies").transform.position.y) + 0.03f, transform.position.z);


                if (col.transform.tag == "Suelo" || (col.transform.tag == "Obstaculo" && velocidad.y < 0) || (col.transform.tag == "Plataforma" && velocidad.y < 0))
                {
                    if (!is_grounded)
                    {
                        is_grounded = true;
                        velocidad.y = 0;

                        if (velocidad.x != 0) //Si la velocidad es diferente a 0, es porque me estoy moviendo hacia algun lado
                            GetComponent<Animator>().SetInteger("estado", 1); //Cambio estado 1 (animacion caminar)
                        else
                            GetComponent<Animator>().SetInteger("estado", 0); //Cambio estado 0 (animacion idle)

                        spr_sup.GetComponent<Animator>().SetInteger("estado", 0); //CAmbio a idle la parte de arriba tambien

                        if (!spr1.GetComponent<SpriteRenderer>().flipX)
                        {
                            spr2.transform.position += new Vector3(0.09f, 0, 0);
                        }
                        else
                        {
                            spr2.transform.position += new Vector3(-0.09f, 0, 0);
                        }

                        if (col.transform.tag == "Plataforma")
                            plataforma = true;
                        else
                            plataforma = false;

                        if (estado_actual == estados.dead)
                            velocidad.x = 0;
                    }
                }
    

            }
        }
        else
        {
            if(GetComponent<Animator>().GetInteger("estado") != 3)
                transform.position = new Vector3(transform.position.x, last_collider.y + (transform.position.y - transform.Find("Pies").transform.position.y) + 0.1f, transform.position.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.transform.tag == "Bala_E" || collision.gameObject.transform.tag == "Enemigo") //Si colisiono con bala de enemigo
        {
            if (collision.gameObject.transform.tag == "Enemigo" && collision.gameObject.transform.GetComponent<enemy1_handler>().estado_actual == enemy1_handler.estados.dead)
            {
                return; //Salgo y no ejecuto muerte porque el enemigo ya esta muerto y no quiero que afecte al personaje
            }
            muerte();

            if (is_grounded) //Si esta en el suelo cuando murio
                velocidad.x = 0; //Detener
        }
        else if (collision.gameObject.transform.tag == "Plataforma" && velocidad.y > 0 && teclas[0] == true)
        {
            GetComponent<Animator>().SetInteger("estado", 4); //Cambio estado trepar
            spr_sup.GetComponent<Animator>().SetInteger("estado", 6);
            velocidad.y += vel_salto * 1.2f;
        }
        
    }

    void check_limites()
    {
        if(GetComponent<Rigidbody2D>().position.x > pos_max.x)
        {
            GetComponent<Rigidbody2D>().position = new Vector2(pos_max.x, GetComponent<Rigidbody2D>().position.y);
        }
        else if(GetComponent<Rigidbody2D>().position.x < pos_min.x)
        {
            GetComponent<Rigidbody2D>().position = new Vector2(pos_min.x, GetComponent<Rigidbody2D>().position.y);
        }

        float ancho_camara = Camera.main.aspect * Camera.main.orthographicSize; //Obtengo mitad del ancho de la camara segun su relacion de aspecto en la proyeccion ortografica

        if (GetComponent<Rigidbody2D>().position.x < Camera.main.transform.position.x - ancho_camara) //Si me fui en X al inferior de la camara entonces
        {
            GetComponent<Rigidbody2D>().position = new Vector2(Camera.main.transform.position.x - ancho_camara, GetComponent<Rigidbody2D>().position.y);
        }

        if (GetComponent<Rigidbody2D>().position.x > GameObject.Find("cam_max").transform.position.x) //Si me fui en X al inferior de la camara entonces
        {
            GetComponent<Rigidbody2D>().position = new Vector2(GameObject.Find("cam_max").transform.position.x, GetComponent<Rigidbody2D>().position.y);
        }
    }






    void cayendo()
    {
        is_grounded = false; //Habilito la caida
        GetComponent<Animator>().SetInteger("estado", 2); //Estado cayendo (animacion)

        if (!spr1.GetComponent<SpriteRenderer>().flipX)
        {
            spr2.transform.position += new Vector3(-0.09f, 0, 0);
        }
        else
        {
            spr2.transform.position += new Vector3(0.09f, 0, 0);
        }
    }

    void habilitar_cooldown()
    {
        cooldown = true;
        spr_sup.GetComponent<Animator>().SetInteger("estado", 0);
    }

    void chequear_teclas()
    {
        Vector3 posicion = new Vector3();

        if (teclas[0]) //Arriba
        {
            if (teclas[2])
            {
                direccion = direcciones.izqarr;
                spr_sup.GetComponent<Animator>().SetInteger("estado", 2);
                posicion = spawns.transform.Find("Spawn_IARR").transform.position;
            }
            else if (teclas[3])
            {
                direccion = direcciones.derarr;
                spr_sup.GetComponent<Animator>().SetInteger("estado", 2);
                posicion = spawns.transform.Find("Spawn_DARR").transform.position;
            }
            else
            {
                direccion = direcciones.arriba;
                spr_sup.GetComponent<Animator>().SetInteger("estado", 4);
                posicion = spawns.transform.Find("Spawn_AR").transform.position;
            }

        }
        else if (teclas[1]) //Abajo
        {
            if (teclas[2])
            {
                direccion = direcciones.izqab;
                spr_sup.GetComponent<Animator>().SetInteger("estado", 3);
                posicion = spawns.transform.Find("Spawn_IAB").transform.position;
            }

            else if (teclas[3])
            {
                direccion = direcciones.derab;
                spr_sup.GetComponent<Animator>().SetInteger("estado", 3);
                posicion = spawns.transform.Find("Spawn_DAB").transform.position;
            }
            else
            {
                direccion = direcciones.abajo;
                spr_sup.GetComponent<Animator>().SetInteger("estado", 5);
                posicion = spawns.transform.Find("Spawn_AB").transform.position;
                
            }

        }
        else if (teclas[2]) //Izquierda
        {
            direccion = direcciones.izquierda;
            spr_sup.GetComponent<Animator>().SetInteger("estado", 1);
            posicion = spawns.transform.Find("Spawn_I").transform.position;
            
        }
        else if (teclas[3]) //Derecha
        {
            direccion = direcciones.derecha;
            spr_sup.GetComponent<Animator>().SetInteger("estado", 1);
            posicion = spawns.transform.Find("Spawn_D").transform.position;
        }
        else
        {
            spr_sup.GetComponent<Animator>().SetInteger("estado", 1);
            if(spr1.GetComponent<SpriteRenderer>().flipX)
            {
                posicion = spawns.transform.Find("Spawn_I").transform.position;
                direccion = direcciones.izquierda;
            }
            else
            {
                posicion = spawns.transform.Find("Spawn_D").transform.position;
                direccion = direcciones.derecha;

            }
                
        }

        check_direccion(posicion);


    }

    public void check_direccion(Vector3 posicion)
    {
        GameObject newBala = Instantiate(bala, posicion, Quaternion.identity);
        newBala.GetComponent<bala>().n_jugador = 0; //Le identificamos la bala como del player 0 (player 1)

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
}
