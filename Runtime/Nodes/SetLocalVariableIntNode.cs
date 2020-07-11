using SimpleJSON;
using System;
using UnityEngine;

namespace DD
{
	public enum VariableOperationType
	{
		Set,
		Add,
		Subtract,
	}

	public class SetLocalVariableIntNode : SetLocalVariableNode
	{
		/// <summary>
		/// The operation to perform on the variable.
		/// </summary>
		[SerializeField]
		public VariableOperationType OperationType { get; private set; }

		/// <summary>
		/// The operation value.
		/// </summary>
		[SerializeField]
		public int Value { get; private set; }

		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);

			switch (node.GetStringChild("operation_type"))
			{
				case "ADD":
					OperationType = VariableOperationType.Add;
					break;
				case "SUB":
					OperationType = VariableOperationType.Subtract;
					break;
				case "SET":
					OperationType = VariableOperationType.Set;
					break;
			}
			Value = node.GetIntChild("value");
		}

		/// <summary>
		/// Executes this node on the specified dialogue state.
		/// </summary>
		public override void PerformNode(DialoguePlayer state)
		{
			int value;
			if (state.IntVariables.TryGetValue(VariableName, out value))
			{
				switch (OperationType)
				{
					case VariableOperationType.Set:
						state.IntVariables[VariableName] = Value;
						break;
					case VariableOperationType.Add:
						state.IntVariables[VariableName] += Value;
						break;
					case VariableOperationType.Subtract:
						state.IntVariables[VariableName] -= Value;
						break;
					default:
						throw new NotImplementedException(string.Format("VariableOperationType.{0}", OperationType.ToString()));
				}
			}
			else
			{
				Debug.LogWarningFormat("Set Local Variable: node {0} has no int-typed variable '{1}'.", GetDebugName(), VariableName);
			}

			base.PerformNode(state);
		}
	}
}
