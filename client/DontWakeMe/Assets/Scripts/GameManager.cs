using System.Collections;
using System.Collections.Generic;
using DWM;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public MapContainer mapContainer;
    public EarthsManager earthManager;
    public InputController p1Controller;
    public InputController p2Controller;
    public GameObject upLadders;
    public GameObject botLadders;
    public GameObject upBG;
    public GameObject botBG;

    public float winRate = 0.75f;
    public float switchTime = 15f;

    public Scrollbar winBar;

    public Text p1Score;

    public Text p2Score;

    public Text timeText;

    public Text winText;

    private float totalScore;
    private float winScore;
    private float loseScore;
    private float diffScore;
    private float nowLeftSwitchTime;

    // Use this for initialization
    void Start() {
        totalScore = CalculateTotalScore();
        winScore = totalScore * winRate;
        loseScore = totalScore - winScore;
        diffScore = winScore - loseScore;
        winText.gameObject.SetActive(false);
        nowLeftSwitchTime = switchTime;
        upLadders.SetActive(true);
        botLadders.SetActive(false);
        upBG.transform.rotation = Quaternion.identity;
        botBG.transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update() {
        nowLeftSwitchTime -= Time.deltaTime;
        if (nowLeftSwitchTime < 0) {
            UpsideDown();
        }
        timeText.text = nowLeftSwitchTime.ToString("0.0");
        UpdateScore(CalculateWaterScore());
        if (Input.GetKeyDown(KeyCode.F2)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            UpsideDown();
        }
    }

    void UpdateScore(float _waterScore) {
        if (p1Controller.isWater) {
            if (_waterScore > winScore) {
                //p1 win
                ShowWin(true);
            }
            else if (_waterScore < loseScore) {
                //p2 win
                ShowWin(false);
            }
            else {
                winBar.value = (_waterScore - loseScore) / diffScore;
            }
            p1Score.text = _waterScore.ToString("0");
            p2Score.text = (totalScore - _waterScore).ToString("0");
        }
        else {
            if (_waterScore > winScore) {
                //p2 win
                ShowWin(false);
            }
            else if (_waterScore < loseScore) {
                //p1 win
                ShowWin(true);
            }
            else {
                winBar.value = (winScore - _waterScore) / diffScore;
            }
            p1Score.text = (winScore - _waterScore).ToString("0");
            p2Score.text = _waterScore.ToString("0");
        }
    }

    float CalculateTotalScore() {
        float totalScore = 0;
        var iter = mapContainer.Map.upTree.branchGroupMap.GetEnumerator();
        while (iter.MoveNext()) {
            Group group = iter.Current.Value;
            totalScore += 100 * group.value;
        }
        return totalScore;
    }

    float CalculateWaterScore() {
        float waterScore = 0;
        if (p1Controller.isWater) {
            var iter = mapContainer.Map.upTree.branchGroupMap.GetEnumerator();
            while (iter.MoveNext()) {
                Group group = iter.Current.Value;
                waterScore += group.hp * group.value;
            }
        }
        else {
            var iter = mapContainer.Map.botTree.branchGroupMap.GetEnumerator();
            while (iter.MoveNext()) {
                Group group = iter.Current.Value;
                waterScore += group.hp * group.value;
            }
        }
        return waterScore;
    }

    void ShowWin(bool player1Win) {
        if (player1Win) {
            winText.text = "Player 1 wins~";
            winBar.value = 1;
        }
        else {
            winText.text = "Player 2 wins~";
            winBar.value = 0;
        }
        winText.gameObject.SetActive(true);
    }

    void UpsideDown() {
        nowLeftSwitchTime = switchTime;
        earthManager.SwitchMode();
        upLadders.gameObject.SetActive(!upLadders.activeInHierarchy);
        botLadders.gameObject.SetActive(!botLadders.activeInHierarchy);
        upBG.transform.eulerAngles = new Vector3(0, 0, upBG.transform.eulerAngles.z + 180);
        botBG.transform.eulerAngles = new Vector3(0, 0, botBG.transform.eulerAngles.z + 180);
        mapContainer.isReverse = !mapContainer.isReverse;
        mapContainer.RefreshAllAlpha();
    }
    
}