using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HideAfterTime : MonoBehaviour
{
    [SerializeField] float seconds = 2;
    private void Start()
    {

        if (this.gameObject.GetComponent<ParticleSystem>() != null)
            seconds = GetComponent<ParticleSystem>().main.duration; 

        if (this.gameObject.tag == "LauncherNade")
        {
            Invoke("Boom", 5f);
            seconds = 6;
        }
        
        StartCoroutine(Waiting());
    }
    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(seconds);
        this.gameObject.SetActive(false);
    }


    private poolMono<HideAfterTime> pool;
    [SerializeField] private HideAfterTime explosion;
    void Boom()
    {
        this.gameObject.GetComponent<SphereCollider>().enabled = false;
        pool = new poolMono<HideAfterTime>(this.explosion, 10, this.transform);
        pool.autoExpand = true;
        var explosionn = this.pool.GetFreeElement();
        explosionn.transform.position = this.transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, 5f);
        
        foreach (Collider col in objectsInRange)
        {
            switch (col.tag)
            {
                case "Enemy":
                    col.GetComponent<DamageTake>().Damag(150f); 
                    break;
                case "Breakable":
                    col.gameObject.SetActive(false); 
                    break;
                case "Player":
                    col.GetComponent<health>().TakeDamage(50, 125);
                    break;
            }
        }
        Boom();
    }
    private void FixedUpdate()
    {
        if (this.gameObject.tag == "PopUpText")
            this.gameObject.transform.Translate(Vector3.up * 0.3f);
    }
}
