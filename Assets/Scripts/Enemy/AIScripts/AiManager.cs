using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

[DefaultExecutionOrder(0)]
public class AIManager : MonoBehaviour
{
    //public float RadiusAroundTarget = 3f;
    public Transform[] Targets = new Transform[20];
    public AiAgent[] Units = new AiAgent[20];


    public AIManager HostileAiManager;
    public List<EnemySpawner> HostileEnemySpawner = new List<EnemySpawner>();
    public List<EnemySpawner> AlliedEnemySpawner = new List<EnemySpawner>();

    private Dictionary<Transform, bool> MapTargets = new Dictionary<Transform, bool>();

    public void Start()
    {
        Targets = new Transform[20];
        Units = new AiAgent[20];
        StartCoroutine(RefreshTargets());
        StartCoroutine(CircleTargets());
    }

    private IEnumerator RefreshTargets()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(5f);
            SeekOutTarget();
            yield return new WaitForSeconds(Random.Range(3f, 8f));
            MakeAgentsCircleTarget();
        }
    }
    private IEnumerator CircleTargets()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            MakeAgentsCircleTarget();
        }
    }

    public void SeekOutTarget()
    {
        //Debug.Log("SeekOutTargets called.");
        for (int i = 0; i < 20; i++)
        {
            if (HostileAiManager.Units[i] != null && HostileAiManager.Units[i].transform == Targets[i]) // target already exists
            {
                break;
            }
        }

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                if (HostileAiManager.Units[j] != null) // will only work when unit is not null
                {
                    Transform target = HostileAiManager.Units[j].transform.GetComponentInChildren<Health>().transform;
                    if (Targets[i] == null)
                    {
                        if (!MapTargets.ContainsKey(target))
                        {
                            Targets[i] = target;
                            MapTargets[target] = true;
                            break;
                        }
                        else
                        {
                            if (MapTargets[target] == false)
                            {
                                Targets[i] = target;
                                MapTargets[target] = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (!Targets[i].gameObject.activeSelf)
                        {
                            MapTargets[Targets[i]] = false;
                            if (!MapTargets.ContainsKey(target))
                            {
                                Targets[i] = target;
                                MapTargets[target] = true;
                                break;
                            }
                            else
                            {
                                if (MapTargets[target] == false)
                                {
                                    Targets[i] = target;
                                    MapTargets[target] = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        //Debug.Log("SeekOutTargets end.");

    }

    public void SeekOutAgent(AiAgent agent)
    {
        //foreach (var a in Units) // agent already exists in the list
        //{
        //    if (a == agent)
        //        return;
        //}
        if (agent == null) return;
        for (int i = 0; i < 20; i++)
        {
            if (Units[i] == null)
            {
                Units[i] = agent;
                break;
            }
            else if(!Units[i].gameObject.activeSelf)
            {
                Units[i] = agent;
                break;
            }
        }
    }

    private void MakeAgentsCircleTarget()
    {
        foreach (Transform T in Targets)
        {
            if (T != null && T.gameObject.activeSelf)
            {
                int i = 0;
                foreach (var unit in Units)
                {
                    if (unit != null && unit.gameObject.activeSelf && unit.targeting.HasTarget && unit.targeting.Target == T.gameObject) // check how many agents are currently target that TARGET
                    {

                        Vector3 position = new Vector3(
                        T.position.x + unit.AiConfig.SurroundDistance * Mathf.Cos(2 * Mathf.PI * Random.Range(i, 20 - i) / 20),
                        T.position.y,
                        T.position.z + unit.AiConfig.SurroundDistance * Mathf.Sin(2 * Mathf.PI * Random.Range(i, 20 - i) / 20)
                        );
                        if(NavMesh.SamplePosition(position,out NavMeshHit hit, 2f, -1)){
                            unit.navMeshAgent.SetDestination(hit.position);
                        }
                        i++;
                    }
                }
            }
        }
    }
}