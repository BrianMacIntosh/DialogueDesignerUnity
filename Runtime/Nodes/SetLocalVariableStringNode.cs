﻿using SimpleJSON;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Dialogue node that sets the value of a string local variable.
	/// </summary>
	public class SetLocalVariableStringNode : SetLocalVariableNode
	{
		/// <summary>
		/// The value to set the variable to.
		/// </summary>
		[SerializeField]
		public string Value { get; private set; }

		/// <summary>
		/// Fills this object's data from a <see cref="JSONNode"/>.
		/// </summary>
		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);
			
			Value = node.GetStringChild("value");
		}

		/// <summary>
		/// Executes this node on the specified dialogue state.
		/// </summary>
		public override void PerformNode(DialoguePlayer state)
		{
			if (state.StringVariables.ContainsKey(VariableName))
			{
				state.StringVariables[VariableName] = Value;
			}
			else
			{
				Debug.LogWarningFormat("Set Local Variable: node {0} has no string-typed variable '{1}'.", GetDebugName(), VariableName);
			}

			base.PerformNode(state);
		}
	}
}
