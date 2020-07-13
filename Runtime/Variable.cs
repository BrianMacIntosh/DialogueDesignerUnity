using System;
using SimpleJSON;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Enumeration of possible data types for conversation local variables.
	/// </summary>
	public enum VariableType
	{
		/// <summary>
		/// String variable.
		/// </summary>
		String = 0,

		/// <summary>
		/// Integer variable.
		/// </summary>
		Integer = 1,

		/// <summary>
		/// Boolean variable.
		/// </summary>
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

		/// <summary>
		/// If the variable is a <see cref="VariableType.String"/>, the value.
		/// </summary>
		[field: SerializeField]
		public string StringValue { get; private set; }

		/// <summary>
		/// If the variable is a <see cref="VariableType.Integer"/>, the value.
		/// </summary>
		[field: SerializeField]
		public int IntValue { get; private set; }

		/// <summary>
		/// If the variable is a <see cref="VariableType.Bool"/>, the value.
		/// </summary>
		[field: SerializeField]
		public bool BoolValue { get; private set; }

		/// <summary>
		/// Fills this object's data from a <see cref="JSONNode"/>.
		/// </summary>
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
