using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDSway : MonoBehaviour
{
    public float amount = 0.1f;
    public float maxAmount = 0.3f;
    public float smoothAmount = 6.0f;

    private Vector3 initPos;

    Vector3 finalPosToMove;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = -Input.GetAxis("Mouse X") * amount;
        float moveY = -Input.GetAxis("Mouse Y") * amount;
        moveX = Mathf.Clamp(moveX, -maxAmount, maxAmount);
        moveY = Mathf.Clamp(moveY, -maxAmount, maxAmount);

        KeyBoardMovement();
     
        //Mouse movement
        finalPosToMove = new Vector3(moveX, moveY, 100f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosToMove + initPos, Time.deltaTime * smoothAmount);
    }

    void KeyBoardMovement()
    {
        //Keyboard movement
        if (Input.GetKey(KeyCode.W))
        {
            finalPosToMove = new Vector3(-20f, -10f, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosToMove + initPos, Time.deltaTime * smoothAmount);
        }

        if (Input.GetKey(KeyCode.A))
        {
            finalPosToMove = new Vector3(30f, 0f, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosToMove + initPos, Time.deltaTime * smoothAmount);
        }

        if (Input.GetKey(KeyCode.D))
        {
            finalPosToMove = new Vector3(-30f, 0f, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosToMove + initPos, Time.deltaTime * smoothAmount);
        }

        if (Input.GetKey(KeyCode.S))
        {
            finalPosToMove = new Vector3(20f, 10f, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosToMove + initPos, Time.deltaTime * smoothAmount);
        }


        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            finalPosToMove = new Vector3(-40f, -20f, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosToMove + initPos, Time.deltaTime * smoothAmount);
        }
    }
}
