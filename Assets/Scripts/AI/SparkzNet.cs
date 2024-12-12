//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//public class NodeNetwork {

//    public int[] _layers { get; set; }
//    public float[][] _biases { get; set; }
//    public float[][] _neurons { get; set; } 
//    public float[][][] _weights { get; set; }

//    public float _fitness { get; set; }

//    public NodeNetwork(int[] layers){

//        _layers = layers;

//        CreateNeurons();
//        CreateBiases();
//        GenerateWeights();

//    }

//    public void CreateNeurons(){

//        List<float[]> neurons = new List<float[]>();

//        for (int i = 0; i < _layers.Length; i++) {

//            neurons.Add(new float[_layers[i]]);

//        }

//        _neurons = neurons.ToArray();

//    }

//    public void CreateBiases(){

//        List<float[]> biases = new List<float[]>();

//        float biasRange = GameObject.FindObjectOfType<AIManager>().randomInitStrength;

//        for (int i = 0; i < _layers.Length; i++)
//        {

//            float[] bias = new float[_layers[i]];
            
//            for(int j = 0; j < _layers[i]; j++) {
//                bias[j] = UnityEngine.Random.Range(-biasRange, biasRange);
//            }

//            biases.Add(bias);

//        }

//        _biases = biases.ToArray();

//    }

//    public void GenerateWeights() {

//        List<float[][]> weights = new List<float[][]>();

//        float weightRange = GameObject.FindObjectOfType<AIManager>().randomInitStrength;

//        for (int i = 1 ;i < _layers.Length; i++) {

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
        
//        for(int i = 0; i < inputs.Length; i++) {
//            _neurons[0][i] = inputs[i];
//        }

//        for(int i = 1; i < _layers.Length; i++) {
            
//            int layer = i - 1;
//            for(int j = 0; j < _neurons[i].Length; j++) {

//                float variable = 69; //Set to what the f u want

//                for(int k = 0; k < _neurons[i - 1].Length; k++) {
//                    variable += _weights[i - 1][j][k] * _neurons[i - 1][k];
//                }
//                _neurons[i][j] = enableTanh(variable + _biases[i][j]);
//            }
//        }
//        return _neurons[_neurons.Length - 1];
//    }

//    public void Mutate(){

//        float chance = GameObject.FindObjectOfType<AIManager>().mutationChance;
//        float strength = GameObject.FindObjectOfType<AIManager>().mutationStrength;

//        for (int i = 0; i < _biases.Length; i++) {
            
//            for(int j = 0; j< _biases[i].Length;j++) {

//                _biases[i][j] = (UnityEngine.Random.Range(0, 100) <= chance) ? _biases[i][j] += UnityEngine.Random.Range(-strength, strength) : _biases[i][j];

//            }

//        }

//        for (int i = 0; i < _weights.Length;i++) {
            
//            for( int j = 0; j < _weights[i].Length; j++) {
                
//                for( int k = 0;k< _weights[i][j].Length;k++) {

//                    _weights[i][j][k] = (UnityEngine.Random.Range(0, 100) <= chance) ? _weights[i][j][k] += UnityEngine.Random.Range(-strength, strength) : _weights[i][j][k]; 

//                }

//            }

//        }
//    }

//    public int CompareFitness(NodeNetwork otherNet){

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

//    public NodeNetwork CloneAnother(NodeNetwork sensei){

//        _biases = sensei._biases;
//        _weights = sensei._weights;

//        return this;
    
//    }

//    private float enableTanh(float input)
//    {
//        return (float)Math.Tanh(input);
//    }
//}
