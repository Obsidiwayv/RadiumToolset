namespace RadiumCommon;

public class RadiumKeyValue<T>(string k, T v)
{
    public string Key = k;
    public T Value = v;
}

public class RadiumDictionary<T>(RadiumKeyValue<T>[] passthrough)
{
    public RadiumKeyValue<T>[] Passthrough = passthrough;

    public void Push(RadiumKeyValue<T> value)
    {
        var Index = Array.IndexOf(Passthrough, default);
        if (Index != -1)
        {
            Passthrough[Index] = value;
        }
    }

    public RadiumKeyValue<T>? Get(string searchKey)
    {
        foreach (var item in Passthrough)
        {
            if (item.Key.Equals(searchKey)) return item;
        }
        return null;
    }
}