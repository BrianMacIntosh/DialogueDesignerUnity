using SimpleJSON;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Base class for a node in a dialogue.
	/// </summary>
	public abstract class BaseNode : ScriptableObject, IJsonDeserializable
	{
		/// <summary>
		/// The unique ID of this node.
		/// </summary>
		[field: SerializeField]
		public string NodeName { get; private set; }

		/// <summary>
		/// The node's title.
		/// </summary>
		[field: SerializeField]
		public string Title { get; private set; }

		public virtual void Deserialize(JSONNode node)
		{
			NodeName = node.GetStringChild("node_name");
			Title = node.GetStringChild("title");

			name = Title;
		}

		public string GetDebugName()
		{
			return string.Format("{0} ({1})", Title, NodeName);
		}

		/// <summary>
		/// Executes this node on the specified dialogue state.
		/// </summary>
		public abstract void PerformNode(DialoguePlayer state);
	}
}
