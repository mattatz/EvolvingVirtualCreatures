using UnityEngine;
using System.Collections;

namespace mattatz.EvolvingVirtualCreatures {

	[RequireComponent (typeof(Rigidbody))]
	public class Part : MonoBehaviour {

		public bool Contact { get { return contact; } } 

		public Rigidbody Body { 
			get { 
				if(body == null) {
					body = GetComponent<Rigidbody>();
				}
				return body;
			} 
		}

		private Rigidbody body;
		private bool contact;

		protected virtual void OnCollisionEnter (Collision collision) {
			contact = (collision.gameObject.layer == LayerMask.NameToLayer("Floor")); 
		}

		protected virtual void OnCollisionExit (Collision collision) {
			contact = false;
		}

	}

}

