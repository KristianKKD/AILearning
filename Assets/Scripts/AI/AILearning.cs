using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILearning : MonoBehaviour {

    AIInterface ai;

    public float speedScoreMultiplier = 0.4f;
    public float distanceScoreMultiplier = 0.8f;
    public float timeAliveMultiplier = 0.1f;
    public float closestScoreMultiplier = 0.8f;
    public float collectedScoreMutliplier = 2f;
    public float deathScorePunishment = 2f;

    public int generation = 0;
    public int collectedTargets = 0;
    public float closestDistance = 9999;
    public float currentDistance = 9999;
    public float lifeRemaining = 0;
    public float fitnessScore = 0;
    float timeSpentAlive = 0;

    private void Awake() {
        ai = GetComponent<AIInterface>();
    }

    public void CollectStats() {
        if(generation > 0)
            UIManager.ui.CollectData(timeSpentAlive, fitnessScore, collectedTargets);
    }

    public void ResetStats() {
        generation++;
        timeSpentAlive = 0;
        collectedTargets = 0;
        closestDistance = 99999;
        lifeRemaining = AIManager.manager.initialLifetime;
    }

    public void EdgePunish() {
        fitnessScore -= deathScorePunishment;
    }

    public void Mutate() {
        ai.net.Mutate(AIManager.manager.mutationChance, AIManager.manager.mutationStrength); //mutate
        //ai.net.Mutate(); //sparkz mutate
    }

    private void Update() {
        if (ai.died)
            return;

        lifeRemaining -= Time.deltaTime;
        timeSpentAlive += Time.deltaTime;

        ScoreAI();
        
        float distance = Mathf.Abs((ai.targetManager.target.transform.position - transform.position).magnitude) + 0.00001f;
        if (distance < closestDistance)
            closestDistance = distance;
        currentDistance = distance;

        if (lifeRemaining <= 0)
            ai.Death(false);
    }

    public float ScoreAI() {
        fitnessScore = 0;

        //ending distance from target score
        fitnessScore += 1/currentDistance * distanceScoreMultiplier;
        
        //closest distance from target score
        fitnessScore += 1/closestDistance * closestScoreMultiplier;

        fitnessScore += timeSpentAlive * timeAliveMultiplier;

        //collected targets score
        fitnessScore += collectedTargets * collectedScoreMutliplier;

        //Debug.Log("ID: " + net.NameNetwork() + " got score of: " + fitnessScore);
        return fitnessScore;
    }

    public void CollectedTarget() {
        collectedTargets++;
        ScoreAI(); //have to score the ai here because the update function may not take into consideration the final score of the AI
    }

}
