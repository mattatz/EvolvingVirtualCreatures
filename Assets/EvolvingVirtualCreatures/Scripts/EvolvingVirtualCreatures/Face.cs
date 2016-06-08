using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using mattatz.Utils;

namespace mattatz.EvolvingVirtualCreatures {

	public class Face : MonoBehaviour {

		public Collider collider { get { return GetComponent<Collider>(); } }
		public bool Contact { 
			get { 
				// return contact; 
				return Collide();
			} 
		}

		public float Contact01 { get { return Contact ? 1f : 0f; } }

		Material material;
		bool contact = false;

		Plane floor;

		class Edge {
			public Vector3 v0, v1;
			public Edge (Vector3 v0, Vector3 v1) {
				this.v0 = v0;
				this.v1 = v1;
			}
		}

		List<Edge> edges;

		void Awake () {
			material = GetComponent<MeshRenderer>().material;
			floor = new Plane(Vector3.up, 0f);

			edges = new List<Edge>();

			var bounds = GetComponent<MeshFilter>().sharedMesh.bounds;
			Vector3[] corners = new Vector3[] {
				bounds.min,
				bounds.min + Vector3.right * bounds.size.x,
				bounds.max,
				bounds.max - Vector3.right * bounds.size.x
			};
			for(int i = 0, n = corners.Length; i < n; i++) {
				edges.Add(new Edge(corners[i], corners[(i + 1) % n]));
			}

		}

		void Update () {
		}

		bool Collide () {
			var found = edges.Find(edge => !floor.SameSide(transform.TransformPoint(edge.v0), transform.TransformPoint(edge.v1)));

			if(found != null) {
				material.SetColor("_Color", Color.red);
			} else {
				material.SetColor("_Color", Color.white);
			}

			return found != null;
		}

		void OnCollisionEnter (Collision collision) {
			contact = (collision.gameObject.layer == LayerMask.NameToLayer("Floor")); 
			material.SetColor("_Color", Color.red);
		}

		void OnCollisionExit (Collision collision) {
			contact = false;
			material.SetColor("_Color", Color.white);
		}

		void OnDrawGizmos () {
			/*
			if(edges == null) return;
			Gizmos.color = Color.red;
			for(int i = 0, n = edges.Count; i < n; i++) {
				var edge = edges[i];
				Gizmos.DrawLine(edge.v0, edge.v1);
			}
			*/
		}

		void OnDestroy () {
			Destroy(material);
		}

	}

}

