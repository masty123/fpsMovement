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
        }
    }
}
