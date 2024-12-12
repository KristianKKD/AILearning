using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetSpawn : MonoBehaviour {

    public static TargetSpawn ts;

    public GameObject targetPrefab;

    public GameObject target;

    public float maxPadding = 100;

    public float minimumDistance = 500;

    private void Awake() {
        ts = this;
        Respawn();
    }

    public void Respawn() {
        if (target != null)
            Destroy(target);

        Vector2 randomPoint = RandomRespawn(maxPadding);

        target = Instantiate(targetPrefab, randomPoint, Quaternion.identity, FindObjectOfType<Canvas>().transform);
    }

    public static Vector2 RandomRespawn(float padding) {
        Canvas background = FindObjectOfType<Canvas>();
        float maxX = background.pixelRect.width;
        float maxY = background.pixelRect.height;

        Vector2 randomPoint = new Vector2(Random.Range(padding, maxX - padding),
            Random.Range(padding, maxY - padding));

        return randomPoint;
    }

}
