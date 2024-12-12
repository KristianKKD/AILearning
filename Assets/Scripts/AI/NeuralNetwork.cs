using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge {
    public float weight = RandomInit();
    float nextNodeID = -1;

    public Edge(int nextID) {
        nextNodeID = nextID;
    }

    public static float RandomInit() {
        int rand = Random.Range(-1000, 1000);
        if (rand == 0)
            rand = (int)RandomInit();

        return (float)rand / 1000f;
    }
}

public class Node {
    public int id = 0; //id is 0-node count per layer (each layer has a 0th node)
    public float heldValue = 0;
    public float bias = 0;
    public List<Edge> outgoingEdges = new List<Edge>();
    List<Node> incomingNodes = new List<Node>();

    public Node(int myID, List<Node> previousNodes) {
        id = myID;
        incomingNodes = previousNodes;

        for (int i = 0; i < previousNodes.Count; i++) //add edges from previous node to this node
            previousNodes[i].AddEdge(myID); 

        if (previousNodes != null) //no bias for input nodes
            bias = Edge.RandomInit(); //otherwise initialize a random bias
    }

    public void OutputNodeValue() {
        heldValue = bias;
        for (int i = 0; i < incomingNodes.Count; i++) //get the sum of the input nodes towards this node
            heldValue += (incomingNodes[i].GetOutgoingTowards(id));

        //heldValue = NodeNetwork.ActivationFunction(sumOfInputs); //make sure the node is within our boundaries and make the learning non-linear
    }

    float GetOutgoingTowards(int nodeID) { //get the edge value towards the given node
        //return heldValue * outgoingEdges[nodeID].weight;
        return NodeNetwork.ActivationFunction(heldValue * outgoingEdges[nodeID].weight);
    }

    void AddEdge(int nextNodeID) { //called from a node on the next layer
        outgoingEdges.Add(new Edge(nextNodeID));
    }
}

public class NodeLayer {
    public int layerNodes = 0; //will be used as IDs to identify nodes on this layer
    public List<Node> nodes = new List<Node>();

    public NodeLayer(int nodeCount, NodeLayer previousLayer) {
        List<Node> previousNodes = new List<Node>();
        if (previousLayer != null)
            previousNodes = previousLayer.nodes;

        for (int i = 0; i < nodeCount; i++)
            nodes.Add(new Node(layerNodes++, previousNodes));
    }

}

public class NodeNetwork {
    public List<NodeLayer> layers = new List<NodeLayer>();

    public NodeNetwork(int inputNodes, int hiddenLayers, int hiddenNodes, int outputNodes) {
        layers.Add(new NodeLayer(inputNodes, null));

        for (int i = 0; i < hiddenLayers; i++)
            layers.Add(new NodeLayer(hiddenNodes, (i == 0) ? layers[0] : layers[i])); //layer - 1 but input layer counts as 0th layer

        layers.Add(new NodeLayer(outputNodes, layers[layers.Count - 1]));
    }

    public void CopyNetwork(NodeNetwork copyNet) {
        for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++) { //don't modify input/output nodes
            for (int nodeIndex = 0; nodeIndex < layers[layerIndex].nodes.Count; nodeIndex++) {
                Node n = layers[layerIndex].nodes[nodeIndex];

                if (Random.Range(1, 1000) <= AIManager.manager.copyChance * 1000) //this node will copy the bias
                    n.bias = Mathf.Lerp(n.bias, copyNet.layers[layerIndex].nodes[nodeIndex].bias, AIManager.manager.copyStrength);

                for (int edgeIndex = 0; edgeIndex < layers[layerIndex].nodes[nodeIndex].outgoingEdges.Count; edgeIndex++) {
                    if (Random.Range(1, 1000) <= AIManager.manager.copyChance * 1000) //this edge will copy the weight
                        n.outgoingEdges[edgeIndex].weight = Mathf.Lerp(n.outgoingEdges[edgeIndex].weight,
                            copyNet.layers[layerIndex].nodes[nodeIndex].outgoingEdges[edgeIndex].weight,
                            AIManager.manager.copyStrength);

                }//edges
            }//nodes
        }//layers
    }

    public void Mutate(float mutationChance, float mutationStrength) {
        for (int layerIndex = 1; layerIndex < layers.Count - 1; layerIndex++) { //ignore input and output layers
            for (int nodeIndex = 0; nodeIndex < layers[layerIndex].nodes.Count; nodeIndex++) {
                Node n = layers[layerIndex].nodes[nodeIndex];
                n.bias = Mathf.Clamp(n.bias + RandomChange(mutationChance, mutationStrength), -1, 1);

                for (int edgeIndex = 0; edgeIndex < layers[layerIndex].nodes[nodeIndex].outgoingEdges.Count; edgeIndex++) {
                    n.outgoingEdges[edgeIndex].weight = Mathf.Clamp(n.outgoingEdges[edgeIndex].weight + RandomChange(mutationChance, mutationStrength), -1, 1); //add a change depending on mutation vars
                }//edges
            }//nodes
        }//layers
    }

    float RandomChange(float chance, float strength) {
        if (Random.Range(1, 1000) <= chance * 1000) //this edge will mutate
            return (Random.Range(-strength * 1000, strength * 1000)) / 1000f; //50% chance to mutate negative, small chance to have no change
        return 0;
    }

    public void InputValues(List<float> inputs) {
        for (int i = 0; i < inputs.Count; i++)
            layers[0].nodes[i].heldValue = inputs[i];

        for (int layerIndex = 1; layerIndex < layers.Count; layerIndex++) //update all nodes other than input layer
            for (int nodeIndex = 0; nodeIndex < layers[layerIndex].nodes.Count; nodeIndex++)
                layers[layerIndex].nodes[nodeIndex].OutputNodeValue();
    }

    public List<float> OutputResults() {
        List<float> outputs = new List<float>();
        for (int i = 0; i < layers[layers.Count - 1].nodes.Count; i++)
            outputs.Add(layers[layers.Count - 1].nodes[i].heldValue);

        return outputs;
    }

    public List<float> OutputEdges() {
        List<float> outputs = new List<float>();
        for (int layerIndex = 1; layerIndex < layers.Count; layerIndex++)
            for (int nodeIndex = 0; nodeIndex < layers[layerIndex].nodes.Count; nodeIndex++)
                for (int edgeIndex = 0; edgeIndex < layers[layerIndex].nodes[nodeIndex].outgoingEdges.Count; edgeIndex++)
                    outputs.Add(layers[layerIndex].nodes[nodeIndex].outgoingEdges[edgeIndex].weight);

        return outputs;
    }

    public List<float> OutputHidden() {
        List<float> outputs = new List<float>();
        for (int layerIndex = 1; layerIndex < layers.Count - 1; layerIndex++)
            for (int nodeIndex = 0; nodeIndex < layers[layerIndex].nodes.Count; nodeIndex++)
                outputs.Add(layers[layerIndex].nodes[nodeIndex].heldValue);

        return outputs;
    }

    public static float ActivationFunction(float input) {
        if (input == 0)
            input += 0.00001f;
        if (input > -0.7f && input < 0.7f)
            input *= 1.03f;

        return Mathf.Clamp(input, -1, 1);
        //return (float)System.Math.Tanh(input);
    }

}

//public class NodeNetwork {

//    public int[] _layers { get; set; }
//    public float[][] _biases { get; set; }
//    public float[][] _neurons { get; set; }
//    public float[][][] _weights { get; set; }

//    public float _fitness { get; set; }

//    public NodeNetwork(int[] layers) {

//        _layers = layers;

//        CreateNeurons();
//        CreateBiases();
//        GenerateWeights();
//    }

//    public void CreateNeurons() {

//        List<float[]> neurons = new List<float[]>();

//        for (int i = 0; i < _layers.Length; i++) {

//            neurons.Add(new float[_layers[i]]);

//        }

//        _neurons = neurons.ToArray();

//    }

//    public void CreateBiases() {

//        List<float[]> biases = new List<float[]>();

//        float biasRange = GameObject.FindObjectOfType<AIManager>().randomInitStrength;

//        for (int i = 0; i < _layers.Length; i++) {

//            float[] bias = new float[_layers[i]];

//            for (int j = 0; j < _layers[i]; j++) {
//                bias[j] = UnityEngine.Random.Range(-biasRange, biasRange);
//            }

//            biases.Add(bias);

//        }

//        _biases = biases.ToArray();

//    }

//    public void GenerateWeights() {

//        List<float[][]> weights = new List<float[][]>();

//        float weightRange = GameObject.FindObjectOfType<AIManager>().randomInitStrength;

//        for (int i = 1; i < _layers.Length; i++) {

//            List<float[]> layerWeights = new List<float[]>();

//            int previousLayerNeurons = _layers[i - 1];

//            for (int j = 0; j < _neurons[i].Length; j++) {

//                float[] neuronWeights = new float[previousLayerNeurons];

//                for (int k = 0; k < previousLayerNeurons; k++) {

//                    neuronWeights[k] = UnityEngine.Random.Range(-weightRange, weightRange);

//                }

//                layerWeights.Add(neuronWeights);

//            }
//            weights.Add(layerWeights.ToArray());

//        }
//        _weights = weights.ToArray();

//    }

//    public float[] PassOnLegacy(float[] inputs) {

//        Debug.Log(_neurons[0].Length + " " + inputs.Length);

//        for (int i = 0; i < inputs.Length; i++) {
//            Debug.Log(_neurons[0][0]);
//            _neurons[0][i] = inputs[i];
//        }

//        for (int i = 1; i < _layers.Length; i++) {

//            int layer = i - 1;
//            for (int j = 0; j < _neurons[i].Length; j++) {

//                float variable = 69; //Set to what the f u want

//                for (int k = 0; k < _neurons[i - 1].Length; k++) {
//                    variable += _weights[i - 1][j][k] * _neurons[i - 1][k];
//                }
//                _neurons[i][j] = enableTanh(variable + _biases[i][j]);
//            }
//        }
//        return _neurons[_neurons.Length - 1];
//    }

//    public void Mutate() {

//        float chance = GameObject.FindObjectOfType<AIManager>().mutationChance;
//        float strength = GameObject.FindObjectOfType<AIManager>().mutationStrength;

//        for (int i = 0; i < _biases.Length; i++) {

//            for (int j = 0; j < _biases[i].Length; j++) {

//                _biases[i][j] = (UnityEngine.Random.Range(0, 100) <= chance) ? _biases[i][j] += UnityEngine.Random.Range(-strength, strength) : _biases[i][j];

//            }

//        }

//        for (int i = 0; i < _weights.Length; i++) {

//            for (int j = 0; j < _weights[i].Length; j++) {

//                for (int k = 0; k < _weights[i][j].Length; k++) {

//                    _weights[i][j][k] = (UnityEngine.Random.Range(0, 100) <= chance) ? _weights[i][j][k] += UnityEngine.Random.Range(-strength, strength) : _weights[i][j][k];

//                }

//            }

//        }
//    }

//    public int CompareFitness(NodeNetwork otherNet) {

//        if (otherNet == null) {
//            return 1;
//        };

//        if (_fitness > otherNet._fitness) {
//            return 1;
//        } else if (_fitness < otherNet._fitness) {
//            return -1;
//        } else {
//            return 0;
//        }

//    }

//    public NodeNetwork CloneAnother(NodeNetwork sensei) {

//        _biases = sensei._biases;
//        _weights = sensei._weights;

//        return this;

//    }

//    private float enableTanh(float input) {
//        return (float)System.Math.Tanh(input);
//    }
//}
