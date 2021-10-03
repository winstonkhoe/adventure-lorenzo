using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveSystemData ()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/GameData";
        FileStream stream = new FileStream(path, FileMode.Create);

        OptionSystemData data = new OptionSystemData();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static OptionSystemData LoadSystemData()
    {
        string path = Application.persistentDataPath + "/GameData";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            OptionSystemData data = formatter.Deserialize(stream) as OptionSystemData;
            
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Data File not found in " + path);
            return null;
        }
    }
}
