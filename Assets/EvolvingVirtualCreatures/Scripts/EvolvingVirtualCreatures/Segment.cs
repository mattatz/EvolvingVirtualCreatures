using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace mattatz.EvolvingVirtualCreatures {

	public class Segment : Part {

		public float Angle0 { 
			get {
				var parent = joint.connectedBody.transform;
				var center = parent.position;
				var anchor = transform.TransformPoint(joint.anchor);
				var dir = anchor - center;
				var axis = transform.TransformVector(joint.axis);
				return Vector3.Angle(dir, axis) / 180f;
			} 
		}

		public float Angle1 { 
			get {
				var parent = joint.connectedBody.transform;
				var center = parent.position;
				var anchor = transform.TransformPoint(joint.anchor);
				var dir = anchor - center;
				var axis = transform.TransformVector(joint.swingAxis);
				return Vector3.Angle(dir, axis) / 180f;
			} 
		}

		protected CharacterJoint joint;
		protected SideType side;
        protected float axisForce, swingAxisForce;

		// usage of CharacterJoint
		// http://d.hatena.ne.jp/hidetobara/20111005/1317841046
		CharacterJoint CreateJoint (float limit = 120f) {
			joint = gameObject.AddComponent<CharacterJoint>();

			var highTwistLimit = joint.highTwistLimit;
			highTwistLimit.limit = limit;
			joint.highTwistLimit = highTwistLimit;

			var lowTwistLimit = joint.lowTwistLimit;
			lowTwistLimit.limit = -limit;
			joint.lowTwistLimit = lowTwistLimit;

            var swing1Limit = joint.swing1Limit;
            swing1Limit.limit = limit;
            joint.swing1Limit = swing1Limit;

            var swing2Limit = joint.swing2Limit;
            swing2Limit.limit = limit;
            joint.swing2Limit = swing2Limit;

			joint.enableCollision = true;
			joint.enablePreprocessing = false;

			return joint;
		}

		public void Init (Rigidbody parent) {
			Body.velocity *= 0f;
			Body.angularVelocity *= 0f;
			transform.position = parent.transform.position + Helper.directions[side] * 1.25f;

			// need to add HingeJoint after setting position
			if(this.joint == null) {
				ActivateJoint(parent, side);
			}
		}

		public void ActivateJoint (Rigidbody parent, SideType side) {
			var joint = CreateJoint();
			joint.connectedBody = parent;
			joint.axis = Helper.directions[Helper.Axis(side)];
			joint.swingAxis = Helper.directions[Helper.SwingAxis(side)];
			joint.anchor = Helper.directions[Helper.Inverse(side)] * 0.5f;
		}

		public void Connect (Rigidbody parent, SideType side) {
			this.side = side;
			Init (parent);
		}

		public void Move (float axisForce, float swingAxisForce) {
			if(joint == null) return;

            this.axisForce = axisForce;
            this.swingAxisForce = swingAxisForce;
			
			// var axis = transform.TransformDirection(joint.axis);
			// Body.AddTorque(axis * axisForce);
			// Body.AddRelativeTorque(joint.axis * axisForce);

			// var swingAxis = transform.TransformDirection(joint.swingAxis);
			// Body.AddTorque(swingAxis * swingAxisForce);
			// Body.AddRelativeTorque(joint.swingAxis * swingAxisForce);
		}

        void FixedUpdate () {
			if(joint == null) return;

			Body.AddRelativeTorque(joint.axis * axisForce);
			Body.AddRelativeTorque(joint.swingAxis * swingAxisForce);
        }

		void OnCollisionEnter () {
		}

		void OnCollisionExit () {
		}

	}

}

