using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AIManager : MonoBehaviour {

    public static AIManager manager;

    public GameObject agentPrefab;

    public float initialLifetime = 10f;
    public int numberOfAgents = 1;

    public int epoch = 0;
    public float badPerformanceMultiplier = 3f;
    public float mutationChance = 0.05f;
    public float mutationStrength = 0.01f;
    public float minimumCopyFitness = 0.2f;
    public float copyChance = 0.9f;
    public float copyStrength = 0.9f;

    public float countdownTime;

    public List<GameObject> agents = new List<GameObject>();

    public Transform respawnPoint;

    [SerializeField]
    int activeAgents = 0;

    [Range(1, 5)]
    public float timeScale = 1;

    public AIInterface fittestAI = null;

    public List<float> fittestData = new List<float>();

    private void Awake() {
        manager = this;

        epoch = 0;
        countdownTime = initialLifetime;

        Canvas background = FindObjectOfType<Canvas>();

        for (int i = 0; i < numberOfAgents; i++) {
            GameObject agent = Instantiate(agentPrefab, background.transform);
            agents.Add(agent);
            agent.GetComponent<AIInterface>().ResetAgent();
            agent.gameObject.name = i.ToString();
        }

        activeAgents = numberOfAgents;
    }

    private void Update() {
        Time.timeScale = timeScale;

        fittestAI = null;
        float highestTime = 0;
        foreach (GameObject agent in agents) { //get the best agent from epoch
            AIInterface ai = agent.GetComponent<AIInterface>();

            if (!ai.died && ai.learning.lifeRemaining > highestTime)
                countdownTime = ai.learning.lifeRemaining;

            if (fittestAI == null || (fittestAI != null && ai.learning.fitnessScore > fittestAI.learning.fitnessScore))
                fittestAI = ai;
        }
    }

    void EndEpoch() {
        countdownTime = initialLifetime;

        epoch++;

        float normalMutationChance = mutationChance;
        float normalMutationStrength = mutationStrength;

        if (fittestAI.learning.fitnessScore > minimumCopyFitness) {
            mutationChance *= badPerformanceMultiplier;
            mutationStrength *= badPerformanceMultiplier;
        }

        fittestData.Add(fittestAI.learning.fitnessScore);

        //if (epoch <= 99) {
        //    mutationChance *= 1 - (epoch / 100f);
        //    mutationStrength *= 1 - (epoch / 100f);
        //}

        foreach (GameObject agent in agents) { //copy the best agent (mostly) and reset
            agent.gameObject.SetActive(true);

            AIInterface ai = agent.GetComponent<AIInterface>();
            ai.ResetAgent();

            if (ai != fittestAI)
                ai.CopyAndMutate((fittestAI.learning.fitnessScore > minimumCopyFitness) ? fittestAI : null); //don't copy if the ai didn't meet minimum requirements
            else
                Debug.Log("Not modifying: " + agent.name);
        }

        mutationChance = normalMutationChance;
        mutationStrength = normalMutationStrength;
    }

    public void AddPoint(GameObject player) {
        FindObjectOfType<TargetSpawn>().Respawn();
        player.GetComponent<AIInterface>().AddScore();
        FindObjectOfType<AIManager>().EndEpoch();
    }

    public void UpdateStatus() {
        activeAgents = 0;
        foreach (GameObject agent in agents)
            if (!agent.GetComponent<AIInterface>().died)
                activeAgents++;

        if (activeAgents == 0)
            EndEpoch();
    }

}
