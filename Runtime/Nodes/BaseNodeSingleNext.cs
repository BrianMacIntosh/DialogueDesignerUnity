using SimpleJSON;
using UnityEngine;

namespace DD
{
	public abstract class BaseNodeSingleNext : BaseNode
	{
		/// <summary>
		/// Identifier of the next node after this one.
		/// </summary>
		[field: SerializeField]
		public string Next
		{
			get; private set;
		}

		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);

			Next = node.GetStringChild("next");
		}

		/// <summary>
		/// Executes this node on the specified dialogue state.
		/// </summary>
		public override void PerformNode(DialoguePlayer state)
		{
			state.MoveToNode(Next);
		}
	}
}
