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
            string errorMessage = "No Error";

            XmlSchemaSet sc = new XmlSchemaSet();
            // Obviously, we compare to this scheme we wrote
            sc.Add(null, xsdUrl);
            
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;

            //object sender, ValidationEventArgs e
            settings.ValidationEventHandler += (sender, e) =>
            {
                errorMessage = $"Validation Error: {e.Message}";
            };

            //We read the 10 hotels.
            XmlReader reader = new XmlReader.Create(xmlUrl);
            while (reader.Read())
            {

            }

            return errorMessage;
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
}

