using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace QuranX.DocumentModel.Factories
{
    public class HadithCollectionFactory
    {
        HadithCollection Collection;
        Dictionary<HadithReference, HashSet<VerseRangeReference>> VersesByHadith;

        public HadithCollection Create(string hadithFilePath, string additionalHadithXRefsDirectory)
        {
            var doc = XDocument.Load(File.OpenText(hadithFilePath));
            var collectionNode = doc.Document.Root;

            string code = collectionNode.Element("code").Value;
            string name = collectionNode.Element("name").Value;
            string copyright = collectionNode.Element("copyright").Value;
            HadithReferenceDefinition[] referenceDefinitions = ReadReferenceDefinitions(collectionNode);

            Collection = new HadithCollection(
                    code: code,
                    name: name,
                    copyright: copyright,
                    referenceDefinitions: referenceDefinitions
                );

            CreateAdditionalHadithXRefs(
                    tafsirCode: code,
                    xrefsDirectory: additionalHadithXRefsDirectory
                );

            ReadHadiths(collectionNode);
            return Collection;
        }

        void CreateAdditionalHadithXRefs(string tafsirCode, string xrefsDirectory)
        {
            VersesByHadith = new Dictionary<HadithReference, HashSet<VerseRangeReference>>();
            string xrefsFilePath = Path.Combine(xrefsDirectory, tafsirCode + ".txt");
            if (!File.Exists(xrefsFilePath))
                return;

            string[] lines = File.ReadAllLines(xrefsFilePath);
            foreach (string line in lines)
            {
                string[] lineValues = line.Split('\t');
                if (string.IsNullOrEmpty(lineValues[0]))
                    continue;

                HadithReference hadithReference;
                string[] referenceCodeAndValue = lineValues[0].Split('/');
                if (referenceCodeAndValue.Length == 1)
                    hadithReference = new HadithReference(Collection.PrimaryReferenceDefinition.Code, lineValues[0].Split('.'), null);
                else
                    hadithReference = new HadithReference(referenceCodeAndValue[0], referenceCodeAndValue[1].Split('.'), null);
                foreach (string verseRangeReferenceText in lineValues.Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(verseRangeReferenceText))
                        continue;
                    AddVerseReference(
                            hadithReference: hadithReference,
                            verseRangeReferenceText: verseRangeReferenceText
                        );
                }
            }
        }

        void AddVerseReference(HadithReference hadithReference, string verseRangeReferenceText)
        {
            var verseRangeReference = VerseRangeReference.Parse(verseRangeReferenceText);
            HashSet<VerseRangeReference> verseRangeReferences;
            if (!VersesByHadith.TryGetValue(hadithReference, out verseRangeReferences))
            {
                verseRangeReferences = new HashSet<VerseRangeReference>();
                VersesByHadith[hadithReference] = verseRangeReferences;
            }
            verseRangeReferences.Add(verseRangeReference);
        }

        void ReadHadiths(XElement collectionNode)
        {

            foreach (XElement hadithNode in collectionNode.Descendants("hadith"))
            {
                ReadHadith(hadithNode);
            }
        }

        void ReadHadith(XElement hadithNode)
        {
            var references = ReadReferences(hadithNode.Element("references"));
            var verseReferences = ReadVerseReferences(hadithNode.Element("verseReferences"));
            var englishTextNode = hadithNode.Element("english");
            var englishText = englishTextNode.Elements("text").Select(x => x.Value);
            var arabicTextNode = hadithNode.Element("arabic");
            var arabicText = arabicTextNode.Elements("text").Select(x => x.Value);

            foreach (HadithReference reference in references)
            {
                HashSet<VerseRangeReference> additionalVerseReferences;
                if (VersesByHadith.TryGetValue(reference, out additionalVerseReferences))
                {
                    verseReferences = verseReferences.Concat(additionalVerseReferences);
                    break;
                }
            }

            var hadith = new Hadith(
                collection: Collection,
                references: references,
                arabicText: arabicText,
                englishText: englishText,
                verseReferences: verseReferences);
            Collection.AddHadith(hadith);
        }

        IEnumerable<HadithReference> ReadReferences(XElement referencesNode)
        {
            var result = new List<HadithReference>();
            if (referencesNode != null)
            {
                foreach (XElement referenceNode in referencesNode.Elements("reference"))
                {
                    string code = referenceNode.Element("code").Value;
                    string suffix = referenceNode.Element("suffix")?.Value;
                    var parts = new List<string>();
                    XElement partsNode = referenceNode.Element("parts");
                    foreach (XElement partNode in partsNode.Elements("part"))
                        parts.Add(partNode.Value);
                    var reference = new HadithReference(code, parts, suffix);
                    result.Add(reference);
                }
            }
            return result;
        }

        IEnumerable<KeyValuePair<string, string>> ReadSecondaryReferences(XElement parentNode)
        {
            return parentNode.Elements("secondaryReference")
                .Select(x => new KeyValuePair<string, string>(
                            key: x.Element("type").Value,
                            value: x.Element("value").Value
                        )
                    );
        }

        IEnumerable<VerseRangeReference> ReadVerseReferences(XElement parentNode)
        {
            return parentNode.Elements("reference")
                .Select(x => VerseRangeReference.ParseXml(x));
        }

        string[] ReadReferenceDefinition(XElement rootNode)
        {
            return rootNode
                .Element("referenceDefinition")
                .Elements("definition")
                .Select(x => x.Value)
                .ToArray();
        }

        HadithReferenceDefinition[] ReadReferenceDefinitions(XElement rootNode)
        {
            var result = new List<HadithReferenceDefinition>();
            var referenceDefinitionsNode = rootNode.Element("referenceDefinitions");
            if (referenceDefinitionsNode != null)
            {
                foreach (XElement referenceDefinitionNode in referenceDefinitionsNode.Elements("referenceDefinition"))
                {
                    bool isPrimary = bool.Parse(referenceDefinitionNode.Element("isPrimary").Value);
                    string code = referenceDefinitionNode.Element("code").Value;
                    string name = referenceDefinitionNode.Element("name").Value;
                    string valuePrefix = referenceDefinitionNode.Element("valuePrefix")?.Value;
                    var partNames = new List<string>();
                    var partsNode = referenceDefinitionNode.Element("parts");
                    foreach (XElement partNode in partsNode.Elements("part"))
                        partNames.Add(partNode.Value);
                    var definition = new HadithReferenceDefinition(
                        isPrimary: isPrimary,
                        code: code,
                        name: name,
                        partNames: partNames,
                        valuePrefix: valuePrefix);
                    result.Add(definition);
                }
            }
            return result.ToArray();
        }

    }
}
