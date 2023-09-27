using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Uzi : Gun
{
    public Uzi(float damage, float shootingTime, AnimationClip clip, GameObject image, int ammo, Transform camera) : base(damage, shootingTime, clip, image, ammo, camera)
    {
    }
    

    public int overheat = 0;
    public bool coolingDown = false;
    public void ShootCh()
    {
        overheat += 2;
        base.Shoot();
    }

}
