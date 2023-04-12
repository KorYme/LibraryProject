using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string _dataDirPath = "";
    private string _dataFileName = "";
    private EncryptionType _encryptionType = EncryptionType.None;
    private readonly string encryptionString = "hey";

    public FileDataHandler(string dataDirPath, string dataFileName, EncryptionType encryptionType)
    {
        this._dataDirPath = dataDirPath;
        this._dataFileName = dataFileName;
        this._encryptionType = encryptionType;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(_dataDirPath, _dataFileName);
        if (!File.Exists(fullPath)) return null;
        try
        {
            string dataToLoad = "";
            using (FileStream stream = new FileStream(fullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    dataToLoad = Encrypt(reader.ReadToEnd(), _encryptionType, false);
                }
            }
            return JsonUtility.FromJson<GameData>(dataToLoad);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error occured when trying to save data to file: " + fullPath + "\n" + e);
            return null;
        }
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(_dataDirPath, _dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = Encrypt(JsonUtility.ToJson(data, true), _encryptionType, true);
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }



    //A mettre dans la class Utility ou dans une nouvelle classe
    public enum EncryptionType
    {
        None,
        XOR,
    }

    private string Encrypt(string data, EncryptionType encryptionType, bool isEncrypting)
    {
        switch (encryptionType)
        {
            case EncryptionType.None:
                return data;
            case EncryptionType.XOR:
                return XOREncrypting(data);
            default:
                return "";
        }
    }

    private string XOREncrypting(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionString[i % encryptionString.Length]);
        }
        return modifiedData;
    }
}
