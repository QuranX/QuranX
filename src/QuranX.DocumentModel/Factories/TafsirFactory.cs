using System.IO;
using System.Linq;
using System.Xml.Linq;
using QuranX.DocumentModel;
using QuranX.Shared.Models;

namespace QuranX.DocumentModel.Factories
{
	public class TafsirFactory
	{
		Tafsir Tafsir;

		public Tafsir Create(string tafsirFilePath)
		{
			var doc = XDocument.Load(File.OpenText(tafsirFilePath));
			var tafsirNode = doc.Document.Root;

			string code = tafsirNode.Element("code").Value;
			string mufassir = tafsirNode.Element("mufassir").Value;
			string copyRight = tafsirNode.Element("copyright").Value;
			bool isTafsir = tafsirNode.Element("isTafsir").Value == "Y";

			Tafsir = new Tafsir(
					code: code,
					mufassir: mufassir,
					isTafsir: isTafsir,
					copyright: copyRight
				);
			ReadChapters(tafsirNode);
			return Tafsir;
		}

		void ReadChapters(XElement tafsirNode)
		{
			foreach (XElement chapterNode in tafsirNode.Elements("chapter"))
			{
				int chapterIndex = int.Parse(chapterNode.Attribute("index").Value);
				ReadCommentaries(chapterNode, chapterIndex);
			}
		}

		void ReadCommentaries(XElement chapterNode, int chapterIndex)
		{
			foreach (XElement commentaryNode in chapterNode.Elements("commentary"))
			{
				ReadCommentary(chapterIndex, commentaryNode);
			}
		}

		void ReadCommentary(int chapterIndex, XElement commentaryNode)
		{
			int firstVerse = int.Parse(commentaryNode.Element("firstVerse").Value);
			int lastVerse = int.Parse(commentaryNode.Element("lastVerse").Value);
			var verseRangeReference = new VerseRangeReference(
					chapter: chapterIndex,
					firstVerse: firstVerse,
					lastVerse: lastVerse
				);
			var text = commentaryNode.Elements("text").Select(x => x.Value);

			var comment = new TafsirComment(
					verseReference: verseRangeReference,
					text: text
				);
			Tafsir.AddComment(comment);
		}
	}
}
