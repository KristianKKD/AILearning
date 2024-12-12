using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIInterface : MonoBehaviour {

    [HideInInspector]
    public NodeNetwork net;

    [HideInInspector]
    public PlayerMove player;

    [HideInInspector]
    public TargetSpawn targetManager;

    [HideInInspector]
    public AILearning learning;

    [SerializeField]
    List<float> inputValues = new List<float>();
    [SerializeField]
    List<float> outputEdges = new List<float>();
    [SerializeField]
    List<float> outputHidden = new List<float>();
    [SerializeField]
    List<float> outputValues = new List<float>();

    public GameObject selectIndicator;

    public bool died = false;

    float maxX, maxY;

    private void Awake() {
        net = new NodeNetwork(4, //4 input nodes //displacementX, displacementY, velocityX, velocityY
                                          3, //layers (+input+output)
                                          8, //nodes in the layers (not including input/output)
                                          2); //2 output nodes //inputX, inputY

        //net = new NodeNetwork(new int[4]);

        Canvas background = FindObjectOfType<Canvas>();
        maxX = background.pixelRect.width;
        maxY = background.pixelRect.height;

        player = GetComponent<PlayerMove>();
        learning = GetComponent<AILearning>();
        targetManager = FindObjectOfType<TargetSpawn>();
    }

    void Update() {
        if (died)
            return;

        List<float> inputs = new List<float>() {
            Mathf.Clamp(targetManager.target.transform.position.x / maxX, -1, 1), //target position x
            Mathf.Clamp(targetManager.target.transform.position.y / maxY, -1, 1), //target y
            Mathf.Clamp(transform.position.x / maxX, -1, 1), //my position x
            Mathf.Clamp(transform.position.y / maxY, -1, 1), //y
        };

        //float[] outputs = net.PassOnLegacy(inputs.ToArray());

        net.InputValues(inputs);
        List<float> aiMovements = net.OutputResults();
        player.AIInput(aiMovements[0], aiMovements[1]); //x, y movement

        gameObject.GetComponent<Image>().color = new Color(
            aiMovements[0],
            aiMovements[1],
            0.5f);

        inputValues = inputs;
        outputEdges = net.OutputEdges();
        outputHidden = net.OutputHidden();
        outputValues = aiMovements;
    }

    public void CopyAndMutate(AIInterface copyAI) {
        if(copyAI != null)
            Debug.Log("Fittest agent: " + copyAI.gameObject.name);

        if (copyAI != null)
            net.CopyNetwork(copyAI.net);

        learning.Mutate(); //mutate the network
    }

    public void Death(bool edge) {
        if (died)
            return;

        died = true;
        GetComponent<Image>().enabled = false;

        if (edge)
            learning.EdgePunish();

        learning.CollectStats();
        AIManager.manager.UpdateStatus();
    }

    public void ResetAgent() {
        GetComponent<Image>().enabled = true;

        if (!died)
            learning.CollectStats();

        transform.position = AIManager.manager.respawnPoint.position;
        died = false;

        player.ResetVelocity();
        learning.ResetStats();
        AIManager.manager.UpdateStatus();
    }

    public void AddScore() {
        learning.CollectedTarget();
    }

    public void OnAgentHover() {
        UIManager.ui.OnAgentHover(gameObject);
    }

}
