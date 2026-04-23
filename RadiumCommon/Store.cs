namespace RadiumCommon;

public class RadiumKeyValueStore(string key, List<string> anyValue)
{
    public string Key = key;

    public List<string> Value = anyValue;

    /// <summary>
    ///  Will return A string or array depending on the generic provided
    /// </summary>
    public dynamic Get<T>()
    {
        if (typeof(T) == typeof(string))
        {
            return Value[0];
        } else
        {
            return Value;
        }
    }

    public bool IsEmpty()
    {
        return Value.Count < 0;
    }
}

public class KeyValueUtil
{
    public static RadiumKeyValueStore GetStoreFromName(List<RadiumKeyValueStore> stores, string keyName)
    {
        foreach (RadiumKeyValueStore KeyValueStore in stores)
        {
            if (KeyValueStore.Key.Equals(keyName)) return KeyValueStore;
        }
        // Instead OF A null value we return a regular class with
        // the same name but the value will be empty
        return new RadiumKeyValueStore(keyName, []);
    }
}