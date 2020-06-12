using System;
using UnityEngine;

/* 屬性系統
 * 教學:https://forum.unity.com/threads/tutorial-character-stats-aka-attributes-system.504095/
 *
 * 不同系統對屬性的疊加(EX:成就獎勵、裝備、技能、消耗品)
 * 改善:將buff內容轉變成AttributeModier
 *
 * Buff類型
 * 種類
 * 1.時間性，當時間結束buff被移除。
 * 2.週期性，每隔一段時間發動一次
 * 3.持續到滿足條件為止。
 * 
 * 衝突的對應（如果已經有這種類型）
 * 1.失敗
 * 2.重新設定壽命
 * 3.新的覆蓋舊的
 * 4.高等的覆蓋低等的
 * 5.疊加
 */

public enum BuffType { Time, Passive }

[Serializable]
public struct Buff
{
    public BuffType type;
    public object source;
    [Tooltip("Only Effect Time Type")]
    public float duration;
    [Space()]
    public float moveSpeedFlat;
    public float staminaFlat;

    /// <summary>
    /// construct
    /// </summary>
    /// <param name="type">buff type</param>
    /// <param name="source">source</param>
    /// <param name="duration">if Passive type, not need this value</param>
    public Buff(BuffType type, object source, float duration) {
        this.type = type;
        this.source = source;
        this.duration = duration;

        moveSpeedFlat = 0;
        staminaFlat = 0;
    }

    public void SetFlat(float moveSpeedFlat, float staminaFlat) {
        this.moveSpeedFlat = moveSpeedFlat;
        this.staminaFlat = staminaFlat;
    }
}
