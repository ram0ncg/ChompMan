using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavJugador : MonoBehaviour
{
    public float velocidad = 3f;
    private GameManager gm;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveX, 0f, moveY);
        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }
        transform.position += movement * velocidad * Time.fixedDeltaTime;
        Vector3 anguloTeclas = new Vector3(moveX, 0f, moveY).normalized;
        if (anguloTeclas != null && anguloTeclas != Vector3.zero)
        {
            transform.forward = anguloTeclas * 1;
            transform.rotation = Quaternion.LookRotation(anguloTeclas);
        }
      
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Point") {
            GameManager.points--;
            other.gameObject.SetActive(false);
        }
        else if(other.tag == "Cherry")
        {
            StartCoroutine(gm.CherryAction(other.gameObject));
        }
    }

}