using System;
using SimpleJSON;
using UnityEngine;

namespace DD
{
	public enum VariableType
	{
		String = 0,
		Integer = 1,
		Bool = 2,
	}

	/// <summary>
	/// Holds data about a local variable in a dialogue.
	/// </summary>
	[Serializable]
	public class Variable : IJsonDeserializable
	{
		/// <summary>
		/// The name of the variable.
		/// </summary>
		[field: SerializeField]
		public string Name { get; private set; }

		/// <summary>
		/// The ID of the 'from' node.
		/// </summary>
		[field: SerializeField]
		public VariableType Type { get; private set; }

		[field: SerializeField]
		public string StringValue { get; private set; }

		[field: SerializeField]
		public int IntValue { get; private set; }

		[field: SerializeField]
		public bool BoolValue { get; private set; }

		public void Deserialize(JSONNode node)
		{
			Type = (VariableType)node.GetIntChild("type");
			switch (Type)
			{
				case VariableType.String:
					StringValue = node.GetStringChild("value");
					break;
				case VariableType.Integer:
					IntValue = node.GetIntChild("value");
					break;
				case VariableType.Bool:
					BoolValue = node.GetBoolChild("value");
					break;
				default:
					throw new NotImplementedException();
			}
		}

		internal void SetName(string name)
		{
			Name = name;
		}
	}
}
