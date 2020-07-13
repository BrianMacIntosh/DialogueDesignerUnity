using SimpleJSON;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Dialogue node that branches to one of two possible next nodes based on a random chance.
	/// </summary>
	public class ChanceBranchNode : BaseNode
	{
		/// <summary>
		/// The chance of selecting branch 1. (0-1)
		/// </summary>
		[SerializeField]
		public float Chance1 { get; private set; }

		/// <summary>
		/// The next node if the condition result is 'false'.
		/// </summary>
		[field: SerializeField]
		public string Branch1 { get; private set; }

		/// <summary>
		/// The next node if the condition result is 'true'.
		/// </summary>
		[field: SerializeField]
		public string Branch2 { get; private set; }

		/// <summary>
		/// Fills this object's data from a <see cref="JSONNode"/>.
		/// </summary>
		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);

			Chance1 = node.GetFloatChild("chance_1") / 100f;

			JSONNode branchesNode = node["branches"];
			if (branchesNode == null)
			{
				Debug.LogErrorFormat("JSON deserialization: expected 'branches' to be Object.");
			}
			else
			{
				Branch1 = branchesNode.GetStringChild("1");
				Branch2 = branchesNode.GetStringChild("2");
			}
		}

		/// <summary>
		/// Executes this node on the specified dialogue state.
		/// </summary>
		public override void PerformNode(DialoguePlayer state)
		{
			if (state.RandomFloat() < Chance1)
			{
				state.MoveToNode(Branch1);
			}
			else
			{
				state.MoveToNode(Branch2);
			}
		}
	}
}
