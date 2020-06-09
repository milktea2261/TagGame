using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarLayout : MonoBehaviour
{
    [SerializeField] AvatarData[] avatarDatas = new AvatarData[0];//角色資料

    [SerializeField] Transform[] avatarPoints = new Transform[0];//角色模型位置
    [SerializeField] Camera avatarCam = null;//看向角色模型的攝影機
    [SerializeField] RadarChart avatarChart = null;//角色的素質圖

    public bool showAttritubeName = true;
    [SerializeField] int index = 0;//角色索引值
    Vector3 offsetCamtoAvatar = new Vector3(0, 1.5f, -2);

    
    [SerializeField] Text avatarName = null;
    [SerializeField] Text avatarDescription = null;
    [SerializeField] Text avatarAblity = null;

    private void Start() {
        Debug.Log("Create");
        ChartAttritube[] attritubes = AvatarAttritubes(avatarDatas[0]);
        avatarChart.CreateChart(attritubes, showAttritubeName);
    }

    public void NextAvatar() {
        index = (index + 1) % avatarDatas.Length;
        ShowAvatar();
    }

    public void PreviousAvatar() {
        index = (index + avatarDatas.Length - 1) % avatarDatas.Length;
        ShowAvatar();
    }


    public void ShowAvatar() {
        AvatarData data = avatarDatas[index];

        avatarCam.transform.position = avatarPoints[index].position + offsetCamtoAvatar;
        avatarName.text = data.name;
        avatarDescription.text = data.description;
        avatarAblity.text = data.ability;

        ChartAttritube[] attritubes = AvatarAttritubes(data);
        avatarChart.UpdateChart(attritubes);

    }

    ChartAttritube[] AvatarAttritubes(AvatarData data) {
        ChartAttritube[] attritubes = new ChartAttritube[3];
        attritubes[0] = new ChartAttritube("walkSPD", data.walkSpeed, new Vector2(0, 2));
        attritubes[1] = new ChartAttritube("runSPD", data.runSpeed, new Vector2(0, 3));
        attritubes[2] = new ChartAttritube("stamina", data.maxStamina, new Vector2(0, 200));
        return attritubes;
    }

}
