using UnityEngine;
using Random = UnityEngine.Random;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using mattatz.GeneticAlgorithm;
using mattatz.NeuralNetworks;
using Network = mattatz.NeuralNetworks.Network;

namespace mattatz.EvolvingVirtualCreatures {

	public class SampleCreature : Creature {

		public Segment Body { get { return body; } }

		protected Network network;

		protected Segment body;
		protected List<Sensor> sensors = new List<Sensor>();
		protected List<Effector> effectors = new List<Effector>();

		protected Vector3 target;
		protected float distance = 50f;
		protected Vector3 origin;
		protected Vector3 forward;

		public SampleCreature (Segment body) {
			this.target = body.transform.position + body.transform.forward * distance;

			GetAllSegments(body);

			this.body = body;
			this.dna = new DNA(GetGenesCount(), new Vector2(-1f, 1f));
			network = new Network(GetLayersCount());
		}

		public SampleCreature (Segment body, DNA dna) {
			this.target = body.transform.position + body.transform.forward * distance;

			GetAllSegments(body);

			this.body = body;
			this.dna = dna;
			network = new Network(GetLayersCount(), this.dna.genes);
		}

		public override void Setup() {
			base.Setup();

			origin = body.transform.position;
			forward = body.transform.forward;
		}

		public override Creature Generate(DNA dna) {
			body.transform.position = origin;
			body.transform.localRotation = Quaternion.identity;
			body.Init();
			return new SampleCreature(body, dna);
		}

		public override void Work (float dt) {

			// Sensing
			var inputs = new List<float>();
			sensors.ForEach(sensor => {
				inputs.AddRange(sensor.Output());
			});

			float[] output = network.FeedForward(inputs.ToArray());

			int offset = 0;
			effectors.ForEach(effector => {
				int inputCount = effector.InputCount();
				float[] input = new float[inputCount];
				Array.Copy(output, offset, input, 0, inputCount);
				effector.Affect(input, dt);
				offset += inputCount;
			});

		}

		public override float ComputeFitness () {
			/*
			var d = distance / (body.transform.position - target).magnitude;
			if(d <= 1f) {
				fitness = 0f;
			} else {
				fitness = Mathf.Pow (d, 2f);
			}
			*/

			fitness = (body.transform.position - origin).magnitude;
			return fitness;
		}

		public override int GetGenesCount () {
			var layersCount = GetLayersCount();
			int count = 0;
			for(int i = 0, n = layersCount.Length; i < n - 1; i++) {
				count += layersCount[i] * layersCount[i + 1];
			}
			return count;
		}

		void GetAllSegments (Segment root) {
			// sensors.Add(new JointSensor(root.transform));
			sensors.Add(new AngleSensor(root));
			sensors.Add(new ContactSensor(root));
			// sensors.Add(new DirectionSensor(root.transform, target));

			effectors.Add(root);

			root.Segments.ForEach(child => {
				GetAllSegments(child);
			});
		}

		public int[] GetLayersCount () {
			// target direction eular angles(x,y,z) and each segments rotation eular angles(x,y,z) and contact states(forward,back,right,left,up,down) for each faces
			// var inputLayer = count * (3 + 6);
			var inputLayer = sensors.Aggregate(0, (prod, next) => prod + next.OutputCount());

			const int hiddenDepth = 4;
			var hiddenLayer = inputLayer;

			// (each segments - body) axis forces and swing axis forces
			var outputLayer = effectors.Aggregate(0, (prod, next) => prod + next.InputCount());

			int[] layersCount = new int[1 + hiddenDepth + 1];
			layersCount[0] = inputLayer;
			for(int i = 1; i < hiddenDepth + 1; i++) {
				layersCount[i] = hiddenLayer;
			}
			layersCount[layersCount.Length - 1] = outputLayer;

			return layersCount;
		}

		public override void WakeUp() {
			body.WakeUp();
		}

		public override void Sleep() {
			body.Sleep();
		}

		public override void DrawGizmos () {
			Gizmos.color = Color.red;
			Gizmos.DrawLine(body.transform.position, target);
			Gizmos.DrawWireSphere(target, 1f);
		}

	}

}

