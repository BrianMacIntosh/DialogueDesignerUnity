using SimpleJSON;
using UnityEngine;

namespace DD
{
	public class RepeatNode : BaseNode
	{
		/// <summary>
		/// The node to repeat.
		/// </summary>
		[field: SerializeField]
		public string Next { get; private set; }

		/// <summary>
		/// The next node after repeating is finished.
		/// </summary>
		[field: SerializeField]
		public string NextDone { get; private set; }

		/// <summary>
		/// The number of times to repeat.
		/// </summary>
		[field: SerializeField]
		public int Value { get; private set; }

		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);

			Next = node.GetStringChild("next");
			NextDone = node.GetStringChild("next_done");
			Value = node.GetIntChild("value");
		}

		/// <summary>
		/// Executes this node on the specified dialogue state.
		/// </summary>
		public override void PerformNode(DialoguePlayer state)
		{
			state.PushRepeatNode(this);
		}
	}
}
