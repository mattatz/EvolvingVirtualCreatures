using UnityEngine;
using Random = UnityEngine.Random;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace mattatz.EvolvingVirtualCreatures {

	public enum SideType {
		Front 	= 1 << 0,
		Back 	= 1 << 1,
		Left 	= 1 << 2,
		Right 	= 1 << 3,
		Up 		= 1 << 4,
		Down 	= 1 << 5
	};

	public class Helper {

		public static Dictionary<SideType, Vector3> directions = new Dictionary<SideType, Vector3>() {
			{ SideType.Front, Vector3.forward },
			{ SideType.Back, Vector3.back },
			{ SideType.Left, Vector3.left },
			{ SideType.Right, Vector3.right },
			{ SideType.Up, Vector3.up },
			{ SideType.Down, Vector3.down },
		};

		public static SideType Axis (SideType type) {
			switch(type) {

			case SideType.Front:
				return SideType.Up;
			case SideType.Back:
				return SideType.Down;

			case SideType.Left:
				return SideType.Up;
			case SideType.Right:
				return SideType.Down;

			case SideType.Up:
				return SideType.Front;
			case SideType.Down:
				return SideType.Back;

			}
			return SideType.Up;
		}

		public static SideType SwingAxis (SideType type) {
			switch(type) {

			case SideType.Front:
				return SideType.Left;
			case SideType.Back:
				return SideType.Right;

			case SideType.Left:
				return SideType.Front;
			case SideType.Right:
				return SideType.Back;

			case SideType.Up:
				return SideType.Left;
			case SideType.Down:
				return SideType.Right;

			}
			return SideType.Up;
		}

		public static SideType Inverse (SideType type) {
			switch(type) {
			case SideType.Front:
				return SideType.Back;
			case SideType.Back:
				return SideType.Front;
			case SideType.Left:
				return SideType.Right;
			case SideType.Right:
				return SideType.Left;
			case SideType.Up:
				return SideType.Down;
			case SideType.Down:
				return SideType.Up;
			}

			return SideType.Front;
		}

	}

}

