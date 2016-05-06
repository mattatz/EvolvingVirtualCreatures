using UnityEngine;
using Random = UnityEngine.Random;

using System;
using System.Collections;
using System.Collections.Generic;

using mattatz.GeneticAlgorithm;
using mattatz.NeuralNetworks;
using Network = mattatz.NeuralNetworks.Network;

namespace mattatz.EvolvingVirtualCreatures {

	public class SampleCreature : Creature {

		public Body Body { get { return body; } }

		protected Network network;
		protected Body body;

		protected Vector3 origin;
		protected Vector3 forward;

		protected Vector3 prev;
		protected float distance;

		public SampleCreature (Body body) { 
			this.body = body;
			this.dna = new DNA(GetGenesCount(), new Vector2(-1f, 1f));
			network = new Network(GetLayersCount());
		}

		public SampleCreature (Body body, DNA dna) {
			// each segment using dna.genes length is segments.Count * 2 + 1 (input layer) plus segments.Count (hiden layer)
			// input layer count : 
			// each segment's angles (0.0 ~ 360.0 ?) and body and segments contact state (0.0 or 1.0)
			this.body = body;
			this.dna = dna;
			network = new Network(GetLayersCount(), this.dna.genes);
		}

		public override void Setup() {
			base.Setup();

			origin = body.transform.position;
			forward = body.transform.forward;

			prev = origin;
			prev.y = 0f;
			distance = 0f;
		}

		public override Creature Generate(DNA dna) {
			body.transform.position = origin;
			body.transform.localRotation = Quaternion.identity;
			body.Init();
			return new SampleCreature(body, dna);
		}

		public override void Work () {

			var inputs = new List<float>();

			body.Segments.ForEach(segment => {
				var ang = segment.transform.localRotation.eulerAngles / 360f;
				inputs.Add(ang.x);
				inputs.Add(ang.y);
				inputs.Add(ang.z);
				inputs.Add(segment.Contact ? 1f : 0f);
			});

			// inputs.Add(body.Contact ? 1f : 0f);
			var contacts = body.Contacts;
			for(int i = 0, n = contacts.Length; i < n; i++) {
				inputs.Add(contacts[i] ? 1f : 0f);
			}

			var angles = body.transform.localRotation.eulerAngles / 360f;
			inputs.Add(angles.x);
			inputs.Add(angles.y);
			inputs.Add(angles.z);

			// CAUTION:
			// FeedForward inputs count is not equal to input layer neurons count.
			// because of bias input neuron.
			var output = network.FeedForward(inputs.ToArray());

			// output includes each segment' axis force and swing axis force
			Debug.Assert(output.Length == (Body.Segments.Count * 2));
			for(int i = 0, n = Body.Segments.Count; i < n; i++) {
				var axisForce = output[i];
				var swingAxisForce = output[i + n];
				axisForce = Mathf.Lerp (body.ForceRange.x, body.ForceRange.y, axisForce); // map 0.0 ~ 1.0 value to force min max
				swingAxisForce = Mathf.Lerp (body.ForceRange.x, body.ForceRange.y, swingAxisForce); // map 0.0 ~ 1.0 value to force min max
				body.Segments[i].Move(axisForce, swingAxisForce);
			}

			var cur = body.transform.position;
			cur.y = 0f;

			distance += (cur - prev).magnitude;

			prev = cur;
		}

		public override float ComputeFitness () {
			var dir = body.transform.position - origin;
			var proj = Vector3.Project(dir, forward);

			fitness = Mathf.Pow(Mathf.Max (0f, proj.magnitude), 2.0f);
			body.SetScore(fitness);

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

		public int[] GetLayersCount () {
			// each segments rotation eular angles and contact state +
			// body contact states for each faces (front, back, left, right, up, down) + 
			// body xyz euler angles
			// var inputLayer = body.Segments.Count * 3 + 6 + 3;
			var inputLayer = body.Segments.Count * 4 + 6 + 3;

			const int hiddenDepth = 2;
			var hiddenLayer = body.Segments.Count * 10;

			// each segments axis forces and swing axis forces
			var outputLayer = body.Segments.Count * 2;

			int[] layersCount = new int[1 + hiddenDepth + 1];
			layersCount[0] = inputLayer;
			for(int i = 1; i < hiddenDepth + 1; i++) {
				layersCount[i] = hiddenLayer;
			}
			layersCount[layersCount.Length - 1] = outputLayer;

			return layersCount;
		}

	}

}

