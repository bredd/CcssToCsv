// See https://aka.ms/new-console-template for more information
using CcssToCsv;
using Microsoft.VisualBasic.FileIO;
using System.Security.AccessControl;
using System.Xml.XPath;
using FileMeta;

Converter.Go(@"C:\Users\brand\Source\bredd\CoreStandards\math.xml", @"C:\Users\brand\Source\bredd\CoreStandards\math.csv");
Console.WriteLine("Done.");


class Converter
{
    public static void Go(string inputFilename, string outputFilename)
    {
        // Load the document
        XPathDocument doc = new XPathDocument(inputFilename);

        // Iterate once to create a dictionary to translate from UUID to URL
        var UuidToUrl = new Dictionary<string, string>();
        foreach (XPathNavigator item in doc.CreateNavigator().Select(" / LearningStandards/LearningStandardItem"))
        {
            var uuid = item.XPVal("@RefID");
            var url = item.XPVal("RefURI");
            UuidToUrl[uuid] = url;
        }

        using (var writer = new CsvWriter(outputFilename))
        {
            // Write the header
            writer.Write("eleType", "name", "url", "about", "abstract", "identifier", "educationalLevel", "creator", "mainEntity", "isPartOf");

            // Iterate again and output the result
            foreach (XPathNavigator item in doc.CreateNavigator().Select(" / LearningStandards/LearningStandardItem"))
            {
                string eleType = "cs";  // Competency Statement
                string name = Name(item);
                string url = item.XPVal("RefURI");
                string about = About(item);
                string abstrct = item.XPVal("Statements/Statement").Trim();
                string identifier = item.XPVal("StatementCodes/StatementCode");
                string educationalLevel = EdLevel(item);
                string creator = "CCSSO";
                string mainEntity = UuidToUrl[item.XPVal("LearningStandardDocumentRefId")];
                var uuid = item.XPVal("RelatedLearningStandardItems/LearningStandardItemRefId[@RelationshipType='childOf']");
                string isPartOf = string.IsNullOrWhiteSpace(uuid) ? string.Empty : UuidToUrl[uuid];

                writer.Write(eleType, name, url, about, abstrct, identifier, educationalLevel, creator, mainEntity, isPartOf);
            }
        }

    }

    const string c_standardsSuffix = " Standards";

    static string Name(XPathNavigator item)
    {
        var level = item.XPVal("StandardHierarchyLevel/number");
        if (string.Equals(level, "2"))
        {
            var statement = item.XPVal("Statements/Statement");
            if (statement.EndsWith(c_standardsSuffix))
                statement = statement.Substring(0, statement.Length - c_standardsSuffix.Length);
            return "Common Core State Standards for " + statement;
        }
        return item.XPVal("StatementCodes/StatementCode");
    }

    static string About(XPathNavigator item)
    {
        var parts = item.XPVal("StatementCodes/StatementCode").Split('.');
        return (parts[0] == "CCSS") ? parts[1] : parts[0];
    }

    static string EdLevel(XPathNavigator item)
    {
        var grades = new List<string>();
        foreach (XPathNavigator ele in item.Select("GradeLevels/GradeLevel"))
        {
            grades.Add(ele.Value);
        }
        if (grades.Count == 0) return string.Empty;
        if (grades.Count == 1) return grades[0];
        if (grades[0] == "K" && grades[grades.Count - 1] == "12") return "K-12";
        if (grades[0] == "09" && grades[grades.Count - 1] == "12") return "HS";
        return string.Empty;
    }

}