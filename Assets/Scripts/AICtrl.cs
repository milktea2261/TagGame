using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AISensor))]
//決策，執行
public class AICtrl : AvatarCtrl {
    public enum AIState { Idle, Wonder, Flee, Hunt }
    [Header("AI Part")]
    public AIState state = AIState.Idle;
    [SerializeField] AISensor sensor = null;//感測
    [SerializeField] NavMeshAgent agent = null;//輔助，獲得路線

    [SerializeField] bool isChangeState = false;
    [SerializeField] float stateTimer = 0;
    [SerializeField] AvatarCtrl m_target;//目標
    Vector3 targetLastPosition;

    protected override void Update() {
        base.Update();

        agent.nextPosition = transform.position;//刷新代理的位置
        
        //獲取資訊
        Collider[] colliders = sensor.DetectInRange();
        m_target = GetTarget(colliders);

        #region 轉換狀態

        if(state == AIState.Idle && !IsFreeze) {
            state = AIState.Wonder;
            isChangeState = true;
        }

        //Tagger方
        if(gameObject.CompareTag(AvatarTag.Tagger.ToString())) {
            if(state == AIState.Wonder && m_target != null) {
                state = AIState.Hunt;
                isChangeState = true;
            }
            if(state == AIState.Hunt && m_target != null) {
                if(m_target.IsFreeze) {
                    state = AIState.Wonder;
                    isChangeState = true;
                }
            }
        }
        //Runner方
        else {
            if(state == AIState.Wonder && m_target != null) {
                if(m_target.CompareTag(AvatarTag.Runner.ToString())) {
                    state = AIState.Hunt;
                    isChangeState = true;
                }
                else {
                    state = AIState.Flee;
                    isChangeState = true;
                }
            }
            if(state == AIState.Hunt && m_target != null) {
                if(m_target.CompareTag(AvatarTag.Tagger.ToString())) {
                    state = AIState.Flee;
                    isChangeState = true;
                }else if(!m_target.IsFreeze) {
                    state = AIState.Wonder;
                    isChangeState = true;
                }
            }
        }
        
        if(IsFreeze) {
            state = AIState.Idle;
            isChangeState = true;
        }
        else if((state == AIState.Hunt || state == AIState.Flee) && m_target == null && stateTimer <= 0) {
            state = AIState.Wonder;
            isChangeState = true;
        }

        #endregion
        if(m_target != null) {
            targetLastPosition = m_target.transform.position;
            stateTimer = stateTime;
        }
        stateTimer -= Time.deltaTime;

        //執行行為
        switch(state) {
            case AIState.Idle:
                Idle();
                break;
            case AIState.Wonder:
                Wonder();
                break;
            case AIState.Flee:
                Flee();
                break;
            case AIState.Hunt:
                Hunt();
                break;
            default:
                break;
        }
    }

    private AvatarCtrl GetTarget(Collider[] colliders) {
        AvatarCtrl closetTarget = null;
        float closetDst = float.MaxValue;
        /*
        if(m_target != null) {
            closetDst = Vector3.Distance(transform.position, m_target.transform.position);
            closetTarget = m_target;
        }
        */
        if(gameObject.CompareTag(AvatarTag.Runner.ToString())) {
            //Runner的選擇邏輯: 對方是Tagger 或是 對方是runner且被抓到，最靠近自己
            foreach(Collider c in colliders) {
                if(c.CompareTag(AvatarTag.Tagger.ToString())) {
                    closetTarget = c.GetComponent<AvatarCtrl>();
                    break;
                }
                else {
                    
                    AvatarCtrl controller = c.GetComponent<AvatarCtrl>();
                    if(controller.IsFreeze) {
                        float dst = Vector3.Distance(transform.position, controller.transform.position);
                        if(dst < closetDst) {
                            closetTarget = controller;
                            closetDst = dst;
                        }
                    }
                }
            }
        }
        else if(gameObject.CompareTag(AvatarTag.Tagger.ToString())) {
            //Tagger的選擇邏輯:對方是Runner且沒有被抓到，最靠近自己
            foreach(Collider c in colliders) {
                if(c.CompareTag(AvatarTag.Runner.ToString())){
                    AvatarCtrl controller = c.GetComponent<AvatarCtrl>();
                    if(!controller.IsFreeze) {
                        float dst = Vector3.Distance(transform.position, controller.transform.position);
                        if(dst < closetDst) {
                            closetTarget = controller;
                            closetDst = dst;
                        }
                    }
                }
            }
        }

        return closetTarget;
    }

    /// <summary>
    /// 透過在某方向採樣取點的方式，獲得與現在位置一定距離的新點。
    /// 採樣的方向順序，前方，右前X度，左前X度，右前2X度，左前2X度，以此類推。
    /// </summary>
    /// <param name="currentPos">現在的位置</param>
    /// <param name="currentDir">現在的方向</param>
    /// <param name="minDistance">最短距離</param>
    /// <returns></returns>
    /// 

    List<Vector3> dirP = new List<Vector3>();//大圓中心
    List<Vector3> randP = new List<Vector3>();//小圓中心
    float randRange = 5f;
    float sampleRange = 1.5f;//採樣半徑
    Vector3 GetRandomPoint(Vector3 currentPos, Vector3 currentDir, float minDistance) {
        Vector3 randomPoint = Vector3.zero;

        dirP = new List<Vector3>();
        randP = new List<Vector3>();
        float value = 45f;
        float sign = 1;
        int counter = 0;

        while(randomPoint == Vector3.zero) {
            float angle = value * Mathf.CeilToInt(counter / 2f) * sign;
            Vector3 newDir = Quaternion.AngleAxis(angle, Vector3.up) * currentDir;
            Vector3 dirPoint = currentPos + newDir * minDistance;
            Vector3 randPoint = dirPoint + Random.insideUnitSphere * randRange / 2;//隨機的方向
            bool foundPoint = NavMesh.SamplePosition(randPoint, out NavMeshHit hit, sampleRange, NavMesh.AllAreas);

            dirP.Add(dirPoint);
            randP.Add(randPoint);

            counter++;
            sign = -sign;

            if(foundPoint && Vector3.Distance(currentPos, hit.position) > minDistance) {
                randomPoint = hit.position;
            }
        }

        return randomPoint;
    }

    #region AI_Behaviour
    float stateTime = 5f;

    private void Idle() {
        agent.ResetPath();

        if(isChangeState) {
            m_target = null;

            agent.isStopped = true;
            isMoving = false;
            agent.speed = 0;

            isChangeState = false;
        }
    }

    /// <summary>
    /// 徘徊，在NavMesh上獲取一點，排除點在移動範圍外的可能性
    /// </summary>
    /// 
    private void Wonder() {
        //Debug.Log(name + " Wonder");
        if(isChangeState || agent.remainingDistance < 1) {
            //Debug.Log(name + " Set Wonder Path");

            Vector3 nextPoint = GetRandomPoint(transform.position, transform.forward, randRange);
            agent.SetDestination(nextPoint);
        }

        if(isChangeState) {
            m_target = null;

            agent.isStopped = false;
            isMoving = true;
            agent.speed = MoveSpeed(false);

            stateTimer = 0;
            isChangeState = false;
        }
    }

    /// <summary>
    /// 逃離，如果方向朝向目標更換路線
    /// </summary>
    /// 
    private void Flee() {
        //Debug.Log(name + " Flee " + m_target.name);
        if(isChangeState || agent.remainingDistance < 1) {
            //Debug.Log(name + " Set Flee Path");
            Vector3 fleeDir = (transform.position - targetLastPosition).normalized;
            Vector3 nextPoint = GetRandomPoint(transform.position, fleeDir, randRange);
            agent.SetDestination(nextPoint);
        }

        if(isChangeState) {
            agent.isStopped = false;
            isMoving = true;
            agent.speed = MoveSpeed(true);

            stateTimer = stateTime;
            isChangeState = false;
        }
    }

    /// <summary>
    /// 追捕
    /// </summary>
    private void Hunt() {

        if(m_target == null && agent.remainingDistance < 1) {
            stateTimer = 0;
        }
        else {
            agent.SetDestination(targetLastPosition);
        }

        if(isChangeState) {
            agent.isStopped = false;
            isMoving = true;
            agent.speed = MoveSpeed(true);

            stateTimer = stateTime;
            isChangeState = false;
        }
    }

    #endregion

    private void OnDrawGizmos() {
        if(agent == null) { return; }

        //路線
        Gizmos.color = gameObject.CompareTag(AvatarTag.Tagger.ToString()) ? Color.red : Color.green ;
        Vector3[] path = agent.path.corners;
        if(path != null) {
            if(path.Length > 0) {
                for(int i = 0; i < path.Length - 1; i++) {
                    Gizmos.DrawLine(path[i], path[i + 1]);
                }
            }
        }

        /*採樣時的點
        if(gameObject.CompareTag(AvatarTag.Runner.ToString())) {
            //隨機找點的範圍
            Gizmos.color = Color.white;

            foreach(Vector3 p in dirP) {
                Gizmos.DrawWireSphere(p, randRange/2);
            }
            foreach(Vector3 p in randP) {
                Gizmos.DrawWireSphere(p, sampleRange);
            }
        }
        */

    }

}
