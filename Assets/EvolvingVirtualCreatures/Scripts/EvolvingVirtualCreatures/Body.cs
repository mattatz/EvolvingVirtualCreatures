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

	public class Body : Part {

		public Vector2 ForceRange { get { return forceRange; } }
		public List<Segment> Segments { get { return segments; } }

		[SerializeField] TextMesh scoreLabel;
		[SerializeField] Vector2 forceRange = new Vector2(-300f, 300f);

		List<Segment> segments = new List<Segment>();

		public void Init () {
			Body.velocity *= 0f;
			Body.angularVelocity *= 0f;
			segments.ForEach(s => {
				s.Init (Body);
			});
		}

		public void SetScore (float score) {
			scoreLabel.text = score.ToString();
		}

		public void AddSegment (Segment segment) {
			segments.Add(segment);
		}

		void OnDrawGizmos () {
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position + transform.forward);
		}

	}

}

