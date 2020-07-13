using SimpleJSON;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Dialogue node that branches to one of two next nodes based on the result of a condition.
	/// </summary>
	public class ConditionBranchNode : BaseNode
	{
		/// <summary>
		/// The text of the condition to check.
		/// </summary>
		[SerializeField]
		public string Text { get; private set; }

		/// <summary>
		/// The next node if the condition result is 'false'.
		/// </summary>
		[field: SerializeField]
		public string BranchFalseNext { get; private set; }

		/// <summary>
		/// The next node if the condition result is 'true'.
		/// </summary>
		[field: SerializeField]
		public string BranchTrueNext { get; private set; }

		/// <summary>
		/// Fills this object's data from a <see cref="JSONNode"/>.
		/// </summary>
		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);

			Text = node.GetStringChild("text");

			JSONNode branchesNode = node["branches"];
			if (branchesNode == null)
			{
				Debug.LogErrorFormat("JSON deserialization: expected 'branches' to be Object.");
			}
			else
			{
				BranchFalseNext = branchesNode.GetStringChild("False");
				BranchTrueNext = branchesNode.GetStringChild("True");
			}
		}

		/// <summary>
		/// Executes this node on the specified dialogue state.
		/// </summary>
		public override void PerformNode(DialoguePlayer state)
		{
			if (state.EvaluateCondition(Text))
			{
				state.MoveToNode(BranchTrueNext);
			}
			else
			{
				state.MoveToNode(BranchFalseNext);
			}
		}
	}
}
