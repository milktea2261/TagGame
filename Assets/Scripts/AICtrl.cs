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

    float stateTime = 5f;
    float randRange = 5f;
    float sampleRange = 1f;


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
        if(m_target != null) {
            closetDst = Vector3.Distance(transform.position, m_target.transform.position);
            closetTarget = m_target;
            
        }

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

    #region AI_Behaviour
    Vector3 p;
    Vector3 randP;

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

            Vector3 wanderPoint = transform.position + transform.forward * randRange + Random.insideUnitSphere * randRange;//隨機的方向
            NavMesh.SamplePosition(wanderPoint, out NavMeshHit hit, sampleRange, NavMesh.AllAreas);
            agent.SetDestination(hit.position);

            p = transform.position + transform.forward * randRange;
            randP = wanderPoint;
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
            Vector3 fleePoint = transform.position + fleeDir * randRange + Random.insideUnitSphere * randRange;//隨機的方向
            NavMesh.SamplePosition(fleePoint, out NavMeshHit hit, sampleRange, NavMesh.AllAreas);

            agent.SetDestination(hit.position);

            p = transform.position + fleeDir * randRange;
            randP = fleePoint;
        }

        if(isChangeState) {
            m_target = null;//轉身後看不到對像

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
            //Debug.Log(name + " Set Wonder Path");
            float randomRange = 5f;

            Vector3 randPoint = transform.position + transform.forward * randomRange / 2 + Random.insideUnitSphere * randomRange;//隨機的方向
            NavMesh.SamplePosition(randPoint, out NavMeshHit hit, 3f, NavMesh.AllAreas);

            agent.SetDestination(hit.position);
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
        Gizmos.color = Color.gray;
        Vector3[] path = agent.path.corners;
        if(path != null) {
            if(path.Length > 0) {
                for(int i = 0; i < path.Length - 1; i++) {
                    Gizmos.DrawLine(path[i], path[i + 1]);
                }
            }
        }

        //隨機找點的範圍
        if(state == AIState.Wonder) {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(p, randRange);
            Gizmos.DrawWireSphere(randP, sampleRange);
        }

        if(state == AIState.Flee) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(p, randRange);
            Gizmos.DrawWireSphere(randP, sampleRange);
        }
    }

}
