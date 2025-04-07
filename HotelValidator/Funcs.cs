using System;
using System.Net;
using System.Xml;
using System.Xml.Schema;

public class XmlUtilities
{
    //View Slide 5, XML Validation PDF for reference
    public static string Verification(string xmlUrl, string xsdUrl)
    {
        try
        {
            List<string> errors = new List<string>();

            XmlSchemaSet sc = new XmlSchemaSet();
            // Obviously, we compare to this scheme we wrote
            sc.Add(null, xsdUrl);
            
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;

            //object sender, ValidationEventArgs e
            settings.ValidationEventHandler += (sender, e) =>
            {
                errors.Add($"Validation Error: {e.Message}");
            };

            //We read the 10 hotels.
            using (XmlReader reader = XmlReader.Create(xmlUrl, settings))
            {
                while (reader.Read()) { }  
            }

            if (errors.Count == 0)
            {
                return "No errors are found.";
            }
            return string.Join(Environment.NewLine, errors);
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
}

