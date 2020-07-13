using SimpleJSON;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Dialogue node that branches to a random next node with even probability.
	/// </summary>
	public class RandomBranchNode : BaseNode
	{
		/// <summary>
		/// The set of possible next nodes.
		/// </summary>
		[field: SerializeField]
		public string[] Branches { get; private set; }

		/// <summary>
		/// Fills this object's data from a <see cref="JSONNode"/>.
		/// </summary>
		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);

			int possibilities = node.GetIntChild("possibilities");

			JSONNode branchesNode = node["branches"];
			if (branchesNode == null)
			{
				Debug.LogErrorFormat("JSON deserialization: expected 'branches' to be Object.");
			}
			else
			{
				Branches = new string[possibilities];
				for (int i = 0; i < possibilities; i++)
				{
					Branches[i] = branchesNode.GetStringChild((i + 1).ToString());
				}
			}
		}

		/// <summary>
		/// Executes this node on the specified dialogue state.
		/// </summary>
		public override void PerformNode(DialoguePlayer state)
		{
			int randomIndex = state.RandomInt(Branches.Length);
			state.MoveToNode(Branches[randomIndex]);
		}
	}
}
