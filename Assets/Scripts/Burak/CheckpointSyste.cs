using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSyste : MonoBehaviour
{
    private CinemachineFollow vcam;
    private PlayerController playerController;
    private float retryTimeCounter = 1.5f;
    private float retryTimer;

    private void Start() {
        vcam = FindObjectOfType<CinemachineFollow>();
        playerController = FindObjectOfType<PlayerController>();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.R)) {
            retryTimer += Time.deltaTime;
            if(retryTimer > retryTimeCounter) {
                TeleportCheckpoint();
                retryTimer = 0f;
            }
        }
    }

    public void TeleportCheckpoint() {
        playerController.current.transform.position = vcam.currentStartPos.position;
    }
}
