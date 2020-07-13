using SimpleJSON;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Dialogue node that waits for a specified number of seconds before advancing to the next node.
	/// </summary>
	public class WaitNode : BaseNodeSingleNext
	{
		/// <summary>
		/// The time to wait for, in seconds.
		/// </summary>
		[SerializeField]
		public float Time { get; private set; }

		/// <summary>
		/// Fills this object's data from a <see cref="JSONNode"/>.
		/// </summary>
		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);

			Time = node.GetFloatChild("time");
		}

		/// <summary>
		/// Executes this node on the specified dialogue state.
		/// </summary>
		public override void PerformNode(DialoguePlayer state)
		{
			// handled by DialoguePlayer.Update
		}
	}
}
