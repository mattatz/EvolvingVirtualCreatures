using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using mattatz.GeneticAlgorithm;
using mattatz.NeuralNetworks;

using mattatz.Utils;

namespace mattatz.EvolvingVirtualCreatures {

	public class JointFactory : MonoBehaviour {

		Population population;

		[SerializeField] Text generationLabel;
		[SerializeField] Text automationLabel;

		[SerializeField] float size = 5f;
		[SerializeField] int count = 50;
		[SerializeField] Vector3 offset = Vector3.up;

		[SerializeField] GameObject scoreLabelPrefab;
		[SerializeField] GameObject segmentPrefab;

		[SerializeField, Range(0.0f, 1.0f)] float mutationRate = 0.2f;

        [SerializeField] float wps = 30f; // working per seconds

		[SerializeField] bool automatic = true;
		[SerializeField] float automaticInterval = 40f;

		[SerializeField] string fileName = "DNA";

		Coroutine routine;
		bool stopping = false;

		Dictionary<Creature, TextMesh> scoreLabels = new Dictionary<Creature, TextMesh>();
		Node root;

		void Start () {
			// Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Creature"), LayerMask.NameToLayer("Creature"));

			population = new Population(mutationRate);

			Setup();

			wps = Mathf.Max(10f, wps);

			float dt = 1f / wps;
            StartCoroutine(Repeat(dt, () => {
				if(!stopping) {
					population.Work(dt);
					population.ComputeFitness();
				}
            }));

			if(automatic) {
				Automation();
			}
		}

		public void Setup (DNA dna = null) {

			root = new Node(Vector3.one * 0.5f);
			var leftArm0 = new Node(new Vector3(0.8f, 0.2f, 0.2f));
			var rightArm0 = new Node(new Vector3(0.8f, 0.2f, 0.2f));

			var leftArm1 = new Node(new Vector3(0.6f, 0.15f, 0.15f));
			var rightArm1 = new Node(new Vector3(0.6f, 0.15f, 0.15f));

			root.Connect(leftArm0, SideType.Left);
			root.Connect(rightArm0, SideType.Right);

			leftArm0.Connect(leftArm1, SideType.Left);
			rightArm0.Connect(rightArm1, SideType.Right);

			int hcount = Mathf.FloorToInt(count * 0.5f);
			for(int i = 0; i < count; i++) {
				var position = new Vector3((i - hcount) * size, 0f, 0f) + offset;
				JointSampleCreature creature = CreateCreature(i.ToString(), root, position, dna);
				population.AddCreature(creature);
			}

			population.Setup();


		}

		void Update () {
			foreach(Creature c in scoreLabels.Keys) {
				var label = scoreLabels[c];
				label.text = c.Fitness.ToString();
			}
		}

        IEnumerator Repeat(float interval, Action action) {
            yield return 0;
			while(true) {
				yield return new WaitForSeconds(interval);
                action();
			}
        }

		public void Reproduction () {
			population.ComputeFitness();

			var ancestors = new List<Creature>(population.Creatures);
			int hcount = Mathf.FloorToInt(ancestors.Count * 0.5f);

			scoreLabels.Clear();

			population.mutationRate = mutationRate;
			population.Reproduction((DNA dna, int index, int count) => {
				var position = new Vector3((index - hcount) * size, 0f, 0f) + offset;
				return CreateCreature(index.ToString(), root, position, dna);
			}, new Vector2(-1f, 1f));

			ancestors.ForEach(ancestor => {
				(ancestor as JointSampleCreature).Destroy();
			});

			population.Setup();
			generationLabel.text = population.Generations.ToString();
		}

		void Clear () {
			var creatures = population.Creatures;
			creatures.ForEach(c => {
				(c as JointSampleCreature).Destroy();
			});
			creatures.Clear();
			scoreLabels.Clear();
		}

		public void Automation () {
			if(routine != null) { 
				automationLabel.text = "Automation";
				StopCoroutine(routine); 
				routine = null;
			} else {
				automationLabel.text = "Manual";
				routine = StartCoroutine(Repeat(automaticInterval, () => {
					if(!stopping) Reproduction();
				}));
			}
		}

		public void Stop () {
			stopping = !stopping;
			if(stopping) {
				population.Creatures.ForEach(c => c.Sleep());
			} else {
				population.Creatures.ForEach(c => c.WakeUp());
			}
		}

		public void Save () {

			var creatures = population.Creatures;
			var c = creatures.First();
			var max = c.ComputeFitness();
			for(int i = 1, n = creatures.Count; i < n; i++) {
				var c2 = creatures[i];
				var fitness = c2.ComputeFitness();
				if(fitness > max) {
					c = c2;
					max = fitness;
				}
			}

			var dna = c.DNA;

			string path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName + ".csv");

			FileInfo fi = new FileInfo(path);
			StreamWriter sw = fi.CreateText();
			sw.WriteLine(string.Join(",", dna.genes.Select(f => f.ToString()).ToArray()));
			sw.Flush();
			sw.Close();
		}

		public void Load () {
			string path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName + ".csv");
			string text = System.IO.File.ReadAllText(path);
			float[] genes = text.Split(',').Select(str => float.Parse(str)).ToArray();

			Clear();

			var dna = new DNA(genes);
			Setup (dna);
		}

		JointSampleCreature CreateCreature (string label, Node root, Vector3 position, DNA dna = null) {

			var group = new GameObject("Creature");
			var body = Build (group.transform, root);
			group.transform.position = position;

			JointSampleCreature creature;
			if(dna == null) {
				creature = new JointSampleCreature(body);
			} else {
				creature = new JointSampleCreature(body, dna);
			}

			var tm = Instantiate(scoreLabelPrefab).GetComponent<TextMesh>();
			tm.transform.parent = creature.Body.transform;
			tm.transform.localPosition = Vector3.up;
			scoreLabels.Add(creature, tm);

			return creature;
		}

		public JointSegment Build (Transform group, Node root) {
			var parent = Instantiate(segmentPrefab).GetComponent<JointSegment>();
			parent.transform.parent = group;
			parent.transform.localScale = root.Size;
			var cons = root.GetConnections();
			for(int i = 0, n = cons.Length; i < n; i++) {
				var con = cons[i];
				Build (group, parent, con.Side, con.To);
			}
			return parent;
		}

		void Build (Transform group, JointSegment parentSegment, SideType side, Node childNode) {
			var cur = Instantiate(segmentPrefab).GetComponent<JointSegment>();
			cur.transform.parent = group;
			cur.transform.localScale = childNode.Size;
			cur.Connect(parentSegment, side);
			var cons = childNode.GetConnections();
			for(int i = 0, n = cons.Length; i < n; i++) {
				var con = cons[i];
				Build (group, cur, con.Side, con.To);
			}
		}

		public void OnTimeScaleChanged (float scale) {
			Time.timeScale = scale;
		}

		void OnDrawGizmos () {
			if(!Application.isPlaying) return;
			population.Creatures.ForEach(c => c.DrawGizmos());
		}

	}

}

