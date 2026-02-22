using QuranX.Shared.Models;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace QuranX.DocumentModel.Factories;

public class QuranDocumentFactory
{
    QuranDocument Document;
    string GeneratedTranslationsDirectory;

    public QuranDocument Create(string generatedTranslationsDirectory)
    {
        Document = new QuranDocument();
        GeneratedTranslationsDirectory = generatedTranslationsDirectory;
        ReadArabic();
        ReadTranslations();
        return Document;
    }

    void ReadArabic()
    {
        var tanzilQuranDoc = XDocument.Parse(QuranX.DocumentModel.Properties.Resources.quran_uthmani);
        var tanzilQuranNode = tanzilQuranDoc.Document.Root;

        foreach (var arabicChapterNode in tanzilQuranNode.Descendants("sura"))
            ReadArabicChapter(arabicChapterNode);
    }

    void ReadArabicChapter(XElement arabicChapterNode)
    {
        int chapterNumber = int.Parse(arabicChapterNode.Attribute("index").Value);
        ChapterData chapterData = QuranStructure.Chapter(chapterNumber);

        var chapter = new Chapter(
                number: chapterNumber,
                englishName: chapterData.EnglishName,
                arabicName: chapterData.ArabicName
            );
        Document.AddChapter(chapter);
        ReadArabicVerses(
                chapterNode: arabicChapterNode,
                chapter: chapter
            );
    }

    void ReadArabicVerses(XElement chapterNode, Chapter chapter)
    {
        foreach (XElement verseNode in chapterNode.Descendants("aya"))
        {
            int verseIndex = int.Parse(verseNode.Attribute("index").Value);
            string arabicText = verseNode.Attribute("text").Value;

            var verse = new Verse(
                    index: verseIndex,
                    arabicText: arabicText
                );
            chapter.AddVerse(verse);
        }
    }

    void ReadTranslations()
    {
        foreach (string translationFilePath in Directory.GetFiles(GeneratedTranslationsDirectory, "*.xml"))
        {
            ReadTranslation(translationFilePath);
        }
    }

    void ReadTranslation(string translationFilePath)
    {
        var doc = XDocument.Load(File.OpenRead(translationFilePath));
        var quranNode = doc.Document.Root;
        string translatorCode = quranNode.Element("translatorCode").Value;
        string translatorName = quranNode.Element("translatorName").Value;

        foreach (XElement chapterNode in quranNode.Descendants("chapter"))
        {
            int chapterIndex = int.Parse(chapterNode.Attribute("index").Value);
            foreach (XElement verseNode in chapterNode.Descendants("verse"))
            {
                int verseIndex = int.Parse(verseNode.Attribute("index").Value);

                var verseReference = new VerseReference(
                        chapter: chapterIndex,
                        verse: verseIndex
                    );
                var verseTranslation = new VerseTranslation(
                        translatorCode: translatorCode,
                        translatorName: translatorName,
                        text: verseNode.Value
                    );

                AddTranslation(verseReference, verseTranslation);
            }
        }
    }

    void AddTranslation(
        VerseReference verseReference,
        VerseTranslation verseTranslation)
    {
        var verse = Document[verseReference.Chapter, verseReference.Verse];
        verse.AddTranslation(verseTranslation);
    }
}
