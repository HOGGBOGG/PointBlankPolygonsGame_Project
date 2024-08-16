using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(0)]
public class EnemySpawner : MonoBehaviour
{
    public AIManager aiManager; // for surround ai
    public AiAgent[] HostileBases = new AiAgent[3];
    public AiAgent HostileBase;
    public int InitialPoolSize = 10;
    public int NumberOfEnemiesToSpawn = 5;
    public float SpawnDelay = 1f;
    public List<WeightedSpawnScriptableObject> WeightedEnemies = new List<WeightedSpawnScriptableObject>();
    public AiUnitScalingScriptableObject Scaling;
    public SpawnMethod EnemySpawnMethod = SpawnMethod.RoundRobin;
    public bool ContinuousSpawning;
    [Space]
    [Header("Read At Runtime")]
    [SerializeField]
    private int Level = 0;
    [SerializeField]
    private List<AiUnitScriptableObject> ScaledEnemies = new List<AiUnitScriptableObject>();

    [SerializeField]
    private float[] Weights;

    private int EnemiesAlive = 0;
    private int SpawnedEnemies = 0;
    private int InitialEnemiesToSpawn;
    private float InitialSpawnDelay;

    public float InitialWaveSpawnDelay = 20f;
    //Spawn anywhere in the map
    //private NavMeshTriangulation Triangulation;
    private Dictionary<int, ObjectPool> EnemyObjectPools = new Dictionary<int, ObjectPool>();

    //Spawn within the defined bounds, We will be using this...
    [SerializeField]
    private Collider SpawnCollider;
    private Bounds Bounds;
    public bool HostileSpawner = false;

    private void Awake()
    {
        SpawnCollider = GetComponent<Collider>();
        for (int i = 0; i < WeightedEnemies.Count; i++)
        {
            EnemyObjectPools.Add(i, ObjectPool.CreateInstance(WeightedEnemies[i].Enemy.Prefab, InitialPoolSize));
        }
        Weights = new float[WeightedEnemies.Count];
        InitialEnemiesToSpawn = NumberOfEnemiesToSpawn;
        InitialSpawnDelay = SpawnDelay;
        if(SpawnCollider == null)
        {
            //Debug.LogError("SpawnCollider not found...");
        }
        Bounds = SpawnCollider.bounds;
    }

    //private void Start()
    //{
    //    for (int i = 0; i < WeightedEnemies.Count; i++)
    //    {
    //        ScaledEnemies.Add(WeightedEnemies[i].Enemy.ScaleUpForLevel(Scaling, 0));
    //    }
    //    //StartCoroutine(SpawnEnemies());
    //    StartCoroutine(WaveSpawnEnemies());
    //}

    private void OnEnable()
    {
        for (int i = 0; i < WeightedEnemies.Count; i++)
        {
            ScaledEnemies.Add(WeightedEnemies[i].Enemy.ScaleUpForLevel(Scaling, 0));
        }
        //StartCoroutine(SpawnEnemies());
        StartCoroutine(WaveSpawnEnemies());
    }

    private Coroutine SpawnEnemiesCoroutine;
    private IEnumerator WaveSpawnEnemies()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            if (SpawnEnemiesCoroutine == null)
            {
                //Debug.LogWarning("SPAWN ENEMIES COROUTINE CALLED.");
                SpawnEnemiesCoroutine = StartCoroutine(SpawnEnemies());
            }
            yield return new WaitForSeconds(InitialWaveSpawnDelay);// * Scaling.DamageCurve.Evaluate(Level));  // USE DAMAGE CONFIG TO EVALUATE SPAWN DELAY..
            ScaleUpSpawns();
        }
    }


    private Vector3 GetRandomPositionInBounds()
    {
        return new Vector3(Random.Range(Bounds.min.x, Bounds.max.x), Bounds.min.y, Random.Range(Bounds.min.z, Bounds.max.z));
    }
    private IEnumerator SpawnEnemies()
    {
        Level++;
        SpawnedEnemies = 0;
        EnemiesAlive = 0;
        for (int i = 0; i < WeightedEnemies.Count; i++)
        {
            ScaledEnemies[i] = WeightedEnemies[i].Enemy.ScaleUpForLevel(Scaling, Level);
        }

        ResetSpawnWeights();
        
        WaitForSeconds Wait = new WaitForSeconds(SpawnDelay);
        yield return Wait;

        while (SpawnedEnemies < NumberOfEnemiesToSpawn)
        {
            if (HostileSpawner)
            {
                while (GameManager.Instance.CurrentNumberEnemiesHostileTeam >= GameManager.MaxEnemiesPerTeam)
                {
                    yield return new WaitForSeconds(5f);
                }
            }
            else
            {
                while (GameManager.Instance.CurrentNumberEnemiesAllyTeam >= GameManager.MaxEnemiesPerTeam)
                {
                    yield return new WaitForSeconds(5f);
                }
            }
            if (HostileSpawner)
            {
                GameManager.Instance.CurrentNumberEnemiesHostileTeam++;
            }
            else
            {
                GameManager.Instance.CurrentNumberEnemiesAllyTeam++;
            }
            if (EnemySpawnMethod == SpawnMethod.RoundRobin)
            {
                SpawnRoundRobinEnemy(SpawnedEnemies);
            }
            else if (EnemySpawnMethod == SpawnMethod.Random)
            {
                SpawnRandomEnemy();
            }
            else if(EnemySpawnMethod == SpawnMethod.WeightedRandom)
            {
                SpawnWeightedRandomEnemy();
            }
            
            SpawnedEnemies++;
            yield return Wait;
        }

        //if (ContinuousSpawning)
        //{
        //    ScaleUpSpawns();
        //    StartCoroutine(SpawnEnemies());
        //}
        SpawnEnemiesCoroutine = null;
    }
    private void ResetSpawnWeights()
    {
        float TotalWeight = 0;

        for (int i = 0; i < WeightedEnemies.Count; i++)
        {
            Weights[i] = WeightedEnemies[i].GetWeight() * WeightedEnemies[i].Enemy.Scaling.SpawnRateCurve.Evaluate(Level);
            TotalWeight += Weights[i];
        }
        for (int i = 0; i < Weights.Length; i++)
        {
            Weights[i] = Weights[i] / TotalWeight;
        }
    }
    private void SpawnWeightedRandomEnemy()
    {
        float Value = Random.value;

        for (int i = 0; i < Weights.Length; i++)
        {
            if (Value < Weights[i])
            {
                DoSpawnEnemy(i, GetRandomPositionInBounds());
                return;
            }

            Value -= Weights[i];
        }

        Debug.LogError("Invalid configuration! Could not spawn a Weighted Random Enemy. Did you forget to call ResetSpawnWeights()?");
    }
    public void DoSpawnEnemy(int SpawnIndex, Vector3 SpawnPosition)
    {
        PoolableObject poolableObject = EnemyObjectPools[SpawnIndex].GetObject();

        if (poolableObject != null)
        {
            AiAgent enemy = poolableObject.GetComponent<AiAgent>();
            ScaledEnemies[SpawnIndex].SetupAiUnit(enemy);


            NavMeshHit Hit;
            if (NavMesh.SamplePosition(SpawnPosition, out Hit, 2f, -1))
            {
                enemy.navMeshAgent.Warp(Hit.position);
                // enemy needs to get enabled and start chasing now.
                //enemy.EnemyBase = HostileBase;
                for (int i = 0; i < 3; i++)
                {
                    if (HostileBases[i].gameObject.activeInHierarchy)
                    {
                        enemy.EnemyBases[i] = HostileBases[i];
                    }
                }
                enemy.navMeshAgent.enabled = true;
                enemy.health.DecreaseAliveEnemies = HandleEnemyCounter;
                //enemy.health.OnDeath -= HandleEnemyDeath;
                //enemy.health.OnDeath += HandleEnemyDeath;
                if(aiManager != null) // VIMP CHANGEEE
                aiManager.SeekOutAgent(enemy); // for surround ai
                EnemiesAlive++;
            }
            else
            {
                Debug.LogError($"Unable to place NavMeshAgent on NavMesh. Tried to use {SpawnPosition}");
            }
        }
        else
        {
            Debug.LogError($"Unable to fetch enemy of type {SpawnIndex} from object pool. Out of objects?");
        }
    }
    private void ScaleUpSpawns()
    {
        NumberOfEnemiesToSpawn = Mathf.FloorToInt(InitialEnemiesToSpawn * Scaling.SpawnCountCurve.Evaluate(Level + 1));
        SpawnDelay = InitialSpawnDelay * Scaling.SpawnRateCurve.Evaluate(Level + 1);
    }

    private void HandleEnemyCounter()
    {
        if (HostileSpawner)
        {
            GameManager.Instance.CurrentNumberEnemiesHostileTeam--;
        }
        else
        {
            GameManager.Instance.CurrentNumberEnemiesAllyTeam--;
        }
    }
    private void HandleEnemyDeath(Vector3 pos)
    {
        EnemiesAlive--;
        if (HostileSpawner)
        {
            GameManager.Instance.CurrentNumberEnemiesHostileTeam--;
        }
        else
        {
            GameManager.Instance.CurrentNumberEnemiesAllyTeam--;
        }
        if (EnemiesAlive == 0 && SpawnedEnemies == NumberOfEnemiesToSpawn)
        {
            //ScaleUpSpawns();
            //StartCoroutine(SpawnEnemies());   // FOR WAVE SPAWNING ENEMIES !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }
    }

    private void SpawnRoundRobinEnemy(int SpawnedEnemies)
    {
        int SpawnIndex = SpawnedEnemies % WeightedEnemies.Count;

        DoSpawnEnemy(SpawnIndex, GetRandomPositionInBounds());
    }
    private void SpawnRandomEnemy()
    {
        DoSpawnEnemy(Random.Range(0, WeightedEnemies.Count), GetRandomPositionInBounds());
    }
    public enum SpawnMethod
    {
        RoundRobin,
        Random,
        WeightedRandom
        // Other spawn methods can be added here
    }
}