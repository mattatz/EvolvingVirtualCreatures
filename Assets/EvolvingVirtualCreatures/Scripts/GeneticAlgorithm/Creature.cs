using UnityEngine;
using System.Collections;

namespace mattatz.GeneticAlgorithm {

	public abstract class Creature {

		public float Fitness { get { return fitness; } }
		public DNA DNA { get { return dna; } }

		protected DNA dna;
		protected float fitness;

		public Creature () {}

		public Creature (DNA dna) {
			this.dna = dna;
		}

		public virtual void Setup() {
			// do nothing
		}

		public abstract Creature Generate(DNA dna);
		public abstract void Work();
		public abstract float ComputeFitness ();
		public abstract int GetGenesCount ();

	}

}

