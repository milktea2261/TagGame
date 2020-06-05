using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 感知周遭環境，向大腦提供訊息
/// </summary>
public class AISensor : MonoBehaviour
{
    public LayerMask viewMask;
    public float viewAngle = 30;//視野角度
    public float viewDst = 10f;//視野距離

    //public Transform lastTarget = null;
    //protected float lostTargetSecond = 3f;//數秒後沒看到目標則失去對象
    //protected float targetTimer = 0f;//失去目標的計數器


    private void Update() {
        Debug.DrawRay(transform.position, transform.forward, Color.white);//前進的方向
        Vector3 dir1 = new Vector3(Mathf.Sin(viewAngle / 2 * Mathf.Deg2Rad), 0, Mathf.Cos(viewAngle / 2 * Mathf.Deg2Rad)).normalized;
        dir1 = transform.TransformDirection(dir1);
        Vector3 dir2 = new Vector3(Mathf.Sin(-viewAngle / 2 * Mathf.Deg2Rad), 0, Mathf.Cos(-viewAngle / 2 * Mathf.Deg2Rad)).normalized;
        dir2 = transform.TransformDirection(dir2);
        Debug.DrawRay(transform.position, dir1 * viewDst, Color.white);
        Debug.DrawRay(transform.position, dir2 * viewDst, Color.white);
    }

    /// <summary>
    /// 先取得球型碰撞區內的碰撞體，計算兩向量(自身前方、面向對方)夾角來獲得視角內的碰撞體
    /// 透過射線(自己-目標)'目視'到目標
    /// </summary>
    /// <returns></returns>
    public Collider[] DetectInRange() {
        List<Collider> collidersInView = new List<Collider>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, viewDst, viewMask);
        foreach(Collider c in colliders) {
            Vector3 dirToCollider = (c.transform.position - transform.position).normalized;
            if(c.gameObject != gameObject) {//排除自己
                if(Vector3.Angle(dirToCollider, transform.forward) < viewAngle / 2) {
                    //射線檢測
                    if(Physics.Raycast(transform.position, dirToCollider, out RaycastHit hit, viewDst)) {
                        if(hit.collider.gameObject == c.gameObject) {
                            //Debug.Log(name + " See " + c.name);
                            collidersInView.Add(c);
                        }
                    }
                    
                }
            }
        }
        return collidersInView.ToArray();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDst);
    }

}
