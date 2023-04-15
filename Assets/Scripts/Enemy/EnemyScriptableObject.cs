using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ScriptableObject that holds the BASE STATS for an enemy. These can then be modified at object creation time to buff up enemies
/// and to reset their stats if they died or were modified at runtime.
/// </summary>
[CreateAssetMenu(fileName = "Enemy Configuration", menuName = "ScriptableObject/Enemy Configuration")]
public class EnemyScriptableObject : ScriptableObject
{
    public Enemy EnemyPrefab;
    public AttackScriptableObject AttackConfiguration;

    //Enemy Stats
    public int Health = 100;

    //NavmeshAgent Config
    public float AIUpdateInterval = 0.1f;

    public float Acceleration = 8;
    public float Angularspeed = 120;
    // -1 means everything
    public int Areamask = -1;
    public int AvoidancePriority = 50;
    public float BaseOffset = 0;
    public float Height = 2f;
    public ObstacleAvoidanceType ObstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    public float Radius = 0.5f;
    public float Speed = 3f;
    public float StoppingDistance = 0.5f;

    public void SetupEnemy(Enemy enemy)
    {
        enemy.Agent.acceleration = Acceleration;
        enemy.Agent.angularSpeed = Angularspeed;
        enemy.Agent.areaMask = Areamask;
        enemy.Agent.avoidancePriority = AvoidancePriority;
        enemy.Agent.baseOffset = BaseOffset;
        enemy.Agent.height = Height;
        enemy.Agent.obstacleAvoidanceType = ObstacleAvoidanceType;
        enemy.Agent.radius = Radius;
        enemy.Agent.speed = Speed;
        enemy.Agent.stoppingDistance = StoppingDistance;

        enemy.Movement.UpdateRate = AIUpdateInterval;

        enemy.Health = Health;

        AttackConfiguration.SetupEnemy(enemy);
    }

}
