using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {

    public static UIManager ui;

    public TMP_Text agentName, lifeText, generationText, fitnessText; //hidden until selected agent
    public TMP_Text avgScore, bestScore, lastScore, avgLife, avgTargets, fittestName, deaths; //always visible data
    public TMP_Text resetTime; //other

    AIInterface currentAI;

    [SerializeField]
    float averageLifetime, averageScore = 0;
    [SerializeField]
    int averageTargetsCollected;
    [SerializeField]
    int dataPoints = 0;

    bool tracking = true;

    private void Awake() {
        ui = this;
    }

    private void Update() {
        GlobalStats();

        AIInterface fittestAI = AIManager.manager.fittestAI;
        if (fittestAI != null)
            fittestName.text = fittestAI.gameObject.name;
        else
            fittestName.text = "None";

        if (Input.GetMouseButtonUp(0)) {
            tracking = false;
            Deselect();
        } if (Input.GetMouseButtonUp(1)) {
            tracking = !tracking;
            if (!tracking)
                Deselect();
        }

        if (tracking) {
            if (AIManager.manager.fittestAI != null)
                OnAgentHover(AIManager.manager.fittestAI.gameObject);
            else
                Deselect();
        }

        if (currentAI != null) {
            lifeText.text = currentAI.learning.lifeRemaining.ToString("F");
            fitnessText.text = currentAI.learning.fitnessScore.ToString("F");
            generationText.text = currentAI.learning.generation.ToString();
            agentName.text = currentAI.gameObject.name.ToString();
        }
    }

    void GlobalStats() {
        resetTime.text = AIManager.manager.countdownTime.ToString("F");

        averageLifetime = 0;
        averageScore = 0;
        foreach (GameObject agent in AIManager.manager.agents) {
            AILearning l = agent.GetComponent<AILearning>();
            averageLifetime += l.lifeRemaining;
            averageScore += l.fitnessScore;
        }

        bestScore.text = ((AIManager.manager.fittestAI != null) ? AIManager.manager.fittestAI.learning.fitnessScore : 0).ToString("F");

        if (averageLifetime != 0)
            avgLife.text = (averageLifetime / AIManager.manager.agents.Count).ToString("F");
        if (averageScore != 0)
            avgScore.text = (averageScore / AIManager.manager.agents.Count).ToString("F");
        if (AIManager.manager.fittestData.Count > 0)
            lastScore.text = AIManager.manager.fittestData[AIManager.manager.fittestData.Count - 1].ToString("F");
    }

    void Deselect() {
        if (currentAI != null)
            currentAI.selectIndicator.SetActive(false);

        currentAI = null;
        ToggleVisibility(false);
    }

    public void OnAgentHover(GameObject agent) {
        if(currentAI != null)
            currentAI.selectIndicator.SetActive(false);

        currentAI = agent.GetComponent<AIInterface>();
        currentAI.selectIndicator.SetActive(true);

        ToggleVisibility(true);
    }

    void ToggleVisibility(bool visibility) {
        lifeText.transform.parent.gameObject.SetActive(visibility);
        generationText.transform.parent.gameObject.SetActive(visibility);
        fitnessText.transform.parent.gameObject.SetActive(visibility);
        agentName.transform.parent.gameObject.SetActive(visibility);
    }

    public void CollectData(float lifespan, float score, int collectedTargets) {
        dataPoints++;

        //averageLifetime += lifespan;
        //averageScore += score;
        //averageTargetsCollected += collectedTargets;

        //if(averageLifetime != 0)
        //    avgLife.text = (averageLifetime / dataPoints).ToString("F");
        //if (averageScore != 0)
        //    avgScore.text = (averageScore / dataPoints).ToString("F");
        //if (averageTargetsCollected != 0)
        //    avgTargets.text = (averageTargetsCollected / dataPoints).ToString();
        
        deaths.text = dataPoints.ToString();
    }

}
