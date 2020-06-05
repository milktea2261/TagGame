using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public UIManager uiManager;

    [Range(4, 20), SerializeField] int playerCount = 4;//遊戲人數
    [Range(0.05f, 0.5f), SerializeField] float taggerRate = 0.25f;//玩家中扮演Tagger的比例
    public int TaggerCount { get { return Mathf.CeilToInt(playerCount * taggerRate); } }
    public int FreezeCount {
        get {
            int counter = 0;
            foreach(AvatarController player in players) {
                if(player.IsFreeze) counter++;
            }
            return counter;
        } 
    }
    public int UnFrezzePlayer { get { return playerCount - TaggerCount - FreezeCount; } }

    [Range(120f, 300), SerializeField] float gameTime = 180f;//單局遊戲時間
    public float timer = 0;
    private bool isGameOver = false;
    private bool isGameStart = false;

    AvatarController playerControl = null;
    public AvatarController aiPrefab;
    public List<AvatarController> players = new List<AvatarController>();
    [SerializeField]List<AvatarController> unUseAvatars = new List<AvatarController>();

    public Transform[] runnerSpawnPoints;
    public Transform taggerSpawnPoint;

    public Material taggerMat;
    public Material runnerMat;
    public Material freezeMat;

    private void Awake() {
        Instance = this;
        playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<AvatarController>();
    }

    #region Game Start Setting
    //紀錄玩家的設定，到遊戲場景時使用


    private void Start() {
        SetPlayers(playerCount);
        SetTaggerRate(taggerRate);
        SetGameTime(gameTime);
    }


    public void SetPlayers(float num) {
        playerCount = (int)num;
        uiManager.UpdatePlayerCount(playerCount);
    }
    public void SetTaggerRate(float rate) {
        taggerRate = rate;
        uiManager.UpdateTaggerRate(taggerRate, TaggerCount);
    }
    public void SetGameTime(float time) {
        gameTime = time;
        uiManager.UpdateGameTime(gameTime);
    }
    #endregion


    private void Update() {
        if(isGameOver || !isGameStart) {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(Time.timeScale == 0) {
                Resume();
            }
            else {
                Pause();
            }
        }

        uiManager.UpdatePlayerState(playerControl.tag, playerControl.IsFreeze);
        uiManager.UpdatePlayerStamina(playerControl.Stamina / playerControl.maxStamina);

        timer -= Time.deltaTime;
        if(timer <= 0 || UnFrezzePlayer == 0) {
            GameOver();
        }
    }


    //生成玩家等初始設定
    public void GameStart() {
        //生成玩家
        players = new List<AvatarController>();
        for(int i = 0; i < playerCount; i++) {
            AvatarController avatar;
            if(i == 0) {
                avatar = playerControl;//玩家
            }
            else {
                if(unUseAvatars.Count > 0) {
                    avatar = unUseAvatars[0];
                    unUseAvatars.Remove(avatar);
                }
                else {
                    avatar = Instantiate<AvatarController>(aiPrefab);
                }
                avatar.name = string.Format("AI[{0}]", (i - 1).ToString("00"));
            }
            avatar.gameObject.SetActive(false);
            players.Add(avatar);
        }

        //打亂排序
        for(int i = 0; i < players.Count; i++) {
            int randIndex = Random.Range(0, players.Count);
            var temp = players[i];
            players[i] = players[randIndex];
            players[randIndex] = temp;
        }

        //設定玩家身分，初始位置
        for(int i = 0; i < players.Count; i++) {
            if(i < TaggerCount) {
                players[i].SetAvatar(AvatarTag.Tagger);
                players[i].name += " Tagger";
                players[i].transform.position = taggerSpawnPoint.position;
                players[i].transform.rotation = taggerSpawnPoint.rotation;
            }
            else {
                players[i].SetAvatar(AvatarTag.Runner);
                int randIndex = Random.Range(0, runnerSpawnPoints.Length);
                players[i].transform.position = runnerSpawnPoints[randIndex].position;
                players[i].transform.rotation = runnerSpawnPoints[randIndex].rotation;
            }
        }

        //開始遊戲
        timer = gameTime;
        foreach(AvatarController avatar in players) {
            avatar.UnFreeze();
            avatar.gameObject.SetActive(true);
        }

        isGameStart = true;
        isGameOver = false;
        Time.timeScale = 1;
    }

    [ContextMenu("GameOver")]
    public void GameOver() {

        bool isRunnerWin = timer <= 0;
        if(isRunnerWin) {
            Debug.Log("Game Over. Ruuner win.");
        }
        if(isRunnerWin) {
            Debug.Log("Game Over. Tagger win.");
        }

        uiManager.GameOver(isRunnerWin);
        isGameStart = false;
        isGameOver = true;
        Time.timeScale = 0;
    }

    //返回主選單
    public void MainMenu() {
        Debug.LogError("TODO: Go To Main Menu");
    }

    public void Pause() {
        Time.timeScale = 0;
        uiManager.TogglePauseMenu();
    }
    public void Resume() {
        Time.timeScale = 1;
        uiManager.TogglePauseMenu();
    }


    public void ExitApp() {
        Application.Quit();
    }

}
