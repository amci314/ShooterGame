using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeLauncher : Gun
{
    private poolMono<HideAfterTime> granadePool;
    [SerializeField] private HideAfterTime granadePrefab;
    private Transform playerCamera;
    public GranadeLauncher(float damage, float shootingTime, AnimationClip clip, GameObject image, int ammo, Transform camera, Transform container) : base(damage, shootingTime, clip, image, ammo, camera)
    {
        playerCamera = camera;
        granadePool = new poolMono<HideAfterTime>(granadePrefab, 10, container, true);
    }
    public void ShootCh(MonoBehaviour monoBehaviour)
    {
        var granade = granadePool.GetFreeElement();
        granade.transform.position = playerCamera.position;
        granade.transform.rotation = playerCamera.rotation;
        granade.GetComponent<Rigidbody>().AddForce(playerCamera.forward * 80f, ForceMode.Impulse);
        granade.GetComponent<Rigidbody>().AddForce(transform.up * 20f, ForceMode.Impulse);
    }
}
