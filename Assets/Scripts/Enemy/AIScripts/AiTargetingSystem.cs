using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class AiTargetingSystem : MonoBehaviour
{
    public float memorySpan = 3f;
    public float distanceWeight = 1.0f;
    public float angleWeight = 1.0f;
    public float ageWeight = 1.0f;

    public GameObject BestMemory;

    public float timerInterval = 1f; // changes
    float timer = 1f;

    public bool HasTarget
    {
        get
        {
            return bestMemory != null;// || memory.memories.Count != 0;
        }
    }

    public GameObject Target
    {
        get
        {
            return bestMemory.gameObject;
        }
    }

    public Vector3 TargetPosition
    {
        get
        {
            return bestMemory.gameObject.transform.position;
        }
    }

    public bool TargetInSight
    {
        get
        {
            return bestMemory.angle < 0.5f;
        }
    }

    public float TargetDistance
    {
        get
        {
            return bestMemory.distance;
        }
    }

    AiSensoryMemory memory = new AiSensoryMemory(15); // CHANGES           EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
    AiSensor sensor;
    AiMemory bestMemory = null;

    private void Start()
    {
        sensor = GetComponent<AiSensor>();
    }

    private void Update()
    {
        //timer += Time.deltaTime;
        //if (timer > timerInterval)
        //{
        //    memory.UpdateSenses(sensor);
        //    memory.ForgetMemories(memorySpan);
        //    EvaluateScores();
        //    timer = 0f;
        //}
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer > timerInterval)
        {
            memory.UpdateSenses(sensor);
            memory.ForgetMemories(memorySpan);
            EvaluateScores();
            timer = 0f;
        }
    }

    float CalculateScore(AiMemory memory)
    {
        float DistanceScore = Normalize(memory.distance, sensor.distance) * distanceWeight;
        float angleScore = Normalize(memory.angle, sensor.angle) * angleWeight;
        float ageScore = Normalize(memory.Age, memorySpan) * ageWeight;
        return DistanceScore + angleScore + ageScore;
    }

    float Normalize(float value,float maxValue)
    {
        return 1.0f - (value/maxValue);
    }
    void EvaluateScores()
    {
        bestMemory = null;
        foreach(var memory in memory.memories)
        {
            memory.score = CalculateScore(memory);
            if(bestMemory == null || bestMemory.score < memory.score)
            {
                BestMemory = memory.gameObject;
                bestMemory = memory;
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    float maxScore = float.MinValue;
    //    foreach(var memory in memory.memories)
    //    {
    //        maxScore = Mathf.Max(maxScore, memory.score);
    //    }
    //    foreach(var memory in memory.memories)
    //    {
    //        Color color = Color.red;
    //        if(memory == bestMemory)
    //        {
    //            color = Color.yellow;
    //        }
    //        color.a = memory.score / maxScore;
    //        Gizmos.color = color;
    //        Gizmos.DrawSphere(memory.position+Vector3.up,1f);
    //    }
    //}
}
