using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [Range(4, 20), SerializeField] int playerCount = 4;//遊戲人數
    [Range(0.05f, 0.5f), SerializeField] float taggerRate = 0.25f;//玩家中扮演Tagger的比例
    [Range(120f, 300), SerializeField] float gameTime = 180f;//單局遊戲時間
    public int TaggerCount { get { return Mathf.CeilToInt(playerCount * taggerRate); } }

    [SerializeField] Slider playerSlider = null;
    [SerializeField] Slider taggerSlider = null;
    [SerializeField] Slider gameTimeSlider = null;

    [SerializeField] Text playerCountText = null;
    [SerializeField] Text taggerRateText = null;
    [SerializeField] Text taggerCountText = null;
    [SerializeField] Text gameTimeText = null;


    private void Start() {
        playerSlider.value = playerCount;
        taggerSlider.value = taggerRate;
        gameTimeSlider.value = gameTime;

        UpdatePlayerCount(playerSlider.value);
        UpdateTaggerRate(taggerSlider.value);
        UpdateGameTime(gameTimeSlider.value);
    }

    public void UpdatePlayerCount(float count) {
        playerCount = (int)count;
        playerCountText.text = "玩家人數：" + count.ToString("00");
        taggerCountText.text = "獵人數量：" + TaggerCount;
    }
    public void UpdateTaggerRate(float rate) {
        taggerRate = rate;
        taggerRateText.text = "獵人比例：" + rate.ToString("0.00");
        taggerCountText.text = "獵人數量：" + TaggerCount;
    }
    public void UpdateGameTime(float gameTime) {
        this.gameTime = gameTime;
        gameTimeText.text = "遊戲時長：" + gameTime.ToString("000") + " 秒";
    }

    public void StartGame() {
        GameManager.Instance.playerCount = playerCount;
        GameManager.Instance.taggerCount = TaggerCount;
        GameManager.Instance.gameTime = gameTime;

        GameManager.Instance.LoadLevel("000");
    }

}
