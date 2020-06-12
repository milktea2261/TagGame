using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//共通類，遵從遊戲規則，提供控制的方法
public enum AvatarTag { Runner, Tagger }
public class AvatarCtrl : MonoBehaviour
{
    [SerializeField] AvatarData data = null;
    [SerializeField] Animator animator = null;
    [SerializeField] TextMeshPro headText = null;

    public LayerMask avatarMask;
    public bool IsFreeze { get; private set; }
    protected bool isRunning = false;
    protected float rotSpeed = 180;

    public AvatarAttribute moveSpeed;
    public AvatarAttribute maxStamina;

    private float RunMutiple = 1.5f;//跑步速度的倍率
    protected virtual float CurrentSpeed => 0;

    public float Stamina { get; private set; }
    protected float staminaRecovery = 5f;//耐力恢復量
    protected float staminaConsumption = 10f;//耐力消耗量

    public float ablityCoolTime = 30f;//能力冷卻時間
    private float ablityTimer = 0f;

    protected virtual void Start() {
        Stamina = maxStamina.FinalValue;
    }

    protected virtual void Update() {
        ablityTimer -= Time.deltaTime;

        if(isRunning) {
            Stamina = Mathf.Clamp(Stamina - staminaConsumption * Time.deltaTime, 0, maxStamina.FinalValue);
        }
        else {
            Stamina = Mathf.Clamp(Stamina + staminaRecovery * Time.deltaTime, 0, maxStamina.FinalValue);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f, avatarMask); 
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

        animator.SetFloat("MoveSpeed", CurrentSpeed);
        animator.SetBool("IsRunning", isRunning);
    }

    public virtual void Init(Animator animator, AvatarData data) {
        this.data = data;
        this.animator = animator;
    }

    public float MoveSpeed(bool isRun) {
        bool canRun = false;
        canRun = canRun || (isRun && isRunning && Stamina > 0);//玩家正處於奔跑狀態
        canRun = canRun || (isRun && !isRunning && Stamina >= 20f);//玩家處於走路狀態，限制玩家恢復一定體力後才能跑步

        isRunning = isRun && canRun;
        return isRunning ? moveSpeed.FinalValue * RunMutiple : moveSpeed.FinalValue;
    }

    [ContextMenu("Freeze")]
    public void Freeze() {
        if(gameObject.CompareTag(AvatarTag.Runner.ToString())) {
            IsFreeze = true;
            headText.text = "Freeze";
            headText.color = Color.white;
            //avatarCtrl.enabled = false;
        }
    }

    [ContextMenu("UnFreeze")]
    public void UnFreeze() {
        if(gameObject.CompareTag(AvatarTag.Runner.ToString())) {
            IsFreeze = false;
            headText.text = "Runner";
            headText.color = Color.green;
            //avatarCtrl.enabled = true;
        }
    }

    public void SetAvatar(AvatarTag avatar) {
        if(data == null) {
            Debug.LogError("No Avatar Data");
        }

        moveSpeed.baseValue = data.moveSpeed;
        switch(avatar) {
            case AvatarTag.Tagger:
                headText.text = "Tagger";
                headText.color = Color.red;
                break;
            case AvatarTag.Runner:
                headText.text = "Runner";
                headText.color = Color.white;
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