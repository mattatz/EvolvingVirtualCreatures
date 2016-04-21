using UnityEngine;
using Random = UnityEngine.Random;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace mattatz.NeuralNetworks {

	public class Network {

		List<Neuron[]> layers;

		public Network(int[] layersCount) {
			int count = 0;
			for(int i = 0, n = layersCount.Length; i < n - 1; i++) {
				count += layersCount[i] * layersCount[i + 1];
			}

			float[] weights = new float[count];
			for(int i = 0, n = count; i < n; i++) {
				weights[i] = Random.Range(-1f, 1f);
			}

			Setup (layersCount, weights);
		}

		public Network(int[] layersCount, float[] weights) {
			/*
			Debug.Log ("------");
			for(int i = 0, n = weights.Length; i < n; i++) {
				Debug.Log (i + " : " + weights[i]);
			}
			*/
			Setup (layersCount, weights);
		}

		public void Setup (int[] layersCount, float[] weights) {
			layers = new List<Neuron[]>();

			var n = layersCount.Length;
			for(int i = 0; i < n; i++) {
				bool bias = (i != 0 && i != n - 1);

				var m = layersCount[i];
				var layer = new Neuron[m];

				for(int j = 0; j < m - 1; j++) {
					layer[j] = new Neuron();
				}

				if(bias) {
					layer[m - 1] = new Neuron(1f, true);
				} else {
					layer[m - 1] = new Neuron();
				}

				layers.Add(layer);
			}

			int offset = 0;
			for(int i = 0; i < n - 1; i++) {
				var len = layers[i].Length * layers[i + 1].Length;
				float[] subset = new float[len];;
				Array.Copy(weights, offset, subset, 0, len);
				Connect(layers[i], layers[i + 1], subset);

				offset += len;
			}
		}

		void Connect (Neuron[] from, Neuron[] to, float[] weights) {
			int n = from.Length, m = to.Length;
			for(int i = 0; i < n; i++) {
				for(int j = 0; j < m; j++) {
					new Connection(from[i], to[j], weights[i * m + j]);
				}
			}
		}

		public float[] FeedForward (float[] inputs) {

			var inputLayer = layers[0];
			for(int i = 0, n = inputs.Length; i < n; i++) {
				inputLayer[i].Input(inputs[i]);
			}

			for(int i = 1, n = layers.Count; i < n - 1; i++) {
				var layer = layers[i];
				for(int j = 0, m = layer.Length; j < m; j++) {
					layer[j].Compute();
				}
			}

			var outputLayer = layers[layers.Count - 1];
			var output = new float[outputLayer.Length];
			for(int i = 0, n = outputLayer.Length; i < n; i++) {
				outputLayer[i].Compute();
				output[i] = outputLayer[i].Output;
			}

			return output;
		}

	}

}

