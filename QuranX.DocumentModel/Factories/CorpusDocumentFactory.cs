using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuranX.DocumentModel;
using System.Xml.Linq;
using System.IO;

namespace QuranX.DocumentModel.Factories
{
	public class CorpusDocumentFactory
	{
		CorpusDocument Document;
		string GeneratedCorpusXmlFilePath;

		public CorpusDocument Create(string generatedCorpusXmlFilePath)
		{
			GeneratedCorpusXmlFilePath = generatedCorpusXmlFilePath;
			Document = new CorpusDocument();
			ReadDocument();
			return Document;
		}

		void ReadDocument()
		{
			var xmlDoc = XDocument.Load(File.OpenText(GeneratedCorpusXmlFilePath));
			var verses =
				from c in xmlDoc.Document.Root.Elements("chapter")
				from v in c.Elements("verse")
				select new Tuple<int, XElement>(
						int.Parse(c.Attribute("index").Value),
						v
					);

			ReadVerses(verses);
		}

		void ReadVerses(IEnumerable<Tuple<int, XElement>> verses)
		{
			foreach(var verse in verses)
				ReadVerse(verse);
		}

		void ReadVerse(Tuple<int, XElement> verse)
		{
			int chapterIndex = verse.Item1;
			XElement verseNode = verse.Item2;
			int verseIndex = int.Parse(verseNode.Attribute("index").Value);
			string arabicText = verseNode.Element("arabicText").Value;

			var reference = new VerseReference(
					chapter: verse.Item1,
					verse: verseIndex
				);

			var corpusVerse = new CorpusVerse(
					reference: reference,
					arabicText: arabicText
				);
			Document.AddVerse(corpusVerse);
			ReadVerseWords(verseNode, corpusVerse);
		}

		void ReadVerseWords(XElement verseNode, CorpusVerse corpusVerse)
		{
			var wordNodes = verseNode
				.Element("words")
				.Elements("word")
				.ToList();
			for (int index = 0; index < wordNodes.Count; index++)
			{
				XElement currentWordNode = wordNodes[index];
				ReadVerseWord(
						wordNode: currentWordNode,
						corpusVerse: corpusVerse,
						index: index
					);
			}
		}

		void ReadVerseWord(XElement wordNode, CorpusVerse corpusVerse, int index)
		{
			string buckwalter = wordNode.Element("buckwalter").Value;
			string english = wordNode.Element("english").Value;
			var reference = new MultiPartReference(
					new string[] { 
								corpusVerse.Reference.Chapter.ToString(),
								corpusVerse.Reference.Verse.ToString(),
								(index + 1).ToString()
							}
				);

			var corpusVerseWord = new CorpusVerseWord(
					index: index + 1,
					buckwalter: buckwalter,
					english: english
				);
			corpusVerse.AddWord(corpusVerseWord);

			ReadWordParts(wordNode, corpusVerseWord);
		}

		void ReadWordParts(XElement wordNode,CorpusVerseWord corpusVerseWord)
		{
			var partNodes = wordNode
				.Element("wordParts")
				.Elements("wordPart")
				.ToList();
			for (int index = 0; index < partNodes.Count; index++)
			{
				XElement currentWordPartNode = partNodes[index];
				ReadWordPart(currentWordPartNode, corpusVerseWord, index);
			}
		}

		void ReadWordPart(XElement wordPartNode, CorpusVerseWord corpusVerseWord, int index)
		{
			string typeCode = wordPartNode.Element("type").Value;
			string root = wordPartNode.Element("root").Value;
			string[] decorators = wordPartNode
				.Descendants("decorator")
				.Select(x => x.Value)
				.ToArray();
			var wordPart = new CorpusVerseWordPart(
				index: index,
				typeCode: typeCode,
				root: root,
				decorators: decorators
			);
			corpusVerseWord.AddPart(wordPart);
		}

	}
}
