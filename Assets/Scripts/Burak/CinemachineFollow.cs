using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class CinemachineFollow : MonoBehaviour
{
    private CinemachineVirtualCamera vCamClose;
    [SerializeField] CinemachineVirtualCamera vCamStatic;
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject gameManager;
    [SerializeField] GameObject[] LevelBorders;
    [SerializeField] Transform[] levelStartPositions;
    [SerializeField] Transform[] cameraStartPositions;
    public Transform currentStartPos;
    public GameObject currentBorder;
    public int currentLevel = 1;

    private void Start() {
        vCamClose = this.GetComponent<CinemachineVirtualCamera>();
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Update() {
        vCamClose.Follow = gameManager.GetComponent<PlayerController>().current.transform;

            switch (currentLevel) {
            case 1:
                currentStartPos = levelStartPositions[0];
                currentBorder = LevelBorders[0];
                vCamStatic.transform.position = cameraStartPositions[0].gameObject.transform.position;
                break;
            case 2:
                currentStartPos = levelStartPositions[1];
                currentBorder = LevelBorders[1];
                vCamStatic.transform.position = cameraStartPositions[1].gameObject.transform.position;

                break;
            case 3:
                currentStartPos = levelStartPositions[2];
                currentBorder = LevelBorders[2];
                vCamStatic.transform.position = cameraStartPositions[2].gameObject.transform.position;

                break;
            case 4:
                currentStartPos = levelStartPositions[3];
                currentBorder = LevelBorders[3];
                vCamStatic.transform.position = cameraStartPositions[3].gameObject.transform.position;

                break;
            case 5:
                currentStartPos = levelStartPositions[4];
                currentBorder = LevelBorders[4];
                vCamStatic.transform.position = cameraStartPositions[4].gameObject.transform.position;

                break;
            default:
                break;
        }
        vCamClose.GetComponent<CinemachineConfiner>().m_BoundingShape2D = currentBorder.GetComponent<Collider2D>();

    }

    public void TeleportUzayli() {
        playerController.mainObject.SetActive(true);
        playerController.current = playerController.mainObject;
        playerController.current.transform.position = levelStartPositions[currentLevel-1].position;
    }


}
