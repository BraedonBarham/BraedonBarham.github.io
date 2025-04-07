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
                //View Slide 5, XML Validation PDF for reference
                // Essentially, all of this is in there
                // Any how, we create a scheme object that we're essentially comparing to while we
                // Read out xml document. Any errors get logged, and if it successfully reads through
                // as a result of a correct scheme, it returns No Error


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
            //This probably wasn't the intended way to go about this, but I
            // used this "xsdUrl" to be able to select certain parts.

            // Very simply, I'm creating either a dictionary or a list for each part of the Hotel.
            // Hotels is a dictionary because that's our enveloping hotels object.
            // hotel is also a dictionary because each hotel has a few objects it must hold, such as
            // address, phones, nearest airport and the like.
            // the phones get a list because there can be multiple phone strings.
            // the addresses are pretty cut and dry.
            // The nearstairport is as well.
            // The rating requires the existence check because it's an optional parameter.


            var Hotels = new List<Dictionary<string, object>>();
            foreach (var hotel in doc.Root.Elements(xsdUrl + "Hotel"))
                {
                var bigHotel = new Dictionary<string, object>();

                // Do the name
                var name = hotel.Element(xsdUrl + "Name");

          
                    bigHotel["Name"] = name.Value;
                    
                // Next the Phone(s)

                var phoneList = new List<string>();

                foreach(var phone in hotel.Elements(xsdUrl + "Phone"))
                {
                    phoneList.Add(phone.Value);
                }
               
                    bigHotel["Phone"] = phoneList;
                

                // The Address.s...

                var address = hotel.Element(xsdUrl + "Address");
                //Console.WriteLine(phoneList[0]);
                // Even though Number and Zip are integers, they will be parsed as strings.
                var addressDict = new Dictionary<string, string>();

                foreach (var bigGroup in new[] {"Number", "Street", "City", "State", "Zip" })
                {
                    var element = address.Element(xsdUrl + bigGroup);
                    
                        addressDict[bigGroup] = element.Value;
                    
                }

                // Aeroporte

                var Aeroporte = address.Attribute("_NearstAirport");
                if (Aeroporte != null)
                {
                    addressDict["_NearstAirport"] = Aeroporte.Value;
                }

                bigHotel["Address"] = addressDict;

                // Now rating

                var rating = hotel.Attribute("_Rating");
                if (rating != null)
                {
                    bigHotel["_Rating"] = rating.Value;
                }

                Hotels.Add(bigHotel);
            }

            //Finalmente, make root Hotels
            var rootHotels = new Dictionary<string, object>
            {
                {"Hotels", new Dictionary<string, object> { {"Hotel", Hotels} } }
            };

            //and serialze
            // here, stack overflow helped out
            // this takes my object and makes it into a pretty JSON string and not a one liner
            var serialize = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(rootHotels, serialize);



        }
    }
}
