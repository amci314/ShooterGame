using UnityEngine;

public class Shotgun : Gun
{
    private Transform playerCamera;
    public Shotgun(float damage, float shootingTime, AnimationClip clip, GameObject image, int ammo, Transform camera) : base(damage, shootingTime, clip, image, ammo, camera)
    {
        this.playerCamera = camera;
    }
    public void ShootCh()
    {
        for(int i = 0; i < 6; i++)
            base.Shoot(ShotgunSpread(playerCamera));
    }

    private Vector3 targetPos, direction;
    Vector3 ShotgunSpread(Transform playerCamera)
    {
        targetPos = playerCamera.transform.position + playerCamera.transform.forward * 6;
        targetPos = new Vector3(
            targetPos.x + Random.Range(-0.5f, 0.5f),
            targetPos.y + Random.Range(-0.5f, 0.5f),
            targetPos.z + Random.Range(-0.5f, 0.5f)
            );
        direction = targetPos - playerCamera.transform.position;
        return direction.normalized;
    }

}
