using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//共通類，遵從遊戲規則，提供控制的方法
public enum AvatarTag { Runner, Tagger }
public class AvatarCtrl : MonoBehaviour
{
    [SerializeField] AvatarData data = null;
    public Animator animator = null;
    [SerializeField] TextMeshPro headText = null;

    public LayerMask avatarMask;
    public bool IsFreeze { get; private set; }
    protected bool isRunning = false;
    protected float rotSpeed = 180;

    public float walkSpeed = 1f;
    public float RunSpeed = 2f;
    protected virtual float currentSpeed => 0;

    public float Stamina { get; private set; }
    public float maxStamina = 100;
    protected float staminaRecovery = 5f;//耐力恢復量
    protected float staminaConsumption = 10f;//耐力消耗量

    public float ablityCoolTime = 30f;//能力冷卻時間
    private float ablityTimer = 0f;

    protected virtual void Start() {
        Stamina = maxStamina;
    }

    protected virtual void Update() {
        ablityTimer -= Time.deltaTime;

        if(isRunning) {
            Stamina = Mathf.Clamp(Stamina - staminaConsumption * Time.deltaTime, 0, maxStamina);
        }
        else {
            Stamina = Mathf.Clamp(Stamina + staminaRecovery * Time.deltaTime, 0, maxStamina);
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

        animator.SetFloat("MoveSpeed", currentSpeed);
        animator.SetBool("IsRunning", isRunning);
    }


    public float MoveSpeed(bool isRun) {
        bool canRun = false;
        canRun = canRun || (isRun && isRunning && Stamina > 0);//玩家正處於奔跑狀態
        canRun = canRun || (isRun && !isRunning && Stamina >= 20f);//玩家處於走路狀態，限制玩家恢復一定體力後才能跑步

        isRunning = isRun && canRun;
        return isRunning ? RunSpeed : walkSpeed;
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

        walkSpeed = data.walkSpeed;
        RunSpeed = data.runSpeed;
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
