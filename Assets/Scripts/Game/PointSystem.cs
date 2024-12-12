using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointSystem : MonoBehaviour {

    public TMP_Text countdownText;
    public float countdown = 0.0000001f;
    public float maxCountdown;

    private void Update() {
        countdown -= Time.deltaTime;
        if (countdown < 0) {
            countdown = 0;
            ResetGame();
        }

        countdownText.text = countdown.ToString("F");
    }

    public void AddPoint(GameObject player) {
        FindObjectOfType<TargetSpawn>().Respawn();
    }

    private void ResetGame() {
        countdown = maxCountdown;
    }

}
