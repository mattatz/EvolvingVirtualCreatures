using UnityEngine;
using Random = UnityEngine.Random;

using System.Collections;
using mattatz.Utils;

namespace mattatz.GeneticAlgorithm {

	public class DNA {

		public float[] genes;

		public DNA (int n, Vector2 range) {
			genes = new float[n];
			for(int i = 0; i < n; i++) {
				genes[i] = Random.Range(range.x, range.y);
			}
		}

		public DNA (float[] genes) {
			this.genes = genes;
		}

		// CROSSOVER
		// Creates new DNA sequence from two (this & and a partner)
		public DNA Crossover(DNA partner) {
			float[] child = new float[genes.Length];

			int mid = Mathf.FloorToInt(Random.value * genes.Length);

			// Take "half" from one and "half" from the other
			for (int i = 0, n = genes.Length; i < n; i++) {
				if (i > mid) {
					child[i] = genes[i];
				} else {
					child[i] = partner.genes[i];
				}
			}    

			return new DNA(child);
		}
		
		// Based on a mutation probability, picks a new random value
		public void Mutate (float m, Vector2 range) {
			// var c = (range.y - range.x) * 0.5f + range.x;
			for (int i = 0, n = genes.Length; i < n; i++) {
				if (Random.value < m) {
					// genes[i] += Gaussian.Std(c, 0.25f);
					genes[i] += Gaussian.Std(0f, 0.75f);
				}
			}
		}

	}
	
}

