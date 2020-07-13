using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Enumeration defining who the speaker is for a particular node.
	/// </summary>
	public enum SpeakerType
	{
		/// <summary>
		/// Use <see cref="ShowMessageNode.Character"/> to find the character.
		/// </summary>
		Character,

		/// <summary>
		/// Use <see cref="ShowMessageNode.ObjectPath"/> to find the character.
		/// </summary>
		CustomPath,

		/// <summary>
		/// Use the character interacted with.
		/// </summary>
		InteractedCharacter,
	}

	/// <summary>
	/// A text string in a single language.
	/// </summary>
	[Serializable]
	public struct LanguageText
	{
		/// <summary>
		/// The language of this text.
		/// </summary>
		[field: SerializeField]
		public string Language { get; private set; }

		/// <summary>
		/// The text.
		/// </summary>
		[field: SerializeField]
		public string Text { get; private set; }

		/// <summary>
		/// Constructs a new language-text pair.
		/// </summary>
		public LanguageText(string language, string text)
		{
			Language = language;
			Text = text;
		}
	}

	/// <summary>
	/// A text string in multiple languages.
	/// </summary>
	[Serializable]
	public struct NodeText : IJsonDeserializable, ISerializationCallbackReceiver
	{
		/// <summary>
		/// The text in each language it's available in.
		/// </summary>
		/// <remarks>Same content as <see cref="TextsDictionary"/>.</remarks>
		[field: SerializeField]
		public LanguageText[] LanguageTexts { get; private set; }

		/// <summary>
		/// A dictionary mapping each language code to its text.
		/// </summary>
		/// <remarks>Same content as <see cref="LanguageTexts"/>.</remarks>
		[field: NonSerialized]
		public Dictionary<string, string> TextsDictionary { get; private set; }

		/// <summary>
		/// Fills this object's data from a <see cref="JSONNode"/>.
		/// </summary>
		public void Deserialize(JSONNode node)
		{
			JSONObject textObjectNode = node as JSONObject;
			if (textObjectNode == null)
			{
				Debug.LogError("JSON deserialization: expected 'text' to be Object.");
			}
			else
			{
				//NOTE: garbage allocated
				List<LanguageText> textBuffer = new List<LanguageText>();
				foreach (string textKey in textObjectNode.Keys)
				{
					string text = textObjectNode.GetStringChild(textKey);
					textBuffer.Add(new LanguageText(textKey, text));
				}
				LanguageTexts = textBuffer.ToArray();
				OnAfterDeserialize();
			}
		}

		/// <summary>
		/// Gets the text for the specified language.
		/// </summary>
		public string GetText(string language, BaseNode requesterNode)
		{
			string text;
			if (TextsDictionary.TryGetValue(language, out text))
			{
				return text;
			}
			else
			{
				string errorString = string.Format("Node {0} missing text for language '{1}'.", requesterNode.GetDebugName(), language);
				Debug.LogError(errorString, requesterNode);
				return errorString;
			}
		}

		/// <summary>
		/// Unity serialization callback (do not call).
		/// </summary>
		public void OnBeforeSerialize()
		{

		}

		/// <summary>
		/// Unity serialization callback (do not call).
		/// </summary>
		public void OnAfterDeserialize()
		{
			TextsDictionary = new Dictionary<string, string>(LanguageTexts.Length, StringComparer.InvariantCultureIgnoreCase);
			foreach (LanguageText textPair in LanguageTexts)
			{
				TextsDictionary.Add(textPair.Language, textPair.Text);
			}
		}
	}

	/// <summary>
	/// Base class for a dialogue node that displays message text to the player.
	/// </summary>
	public abstract class ShowMessageNode : BaseNode
	{
		/// <summary>
		/// The node's speaker, if <see cref="SpeakerType"/> is <see cref="SpeakerType.Character"/>.
		/// </summary>
		[field: SerializeField]
		public string Character { get; private set; }

		/// <summary>
		/// Should this node use a box display (true) or bubble display (false)?
		/// </summary>
		[field: SerializeField]
		public bool IsBox { get; private set; }

		/// <summary>
		/// If the <see cref="SpeakerType"/> is <see cref="SpeakerType.CustomPath"/>, the path to the speaker.
		/// </summary>
		[field: SerializeField]
		public string ObjectPath { get; private set; }

		/// <summary>
		/// Should the camera track to the node's speaker.
		/// </summary>
		[field: SerializeField]
		public bool SlideCamera { get; private set; }

		/// <summary>
		/// The node's speaker.
		/// </summary>
		[field: SerializeField]
		public SpeakerType SpeakerType { get; private set; }

		/// <summary>
		/// The node's text in all available languages.
		/// </summary>
		[field: SerializeField]
		public NodeText Text { get; private set; }

		/// <summary>
		/// Fills this object's data from a <see cref="JSONNode"/>.
		/// </summary>
		public override void Deserialize(JSONNode node)
		{
			base.Deserialize(node);

			JSONArray characterArrayNode = node.GetArrayChild("character");
			if (characterArrayNode != null)
			{
				Character = characterArrayNode.GetStringChild(0);
			}
			
			IsBox = node.GetBoolChild("is_box");
			ObjectPath = node.GetStringChild("object_path");
			SlideCamera = node.GetBoolChild("slide_camera");
			SpeakerType = (SpeakerType)node.GetIntChild("speaker_type");
			Text = node.GetObjectChild<NodeText>("text");
		}

		/// <summary>
		/// Executes this node on the specified dialogue state.
		/// </summary>
		public override void PerformNode(DialoguePlayer state)
		{
			state.ShowMessage(this);
		}

		/// <summary>
		/// Performs a user choice on this node, moving to the appropriate next node.
		/// </summary>
		public abstract void PerformChoice(DialoguePlayer state, int choice);
	}
}
