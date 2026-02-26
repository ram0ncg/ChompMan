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
    public Material dangerMaterial;
    public bool timeout;
    public bool scaredGhosts;
    public bool danger;
    private float timeoutTime;
    private Vector3 defaultPosition;
    private AudioSource audioSource;


    void Start()
    {
        audioSource = GameObject.Find("Game").GetComponent<AudioSource>();
        timeout = false;
        chomp = GameObject.Find("Chomp");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        defaultMaterial = GetComponent<SkinnedMeshRenderer>().material;
        defaultPosition = Vector3.zero;
        agent = GetComponent<NavMeshAgent>();
        if (GameManager.hardMode)
        {
            baseVelocity = gameObject.name == "BigGhost" ? 2.7f : 3f;
            timeoutTime = 2f;
        }
        else
        {
            baseVelocity = gameObject.name == "BigGhost" ? 2.5f : 2.7f;
            timeoutTime = 3f;
        }    
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = chomp.transform.position;
        if (!timeout) //Si no se encuentra en timeout
        {
            if((position - gameObject.transform.position).sqrMagnitude < 0.5f) //Si el fantasma esta muy cerca del jugador(en contacto)
            {
                if (scaredGhosts)
                {
                    audioSource.clip = gm.ghostChomp;
                    audioSource.Play();
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
                if (scaredGhosts)
                {
                    //audioSource.clip = gm.scaredGhostsClip;
                    //audioSource.Play();
                    velocity = GameManager.hardMode ? baseVelocity + 0.2f : baseVelocity - 0.5f;
                    position = defaultPosition;
                }
                agent.speed = velocity;
                agent.SetDestination(position);
            }    
        }

        if ((position - gameObject.transform.position).sqrMagnitude < 1.5f && scaredGhosts) //Si Vuelen al centro del mapa estando asustados, empieza su timeout
        {
            StartCoroutine(Timeout());
        }
    }
    IEnumerator Timeout()
    {
        float timer = 0;
        timeout = true;
        while (timer < timeoutTime)
        {
            timer += 0.5f;
            if (timer > timeoutTime - 0.6f)
            {
                danger = true;
                ChangeMaterial();
            }
            
            yield return new WaitForSeconds(0.5f);
        }
        timeout = false;
        scaredGhosts = false;
        danger = false;
    }
    public void ChangeMaterial()
    {
        if (scaredGhosts)
        {
            gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial = deadMaterial;
            if (danger)
            {
                gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial = dangerMaterial;
            }    
        }
        else
        {
            gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial = defaultMaterial;
        }
    }
}
