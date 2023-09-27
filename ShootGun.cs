using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootGun : MonoBehaviour
{
    //GUN SYSTEM SCRIPT
    [SerializeField] private bool autoExpand = true;
    [SerializeField] private Transform Container;
    private static poolMono<HideAfterTime> poolHitParticles, poolBlood, poolBreakable, poolPopUps;
    [SerializeField]
    private HideAfterTime Hit, Blood, Breakable, DmgPopUp;


    [SerializeField] private Transform playerCamera, playerBody;
    [SerializeField] private AnimationClip pistolAnimation, shotgunAnimation, uziAnimation, sniperAnimation, LauncherAnimation;
    public GameObject pistolImg, shotgunImg, uziImg, sniperImg, LauncherImg;

    private Gun currentGun;
    private Pistol pistol;
    private Shotgun shotgun;
    private Uzi uzi;
    private Sniper sniper;
    private GranadeLauncher granadeLauncher;
    private List<Gun> gunList;

    [SerializeField] private Text ammoTxt;
    private int ammoBoxAmount;

    [SerializeField] private Image crosshair;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Gun.AssignStuff(Hit, Blood, Breakable, DmgPopUp, Container, autoExpand);

        pistol = new Pistol(10, 1, pistolAnimation, pistolImg, 1, playerCamera);
        shotgun = new Shotgun(0, 0.75f, shotgunAnimation, shotgunImg, 10, playerCamera);
        uzi = new Uzi(10, 0.15f, uziAnimation, uziImg, 50, playerCamera);
        sniper = new Sniper(150, 1, sniperAnimation, sniperImg, 10, playerCamera);
        //granadeLauncher = new GranadeLauncher(0, 0.75f, LauncherAnimation, LauncherImg, 5, playerCamera, Container);

        gunList = new List<Gun>()
        {
            pistol, shotgun, uzi, sniper, //granadeLauncher
        };

       foreach (Gun gun in gunList)
            gun.HideThis();

        currentGun = pistol;
        currentGun.ShowThis();
    }

    private float next = 0f, yRot = 0f;
    [SerializeField] private float Sensitivity = 1000f;

    void Update()
    {
        yRot -= Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
        yRot = Mathf.Clamp(yRot, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(yRot, 0f, 0f);
        playerBody.Rotate(Vector3.up * (Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime));

        if (Input.GetKey("1"))
        {
            currentGun.HideThis();
            currentGun = pistol; //as pistol
            currentGun.ShowThis();
            crosshair.enabled = true;
            DamageTake.noscope = false;
            ammoBoxAmount = 0;
            RePaintAmmo();
        }
        else if (Input.GetKey("2"))
        {
            currentGun.HideThis();
            currentGun = shotgun;
            currentGun.ShowThis();
            crosshair.enabled = true;
            DamageTake.noscope = false;
            ammoBoxAmount = 10;
            RePaintAmmo();
        }
        else if (Input.GetKey("3"))
        {
            currentGun.HideThis();
            currentGun = uzi;
            currentGun.ShowThis();
            crosshair.enabled = true;
            DamageTake.noscope = false;
            ammoBoxAmount = 50;
            RePaintAmmo();
        }else if (Input.GetKey("4"))
        {
            currentGun.HideThis();
            currentGun = sniper;
            currentGun.ShowThis();
            crosshair.enabled = false;
            DamageTake.noscope = true;
            ammoBoxAmount = 50;
            RePaintAmmo();
        }else if (Input.GetKey("5"))
        {
            currentGun.HideThis();
            currentGun = granadeLauncher;
            currentGun.ShowThis();
            crosshair.enabled = true;
            DamageTake.noscope = false;
            ammoBoxAmount = 50;
            RePaintAmmo();
        }

        if (Input.GetButton("Fire1"))
        {
            if (Time.time >= next && currentGun.ammo > 0)
            {
                next = Time.time + currentGun.shootingTime;
                currentGun.ammo--;
                switch (currentGun)
                {
                    case Pistol:
                        ((Pistol)currentGun).ShootCh(this);
                        Invoke("RePaintAmmo", 1.02f); 
                        break;
                    case Shotgun:
                        ((Shotgun)currentGun).ShootCh();
                        break;
                    case Uzi:
                        if (!uzi.coolingDown) ((Uzi)currentGun).ShootCh();
                        else currentGun.ammo++;
                        break;
                    case Sniper:
                        ((Sniper)currentGun).ShootCh();
                        break;
                    case GranadeLauncher:
                        ((GranadeLauncher)currentGun).ShootCh(this);
                        break;
                }

                RePaintAmmo();
            }
        }

        if (currentGun.Equals(sniper))
        {
            if (Input.GetButtonDown("Fire2"))
            {
                DamageTake.noscope = false;
                sniperImg.GetComponent<Animator>().Play("scopingAnim");
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                DamageTake.noscope = true;
                sniperImg.GetComponent<Animator>().Play("New State");
            }
        }

        if (uzi.overheat >= 75 && !uzi.coolingDown)
        {
            StartCoroutine(chill());
        }

        if (!uzi.coolingDown && nextOverHeat > 0 && Time.time >= next)
        {
            next = Time.time + 0.5f;
            uzi.overheat -= 1;
        }
        levelOfOverheating = (byte)(255 - (uzi.overheat * 255 / 75));
        overheatingColor.g = levelOfOverheating;
        overheatingColor.b = levelOfOverheating;
        uziImg.GetComponent<Image>().color = overheatingColor;


    }
    [SerializeField] private AnimationClip overheatUziAnimationClip;
    private float nextOverHeat = 0f;
    private Color32 overheatingColor = new Color32(255, 255, 255, 255);
    private Color32 chilledColor = new Color32(255, 255, 255, 255);
    private byte levelOfOverheating;


    private  IEnumerator chill()
    {
        uzi.coolingDown = true;
        uziImg.GetComponent<Image>().color = chilledColor;
        uziImg.GetComponent<Animator>().Play(overheatUziAnimationClip.name);
        yield return new WaitForSeconds(4);
        uzi.overheat = 0;
        uzi.coolingDown = false;
    }


    private void RePaintAmmo()
    {
        ammoTxt.text = currentGun.ammo.ToString(); 
    }

    public void PlusAmmo()
    {
        currentGun.ammo += ammoBoxAmount;
    }
}
