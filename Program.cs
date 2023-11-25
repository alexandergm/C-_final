using System;
using System.Text.Json;
using System.Xml;

class Program
{
    static void Main()
    {
        string jsonInput = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";

        try
        {
            string xmlOutput = ConvertJsonToXml(jsonInput);

            Console.WriteLine("Converted XML:");
            Console.WriteLine(xmlOutput);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static string ConvertJsonToXml(string json)
    {
        using (JsonDocument document = JsonDocument.Parse(json))
        {
            using (MemoryStream stream = new MemoryStream())
            using (XmlWriter xmlWriter = XmlWriter.Create(stream))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("root");

                ProcessJsonValue(document.RootElement, xmlWriter);

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }

    static void ProcessJsonValue(JsonElement jsonElement, XmlWriter xmlWriter)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.Object:
                xmlWriter.WriteStartElement("object");
                foreach (JsonProperty property in jsonElement.EnumerateObject())
                {
                    xmlWriter.WriteStartElement(property.Name);
                    ProcessJsonValue(property.Value, xmlWriter);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                break;

            case JsonValueKind.Array:
                xmlWriter.WriteStartElement("array");
                foreach (JsonElement item in jsonElement.EnumerateArray())
                {
                    xmlWriter.WriteStartElement("item");
                    ProcessJsonValue(item, xmlWriter);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                break;

            case JsonValueKind.String:
                xmlWriter.WriteElementString("string", jsonElement.GetString());
                break;

            case JsonValueKind.Number:
                xmlWriter.WriteElementString("number", jsonElement.GetRawText());
                break;

            case JsonValueKind.True:
                xmlWriter.WriteElementString("boolean", "true");
                break;

            case JsonValueKind.False:
                xmlWriter.WriteElementString("boolean", "false");
                break;

            case JsonValueKind.Null:
                xmlWriter.WriteElementString("null", "true");
                break;

            default:
                throw new NotSupportedException($"Unsupported JSON value kind: {jsonElement.ValueKind}");
        }
    }
}
