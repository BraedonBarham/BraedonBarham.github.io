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

            // Set up settings for validation
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;

            // Load the XSD from URL
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(null, xsdUrl);
            settings.Schemas = schemas;

            // Set the validation event handler
            settings.ValidationEventHandler += (sender, args) =>
            {
                errorMessage = $"Validation Error: {args.Message}";
            };

            // Create XmlReader using the settings and load from XML URL
            using (XmlReader reader = XmlReader.Create(xmlUrl, settings))
            {
                while (reader.Read()) { }  // Parse the document
            }

            return errorMessage;
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
}

