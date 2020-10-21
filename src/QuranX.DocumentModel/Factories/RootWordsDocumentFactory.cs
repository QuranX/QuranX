using System.Linq;
using System.Xml.Linq;
using QuranX.DocumentModel;

namespace QuranX.DocumentModel.Factories
{
	public class RootWordsDocumentFactory
	{
		public WordsDocument Create(string generatedCorpusXmlFilePath)
		{
			var corpusQuranDoc = XDocument.Load(generatedCorpusXmlFilePath);
			var flattened =
				from chapterNode in corpusQuranDoc.Document.Descendants("chapter")
				from verseNode in chapterNode.Elements("verse")
				from wordNode in verseNode.Element("words").Elements("word")
				from wordPartNode in wordNode.Element("wordParts").Elements("wordPart")
				where (!string.IsNullOrEmpty(wordPartNode.Element("root").Value))
				select new WordReference(
						root: wordPartNode.Element("root").Value,
						chapterIndex: int.Parse(chapterNode.Attribute("index").Value),
						verseIndex: int.Parse(verseNode.Attribute("index").Value),
						wordIndex: int.Parse(wordNode.Attribute("index").Value),
						wordPartIndex: int.Parse(wordPartNode.Attribute("index").Value),
						wordPartType: wordPartNode.Element("type").Value,
						wordPartTypeDescription: WordTypes.Values[wordPartNode.Element("type").Value],
						buckwalterText: wordNode.Element("buckwalter").Value,
						englishText: wordNode.Element("english").Value
					);
			
			var groupedByRoot = 
				from item in flattened
						group item by item.Root into groupedItems
						select groupedItems;

			var referencesByRoot = groupedByRoot
				.ToDictionary(
						x => x.Key,
						x => x
							.OrderBy(r => r.ChapterIndex)
							.ThenBy(r => r.VerseIndex)
							.ThenBy(r => r.WordIndex)
							.ThenBy(r => r.WordPartIndex)
							.ToList()
					);

			var document = new WordsDocument();
			foreach (var item in referencesByRoot)
			{
				document.AddRootWordReferenceGroup(item.Key, item.Value);
			}
			return document;
		}
	}
}
