using UnityEngine;
using System.Collections;

namespace mattatz.EvolvingVirtualCreatures {

	public class Face : MonoBehaviour {

		public bool Contact { get { return contact; } }

		bool contact = false;

		protected virtual void OnCollisionEnter (Collision collision) {
			contact = (collision.gameObject.layer == LayerMask.NameToLayer("Floor")); 
		}

		protected virtual void OnCollisionExit (Collision collision) {
			contact = false;
		}

	}

}

