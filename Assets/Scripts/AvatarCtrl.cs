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
    protected bool isMoving = false;
    protected bool isRunning = false;
    protected float rotSpeed = 180;

    public AvatarAttribute walkSpeed;
    public AvatarAttribute runSpeed;
    public AvatarAttribute maxStamina;

    [SerializeField] private float _stamina;
    bool isExhausted;//體力耗盡
    public float Stamina { get { return _stamina; } private set { _stamina = value; } }
    protected float staminaRecovery = 5f;//耐力恢復量
    protected float staminaConsumption = 10f;//耐力消耗量

    public float ablityCoolTime = 30f;//能力冷卻時間
    private float ablityTimer = 0f;

    protected virtual void Update() {
        ablityTimer -= Time.deltaTime;

        if(isMoving && isRunning) {
            Stamina = Mathf.Clamp(Stamina - staminaConsumption * Time.deltaTime, 0, maxStamina.FinalValue);
        }
        else {
            Stamina = Mathf.Clamp(Stamina + staminaRecovery * Time.deltaTime, 0, maxStamina.FinalValue);
        }
        if(Stamina <= 0) {
            isExhausted = true;
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

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsRunning", isRunning);
    }

    public virtual void Init(Animator animator, AvatarData data) {
        this.data = data;
        this.animator = animator;

        walkSpeed.baseValue = data.walkSpeed;
        runSpeed.baseValue = data.runSpeed;
        maxStamina.baseValue = data.maxStamina;

        Stamina = maxStamina.FinalValue;
    }

    public float MoveSpeed(bool useRun) {
        if(Stamina >= maxStamina.FinalValue / 4f) {
            isExhausted = false;
        }

        isRunning = useRun && !isExhausted;
        return isRunning ? runSpeed.FinalValue : walkSpeed.FinalValue;
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

        walkSpeed.baseValue = data.walkSpeed;
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