using SimpleJSON;
using UnityEngine;

namespace DD
{
	public abstract class SetLocalVariableNode : BaseNodeSingleNext
	{
		/// <summary>
		/// The name of the variable to operate on.
		/// </summary>
		[SerializeField]
		public string VariableName { get; private set; }

		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);
			
			VariableName = node.GetStringChild("var_name");
		}
	}
}
