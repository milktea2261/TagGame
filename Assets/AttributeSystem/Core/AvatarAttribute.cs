using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class AvatarAttribute{

    [SerializeField]private List<AttributeModifier> modifiers = new List<AttributeModifier>();

    public Vector2 valueRange = new Vector2(0f, 1000f);//限制數值的下限和上限
    public float baseValue;
    private bool isDirty = true;//當mod發生改變時重新計算
    private float _laseBaseValue;//紀錄basevalue，當basevalue改變時重新計算
    [SerializeField]private float _finalValue;//紀錄finalValue，避免重複計算
    
    public float FinalValue {
        get {
            if(isDirty || _laseBaseValue != baseValue) {
                _laseBaseValue = baseValue;
                CalculateFinalValue();
                isDirty = false;
            }
            return _finalValue;
        }  
    }
    private void CalculateFinalValue() {
        float flatValue = 0, percentValue = 0;

        foreach(AttributeModifier mod in modifiers) {
            switch(mod.type) {
                case AttributeModifierType.Flat:
                    flatValue += mod.value;
                    break;
                case AttributeModifierType.Percent:
                    percentValue += mod.value;
                    break;
            }
        }

        //最終的計算公式
        _finalValue = (baseValue + flatValue) * (1 + percentValue);
        _finalValue = Mathf.Clamp(_finalValue, valueRange.x, valueRange.y);
    }

    public AvatarAttribute() {
        modifiers = new List<AttributeModifier>();
        isDirty = true;
    }
    public AvatarAttribute(float value) : this() {
        baseValue = value;
    }

    public void AddModifier(AttributeModifier mod) {
        for(int i = modifiers.Count - 1; i >= 0; i--) {
            if(modifiers[i].source == mod.source) {
                //賦予失敗
                Debug.LogWarning("Has add this mod");
                return;
            }
        }
        isDirty = true;
        modifiers.Add(mod);
        CalculateFinalValue();
    }
    public bool RemoveModifier(AttributeModifier mod) {

        for(int i = modifiers.Count -1; i >= 0 ; i--) {
            if(modifiers[i].value == mod.value && modifiers[i].type == mod.type && modifiers[i].source == mod.source) {
                isDirty = true;
                CalculateFinalValue();
                modifiers.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
    public bool RemoveAllModifiersFromSource(object source) {
        bool isRemoved = false;
        for(int i = modifiers.Count - 1; i >= 0; i--) {
            if(modifiers[i].source == source) {
                isDirty = true;
                isRemoved = true;
                modifiers.RemoveAt(i);
            }
        }
        CalculateFinalValue();
        return isRemoved;
    }
}