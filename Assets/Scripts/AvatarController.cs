using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//共通類，遵從遊戲規則，提供控制的方法
public enum AvatarTag { Runner, Tagger }
public class AvatarController : MonoBehaviour
{
    [SerializeField] Rigidbody rig = null;
    [SerializeField] MeshRenderer rend = null;
    [SerializeField] AvatarData data = null;

    public LayerMask playerMask;
    public bool IsFreeze { get; private set; }

    private bool isRunning = false;
    public float walkSpeed = 1f;
    public float RunSpeed = 2f;

    private float rotSpeed = 180;

    public float Stamina { get; private set; }
    public float maxStamina = 100;
    private float staminaRecovery = 5f;//耐力恢復量
    private float staminaConsumption = 10f;//耐力消耗量

    public float ablityCoolTime = 30f;//能力冷卻時間
    private float ablityTimer = 0f;

    public TextMeshPro headText = null;

    private void Start() {
        Stamina = maxStamina;
    }

    private void Update() {
        if(isRunning) {
            Stamina = Mathf.Clamp(Stamina - staminaConsumption * Time.deltaTime, 0, maxStamina);
        }
        else {
            Stamina = Mathf.Clamp(Stamina + staminaRecovery * Time.deltaTime, 0, maxStamina);
        }
        ablityTimer -= Time.deltaTime;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f, playerMask); 
        if(colliders.Length > 1) {
            foreach(Collider c in colliders) {
                if(c.gameObject != gameObject) {
                    if(gameObject.CompareTag(AvatarTag.Tagger.ToString())) {
                        c.SendMessage("Freeze");
                    }
                    else {
                        c.SendMessage("UnFreeze");
                    }
                }
            }
        }
    }

    public float GetMoveSpeed(bool isRun) {
        bool canRun = false;
        canRun = canRun || (isRun && isRunning && Stamina > 0);//玩家正處於奔跑狀態
        canRun = canRun || (isRun && !isRunning && Stamina >= 20f);//玩家處於走路狀態，限制玩家恢復一定體力後才能跑步

        isRunning = isRun && canRun;
        return isRunning ? RunSpeed : walkSpeed;
    }

    public void Move(Vector3 direction, bool isRun) {
        if(IsFreeze) {
            return;
        }
        rig.MovePosition(rig.position + direction * GetMoveSpeed(isRun) * Time.deltaTime);
    }

    //玩家旋轉視角
    public void Rotate(float offset) {
        Quaternion rot = Quaternion.Euler(new Vector3(0, offset, 0) * rotSpeed * Time.deltaTime);
        rig.MoveRotation(rig.rotation * rot);
    }

    //電腦朝向行進方向
    public void LookForward(Vector3 direction) {
        direction.y = 0;//保持平視
        Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, target, rotSpeed * Time.deltaTime);
        
    }

    [ContextMenu("Freeze")]
    public void Freeze() {
        if(gameObject.CompareTag(AvatarTag.Runner.ToString())) {
            IsFreeze = true;
            headText.text = "Freeze";
            headText.color = Color.white;
        }
    }

    [ContextMenu("UnFreeze")]
    public void UnFreeze() {
        if(gameObject.CompareTag(AvatarTag.Runner.ToString())) {
            IsFreeze = false;
            headText.text = "Runner";
            headText.color = Color.green;
        }
    }

    public void SetAvatar(AvatarTag avatar) {
        if(data != null) {
            //Setting by data
        }

        switch(avatar) {
            case AvatarTag.Tagger:
                //Debug.Log("Tagger");
                headText.text = "Tagger";
                headText.color = Color.red;

                walkSpeed = .9f;
                RunSpeed = 2.3f;
                break;
            case AvatarTag.Runner:
                //Debug.Log("Runner");
                headText.text = "Runner";
                headText.color = Color.white;

                walkSpeed = 1f;
                RunSpeed = 2f;
                break;
            default:
                break;
        }

        tag = avatar.ToString();

    }

    public virtual void UseAblity() {
        Debug.Log("Use Ablity");
    }

}
