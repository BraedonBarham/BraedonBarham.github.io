using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Correctly written Hotels.xml File Results:");
        string xmlUrl = "https://braedonbarham.github.io/Hotels.xml";
        string xsdUrl = "https://braedonbarham.github.io/Hotels.xsd";
        

        string result = XmlUtilities.Verification(xmlUrl, xsdUrl);
        Console.WriteLine(result);
        Console.ReadKey();

        Console.WriteLine("Incorrectly written Hotels.xml File Results:");
        string xmlUrl = "https://braedonbarham.github.io/HotelsErrors.xml";
        string result = XmlUtilities.Verification(xmlUrl, xsdUrl);
        Console.WriteLine(result);
        Console.ReadKey();

    }
}
