using UnityEngine;
using System.Collections;

namespace mattatz.EvolvingVirtualCreatures {

	[RequireComponent (typeof(Rigidbody))]
	public class Part : MonoBehaviour {

		public bool[] Contacts { 
			get { 
				return new bool[] { front.Contact, back.Contact, left.Contact, right.Contact, up.Contact, down.Contact };
			} 
		}

		[SerializeField] Face front, back, left, right, up, down;

		public Rigidbody Body { 
			get { 
				if(body == null) {
					body = GetComponent<Rigidbody>();
				}
				return body;
			} 
		}

		private Rigidbody body;

	}

}

