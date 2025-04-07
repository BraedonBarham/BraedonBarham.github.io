using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;


/**
* This template file is created for ASU CSE445 Distributed SW Dev Assignment 4.
* Please do not modify or delete any existing class/variable/method names.
However, you can add more variables and functions.
* Uploading this file directly will not pass the autograder's compilation check,
resulting in a grade of 0.
* **/


namespace ConsoleApp1
{
    public class Program
    {

        public static string xmlURL = "https://braedonbarham.github.io/Hotels.xml";
        public static string xmlErrorURL = "https://braedonbarham.github.io/HotelsErrors.xml";
        public static string xsdURL = "https://braedonbarham.github.io/Hotels.xsd";

        public static void Main(string[] args)
        {
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);


            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);


            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1
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

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlUrl);
            XmlNamespaceManager nameSpace = new XmlNamespaceManager(doc.NameTable);
            nameSpace.AddNamespace("ns", "https://BraedonBarham.github.io/Hotels.xsd");

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

            XmlNodeList hotelNodes = doc.SelectNodes("//ns:Hotel", nameSpace);
            var Hotels = new List<Dictionary<string, object>>();
            foreach (XmlNode hotel in hotelNodes)
                {
                var bigHotel = new Dictionary<string, object>();

                // Do the name
                var name = hotel.SelectSingleNode("ns:Name", nameSpace);

          
                    bigHotel["Name"] = name.InnerText;
                    
                // Next the Phone(s)

                var phoneList = new List<string>();
                XmlNodeList phoneNodes = hotel.SelectNodes("ns:Phone", nameSpace);

                foreach (XmlNode phone in phoneNodes)
                {
                    phoneList.Add(phone.InnerText);
                }
               
                    bigHotel["Phone"] = phoneList;
                

                // The Address.s...

                var address = hotel.SelectSingleNode("ns:Address", nameSpace);
                //Console.WriteLine(phoneList[0]);
                // Even though Number and Zip are integers, they will be parsed as strings.
                var addressDict = new Dictionary<string, string>();

                foreach (var bigGroup in new[] {"Number", "Street", "City", "State", "Zip" })
                {
                    var element = address.SelectSingleNode("ns:" + bigGroup, nameSpace);
                    
                        addressDict[bigGroup] = element.InnerText;
                    
                }

                // Aeroporte

                var Aeroporte = address.Attributes["_NearstAirport"];
                if (Aeroporte != null)
                {
                    addressDict["_NearstAirport"] = Aeroporte.Value;
                }

                bigHotel["Address"] = addressDict;

                // Now rating

                var rating = hotel.Attributes["_Rating"];
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
            var jsonText = JsonConvert.SerializeObject(rootHotels, Newtonsoft.Json.Formatting.Indented);
            return jsonText;



        }
    }
}
