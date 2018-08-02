using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace QuranX.DocumentModel.XmlStreaming
{
    public class HadithCollectionXmlWriter
    {
        HadithCollection Collection;
        XmlWriter Xml;

        public HadithCollectionXmlWriter(HadithCollection collection)
        {
            this.Collection = collection;
        }

        public void WriteXml(string filePath)
        {
            using (Xml = XmlWriter.Create(filePath, new XmlWriterSettings { Encoding = Encoding.Unicode, Indent = true }))
            {
                Xml.WriteStartDocument();
                WriteRootElement();
                Xml.WriteEndDocument();
            }
        }

        void WriteRootElement()
        {
            using (Xml.WriteElement("hadithCollection"))
            {
                Xml.WriteElementString("code", Collection.Code);
                Xml.WriteElementString("name", Collection.Name);
                Xml.WriteElementString("copyright", Collection.Copyright);
                WriteHadithReferenceDefinitions();
                WriteHadiths();
            }
        }

        void WriteHadithReferenceDefinitions()
        {
            using (Xml.WriteElement("referenceDefinitions"))
            {
                foreach (var definition in Collection.ReferenceDefinitions)
                    WriteHadithReferenceDefinition(definition);
            }
        }

        void WriteHadithReferenceDefinition(HadithReferenceDefinition definition)
        {
            using (Xml.WriteElement("referenceDefinition"))
            {
                Xml.WriteElementString("isPrimary", definition.IsPrimary + "");
                Xml.WriteElementString("code", definition.Code);
                Xml.WriteElementString("name", definition.Name);
                Xml.WriteElementString("valuePrefix", definition.ValuePrefix);
                using (Xml.WriteElement("parts"))
                {
                    foreach (string partName in definition.PartNames)
                        Xml.WriteElementString("part", partName);
                }
            }
        }

        void WriteHadiths()
        {
            using (Xml.WriteElement("hadiths"))
                foreach (Hadith hadith in Collection.Hadiths.OrderBy(x => x.PrimaryReference))
                    WriteHadith(hadith);
        }

        void WriteHadith(Hadith hadith)
        {
            using (Xml.WriteElement("hadith"))
            {
                WriteHadithReferences(hadith);
                WriteHadithText(hadith);
                WriteHadithVerseReferences(hadith);
            }
        }

        void WriteHadithReferences(Hadith hadith)
        {
            using (Xml.WriteElement("references"))
                foreach (var reference in hadith.References.OrderBy(x => x.Code))
                    WriteHadithReference(reference);
        }

        void WriteHadithReference(HadithReference reference)
        {
            using (Xml.WriteElement("reference"))
            {
                Xml.WriteElementString("code", reference.Code);
                Xml.WriteElementString("suffix", reference.Suffix);
                using (Xml.WriteElement("parts"))
                    foreach (string partValue in reference)
                        Xml.WriteElementString("part", partValue);
            }
        }

        void WriteHadithReference(MultiPartReference reference)
        {
            using (Xml.WriteElement("reference"))
            {
                foreach (string partValue in reference)
                    Xml.WriteElementString("part", partValue);
            }
        }

        void WriteHadithText(Hadith hadith)
        {
            WriteHadithParagraphs("arabic", hadith.ArabicText);
            WriteHadithParagraphs("english", hadith.EnglishText);
        }

        void WriteHadithParagraphs(string elementName, IEnumerable<string> paragraphs)
        {
            paragraphs = paragraphs ?? new string[0];
            using (Xml.WriteElement(elementName))
                foreach (string paragraph in paragraphs)
                    Xml.WriteElementString("text", paragraph);
        }

        void WriteHadithVerseReferences(Hadith hadith)
        {
            using (Xml.WriteElement("verseReferences"))
                foreach (var verseReference in hadith.VerseReferences)
                    WriteHadithVerseReferences(verseReference);
        }

        void WriteHadithVerseReferences(VerseRangeReference verseReference)
        {
            using (Xml.WriteElement("reference"))
            {
                Xml.WriteElementString("chapter", verseReference.Chapter + "");
                Xml.WriteElementString("firstVerse", verseReference.FirstVerse + "");
                Xml.WriteElementString("lastVerse", verseReference.LastVerse + "");
            }
        }
    }
}
