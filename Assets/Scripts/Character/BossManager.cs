using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{


    public Animator enemyAnimator;
    public float damage = 20f;
    public float health = 100;
    public GameManager gameManager;


    UnityEngine.AI.NavMeshAgent nm;
    public Transform target;
    public enum AIState { idle, chasing, attack };
    public AIState aiState = AIState.idle;
    public float distanceTreshold = 10f;
    public float attackThreshold = 0.5f;


    public AudioSource audioSource;
    public AudioClip[] zombieSounds;

    // Start is called before the first frame update
    void Start()
    {
        nm = GetComponent<UnityEngine.AI.NavMeshAgent>();
        StartCoroutine(Think());
        audioSource = GetComponent<AudioSource>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = zombieSounds[Random.Range(0, zombieSounds.Length)];
            audioSource.Play();
        }
    }


    public void Hit(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            money.player_money += 50;
            enemyAnimator.SetTrigger("dead");
            gameManager.enemiesAlive--;
            Destroy(gameObject, 5f);
            Destroy(GetComponent<UnityEngine.AI.NavMeshAgent>());
            Destroy(GetComponent<EnemyManager>());
            Destroy(GetComponent<CapsuleCollider>());
        }
    }

    IEnumerator Think()
    {
        while (true)
        {
            switch (aiState)
            {
                case AIState.idle:
                    float dist = Vector3.Distance(target.position, transform.position);
                    if (dist < distanceTreshold)
                    {
                        aiState = AIState.chasing;
                        enemyAnimator.SetBool("Chasing", true);
                    }
                    nm.SetDestination(transform.position);
                    break;
                case AIState.chasing:
                    dist = Vector3.Distance(target.position, transform.position);
                    if (dist > distanceTreshold)
                    {
                        aiState = AIState.idle;
                        enemyAnimator.SetBool("Chasing", false);
                    }

                    if (dist < attackThreshold)
                    {
                        aiState = AIState.attack;
                        enemyAnimator.SetBool("Attacking", true);
                        target.GetComponent<PlayerManager>().Hit(damage);
                    }
                    nm.SetDestination(target.position);
                    break;
                case AIState.attack:
                    Debug.Log("Attack");
                    nm.SetDestination(transform.position);
                    dist = Vector3.Distance(target.position, transform.position);
                    if (dist > attackThreshold)
                    {
                        aiState = AIState.chasing;
                        enemyAnimator.SetBool("Attacking", false);
                    }
                    break;
                default:
                    break;
            }


            yield return new WaitForSeconds(0.2f);
        }
    }
}
