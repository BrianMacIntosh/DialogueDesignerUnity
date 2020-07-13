using SimpleJSON;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Dialogue node that sets the value of a local Boolean variable.
	/// </summary>
	public class SetLocalVariableBoolNode : SetLocalVariableNode
	{
		/// <summary>
		/// If set, toggles the variable.
		/// </summary>
		[SerializeField]
		public bool Toggle { get; private set; }

		/// <summary>
		/// If <see cref="Toggle"/> is disabled, the value to set the variable to.
		/// </summary>
		[SerializeField]
		public bool Value { get; private set; }

		/// <summary>
		/// Fills this object's data from a <see cref="JSONNode"/>.
		/// </summary>
		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);
			
			Toggle = node.GetBoolChild("toggle");
			Value = node.GetBoolChild("value");
		}

		/// <summary>
		/// Executes this node on the specified dialogue state.
		/// </summary>
		public override void PerformNode(DialoguePlayer state)
		{
			bool value;
			if (state.BoolVariables.TryGetValue(VariableName, out value))
			{
				if (Toggle)
				{
					state.BoolVariables[VariableName] = !value;
				}
				else
				{
					state.BoolVariables[VariableName] = Value;
				}
			}
			else
			{
				Debug.LogWarningFormat("Set Local Variable: node {0} has no bool-typed variable '{1}'.", GetDebugName(), VariableName);
			}

			base.PerformNode(state);
		}
	}
}
