using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alper_enemyBehaviour : MonoBehaviour
{

    //private Animator enemyAnimator;
    public Transform[] patrolPoints;
    public float moveSpeed;
    public int PatrolDestination;
    public Transform playerCheck;
    public float maxAgroDistance;
    public LayerMask whatIsPlayer;
    private bool playerSeen = false;
    private int pointCounter = 0;
    private bool isTourCompleted = false;

    // Start is called before the first frame update
    void Start()
    {
        //enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!playerSeen)
        {
            transform.position = Vector2.MoveTowards(transform.position, patrolPoints[pointCounter].position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, patrolPoints[pointCounter].position) < .2f)
            {
                if (isTourCompleted)
                {
                    if(patrolPoints[pointCounter].transform.position.x != patrolPoints[pointCounter - 1].transform.position.x)
                    {
                        transform.Rotate(0f, 180f, 0f);
                    }

                    pointCounter--;

                    if (pointCounter == 0)
                    {
                        isTourCompleted = false;
                    }
                }
                else
                {
                    if (pointCounter != patrolPoints.Length - 1)
                    {
                        if (patrolPoints[pointCounter].transform.position.x != patrolPoints[pointCounter + 1].transform.position.x)
                        {
                            transform.Rotate(0f, 180f, 0f);
                        }
                    }

                    pointCounter++;
                }

                if(pointCounter == patrolPoints.Length -1)
                {
                    isTourCompleted = true;
                }
            }
        }

        /*if (PatrolDestination == 1 && !playerSeen)
        {
            transform.position = Vector2.MoveTowards(transform.position, patrolPoints[1].position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, patrolPoints[1].position) < .2f)
            {
                transform.Rotate(0f, 180f, 0f);
                PatrolDestination = 0;
            }
        }*/

        if (!playerSeen)
        {
            playerSeen = CheckPlayerInMaxAgroRange();
        }else
        {
            Debug.Log("Oyun Bitti");
        }

    }

    public virtual bool CheckPlayerInMaxAgroRange()
    {
        return Physics2D.Raycast(playerCheck.position, transform.right,maxAgroDistance,whatIsPlayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * maxAgroDistance), 0.2f);
    }
}
