using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Represents one loaded and parsed dialogue file.
	/// </summary>
	public class Dialogue : ScriptableObject, IJsonDeserializable, ISerializationCallbackReceiver
	{
		/// <summary>
		/// Loads a Dialogue Designer dialogue from a JSON file.
		/// </summary>
		public static Dialogue FromAsset(TextAsset asset)
		{
			return FromJson(asset.text);
		}

		/// <summary>
		/// Loads a Dialogue Designer dialogue from a JSON string.
		/// </summary>
		public static Dialogue FromJson(string text)
		{
			JSONNode rootNode = JSON.Parse(text);
			//HACK: assumes only one object.
			//I don't think it's possible for DD to export more than one.
			JSONNode dialogueNode = rootNode[0];
			Dialogue outputObject = CreateInstance<Dialogue>();
			outputObject.Deserialize(dialogueNode);
			return outputObject;
		}

		/// <summary>
		/// The identifiers of the characters in the dialogue's character database.
		/// </summary>
		[field: SerializeField]
		public string[] Characters { get; private set; }
		
		/// <summary>
		/// The version of Dialogue Designer the dialogue was exported from.
		/// </summary>
		[field: SerializeField]
		public string EditorVersion { get; private set; }

		/// <summary>
		/// The name of the dialogue file (not a path).
		/// </summary>
		[field: SerializeField]
		public string FileName { get; private set; }

		/// <summary>
		/// The language codes this dialogue supports.
		/// </summary>
		[field: SerializeField]
		public string[] Languages { get; private set; }

		/// <summary>
		/// The nodes in the dialogue.
		/// </summary>
		[field: SerializeField]
		public BaseNode[] Nodes { get; private set; }

		/// <summary>
		/// The nodes in the dialogue.
		/// </summary>
		[field: NonSerialized]
		public Dictionary<string, BaseNode> NodesDictionary { get; private set; }

		/// <summary>
		/// The variables in the dialogue's variable database.
		/// </summary>
		[field: SerializeField]
		public Variable[] Variables { get; private set; }

		/// <summary>
		/// Finds and returns the <see cref="StartNode"/>.
		/// </summary>
		public BaseNode GetStartNode()
		{
			foreach (BaseNode node in Nodes)
			{
				if (node is StartNode)
				{
					return node;
				}
			}
			Debug.LogErrorFormat("Dialogue '{0}' has no StartNode.");
			return null;
		}

		/// <summary>
		/// Returns the node with the specified name.
		/// </summary>
		public BaseNode GetNode(string identifier)
		{
			if (string.IsNullOrEmpty(identifier)) return null;

			BaseNode node;
			if (NodesDictionary.TryGetValue(identifier, out node))
			{
				return node;
			}
			else
			{
				throw new KeyNotFoundException(string.Format("Dialogue '{0}' has no node '{1}'", name, identifier));
			}
		}

		/// <summary>
		/// Deserializes a Dialogue Designer dialogue from a JSON node.
		/// </summary>
		public void Deserialize(JSONNode node)
		{
			Characters = node.GetStringArrayChild("characters");
			EditorVersion = node.GetStringChild("editor_version");
			FileName = node.GetStringChild("file_name");
			Languages = node.GetStringArrayChild("languages");

			List<BaseNode> nodesList = new List<BaseNode>();
			JSONArray nodesArrayNode = node.GetArrayChild("nodes");
			foreach (JSONNode arrayElementNode in nodesArrayNode.Values)
			{
				BaseNode newNode = null;
				string nodeType = arrayElementNode.GetStringChild("node_type");
				switch (nodeType)
				{
					case "show_message":
						if (arrayElementNode["choices"] != null)
						{
							newNode = CreateInstance<ShowMessageNodeChoice>();
						}
						else
						{
							newNode = CreateInstance<ShowMessageNodeSimple>();
						}
						break;
					case "start":
						newNode = CreateInstance<StartNode>();
						break;
					case "condition_branch":
						newNode = CreateInstance<ConditionBranchNode>();
						break;
					case "wait":
						newNode = CreateInstance<WaitNode>();
						break;
					case "execute":
						newNode = CreateInstance<ExecuteNode>();
						break;
					case "random_branch":
						newNode = CreateInstance<RandomBranchNode>();
						break;
					case "chance_branch":
						newNode = CreateInstance<ChanceBranchNode>();
						break;
					case "repeat":
						newNode = CreateInstance<RepeatNode>();
						break;
					case "set_local_variable":
						JSONNode valueNode = arrayElementNode["value"];
						if (valueNode is JSONBool)
						{
							newNode = CreateInstance<SetLocalVariableBoolNode>();
						}
						else if (valueNode is JSONNumber)
						{
							newNode = CreateInstance<SetLocalVariableIntNode>();
						}
						else
						{
							newNode = CreateInstance<SetLocalVariableStringNode>();
						}
						break;
					case "comment":
						// skip
						break;
					default:
						Debug.LogErrorFormat("Dialogue deserialization: unknown node type '{0}'", nodeType);
						break;
				}
				if (newNode != null)
				{
					newNode.Deserialize(arrayElementNode);
					nodesList.Add(newNode);
				}
			}
			Nodes = nodesList.ToArray();

			List<Variable> variablesBuffer = new List<Variable>();
			JSONNode variablesNode = node["variables"];
			if (variablesNode == null)
			{
				Debug.LogErrorFormat("JSON deserialization: expected 'variables' to be Object.");
			}
			else
			{
				foreach (string key in variablesNode.Keys)
				{
					Variable newVariable = variablesNode.GetObjectChild<Variable>(key);
					newVariable.SetName(key);
					variablesBuffer.Add(newVariable);
				}
			}
			Variables = variablesBuffer.ToArray();

			name = FileName;

			OnAfterDeserialize();
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
			NodesDictionary = new Dictionary<string, BaseNode>(Nodes.Length, StringComparer.InvariantCulture);
			foreach (BaseNode node in Nodes)
			{
				NodesDictionary.Add(node.NodeName, node);
			}
		}
	}
}
