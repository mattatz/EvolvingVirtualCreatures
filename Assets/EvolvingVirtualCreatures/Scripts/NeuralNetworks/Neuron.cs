using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace mattatz.NeuralNetworks {

	public class Neuron {

		public float Output { get { return output; } }
		public List<Connection> Connections { get { return connections; } }

		protected float output;
		protected List<Connection> connections;
		protected bool bias = false;

		public Neuron (float output = 0f, bool bias = false) {
			this.output = output;
			this.bias = bias;
			connections = new List<Connection>();
		}

		public void Input (float v) {
			this.output = v;
		}

		public void Compute () {
			if(bias) {
				// do nothing
			} else {
				float sum = 0f;
				float b = 0f;
				for(int i = 0, n = connections.Count; i < n; i++) {
					var con = connections[i];
					var from = con.From;
					var to = con.To;
					if(to == this) {
						if(from.bias) {
							b = from.Output * con.Weight;
						} else {
							sum += from.Output * con.Weight;
						}
					}
				}
				output = sigmoid(b + sum);
			}
		}

		protected float sigmoid(float x) {
			return 1f / (1f + Mathf.Exp(-x));
		}

		public void AddConnection(Connection con) {
			connections.Add(con);
		}

	}

}

