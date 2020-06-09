using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public UIManager uiManager;

    public int FreezeCount {
        get {
            int counter = 0;
            foreach(AvatarController player in players) {
                if(player.IsFreeze)
                    counter++;
            }
            return counter;
        }
    }
    public int UnFrezzePlayer { get { return GameManager.Instance.playerCount - GameManager.Instance.taggerCount - FreezeCount; } }

    public float timer = 0;
    private bool isGameOver = false;
    private bool isGameStart = false;

    AvatarController playerControl = null;
    public AvatarController aiPrefab = null;
    public List<AvatarController> players = new List<AvatarController>();
    public GameObject[] models = new GameObject[0];
    public TextMeshPro headText = null;

    public Transform[] runnerSpawnPoints;
    public Transform taggerSpawnPoint;

    private void Awake() {
        playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<AvatarController>();
    }

    //生成玩家，出生位置
    private void Start() {
        //生成玩家
        players = new List<AvatarController>();
        for(int i = 0; i < GameManager.Instance.playerCount; i++) {
            AvatarController avatar;
            if(i == 0) {
                avatar = playerControl;//玩家
            }
            else {
                GameObject model = models[0];
                string avatarName = string.Format("AI[{0}]", (i - 1).ToString("00"));
                avatar = CreateAvatar(model, avatarName);
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

        //設定身分，初始位置
        for(int i = 0; i < players.Count; i++) {
            if(i < GameManager.Instance.taggerCount) {
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

        GameStart();
    }

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
    AvatarController CreateAvatar(GameObject avatarModel, string avatarName) {
        AvatarController avatar = Instantiate<AvatarController>(aiPrefab);
        avatar.name = avatarName;

        GameObject model = Instantiate<GameObject>(avatarModel, avatar.transform);

        TextMeshPro text = Instantiate<TextMeshPro>(headText, avatar.transform);
        avatar.headText = text;
        return avatar;
    }

    //開始遊戲
    public void GameStart() {
        timer = GameManager.Instance.gameTime;
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

}
