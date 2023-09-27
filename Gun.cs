using System;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    private Transform cam;
    public float damage, shootingTime;
    private AnimationClip clip;
    private GameObject image;
    public int ammo;
    private float ForceImpact;

    private static int poolCount = 8;
    private static bool AutoExpand = true;
    private static Transform Container;
    private static HideAfterTime Hit, Blood, Breakable, DmgPopUp;
    private static poolMono<HideAfterTime> poolHitParticles, poolBlood, poolBreakable, poolPopUps;

    private static AudioSource hitSound;

    private Image imageComponent;

    public static void AssignStuff(HideAfterTime hitEffect, HideAfterTime blood, HideAfterTime breakable, HideAfterTime dmgPopup,
        Transform container, bool autoExpand)
    {
        Hit = hitEffect; Blood = blood; Breakable = breakable; DmgPopUp = dmgPopup;
        Container = container; AutoExpand = autoExpand;

        poolHitParticles = new poolMono<HideAfterTime>(Hit, poolCount, Container, AutoExpand);
        poolBlood = new poolMono<HideAfterTime>(Blood, poolCount, Container, AutoExpand);
        poolBreakable = new poolMono<HideAfterTime>(Breakable, poolCount, Container, AutoExpand);
        poolPopUps = new poolMono<HideAfterTime>(DmgPopUp, poolCount, Container, AutoExpand);
    }

    private RaycastHit hit;
    private void Start()
    {
        hitSound = GetComponent<AudioSource>();
    }

    public Gun(float damgae, float shootingTime, AnimationClip clip, GameObject image, int ammo, Transform camera)
    {
        this.cam = camera;
        this.damage = damgae;
        this.shootingTime = shootingTime;
        this.clip = clip;

        this.image = image;
        this.imageComponent = image.GetComponent<Image>();
        this.image.SetActive(true);

        this.ammo = ammo;
        this.ForceImpact = damgae * 10;
    }


    public void Shoot()
    {
        Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity);
        Shot();
        AnimationAndSoundPlay();
    }
    public void Shoot(Vector3 shotgunSpread)
    {
        Physics.Raycast(cam.transform.position, shotgunSpread, out hit, Mathf.Infinity);
        try { this.damage = 400f / Vector3.Distance(hit.collider.gameObject.transform.position, cam.transform.position); } catch { }
        Shot();
        AnimationAndSoundPlay();
    }
    public static bool inScope = false;
    public void Shoot(RaycastHit[] hits)
    {
        hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, Mathf.Infinity);
        for (int i = 0; i < hits.Length; i++)
        {
            hit = hits[i];
            Shot();
        }
        image.GetComponent<AudioSource>().Play();
        if(DamageTake.noscope) image.GetComponent<Animator>().Play(clip.name);
    }

    private void Shot()
    {
        try
        {
            if (hit.collider.tag == "Enemy")
            {
                hitSound.Play();

                var bloodParticle = poolBlood.GetFreeElement();
                bloodParticle.transform.position = hit.point;
                bloodParticle.transform.rotation = Quaternion.LookRotation(hit.normal);

                var popUp = poolPopUps.GetFreeElement();
                popUp.GetComponent<TMPro.TextMeshPro>().text = (int)damage + "";
                popUp.transform.position = hit.point;
                popUp.transform.rotation = Quaternion.LookRotation(-hit.normal);

                hit.transform.position += (-hit.normal * ForceImpact / 50);

                hit.collider.gameObject.GetComponent<DamageTake>().Damag(damage);

            }
            else if (hit.collider.tag == "Breakable")
            {
                hitSound.Play();

                var hitparticle = poolHitParticles.GetFreeElement();
                hitparticle.transform.position = hit.point;
                hitparticle.transform.rotation = Quaternion.LookRotation(hit.normal);

                var popUp = poolPopUps.GetFreeElement();
                popUp.GetComponent<TMPro.TextMeshPro>().text = (int)damage + "";
                popUp.transform.position = hit.point;
                popUp.transform.rotation = Quaternion.LookRotation(-hit.normal);

                var breakableSmoke = poolBreakable.GetFreeElement();
                breakableSmoke.transform.position = hit.point;

                hit.transform.gameObject.SetActive(false);

            }
            else
            {
                var hitparticle = poolHitParticles.GetFreeElement();
                hitparticle.transform.position = hit.point;
                hitparticle.transform.rotation = Quaternion.LookRotation(hit.normal);

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * ForceImpact);
                }
            }
        }
        catch { }
    }
    
    private void AnimationAndSoundPlay()
    {
        image.GetComponent<AudioSource>().Play();
        image.GetComponent<Animator>().Play(clip.name);
    }


    public void HideThis()
    {
        imageComponent.enabled = false;
    }
    public void ShowThis()
    {
        imageComponent.enabled = true;
    }
}
