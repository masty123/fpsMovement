using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseLook : MonoBehaviour
{

    public float mouseSensitivityX = 100f;
    public float mouseSensitivityY = 100f;

    public Transform playerBody;

    private Camera fpsCam;
    public PlayerMovement slide;

    [Tooltip("How fast the camera field of view changes when aiming.")]
    public float fovSpeed = 15.0f;
    //Default camera field of view
    [Tooltip("Default value for camera field of view (40 is recommended).")]
    public float defaultFov = 90.0f;
    public float slideFov = 120.0f;

    float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        fpsCam = this.GetComponent<Camera>();
        slide = this.GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        looking();
        SlideFOV();
    }

    void looking()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void SlideFOV()
    {   
        if(slide.isSlide)
        {
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, slideFov, fovSpeed * Time.deltaTime);
        }
    }
}
