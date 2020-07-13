using SimpleJSON;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Base class for a dialogue node that sets the value of a conversation local variable.
	/// </summary>
	public abstract class SetLocalVariableNode : BaseNodeSingleNext
	{
		/// <summary>
		/// The name of the variable to operate on.
		/// </summary>
		[SerializeField]
		public string VariableName { get; private set; }

		/// <summary>
		/// Fills this object's data from a <see cref="JSONNode"/>.
		/// </summary>
		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);
			
			VariableName = node.GetStringChild("var_name");
		}
	}
}
