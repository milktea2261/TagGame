using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public System.Action OnBuffChanged;

    private AvatarCtrl target;
    [SerializeField] private List<Buff> buffs = new List<Buff>();
    private float timer = 0;

    private void Awake() {
        target = GetComponent<AvatarCtrl>();
    }

    private void Update() {
        if(buffs.Count == 0) {
            return;
        }

        for(int i = buffs.Count - 1; i >= 0; i--) {
            if(timer <= 0) {
                //TODO:DOT傷害等等
            }

            if(buffs[i].type == BuffType.Time) {
                Buff newBuff = buffs[i];
                newBuff.duration -= Time.deltaTime;
                buffs[i] = newBuff;
                //移除Buff
                if(buffs[i].duration <= 0) {
                    RemoveBuff(i);
                }
            }
        }


        if(timer <= 0) {
            timer = 1;
        }
        timer -= Time.deltaTime;
    }

    public void AddBuff(Buff buff) {
        foreach(Buff b in buffs) {
            if(b.source == buff.source) {
                Debug.Log("Find Same Buff");
                return;
            }
        }

        Debug.Log("AddBuff from " + buff.source);
        buffs.Add(buff);
        //add mods
        if(buff.moveSpeedFlat != 0) {
            target.walkSpeed.AddModifier(new AttributeModifier(buff.moveSpeedFlat, AttributeModifierType.Flat, buff.source));
        }
        if(buff.staminaFlat != 0) {
            target.maxStamina.AddModifier(new AttributeModifier(buff.staminaFlat, AttributeModifierType.Flat, buff.source));
        }

        OnBuffChanged.Invoke();
    }
    public void RemoveBuff(int index) {
        Debug.Log("RemoveBUff " + index);

        //rmove mods
        if(buffs[index].moveSpeedFlat != 0) {
            target.walkSpeed.RemoveModifier(new AttributeModifier(buffs[index].moveSpeedFlat, AttributeModifierType.Flat, buffs[index].source));
        }
        if(buffs[index].staminaFlat != 0) {
            target.maxStamina.RemoveModifier(new AttributeModifier(buffs[index].staminaFlat, AttributeModifierType.Flat, buffs[index].source));
        }

        buffs.RemoveAt(index);
        OnBuffChanged.Invoke();
    }
    public void RemoveBuffFromSource(object source) {
        Debug.Log("RemoveBUff from " + source);
        buffs.RemoveAll(x => x.source == source);

        target.walkSpeed.RemoveAllModifiersFromSource(source);
        target.maxStamina.RemoveAllModifiersFromSource(source);
        OnBuffChanged.Invoke();
    }
    
}
