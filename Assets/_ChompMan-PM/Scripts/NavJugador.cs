using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavJugador : MonoBehaviour
{
    float velocidad;
    GameObject GameobjectwithCharacterController;
    CharacterController controller ;
    private GameManager gm;
    public Enemy enemy1;
    public Enemy enemy2;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        controller = this.GetComponent<CharacterController>();
         velocidad = 1.5f;
    }
    void Update()
    {
    }
    void FixedUpdate()
    {

        //Capturo el movimiento en los ejes
        float movimientoH = Input.GetAxis("Horizontal");
        float movimientoV = Input.GetAxis("Vertical");

        Vector3 anguloTeclas = new Vector3(movimientoH, 0f, movimientoV).normalized;
        
        transform.Translate(anguloTeclas * velocidad * Time.deltaTime, Space.World);

        //Genero el vector de movimiento
        //Muevo el jugador
        //transform.position += anguloTeclas * velocidad * Time.deltaTime;
        controller.Move(anguloTeclas * velocidad * Time.deltaTime);
        if (anguloTeclas != null && anguloTeclas != Vector3.zero)
        {
            transform.forward = anguloTeclas * 1;
            transform.rotation = Quaternion.LookRotation(anguloTeclas);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Point") {
            gm.points--;
            other.gameObject.SetActive(false);
        }
        else if(other.gameObject.name == "Cherry Variant")
        {
            enemy1.scaredGhost = true;
            enemy1.ChangeMaterial();
            enemy2.scaredGhost = true;
            enemy2.ChangeMaterial();
            other.gameObject.SetActive(false);
        }
        else if(other.tag == "Player")
        {
            Debug.Log("X");
            gm.gameOver = true;
        }
    }

}