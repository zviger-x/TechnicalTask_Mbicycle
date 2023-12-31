using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class DataIOBase
{
    public static void SaveData<T>(T data, string path) where T : class
    {
        using var fileStream = new FileStream(path, FileMode.Create);
        var formatter = new BinaryFormatter();

        formatter.Serialize(fileStream, data);
        fileStream.Close();
    }

    public static T LoadData<T>(string path) where T : class
    {
        if (!File.Exists(path))
            return null;

        using var fileStream = new FileStream(path, FileMode.Open);
        var formatter = new BinaryFormatter();

        var data = formatter.Deserialize(fileStream) as T;
        fileStream.Close();

        return data;
    }
}
