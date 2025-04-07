﻿using System;

class Program
{
    static void Main(string[] args)
    {
        string xmlUrl = "https://braedonbarham.github.io/Hotels.xml";
        string xsdUrl = "https://braedonbarham.github.io/Hotels.xsd";

        string result = XmlUtilities.Verification(xmlUrl, xsdUrl);
        Console.WriteLine(result);
    }
}
