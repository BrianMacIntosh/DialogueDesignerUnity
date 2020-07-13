using SimpleJSON;
using UnityEngine;

/// <summary>
/// Interface for objects that can be populated from JSON.
/// </summary>
public interface IJsonDeserializable
{
	/// <summary>
	/// Fills this object's data from a <see cref="JSONNode"/>.
	/// </summary>
	void Deserialize(JSONNode node);
}

/// <summary>
/// Contains utility functions for reading JSON properties.
/// </summary>
public static class SimpleJsonUtility
{
	/// <summary>
	/// Gets a typed array from an array child of the specified node.
	/// </summary>
	public static T[] GetArrayChild<T>(this JSONNode parent, string key) where T : IJsonDeserializable, new()
	{
		JSONArray arrayNode = parent.GetArrayChild(key);
		if (arrayNode == null) return null;

		T[] typedArray = new T[arrayNode.Count];
		for (int i = 0; i < typedArray.Length; i++)
		{
			typedArray[i] = arrayNode.GetObjectChild<T>(i);
		}
		return typedArray;
	}

	/// <summary>
	/// Gets a typed array from an array child of the specified node.
	/// </summary>
	public static string[] GetStringArrayChild(this JSONNode parent, string key)
	{
		JSONArray arrayNode = parent.GetArrayChild(key);
		if (arrayNode == null) return null;

		string[] typedArray = new string[arrayNode.Count];
		for (int i = 0; i < typedArray.Length; i++)
		{
			typedArray[i] = arrayNode.GetStringChild(i);
		}
		return typedArray;
	}

	/// <summary>
	/// Gets an array node child from the specified node.
	/// </summary>
	public static JSONArray GetArrayChild(this JSONNode parent, string key)
	{
		JSONNode childNode = parent[key];
		if (childNode == null) return null;

		JSONArray arrayNode = childNode as JSONArray;
		if (arrayNode == null)
		{
			Debug.LogErrorFormat("JSON deserialization: expected '{0}' to be Array.", key);
		}
		return arrayNode;
	}

	public static T GetObjectChild<T>(this JSONNode node, string key) where T : IJsonDeserializable, new()
	{
		JSONNode childNode = node[key];
		if (childNode == null) return default(T);

		JSONObject objectNode = childNode as JSONObject;
		if (objectNode == null)
		{
			Debug.LogErrorFormat("JSON deserialization: expected '{0}' to be Object.", key);
		}

		T newObject = new T();
		newObject.Deserialize(objectNode);
		return newObject;
	}

	public static T GetObjectChild<T>(this JSONArray array, int index) where T : IJsonDeserializable, new()
	{
		JSONNode childNode = array[index];
		if (childNode == null) return default(T);

		JSONObject objectNode = childNode as JSONObject;
		if (objectNode == null)
		{
			Debug.LogErrorFormat("JSON deserialization: expected [{0}] to be Object.", index);
		}

		T newObject = new T();
		newObject.Deserialize(objectNode);
		return newObject;
	}

	public static string GetStringChild(this JSONNode node, string key)
	{
		JSONNode childNode = node[key];
		if (childNode == null) return null;

		JSONString stringNode = childNode as JSONString;
		if (stringNode == null)
		{
			Debug.LogErrorFormat("JSON deserialization: expected '{0}' to be String.", key);
		}
		return stringNode;
	}

	public static string GetStringChild(this JSONArray array, int index)
	{
		JSONNode childNode = array[index];
		if (childNode == null) return null;

		JSONString stringNode = childNode as JSONString;
		if (stringNode == null)
		{
			Debug.LogErrorFormat("JSON deserialization: expected [{0}] to be String.", index);
		}
		return stringNode;
	}

	public static int GetIntChild(this JSONNode node, string key)
	{
		JSONNode childNode = node[key];
		if (childNode == null) return 0;

		JSONNumber numberNode = childNode as JSONNumber;
		if (numberNode == null)
		{
			Debug.LogErrorFormat("JSON deserialization: expected '{0}' to be Number.", key);
		}
		return numberNode;
	}

	public static int GetIntChild(this JSONArray array, int index)
	{
		JSONNode childNode = array[index];
		if (childNode == null) return 0;

		JSONNumber numberNode = childNode as JSONNumber;
		if (numberNode == null)
		{
			Debug.LogErrorFormat("JSON deserialization: expected [{0}] to be Number.", index);
		}
		return numberNode;
	}

	public static float GetFloatChild(this JSONNode node, string key)
	{
		JSONNode childNode = node[key];
		if (childNode == null) return 0;

		JSONNumber numberNode = childNode as JSONNumber;
		if (numberNode == null)
		{
			Debug.LogErrorFormat("JSON deserialization: expected '{0}' to be Number.", key);
		}
		return numberNode;
	}

	public static float GetFloatChild(this JSONArray array, int index)
	{
		JSONNode childNode = array[index];
		if (childNode == null) return 0;

		JSONNumber numberNode = childNode as JSONNumber;
		if (numberNode == null)
		{
			Debug.LogErrorFormat("JSON deserialization: expected [{0}] to be Number.", index);
		}
		return numberNode;
	}

	public static bool GetBoolChild(this JSONNode node, string key)
	{
		JSONNode childNode = node[key];
		if (childNode == null) return false;

		JSONBool boolNode = childNode as JSONBool;
		if (boolNode == null)
		{
			Debug.LogErrorFormat("JSON deserialization: expected '{0}' to be Bool.", key);
		}
		return boolNode;
	}

	public static bool GetBoolChild(this JSONArray array, int index)
	{
		JSONNode childNode = array[index];
		if (childNode == null) return false;

		JSONBool boolNode = childNode as JSONBool;
		if (boolNode == null)
		{
			Debug.LogErrorFormat("JSON deserialization: expected [{0}] to be Bool.", index);
		}
		return boolNode;
	}
}
