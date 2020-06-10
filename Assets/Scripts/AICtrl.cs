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
    [SerializeField] AvatarCtrl m_target;//目標

    float fleeTime = 5f;
    float stateTimer = 0;
    protected override float currentSpeed => agent.velocity.magnitude;

    protected override void Update() {
        base.Update();

        agent.nextPosition = transform.position;//刷新代理的位置

        //獲取資訊
        Collider[] colliders = sensor.DetectInRange();

        AvatarCtrl newTarget = GetTarget(colliders);
        m_target = newTarget;

        #region 轉換狀態

        if(state == AIState.Idle && !IsFreeze) {
            state = AIState.Wonder;
            agent.isStopped = false;
        }

        //Tagger方
        if(gameObject.CompareTag(AvatarTag.Tagger.ToString())) {

            if(state == AIState.Wonder && m_target != null) {
                state = AIState.Hunt;
            }
            if(state == AIState.Hunt && m_target != null) {
                if(m_target.IsFreeze) {
                    state = AIState.Wonder;
                    m_target = null;
                }
            }

        }
        //Runner方
        else {

            if(state == AIState.Wonder && m_target != null) {
                if(m_target.CompareTag(AvatarTag.Runner.ToString())) {
                    state = AIState.Hunt;
                }
                else {
                    state = AIState.Flee;
                }
            }
            if(state == AIState.Hunt && m_target != null) {
                if(gameObject.CompareTag(AvatarTag.Runner.ToString())) {
                    //Debug.Log(name + " try to rescue " + m_target.name + ">" + m_target.IsFreeze);
                    if(!m_target.IsFreeze) {
                        state = AIState.Wonder;
                        m_target = null;
                    }
                }
            }
            if(state == AIState.Flee) {
                if(stateTimer <= 0) {
                    state = AIState.Wonder;
                }
            }
            //確保看到Tagger會逃跑
            if((state == AIState.Wonder || state == AIState.Hunt) && m_target != null) {
                if(m_target.CompareTag(AvatarTag.Tagger.ToString())) {
                    state = AIState.Flee;
                }
            }
        }

        if(m_target == null) {
            state = AIState.Wonder;
        }
        if(IsFreeze) {
            state = AIState.Idle;
        }

        #endregion
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

    private void Idle() {
        m_target = null;
        agent.isStopped = true;
        agent.ResetPath();
    }

    /// <summary>
    /// 徘徊，在NavMesh上獲取一點，排除點在移動範圍外的可能性
    /// </summary>
    private void Wonder() {
        //Debug.Log(name + " Wonder");
        if(agent.remainingDistance < 1) {
            //Debug.Log(name + " Set Wonder Path");
            float randomRange = 5f;

            Vector3 randPoint = transform.position + transform.forward * randomRange / 2 + Random.insideUnitSphere * randomRange;//隨機的方向
            NavMesh.SamplePosition(randPoint, out NavMeshHit hit, 3f, NavMesh.AllAreas);

            agent.speed = MoveSpeed(false);
            agent.SetDestination(hit.position);
        }
    }

    /// <summary>
    /// 逃離，如過方向朝向目標更換路線
    /// </summary>
    private void Flee() {
        //Debug.Log(name + " Flee " + m_target.name);
        if(stateTimer <= 0 || agent.remainingDistance < 1) {
            //Debug.Log(name + " Set Flee Path");
            float fleeRange = 5f;
            Vector3 targetToSelf = (transform.position - m_target.transform.position).normalized;

            Vector3 fleeDir = targetToSelf;
            Vector3 fleePoint = transform.position + fleeDir * fleeRange / 2 + Random.insideUnitSphere * fleeRange;//隨機的方向
            NavMesh.SamplePosition(fleePoint, out NavMeshHit hit, 3f, NavMesh.AllAreas);

            agent.speed = MoveSpeed(true);
            agent.SetDestination(hit.position);

            stateTimer = fleeTime;
        }
    }

    /// <summary>
    /// 追捕
    /// </summary>
    private void Hunt() {
        //Debug.Log(name + " Hunt " + m_target.name);
        agent.speed = MoveSpeed(true);
        agent.SetDestination(m_target.transform.position);
    }

    #endregion

    private void OnDrawGizmos() {
        if(agent == null) { return; }

        Gizmos.color = Color.gray;
        Vector3[] path = agent.path.corners;
        if(path != null) {
            if(path.Length > 0) {
                for(int i = 0; i < path.Length - 1; i++) {
                    Gizmos.DrawLine(path[i], path[i + 1]);
                }
            }
        }
    }

}
