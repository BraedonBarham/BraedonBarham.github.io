using System;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;


namespace ConsoleApp1
{
    class Program
    {

        public static string xmlUrl = "https://braedonbarham.github.io/Hotels.xml";
        public static string xmlErrorUrl = "https://braedonbarham.github.io/HotelsErrors.xml";
        public static string xsdUrl = "https://braedonbarham.github.io/Hotels.xsd";

        static void Main(string[] args)
        {
            string result = Verification(xmlUrl, xsdUrl);
            Console.WriteLine(result);

            result = Verification(xmlErrorUrl, xsdUrl);
            Console.WriteLine(result);

            result = Xml2Json(xmlUrl);
            Console.WriteLine(result);

            Console.ReadKey();
        }

        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                List<string> errors = new List<string>();

                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(null, xsdUrl);

                XmlReaderSettings settings = new XmlReaderSettings
                {
                    ValidationType = ValidationType.Schema,
                    Schemas = schemas
                };

                settings.ValidationEventHandler += (sender, e) =>
                {
                    errors.Add($"Validation Error: {e.Message}");
                };

                using (XmlReader reader = XmlReader.Create(xmlUrl, settings))
                {
                    while (reader.Read()) { }
                }

                if (errors.Count == 0)
                {
                    return "No Error";
                }

                return string.Join(Environment.NewLine, errors);
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
        public static string Xml2Json(string xmlUrl)
        {

            XDocument doc = XDocument.Load(xmlUrl);

            XNamespace xsdUrl = doc.Root.Name.Namespace;


            var Hotels = new List<Dictionary<string, object>>();
            foreach (var hotel in doc.Root.Elements(xsdUrl + "Hotel"))
                {
                var bigHotel = new Dictionary<string, object>();

                // Do the name
                var name = hotel.Element(xsdUrl + "Name");

                if (name != null)
                    {
                    bigHotel["Name"] = name;
                    }
                // Next the Phone(s)

                var phoneList = new List<string>();

                foreach(var phone in hotel.Elements(xsdUrl + "Phone"))
                {
                    phoneList.Add(phone.Value);
                }
                if (phoneList.Count > 0)
                {
                    bigHotel["Phone"] = phoneList;
                }

                // The Address.s...

                var address = hotel.Element(xsdUrl + "Address");

                // Even though Number and Zip are integers, they will be parsed as strings.
                var addressDict = new Dictionary<string, string>();

                foreach (var bigGroup in new[] {"Number", "Street", "City", "State", "Zip" })
                {
                    var element = address.Element(xmlUrl + bigGroup);
                    if (element != null)
                    {
                        addressDict[bigGroup] = element.Value;
                    }
                }

                // Aeroporte

                var Aeroporte = address.Attribute("NearstAirport");
                if (Aeroporte != null)
                {
                    addressDict["NearstAirport"] = Aeroporte.Value;
                }

                bigHotel["Address"] = addressDict;

                // Now rating

                var rating = hotel.Attribute("Rating");
                if (rating != null)
                {
                    bigHotel["Rating"] = rating.Value;
                }

                Hotels.Add(bigHotel);
            }

            //Finalmente, make root Hotels
            var rootHotels = new Dictionary<string, object>
            {
                {"Hotels", new Dictionary<string, object> { {"Hotel", Hotels} } }
            };

            //and serialze

            var serialize = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(rootHotels, serialize);



        }
    }
}
