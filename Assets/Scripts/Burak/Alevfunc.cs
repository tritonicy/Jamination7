using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alevfunc : MonoBehaviour
{
    private CinemachineFollow vcam;
    // Start is called before the first frame update
    void Start()
    {
        vcam = FindObjectOfType<CinemachineFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        vcam.TeleportUzayli();
        }  

}
