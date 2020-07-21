using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn_e1 : MonoBehaviour {

    public int num_enemigo;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnBecameVisible()
    {
        GameObject newEnemy = Instantiate(GameObject.Find("GameHandler").GetComponent<game_handler>().indice_objetos[num_enemigo], transform.position, Quaternion.identity); //Instanciamos nuevo enemigo
        Destroy(gameObject);
    }
}
