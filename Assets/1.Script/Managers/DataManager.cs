using UnityEngine;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using System.Linq;

public interface IIdentifiable
{
    int id { get; set; }
}

[System.Serializable]
public class ScriptData
{
    public int id;
    public int npcID;
    public string text;
}

[System.Serializable]
public class TalkData
{
    public int id;
    public int npcID;
    public string npcName;
    public int patternID;
    public int cameraDirectType;
    public int scriptType;
    public int scriptImageID;
    public int scriptImageColorID;
    public int stringID;
    public int imageID;
    public int visualEffectID;
    public int soundEffectID;
    public int animID;
    public int skipType;
    public int skillID;
    public int nextScriptID;
}

public abstract class XmlConverter
{
    public abstract object Convert(XDocument xmlDocument);
    public XDocument LoadXml(string filePath)
    {
        return XDocument.Parse(filePath);
    }

    public void SaveXml(string filePath, XDocument xmlDocument)
    {
        xmlDocument.Save(filePath);
    }
}

public class ScriptDataConverter : XmlConverter
{
    public override object Convert(XDocument xmlDocument)
    {
        var data = xmlDocument.Descendants("Dialogue")
            .Select(x => new ScriptData
            {
                id = int.Parse(x.Element("id").Value),
                npcID = int.Parse(x.Element("npcid").Value),
                text = x.Element("Descript").Value
            }).ToList();
        return data;
    }
}

public class TalkDataConverter : XmlConverter
{
    public override object Convert(XDocument xmlDocument)
    {
        var data = xmlDocument.Descendants("dialogue")
            .Select(x => new TalkData
            {
                id = ParseIntSafe(x.Element("id")?.Value),
                npcID = ParseIntSafe(x.Element("npcid")?.Value),
                npcName = x.Element("npcName")?.Value ?? "DefaultName",
                patternID = ParseIntSafe(x.Element("patternId")?.Value),
                cameraDirectType = ParseIntSafe(x.Element("cameraDirectionType")?.Value),
                scriptType = ParseIntSafe(x.Element("scriptType")?.Value),
                scriptImageID = ParseIntSafe(x.Element("scriptImageid")?.Value),
                scriptImageColorID = ParseIntSafe(x.Element("scriptImageColorId")?.Value),
                stringID = ParseIntSafe(x.Element("stringId")?.Value),
                imageID = ParseIntSafe(x.Element("imageId")?.Value),
                visualEffectID = ParseIntSafe(x.Element("visualEffectId")?.Value),
                soundEffectID = ParseIntSafe(x.Element("soundEffectId")?.Value),
                animID = ParseIntSafe(x.Element("animId")?.Value),
                skipType = ParseIntSafe(x.Element("skipType")?.Value),
                skillID = ParseIntSafe(x.Element("skillId")?.Value),
                nextScriptID = ParseIntSafe(x.Element("nextScriptId")?.Value)
            }).ToList();

        return data;
    }

    // Helper method to safely parse integers with default value
    private int ParseIntSafe(string value)
    {
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        return 0; // Default value if parsing fails
    }
}

public class DataManager : MonoBehaviour
{
    public TextAsset scriptXML;
    public TextAsset talkXML;
    public List<ScriptData> scriptData;
    public List<TalkData> talkData;
    public List<IIdentifiable> identifiableData;

    // practice crud
    private void Awake()
    {
        // script data
        XmlConverter scriptDataConverter = new ScriptDataConverter();
        XDocument scriptDataDoc = scriptDataConverter.LoadXml(scriptXML.text);
        scriptData = scriptDataConverter.Convert(scriptDataDoc) as List<ScriptData>;
        
        XmlConverter talkDataConverter = new TalkDataConverter();
        XDocument talkDataDoc = talkDataConverter.LoadXml(talkXML.text);
        talkData = talkDataConverter.Convert(talkDataDoc) as List<TalkData>;
    }

    public T GetData<T>(int id) where T : IIdentifiable
    {
        // id를 통해서 데이터를 불러오는 코드
        return scriptData.Cast<T>().FirstOrDefault(item => item.id == id);
    }

    public TalkData getTalkData(int id)
    {
        return talkData.Cast<TalkData>().FirstOrDefault(item => item.id == id);
    }

    public ScriptData GetScriptData(int id)
    {
        return scriptData.Cast<ScriptData>().FirstOrDefault(item => (item.id == id));
    }
}
