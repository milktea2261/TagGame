using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarLayout : MonoBehaviour
{
    [SerializeField] Transform[] avatarPoints = new Transform[0];//角色模型位置
    [SerializeField] Camera avatarCam = null;//看向角色模型的攝影機
    [SerializeField] RadarChart avatarChart = null;//角色的素質圖

    [SerializeField] Text abliltyText = null;
    [SerializeField] Button selectBtn = null;
    [SerializeField] Text selectText = null;

    public bool showAttritubeName = true;
    [SerializeField] int index = 0;//角色索引值
    Vector3 offsetCamtoAvatar = new Vector3(0, 1.5f, -2);

    
    [SerializeField] Text avatarName = null;
    [SerializeField] Text avatarDescription = null;
    [SerializeField] Text avatarAttritube = null;
    

    private void Start() {
        Debug.Log("Create");
        ChartAttritube[] attritubes = AvatarAttritubes(GameManager.Instance.datas[0]);
        avatarChart.CreateChart(attritubes, showAttritubeName);

        ShowSelect();
        ShowAvatar();
    }

    public void NextAvatar() {
        index = (index + 1) % GameManager.Instance.datas.Length;
        ShowSelect();
        ShowAvatar();
    }

    public void PreviousAvatar() {
        index = (index + GameManager.Instance.datas.Length - 1) % GameManager.Instance.datas.Length;
        ShowSelect();
        ShowAvatar();
    }

    public void SelectAvatar() {
        GameManager.Instance.playerAvatarIndex = index;
        ShowSelect();
    }

    void ShowSelect() {
        if(GameManager.Instance.playerAvatarIndex == index) {
            selectBtn.interactable = false;
            selectText.text = "已選擇";
        }
        else {
            selectBtn.interactable = true;
            selectText.text = "未選擇";
        }
    }

    public void ShowAvatar() {
        AvatarData data = GameManager.Instance.datas[index];

        avatarCam.transform.position = avatarPoints[index].position + offsetCamtoAvatar;
        avatarName.text = data.name;
        avatarDescription.text = data.description;
        abliltyText.text = data.ability;
        avatarAttritube.text = "走速: " + data.walkSpeed + "\n";
        avatarAttritube.text += "跑速: " + data.runSpeed + "\n";
        avatarAttritube.text += "體力: " + data.maxStamina + "\n";
        //avatarAttritube.text += "能力: " + data.ability;

        ChartAttritube[] attritubes = AvatarAttritubes(data);
        avatarChart.UpdateChart(attritubes);
    }

    ChartAttritube[] AvatarAttritubes(AvatarData data) {
        ChartAttritube[] attritubes = new ChartAttritube[3];
        attritubes[0] = new ChartAttritube("walk", data.walkSpeed, new Vector2(0, 3));
        attritubes[1] = new ChartAttritube("run", data.runSpeed, new Vector2(0, 5));
        attritubes[2] = new ChartAttritube("SP", data.maxStamina, new Vector2(0, 500));
        return attritubes;
    }

}
