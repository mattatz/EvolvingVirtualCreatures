using UnityEngine;
using Random = UnityEngine.Random;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace mattatz.GeneticAlgorithm {

	public class Population {

		public int Generations { get { return generations; } }
		public List<Creature> Creatures { get { return creatures; } }

		public float mutationRate = 0.2f;
		int generations = 0;

		List<Creature> creatures = new List<Creature>();

		public Population (float rate = 0.2f) {
			mutationRate = Mathf.Clamp01(rate);
		}

		public void Setup () {
			creatures.ForEach(creature => {
				creature.Setup();
			});
		}

		public void Work (float dt) {
			creatures.ForEach(creature => {
				creature.Work(dt);
			});
		}

		public void ComputeFitness () {
			creatures.ForEach(creature => {
				creature.ComputeFitness();
			});
		}

		public void AddCreature (Creature c) {
			creatures.Add(c);
		}

		public List<Creature> Selection () {
			var pool = new List<Creature>();
			float maxFitness = GetMaxFitness();
			creatures.ForEach(c => {
				var fitness = c.Fitness / maxFitness; // normalize
				int n = Mathf.FloorToInt(fitness * 50);
				for(int j = 0; j < n; j++) {
					pool.Add(c);
				}
			});
			return pool;
		}

		public void Reproduction (Func<DNA, int, int, Creature> spawner, Vector2 range) {
			generations++;

			List<Creature> pool = Selection();
			if(pool.Count <= 0) {
				Debug.LogWarning("mating pool is empty.");
				return;
			}

            for(int i = 0, n = creatures.Count; i < n; i++) {
                // Sping the wheel of fortune to pick two parents
				int m = Random.Range(0, pool.Count);
				int d = Random.Range(0, pool.Count);

                // Pick two parents
                DNA mom = pool[m].DNA;
				DNA dad = pool[d].DNA;

                // Mate their genes
                DNA child = mom.Crossover(dad);

                // Mutate their genes
                child.Mutate(mutationRate, range);

                // Fill the new population with the new child
				creatures[i] = spawner(child, i, n);
            }
		}

		float GetMaxFitness () {
			float max = 0f;
			creatures.ForEach(creature => {
				var fitness = creature.Fitness;
				if(fitness > max) {
					max = fitness;
				}
			});
			return max;
		}

	}

}

