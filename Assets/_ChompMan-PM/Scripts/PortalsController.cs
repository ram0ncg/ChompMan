using UnityEngine;

public class PortalsController : MonoBehaviour
{
    public GameObject portalR;
    public GameObject portalL;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();        
    }
    private void OnTriggerEnter(Collider other)
    {
        Vector3 offset = -transform.forward * 2f;
        audioSource.Play();
        other.transform.position = gameObject.name == "PortalR" ? portalL.transform.position : portalR.transform.position;
        other.transform.position += offset;
    }
}
