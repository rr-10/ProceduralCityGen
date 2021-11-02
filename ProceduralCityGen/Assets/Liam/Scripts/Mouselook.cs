using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouselook : MonoBehaviour
{

    public Transform playerBod;

    public float senstivity = 100f;

    float xrotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mousex = Input.GetAxis("Mouse X") * senstivity * Time.deltaTime;
        float mousey = Input.GetAxis("Mouse Y") * senstivity * Time.deltaTime;

        xrotation -= mousey;

        transform.localRotation = Quaternion.Euler(xrotation, 0f, 0f);
        xrotation = Mathf.Clamp(xrotation, -90f, 90f);
        playerBod.Rotate(Vector3.up * mousex);
    }
}
