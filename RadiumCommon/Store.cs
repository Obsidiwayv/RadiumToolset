namespace RadiumCommon;

public class RadiumKeyValueStore<T>(string key)
{
    public string Key = key;

    /// <summary>
    /// A string array property that is created on the call of the class
    /// </summary>
    public List<string> Value = [];

    public T? KeyStoreType;

    /// <summary>
    ///  Will return A string, array or int depending on the generic provided
    /// </summary>
    public dynamic Get<D>()
    {
        if (typeof(D) == typeof(string))
        {
            if (IsEmpty())
            {
                throw new IndexOutOfRangeException($"string was provided but {Key} is empty");
            }
            return Value[0];
        }
        if (typeof(D) == typeof(int))
        {
            return int.Parse(Value[0]);
        }
        // Anything else that is the generic provided will just return the entire list
        return Value;
    }

    public bool IsEmpty()
    {
        return Value.Count == 0;
    }
}

/// <summary>
/// unlike dotnets built in keyvalue class this one can have its properties overwritten at anytime
/// both Properties COULD be null
/// </summary>
public class BasicKeyValue<K, V>
{
    public K? Key;
    public V? Value;
}

public class KeyValueUtil
{
    public static RadiumKeyValueStore<T> GetStoreFromName<T>(List<RadiumKeyValueStore<T>> stores, string keyName)
    {
        foreach (RadiumKeyValueStore<T> KeyValueStore in stores)
        {
            if (KeyValueStore.Key.Equals(keyName)) return KeyValueStore;
        }
        // Instead OF A null value we return a regular class with
        // the same name but the value will be empty
        return new RadiumKeyValueStore<T>(keyName);
    }
}