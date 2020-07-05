using UnityEngine;
using System.Collections;
public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 300f;
    public float fireRate = 15f;
    public float aimSpeed;

    public Camera fpsCam;

    [Header("Gun Camera Options")]
    public ParticleSystem muzzleFlash;
    public Light muzzleFlashLight;
    public float lightDuration;
    public GameObject impactEffect;
    public Animator animator;
    public mouseLook mouselook;

    //percentage
    public float adsSensitivityX;
    public float adsSensitivityY;
    public float defaultX;
    public float defaultY;


    [Header("Gun Camera Options")]
    //How fast the camera field of view changes when aiming 
    [Tooltip("How fast the camera field of view changes when aiming.")]
    public float fovSpeed = 15.0f;
    //Default camera field of view
    [Tooltip("Default value for camera field of view (40 is recommended).")]
    public float defaultFov = 90.0f;
    public float aimFov = 25.0f;

    [Header("Audio Source")]
    //Main audio source
    public AudioSource mainAudioSource;
    //Audio source used for shoot sound
    public AudioSource shootAudioSource;

    [System.Serializable]
    public class soundClips
    {
        public AudioClip shootSound;
        public AudioClip takeOutSound;
        public AudioClip holsterSound;
        public AudioClip reloadSoundOutOfAmmo;
        public AudioClip reloadSoundAmmoLeft;
        public AudioClip aimSound;
    }
    public soundClips SoundClips;
    
    public float nextTimeToFire = 0f;

    //Check if reloading
    private bool isReloading;
    //Holstering weapon
    private bool hasBeenHolstered = false;
    //If weapon is holstered
    private bool holstered;
    //Check if running
    private bool isRunning;
    //Check if aiming
    private bool isAiming;
    //Check if walking
    private bool isWalking;
    //Check if inspecting weapon
    private bool isInspecting;
    //check if sound has played
    private bool soundHasPlayed = false;
    //adjustSen
    private bool isAdjust;

    //Ammo
    public int maxAmmo = 31;
    private int currentAmmo;
    public float reloadTime = 2f;

    private Vector3 originalPosition;
    public Vector3 aimPosition;
    public float adsSpeed = 8f;

    

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        mouselook = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<mouseLook>();
        //Get default sensitivity
        defaultX = mouselook.mouseSensitivityX;
        defaultY = mouselook.mouseSensitivityY;
        currentAmmo = maxAmmo;
        originalPosition = transform.localPosition;
     
    }

    // Update is called once per frame
    void Update()
    {
        if(isReloading)
        {
            return;
        }

        Trigger();
        Reload();
        ADS();
    }

    private void OnEnable()
    {
        isReloading = false;
        animator = this.GetComponent<Animator>();
        isReloading = false;
    }

    public virtual void Trigger()
    {   

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    public virtual void  Shoot()
    {
        muzzleFlash.Play();
        shootAudioSource.clip = SoundClips.shootSound;
        shootAudioSource.Play();
        StartCoroutine(MuzzleFlashLight());
        animator.Play("Fire", 0, 0f);

        //wasting ammo?
        currentAmmo--;

        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward,out hit, range))
        {
            Debug.Log(hit.transform.name);

            TargetObj target = hit.transform.GetComponent<TargetObj>();
            if(target != null)
            {
                target.takeDmg(damage);
            }

            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);
        }

    }

    public void ADS()
    {   
       
        if (Input.GetButton("Fire2") && !isReloading)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, Time.deltaTime * adsSpeed );
            isAiming = true;
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, aimFov, fovSpeed * Time.deltaTime);
            adjustSensitivity();
            // animator.SetBool("Aim", true);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * adsSpeed);
            isAiming = false;
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, defaultFov, fovSpeed * Time.deltaTime);
            adjustSensitivity();
            // animator.SetBool("Aim", false);
        }
    }

    void adjustSensitivity()
    {
       if( isAiming && !isAdjust )
        {
            mouselook.mouseSensitivityX = mouselook.mouseSensitivityX * adsSensitivityX;
            Debug.Log(mouselook.mouseSensitivityX);
            mouselook.mouseSensitivityY = mouselook.mouseSensitivityY * adsSensitivityY;
            Debug.Log(mouselook.mouseSensitivityY);
            isAdjust = true;
        }
       else if (!isAiming && isAdjust)
        {
            mouselook.mouseSensitivityX = defaultX;
            mouselook.mouseSensitivityY = defaultY;
            isAdjust = false;
        }
    }

   /* public void Aim()
    {
        if (Input.GetButton("Fire2") && !isReloading && !isRunning && !isInspecting)
        if (Input.GetButton("Fire2") && !isReloading)
        {

            isAiming = true;
            Start aiming
            animator.SetBool("Aim", true);

            When right click is released
             fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView,
                aimFov, fovSpeed * Time.deltaTime);

            if (!soundHasPlayed)
            {
                mainAudioSource.clip = SoundClips.aimSound;
                mainAudioSource.Play();
                soundHasPlayed = true;
            }
        }
        else
        {
            When right click is released
             fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView,
                 defaultFov, fovSpeed * Time.deltaTime);

            isAiming = false;
            Stop aiming
            animator.SetBool("Aim", false);
            soundHasPlayed = false;
        }

    }*/


    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo >= 0 && currentAmmo != maxAmmo)
        {
            StartCoroutine(ReloadCouroutinewithAmmo());
        }
        else if (Input.GetKeyDown(KeyCode.R) && currentAmmo >= 0 && currentAmmo != maxAmmo && isAiming )
        {
            isAiming = false;
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView,
            defaultFov, fovSpeed * Time.deltaTime);
            StartCoroutine(ReloadCouroutinewithAmmo());
        }
        else if (currentAmmo <= 0)
        {
            StartCoroutine(ReloadCouroutineNoAmmo());
        }
        else if (currentAmmo <= 0 && isAiming)
        {
            isAiming = false;
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView,
            defaultFov, fovSpeed * Time.deltaTime);
            StartCoroutine(ReloadCouroutineNoAmmo());
        }
    }

    IEnumerator ReloadCouroutineNoAmmo()
    {
            isReloading = true;
            animator.Play("Reload Out Of Ammo", 0, 0f);
            shootAudioSource.clip = SoundClips.reloadSoundOutOfAmmo;
            shootAudioSource.Play();
            yield return new WaitForSeconds(reloadTime);
            currentAmmo = maxAmmo;
            isReloading = false;
    }

    IEnumerator ReloadCouroutinewithAmmo()
    {
        isReloading = true;
        animator.Play("Reload Ammo Left", 0, 0f);
        shootAudioSource.clip = SoundClips.reloadSoundAmmoLeft;
        shootAudioSource.Play();
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    private IEnumerator MuzzleFlashLight()
    {

        muzzleFlashLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);
        muzzleFlashLight.enabled = false;
    }
}


