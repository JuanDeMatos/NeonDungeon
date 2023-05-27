using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyCameraRotation : MonoBehaviour
{
    private Transform playerCamera;

    // Start is called before the first frame update
    void Awake()
    {
        playerCamera = GameObject.Find("CM vcam1").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(playerCamera);
        transform.eulerAngles = new Vector3(90,0,transform.eulerAngles.z);
    }
}
