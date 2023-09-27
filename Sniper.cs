using System.Threading.Tasks;
using UnityEngine;

public class Sniper : Gun
{
    public Sniper(float damage, float shootingTime, AnimationClip clip, GameObject image, int ammo, Transform camera) : base(damage, shootingTime, clip, image, ammo, camera)
    {
    }
    private RaycastHit[] hits;
    public void ShootCh()
    {
        base.Shoot(hits);
    }
}
