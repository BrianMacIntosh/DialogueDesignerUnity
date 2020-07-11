using SimpleJSON;
using UnityEngine;

namespace DD
{
	public class ShowMessageNodeSimple : ShowMessageNode
	{
		/// <summary>
		/// Identifier of the next node after this one.
		/// </summary>
		[field: SerializeField]
		public string Next { get; private set; }

		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);

			Next = node.GetStringChild("next");
		}

		/// <summary>
		/// Performs a user choice on this node, moving to the appropriate next node.
		/// </summary>
		public override void PerformChoice(DialoguePlayer state, int choice)
		{
			state.MoveToNode(Next);
		}
	}
}
