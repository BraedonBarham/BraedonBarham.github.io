using System;
using System.Net;
using System.Xml;
using System.Xml.Schema;

public class XmlUtilities
{
    public static string Verification(string xmlUrl, string xsdUrl)
    {
        try
        {
            string errorMessage = "No Error";

 
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(null, xsdUrl);
            settings.Schemas = schemas;


            settings.ValidationEventHandler += (sender, args) =>
            {
                errorMessage = $"Validation Error: {args.Message}";
            };

   
            using (XmlReader reader = XmlReader.Create(xmlUrl, settings))
            {
                while (reader.Read()) { } 
            }

            return errorMessage;
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
}

