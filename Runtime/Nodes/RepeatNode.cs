using SimpleJSON;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Dialogue node that repeats a next node a set number of times, then moves to a second next node.
	/// </summary>
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

		/// <summary>
		/// Fills this object's data from a <see cref="JSONNode"/>.
		/// </summary>
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
