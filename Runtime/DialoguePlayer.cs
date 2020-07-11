using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DD
{
	/// <summary>
	/// Tracks the state of a repeat node in the stack.
	/// </summary>
	public struct DialogueRepeat
	{
		public string NodeName;

		public int CurrentCount;

		public DialogueRepeat(string nodeName)
		{
			NodeName = nodeName;
			CurrentCount = 0;
		}
	}

	/// <summary>
	/// Runtime class that moves through a <see cref="Dialogue"/>.
	/// </summary>
	public class DialoguePlayer
	{
		/// <summary>
		/// The dialogue being played.
		/// </summary>
		public readonly Dialogue Dialogue;

		/// <summary>
		/// Should this dialogue update in unscaled time?
		/// </summary>
		public bool UseUnscaledTime = false;

		/// <summary>
		/// The current node.
		/// </summary>
		public BaseNode CurrentNode { get; private set; }

		/// <summary>
		/// The language to use.
		/// </summary>
		/// <seealso cref="SetLanguage"/>
		public string Language
		{
			get; private set;
		}

		/// <summary>
		/// The current values of each string variable in the conversation.
		/// </summary>
		public readonly Dictionary<string, string> StringVariables = new Dictionary<string, string>(StringComparer.InvariantCulture);

		/// <summary>
		/// The current values of each int variable in the conversation.
		/// </summary>
		public readonly Dictionary<string, int> IntVariables = new Dictionary<string, int>(StringComparer.InvariantCulture);

		/// <summary>
		/// The current values of each bool variable in the conversation.
		/// </summary>
		public readonly Dictionary<string, bool> BoolVariables = new Dictionary<string, bool>(StringComparer.InvariantCulture);

		/// <summary>
		/// The current wait timer, in seconds. Only used if the current node is a <see cref="WaitNode"/>.
		/// </summary>
		public float WaitTimer { get; private set; }

		/// <summary>
		/// Random number generator for this dialogue.
		/// </summary>
		private System.Random m_random = new System.Random();

		/// <summary>
		/// Stack of repeat nodes that have been entered.
		/// </summary>
		private Stack<DialogueRepeat> m_repeatCounts = new Stack<DialogueRepeat>();

		/// <summary>
		/// Event called when an execute script should be run.
		/// </summary>
		public static event ExecuteDelegate GlobalOnExecuteScript;
		/// <summary>
		/// Event called when an execute script should be run. If this is bound, <see cref="GlobalOnExecuteScript"/> will not be used.
		/// </summary>
		public event ExecuteDelegate OverrideOnExecuteScript;

		public delegate void ExecuteDelegate(DialoguePlayer sender, string script);

		/// <summary>
		/// Event called to evaluate a condition script.
		/// </summary>
		public static event ConditionDelegate GlobalOnEvaluateCondition;
		/// <summary>
		/// Event called to evaluate a condition script. If this is bound, <see cref="GlobalOnEvaluateCondition"/> will not be used.
		/// </summary>
		public event ConditionDelegate OverrideOnEvaluateCondition;

		public delegate bool ConditionDelegate(DialoguePlayer sender, string script);

		/// <summary>
		/// Event called when a message should be shown.
		/// </summary>
		/// <remarks>You must call <see cref="AdvanceMessage(int)"/> to advance the node.</remarks>
		public static event ShowMessageDelegate GlobalOnShowMessage;
		/// <summary>
		/// Event called when a message should be shown.  If this is bound, <see cref="GlobalOnShowMessage"/> will not be used.
		/// </summary>
		/// <remarks>You must call <see cref="AdvanceMessage(int)"/> to advance the node.</remarks>
		public event ShowMessageDelegate OverrideOnShowMessage;

		public delegate void ShowMessageDelegate(DialoguePlayer sender, ShowMessageNode node);
		
		/// <summary>
		/// Event called when the dialogue ends.
		/// </summary>
		public event DialogueEndedDelegate OnDialogueEnded;

		public delegate void DialogueEndedDelegate(DialoguePlayer sender);

		public DialoguePlayer(Dialogue dialogue)
		{
			Dialogue = dialogue;
			InitializeVariables();
			CurrentNode = Dialogue.GetStartNode();
		}

		public void Update()
		{
			WaitNode currentWait = CurrentNode as WaitNode;
			if (currentWait != null)
			{
				WaitTimer += UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
				if (WaitTimer >= currentWait.Time)
				{
					WaitTimer = 0f;
					MoveToNode(currentWait.Next);
				}
			}
		}

		/// <summary>
		/// Initializes all variables to their starting values.
		/// </summary>
		private void InitializeVariables()
		{
			foreach (Variable variable in Dialogue.Variables)
			{
				switch (variable.Type)
				{
					case VariableType.String:
						StringVariables.Add(variable.Name, variable.StringValue);
						break;
					case VariableType.Integer:
						IntVariables.Add(variable.Name, variable.IntValue);
						break;
					case VariableType.Bool:
						BoolVariables.Add(variable.Name, variable.BoolValue);
						break;
				}
			}
		}

		/// <summary>
		/// Starts the conversation.
		/// </summary>
		public void Play()
		{
			StartNode startNode = CurrentNode as StartNode;
			if (startNode != null)
			{
				MoveToNode(startNode.Next);
			}
		}

		/// <summary>
		/// If the current node is a <see cref="ShowMessageNode"/>, advances to the next node.
		/// </summary>
		/// <param name="choice">The choice made. Ignored if there are no choices.</param>
		public void AdvanceMessage(int choice)
		{
			ShowMessageNode showMessageNode = CurrentNode as ShowMessageNode;
			if (showMessageNode != null)
			{
				showMessageNode.PerformChoice(this, choice);
			}
			else
			{
				throw new InvalidOperationException("Current node is not a ShowMessageNode.");
			}
		}

		/// <summary>
		/// Moves to the specified node.
		/// </summary>
		internal void MoveToNode(string identifier)
		{
			BaseNode node = Dialogue.GetNode(identifier);
			if (node != null)
			{
				MoveToNode(node);
			}
			else if (m_repeatCounts.Count > 0)
			{
				DialogueRepeat repeatCount = m_repeatCounts.Pop();
				RepeatNode repeatNode = (RepeatNode)Dialogue.GetNode(repeatCount.NodeName);
				if (repeatCount.CurrentCount++ < repeatNode.Value)
				{
					m_repeatCounts.Push(repeatCount);
					MoveToNode(repeatNode.Next);
				}
				else
				{
					MoveToNode(repeatNode.NextDone);
				}
			}
			else
			{
				if (OnDialogueEnded != null)
				{
					OnDialogueEnded(this);
				}
			}
		}
		
		/// <summary>
		/// Moves to the specified node.
		/// </summary>
		private void MoveToNode(BaseNode node)
		{
			CurrentNode = node;
			node.PerformNode(this);
		}

		/// <summary>
		/// Pushes and navigates a new repeat node.
		/// </summary>
		public void PushRepeatNode(RepeatNode node)
		{
			if (node.Value <= 0)
			{
				MoveToNode(node.NextDone);
			}
			else
			{
				DialogueRepeat repeatCount = new DialogueRepeat(node.NodeName);
				repeatCount.CurrentCount++;
				m_repeatCounts.Push(repeatCount);
				MoveToNode(node.Next);
			}
		}

		/// <summary>
		/// Changes the language being used in the dialogue. Takes effect for the next node.
		/// </summary>
		public void SetLanguage(string language)
		{
			if (!Dialogue.Languages.Contains(language, StringComparer.InvariantCultureIgnoreCase))
			{
				Debug.LogErrorFormat("Dialogue '{0}' does not have language '{1}'.");
			}
			else
			{
				Language = language;
			}
		}

		/// <summary>
		/// Returns a random number in the range [0, <paramref name="count"/>).
		/// </summary>
		internal int RandomInt(int count)
		{
			return m_random.Next() % count;
		}

		/// <summary>
		/// Returns a random number in the range [0, 1).
		/// </summary>
		internal float RandomFloat()
		{
			return (float)m_random.NextDouble();
		}

		/// <summary>
		/// Evaluates the specified condition script.
		/// </summary>
		public bool EvaluateCondition(string condition)
		{
			if (OverrideOnEvaluateCondition != null)
			{
				try
				{
					return OverrideOnEvaluateCondition(this, condition);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					return false;
				}
			}
			else if (GlobalOnEvaluateCondition != null)
			{
				try
				{
					return GlobalOnEvaluateCondition(this, condition);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					return false;
				}
			}
			else
			{
				Debug.LogWarning("DialoguePlayer: no OnEvaluateCondition delegate is bound.");
				return false;
			}
		}

		/// <summary>
		/// Executes the specified script.
		/// </summary>
		internal void ExecuteScript(string script)
		{
			if (OverrideOnExecuteScript != null)
			{
				try
				{
					OverrideOnExecuteScript(this, script);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
			else if (GlobalOnExecuteScript != null)
			{
				try
				{
					GlobalOnExecuteScript(this, script);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
			else
			{
				Debug.LogWarning("DialoguePlayer: no OnExecuteScript delegate is bound.");
			}
		}

		/// <summary>
		/// Shows a text node.
		/// </summary>
		internal void ShowMessage(ShowMessageNode node)
		{
			if (OverrideOnShowMessage != null)
			{
				OverrideOnShowMessage(this, node);
			}
			else if (GlobalOnShowMessage != null)
			{
				GlobalOnShowMessage(this, node);
			}
			else
			{
				Debug.LogWarning("DialoguePlayer: no OnShowMessage delegate is bound.");
			}
		}
	}
}