using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunPistol : Gun
{
    public override void Trigger()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();

            //Spawn casing prefab at spawnpoint
            var casing = Instantiate(Prefabs.casingPrefab,
                Spawnpoints.casingSpawnPoint.transform.position,
                Spawnpoints.casingSpawnPoint.transform.rotation);

            //Add velocity to the bullet
            casing.GetComponent<Rigidbody>().velocity =
                 casing.transform.right * 5f;
        }
    }
}
