using SimpleJSON;
using System;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Data about one possible response (choice) for a <see cref="ShowMessageNodeChoice"/>.
	/// </summary>
	[Serializable]
	public struct Choice : IJsonDeserializable
	{
		/// <summary>
		/// The conditional requirement for this choice.
		/// </summary>
		[field: SerializeField]
		public string Condition { get; private set; }

		/// <summary>
		/// Is this choice using the <see cref="Condition"/>>
		/// </summary>
		[field: SerializeField]
		public bool IsCondition { get; private set; }

		/// <summary>
		/// The ID of the node after this choice.
		/// </summary>
		[field: SerializeField]
		public string Next { get; private set; }

		/// <summary>
		/// The choice's text in all available languages.
		/// </summary>
		[field: SerializeField]
		public NodeText Text { get; private set; }

		/// <summary>
		/// Returns true if this choice is enabled.
		/// </summary>
		public bool IsEnabled(DialoguePlayer state)
		{
			return !IsCondition || state.EvaluateCondition(Condition);
		}

		/// <summary>
		/// Fills this object's data from a <see cref="JSONNode"/>.
		/// </summary>
		public void Deserialize(JSONNode node)
		{
			Condition = node.GetStringChild("condition");
			IsCondition = node.GetBoolChild("is_condition");
			Next = node.GetStringChild("next");
			Text = node.GetObjectChild<NodeText>("text");
		}
	}

	/// <summary>
	/// Dialogue node that shows a message and provides the player with a number of choices.
	/// </summary>
	public class ShowMessageNodeChoice : ShowMessageNode
	{
		/// <summary>
		/// The choices available on this node.
		/// </summary>
		[field: SerializeField]
		public Choice[] Choices { get; private set; }

		/// <summary>
		/// Returns the text of the specified choice in the specified language.
		/// </summary>
		public string GetChoiceText(int index, string language)
		{
			if (index < 0 || index >= Choices.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return Choices[index].Text.GetText(language, this);
		}

		/// <summary>
		/// Fills this object's data from a <see cref="JSONNode"/>.
		/// </summary>
		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);

			Choices = node.GetArrayChild<Choice>("choices");
		}

		/// <summary>
		/// Performs a user choice on this node, moving to the appropriate next node.
		/// </summary>
		public override void PerformChoice(DialoguePlayer state, int choice)
		{
			if (choice < 0 || choice >= Choices.Length)
			{
				throw new ArgumentOutOfRangeException("choice");
			}
			Choice choiceData = Choices[choice];
			state.MoveToNode(choiceData.Next);
		}
	}
}
