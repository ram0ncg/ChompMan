using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private GameObject chomp;
    private float baseVelocity;
    private NavMeshAgent agent;
    private Material defaultMaterial;
    private GameManager gm;
    public Material deadMaterial;
    private bool timeout;
    private Vector3 defaultPosition;


    void Start()
    {
        timeout = false;
        chomp = GameObject.Find("Chomp");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        defaultMaterial = GetComponent<SkinnedMeshRenderer>().material;
        defaultPosition = Vector3.zero;
        agent = GetComponent<NavMeshAgent>();
        baseVelocity = gameObject.name == "BigGhost" ? 2.5f : 2.7f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = chomp.transform.position;
        if (gm.leaveBase)
        {
            timeout = false;
            gm.leaveBase = false;
        }
        if (!timeout) //Si no se encuentra en timeout
        {
            if((position - gameObject.transform.position).sqrMagnitude < 0.5f) //Si el fantasma esta muy cerca del jugador(en contacto)
            {
                if (gm.scaredGhosts)
                {
                    GameManager.kills++;
                    Destroy(gameObject);
                    return;//DIE GHOST
                }
                else if(!GameManager.gameOver)
                {
                    GameManager.gameOver = true; //GAMEOVER
                }
            }
            else //Si no esta en contacto, se movera 
            {
                float velocity = baseVelocity;
                ChangeMaterial();
                if (gm.scaredGhosts)
                {
                    velocity = baseVelocity / 2f;
                    position = defaultPosition;
                }
                agent.speed = velocity;
                agent.SetDestination(position);
            }    
        }

        if ((position - gameObject.transform.position).sqrMagnitude < 1.5f && gm.scaredGhosts) //Si Vuelen al centro del mapa estando asustados, empieza su timeout
        {
            timeout = true;
        }
    }
    public void ChangeMaterial()
    {
        if (gm.scaredGhosts)
        {
            gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial = deadMaterial;
        }
        else
        {
            gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial = defaultMaterial;
        }
    }
}
