using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public GameObject chomp;
    private float baseVelocity;
    private NavMeshAgent agent;
    private Material defaultMaterial;
    public Material deadMaterial;
    private Vector3 defaultPosition;
    public bool scaredGhost;
    public bool deadGhost;
    void Start()
    {
        defaultMaterial = GetComponent<MeshRenderer>().material;
        defaultPosition = gameObject.transform.position;
        agent = GetComponent<NavMeshAgent>();
        baseVelocity = gameObject.name == "Ghost" ? 2.2f : 2.7f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = chomp.transform.position;
        if (!deadGhost)
        {
            float velocity;
            if (scaredGhost)
            {
                velocity = 6f;
                position = defaultPosition;
            }
            else
            {
                velocity = baseVelocity;
            }
            agent.speed = velocity;
            agent.SetDestination(position);
            
        }
        if ((position - gameObject.transform.position).sqrMagnitude < 1.5f && scaredGhost)
        {
            deadGhost = true;
            StartCoroutine(DeadGhostTimeout());
        }
    }
    IEnumerator DeadGhostTimeout()
    {
        yield return new WaitForSeconds(1.5f);
        deadGhost = false;
        scaredGhost = false;
        ChangeMaterial();
    }
    public void ChangeMaterial()
    {
        if (scaredGhost)
        {
            gameObject.GetComponent<MeshRenderer>().material = deadMaterial;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
        }
    }
}
