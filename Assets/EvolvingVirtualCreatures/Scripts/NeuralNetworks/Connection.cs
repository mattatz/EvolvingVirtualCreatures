using UnityEngine;
using Random = UnityEngine.Random;

using System.Collections;

namespace mattatz.NeuralNetworks {

	public class Connection {

		public Neuron From { get { return from; } }
		public Neuron To { get { return to; } }
		public float Weight { get { return weight; } }

		protected Neuron from;
		protected Neuron to;
		protected float weight;

		public Connection (Neuron from, Neuron to, float weight) {
			this.from = from;
			this.to = to;
			this.weight = weight;

			this.from.AddConnection(this);
			this.to.AddConnection(this);
		}

		public void AdjustWeight(float deltaWeight) {
			weight += deltaWeight;
		}

	}

}

