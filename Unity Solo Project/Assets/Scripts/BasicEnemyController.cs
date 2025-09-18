using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyController : MonoBehaviour
{
    NavMeshAgent agent;
    public float EnemyVision = 30f;
    public float EnemyAggro = 30f;
    public int WanderChance = 60;
    int IsWander;
    Vector3 EnemyPosition;
    Vector3 PlayerPosition;
    float EnemySight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyPosition = transform.position;
        PlayerPosition = GameObject.Find("Player").transform.position;
        EnemySight = Vector3.Distance(EnemyPosition, PlayerPosition);
        if (EnemySight < EnemyVision)
        {
            agent.destination = GameObject.Find("Player").transform.position;
            EnemyVision = EnemyAggro;
        }
        else
        {
            IsWander = (RandomNumberGenerator.GetInt32(1, WanderChance));
            if (IsWander == 1)
            {
                agent.destination += new Vector3(RandomNumberGenerator.GetInt32(-9, 9), 0, RandomNumberGenerator.GetInt32(-9, 9));
            }
        }

    }
}
