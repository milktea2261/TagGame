using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Text gameText = null;
    [SerializeField] Text avatarStateText = null;
    [SerializeField] Text runToogleText = null;

    [SerializeField] Scrollbar staminaBar = null;

    [SerializeField] GameObject pausePanel = null;

    [SerializeField] GameObject gameOverPanel = null;
    [SerializeField] Text gameOverText = null;

    [SerializeField] LevelManager levelManager = null;

    private void Update() {
        gameText.text = "UnFreeze: " + levelManager.UnFrezzePlayer.ToString("00") + "\t";
        gameText.text += "Left Time: " + levelManager.timer.ToString("000");
    }


    public void UpdatePlayerState(string avatar, bool isFreeze) {
        if(avatar == "Tagger") {
            avatarStateText.text = "Tagger";
        }
        else {
            avatarStateText.text = "Runner";
            if(isFreeze) {
                avatarStateText.text += "[Freeze]";
            }
        }
    }

    public void UpdatePlayerStamina(float percent) {
        staminaBar.size = percent;
    }

    public void RunBtn(bool isRun) {
        runToogleText.text = isRun ? "跑" : "走";
    }

    public void TogglePauseMenu() {
        pausePanel.SetActive(!pausePanel.activeInHierarchy);
    }

    public void GameOver(bool isRunnerWin) {
        gameOverPanel.SetActive(true);
        
        if(isRunnerWin) {
            gameOverText.text = "Runner Win";
        }
        else {
            gameOverText.text = "Runner Lose";
        }
    }
}