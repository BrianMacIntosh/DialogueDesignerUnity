using SimpleJSON;
using UnityEngine;

namespace DD
{
	public class ExecuteNode : BaseNodeSingleNext
	{
		/// <summary>
		/// The text of the script to execute.
		/// </summary>
		[SerializeField]
		public string Text { get; private set; }

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
