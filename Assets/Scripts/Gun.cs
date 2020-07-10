using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Gun : MonoBehaviour
{
    [Header("Gun Properties")]
    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 300f;
    public float fireRate = 15f;
    public float aimSpeed;
    //Ammo
    public int magazine = 180;
    public int maxAmmo = 31;
    public int currentAmmo;
    public float reloadTime = 2f;
    public float nextTimeToFire = 0f;
    //Aiming
    private Vector3 originalPosition;
    public Vector3 aimPosition;
    public float adsSpeed = 8f;

    public Camera fpsCam;

    [Header("Gun Camera Options")]
    public ParticleSystem muzzleFlash;
    public Light muzzleFlashLight;
    public float lightDuration;
    //public GameObject impactEffect;
    public Animator animator;

    [Header("Mouse Sensitivity")]
    private mouseLook mouselook;
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

    //crosshair
    public Image crosshair;


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


    //Boolean Section

    //Check if reloading
    private bool isReloading;
    //Holstering weapon
    //private bool hasBeenHolstered = false;
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
    //private bool soundHasPlayed = false;
    //adjustSen
    private bool isAdjust;




    [Tooltip("How much force is applied to the bullet when shooting.")]
    public float bulletForce = 400.0f;

    //new bullet hole
    public GameObject bulletHole;
    public LayerMask canBeShot;

    private PlayerMovement slide;

    [System.Serializable]
    public class prefabs
    {
        [Header("Prefabs")]
        public Transform bulletPrefab;
        public Transform casingPrefab;
    }
    public prefabs Prefabs;

    [System.Serializable]
    public class spawnpoints
    {
        [Header("Spawnpoints")]
        //Array holding casing spawn points 
        //(some weapons use more than one casing spawn)
        //Casing spawn point array
        public Transform casingSpawnPoint;
        //Bullet prefab spawn from this point
        public Transform bulletSpawnPoint;
    }
    public spawnpoints Spawnpoints;


    private Quaternion originalRotation;
    private bool isCrouch;

    [Header("Melee Section")]
    //Melee Section
    public Transform attackPoint;
    public float attackRange = 0.5f;

    [Header("Text Properties")]
    public Text ammo;
    public Text magAmmo;



    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        mouselook = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<mouseLook>();
        crosshair = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<Image>();
        //Get default sensitivity
        defaultX = mouselook.mouseSensitivityX;
        defaultY = mouselook.mouseSensitivityY;

        //Ammo stuff
        currentAmmo = maxAmmo;
        originalPosition = transform.localPosition;
        slide = GetComponentInParent<PlayerMovement>();

        ammo = GameObject.FindGameObjectWithTag("currentAmmo").GetComponent<Text>();
        magAmmo = GameObject.FindGameObjectWithTag("maxAmmo").GetComponent<Text>();


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
        InspectWeapon();
        Melee();
        SlideAnim();

        updateAmmo();

    }

    void  updateAmmo()
    {
        ammo.text = currentAmmo.ToString();
        magAmmo.text = magazine.ToString();
    }

    private void OnEnable()
    {
        isReloading = false;
        animator = this.GetComponent<Animator>();
        isReloading = false;
        shootAudioSource.clip = SoundClips.takeOutSound;
        shootAudioSource.Play();

    }

    public virtual void Trigger()
    {   

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0f)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            
            /*
            //Spawn bullet from bullet spawnpoint
            var bullet = (Transform)Instantiate(
                Prefabs.bulletPrefab,
                Spawnpoints.bulletSpawnPoint.transform.position,
                Spawnpoints.bulletSpawnPoint.transform.rotation);

            //Add velocity to the bullet
            bullet.GetComponent<Rigidbody>().velocity =
                bullet.transform.forward * bulletForce;
           */

            //Spawn casing prefab at spawnpoint
            GameObject casing = Instantiate(Prefabs.casingPrefab.gameObject, Spawnpoints.casingSpawnPoint.position, Spawnpoints.casingSpawnPoint.rotation);

            //Add velocity to the bullet
           casing.GetComponent<Rigidbody>().velocity = casing.transform.right * 5f;
            Destroy(casing, 5f);

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
        ammo.text = currentAmmo.ToString();

        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward,out hit, range))
        {
            //Debug.Log(hit.transform.name);

            TargetObj target = hit.transform.GetComponent<TargetObj>();
            if(target != null)
            {
                target.takeDmg(damage);
            }

            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
            //GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            //Destroy(impactGO, 2f);
            hit = new RaycastHit();
            if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, canBeShot))
            {
                GameObject impactGO = Instantiate(bulletHole, hit.point + hit.normal * 0.001f, Quaternion.identity);
                impactGO.transform.LookAt(hit.point + hit.normal);
                Destroy(impactGO, 5f);
            }


        }

    }

    public void ADS()
    {   
       
        if (Input.GetButton("Fire2") && !isReloading)
        {
            ZoomIn();
        }
        else
        {
            ZoomOut();
        }
    }


    void ZoomIn()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, Time.deltaTime * adsSpeed);
        isAiming = true;
        fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, aimFov, fovSpeed * Time.deltaTime);
        adjustSensitivity();
        crosshair.color = new Color(crosshair.color.r, crosshair.color.g, crosshair.color.b, 0f);
    }

    void ZoomOut()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * adsSpeed);
        isAiming = false;
        fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, defaultFov, fovSpeed * Time.deltaTime);
        adjustSensitivity();
        crosshair.color = new Color(crosshair.color.r, crosshair.color.g, crosshair.color.b, 0.5f);
    }
    

    void adjustSensitivity()
    {
       if( isAiming && !isAdjust )
        {
            mouselook.mouseSensitivityX = mouselook.mouseSensitivityX * adsSensitivityX;
            //Debug.Log(mouselook.mouseSensitivityX);
            mouselook.mouseSensitivityY = mouselook.mouseSensitivityY * adsSensitivityY;
           // Debug.Log(mouselook.mouseSensitivityY);
            isAdjust = true;
        }
       else if (!isAiming && isAdjust)
        {
            mouselook.mouseSensitivityX = defaultX;
            mouselook.mouseSensitivityY = defaultY;
            isAdjust = false;
        }
    }

    
    void InspectWeapon()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            animator.Play("Inspect", 0, 0f);
        }
    }


    void Melee()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            int rnd = Random.Range(0, 2);
            
            if(rnd == 1)
            {
                 StartCoroutine(meleeOne());
               // meleeOne();
            }
            else
            {
                 StartCoroutine(meleeTwo());
               // meleeTwo();
            }
        }
    }

    IEnumerator meleeOne()
    //void meleeOne()
    {
        animator.Play("Knife Attack 1", 0 , 0f);
        Collider[] hitThing = Physics.OverlapSphere(attackPoint.position, attackRange, canBeShot);

        foreach (Collider enemy in hitThing)
        {
            //Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<Rigidbody>().AddForce(attackPoint.position.normalized * impactForce);
        }
        yield return new WaitForSeconds(2f);
    }

    IEnumerator meleeTwo()
    //void meleeTwo()
    {
        animator.Play("Knife Attack 2", 0, 0f);
        Collider[] hitThing = Physics.OverlapSphere(attackPoint.position, attackRange, canBeShot);

        foreach(Collider enemy in hitThing)
        {
            //Debug.Log("We hit "+ enemy.name);

            if (enemy.GetComponent<Rigidbody>() != null)
            {
                enemy.GetComponent<Rigidbody>().AddForce(attackPoint.position.normalized * impactForce);
            }
        }
        yield return new WaitForSeconds(1f);
    }

    private void OnDrawGizmosSelected()
    {   
        if(attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo >= 0 && currentAmmo != maxAmmo && magazine > 0f)
        {
            StartCoroutine(ReloadCouroutinewithAmmo());
            magAmmo.text = magazine.ToString();
        }
        else if (currentAmmo <= 0 && magazine > 0f)
        {
            StartCoroutine(ReloadCouroutineNoAmmo());
            magAmmo.text = magazine.ToString();

        }
    }

    IEnumerator ReloadCouroutineNoAmmo()
    {       
        if(maxAmmo >= 0)
        {
            isReloading = true;
            animator.Play("Reload Out Of Ammo", 0, 0f);
            shootAudioSource.clip = SoundClips.reloadSoundOutOfAmmo;
            shootAudioSource.Play();
            yield return new WaitForSeconds(reloadTime);
            currentAmmo = maxAmmo;
            magazine -= maxAmmo;
            isReloading = false;
        }
          
    }

    IEnumerator ReloadCouroutinewithAmmo()
    {   
        if(maxAmmo >= 0f)
        {
            isReloading = true;
            animator.Play("Reload Ammo Left", 0, 0f);
            shootAudioSource.clip = SoundClips.reloadSoundAmmoLeft;
            shootAudioSource.Play();
            yield return new WaitForSeconds(reloadTime);
            
            int remainingAmmo = (maxAmmo - currentAmmo);
            int result = (magazine - remainingAmmo);
            if (result > 0)
            {
                currentAmmo += remainingAmmo;
                magazine -= remainingAmmo;
            }
            else
            {
                currentAmmo += magazine;
                magazine = 0;
            }
            isReloading = false;
        }
      
    }

    private IEnumerator MuzzleFlashLight()
    {

        muzzleFlashLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);
        muzzleFlashLight.enabled = false;
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

    /*
    IEnumerator tilt(Vector3 axis, float angle, float duration)
    {

        Quaternion from = transform.rotation;
        Quaternion to = transform.rotation;
        to *= Quaternion.Euler(axis * angle);

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = to;
    }
    */

    /*
    void CrouchRotate()
    {

        if (Input.GetKey(KeyCode.LeftControl) && !isCrouch)
        {
            //transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(0, 0, 400f), 200f * Time.deltaTime);
            //StartCoroutine(tilt(Vector3.forward, 45, 0.2f));
            isCrouch = true;
            //Debug.Log(transform.rotation);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) && isCrouch)
        {
            //transform.localRotation = Quaternion.Lerp(this.transform.localRotation, originalRotation, 200f * Time.deltaTime);
            //StartCoroutine(tilt(Vector3.forward, -45, 0.2f));
            isCrouch = false;
            //Debug.Log(transform.rotation);
        }

    }
    */

    void SlideAnim()
    {
        if(slide.isSlide)
        {
            animator.Play("Idle", 0, 0f);
        }
    }
}


