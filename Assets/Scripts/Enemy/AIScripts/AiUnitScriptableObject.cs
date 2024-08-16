using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "AiUnit",menuName = "AiUnit")]
public class AiUnitScriptableObject : ScriptableObject
{
    [Header("To Spawn enemies")]
    //Only if want to spawn
    public AiAgent Prefab;
    public AiUnitScalingScriptableObject Scaling;

    [Header("Should be filled for all")]
    public int MaxHealth = 100;
    public float SurroundDistance = 10f;
    public int Points = 0;

    // Movement Stats
    public AiStateID DefaultState;
    public float LineOfSightRange = 6f;
    public float FieldOfView = 90f;

    // NavMeshAgent Configs
    public float AIUpdateInterval = 0.1f;
    public float Speed = 5f;
    public float Acceleration = 8;
    public float AngularSpeed = 120;
    public float StoppingDistance = 0.5f;

    //Hostile
    public string HOSTILE_LAYER_NAME = "Character";

    public AiUnitScriptableObject ScaleUpForLevel(AiUnitScalingScriptableObject Scaling, int Level)
    {
        AiUnitScriptableObject scaledUpEnemy = CreateInstance<AiUnitScriptableObject>();

        scaledUpEnemy.name = name;
        scaledUpEnemy.Prefab = Prefab;
        scaledUpEnemy.Points = Points;

        //scaledUpEnemy.Daamage = AttackConfiguration.ScaleUpForLevel(Scaling, Level);

        scaledUpEnemy.MaxHealth = Mathf.FloorToInt(MaxHealth * Scaling.HealthCurve.Evaluate(Level));

        scaledUpEnemy.DefaultState = DefaultState;
        scaledUpEnemy.LineOfSightRange = LineOfSightRange;
        scaledUpEnemy.FieldOfView = FieldOfView;

        scaledUpEnemy.AIUpdateInterval = AIUpdateInterval;
        scaledUpEnemy.Acceleration = Acceleration;
        scaledUpEnemy.AngularSpeed = AngularSpeed;
        scaledUpEnemy.HOSTILE_LAYER_NAME = HOSTILE_LAYER_NAME;

        scaledUpEnemy.Speed = Speed * Scaling.SpeedCurve.Evaluate(Level);
        scaledUpEnemy.StoppingDistance = StoppingDistance;

        return scaledUpEnemy;
    }

    public void SetupAiUnit(AiAgent enemy)
    {
        enemy.Points = Points;
        enemy.navMeshAgent.acceleration = Acceleration;
        enemy.navMeshAgent.angularSpeed = AngularSpeed;
        enemy.navMeshAgent.speed = Speed;
        enemy.navMeshAgent.stoppingDistance = StoppingDistance;

        enemy.AIStateUpdateInterval = AIUpdateInterval;
        enemy.initialState = DefaultState;
        enemy.sensor.distance = LineOfSightRange;
        enemy.sensor.angle = FieldOfView;

        enemy.health.MaxHealth = MaxHealth;
        enemy.health.SetCurrentToMax();
    }
}
