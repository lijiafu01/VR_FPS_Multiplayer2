using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();
    [SerializeField]
    private List<TValue> values = new List<TValue>();
    private Dictionary<TKey, TValue> target = new Dictionary<TKey, TValue>();
    public SerializableDictionary() { }
    public SerializableDictionary(Dictionary<TKey, TValue> dict)
    {
        target = dict;
    }
    public Dictionary<TKey, TValue> ToDictionary()
    {
        return target;
    }
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (var kvp in target)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }
    public void OnAfterDeserialize()
    {
        target = new Dictionary<TKey, TValue>();

        for (int i = 0; i < keys.Count; i++)
        {
            target[keys[i]] = values[i];
        }
    }
}
