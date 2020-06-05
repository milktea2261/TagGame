using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarUI : MonoBehaviour
{
    [SerializeField] AvatarData[] runnerDatas = new AvatarData[0];
    [SerializeField] AvatarData[] taggerDatas = new AvatarData[0];

    [SerializeField] Transform[] runnerPoints = new Transform[0];
    [SerializeField] Transform[] taggerPoints = new Transform[0];
    [Space()]
    [SerializeField] CinemachineVirtualCamera virtualCam = null;
    
    [SerializeField] Text nameText = null;
    [SerializeField] Text descriptionText = null;
    [SerializeField] Text walkSPDText = null;
    [SerializeField] Text runSPDText = null;
    [SerializeField] Text StaminaText = null;
    [SerializeField] Text abilityText = null;

    [SerializeField] bool isTagger = false;
    [SerializeField] int runnerIndex = 0;
    [SerializeField] int taggerIndex = 0;

    private void Start() {
        isTagger = false;
        runnerIndex = 0;
        taggerIndex = 0;

        ShowAvatar();
    }

    public void SetFaction(bool isTagger) {
        this.isTagger = isTagger;
        ShowAvatar();
    }

    public void NextAvatar() {
        if(isTagger) {
            taggerIndex = (taggerIndex + 1) % taggerDatas.Length;
        }
        else {
            runnerIndex = (runnerIndex + 1) % runnerDatas.Length;
        }
        ShowAvatar();
    }

    public void PreviousAvatar() {
        if(isTagger) {
            taggerIndex = (taggerIndex + taggerDatas.Length - 1) % taggerDatas.Length;
        }
        else {
            runnerIndex = (runnerIndex + runnerDatas.Length - 1) % runnerDatas.Length;
        }
        ShowAvatar();
    }

    void ShowAvatar() {
        int index = isTagger ? taggerIndex : runnerIndex;

        AvatarData data = isTagger ? taggerDatas[index] : runnerDatas[index];
        Transform point = isTagger ? taggerPoints[index] : runnerPoints[index];

        virtualCam.m_Follow = point;
        //virtualCam.m_LookAt = point;

        nameText.text = data.name;
        descriptionText.text = data.description;
        walkSPDText.text = data.walkSpeed.ToString();
        runSPDText.text = data.runSpeed.ToString();
        StaminaText.text = data.maxStamina.ToString();
        abilityText.text = data.ability;
    }
}
