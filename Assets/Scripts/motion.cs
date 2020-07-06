using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class motion : MonoBehaviour
{   
    public Camera fpsCam;
    public Transform weapon;

    private Vector3 weaponOrigin;
    // Start is called before the first frame update
    void Start()
    {
        weaponOrigin = weapon.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HeadBob(float z, float x_Intensity, float y_Intensity)
    {
        weapon.localPosition = new Vector3 (Mathf.Sin(z) * x_Intensity, Mathf.Sin(z) * y_Intensity, weaponOrigin.z);
    }
}
