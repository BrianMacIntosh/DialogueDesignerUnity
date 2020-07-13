using SimpleJSON;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Dialogue node that executes a script and moves to the next node.
	/// </summary>
	public class ExecuteNode : BaseNodeSingleNext
	{
		/// <summary>
		/// The text of the script to execute.
		/// </summary>
		[SerializeField]
		public string Text { get; private set; }

		/// <summary>
		/// Fills this object's data from a <see cref="JSONNode"/>.
		/// </summary>
		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);

			Text = node.GetStringChild("text");
		}

		/// <summary>
		/// Executes this node on the specified dialogue state.
		/// </summary>
		public override void PerformNode(DialoguePlayer state)
		{
			state.ExecuteScript(Text);

			base.PerformNode(state);
		}
	}
}
