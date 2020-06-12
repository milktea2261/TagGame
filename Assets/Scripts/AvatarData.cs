using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mine/Avatar")]
public class AvatarData : ScriptableObject
{
    [Multiline(3)] public string description = "";

    public float moveSpeed = 1f;

    public float maxStamina = 100;
    //public float staminaRecovery = 5f;
    //public float staminaConsumption = 20f;

    public string ability = "None";

    public GameObject avatarModel;
}
