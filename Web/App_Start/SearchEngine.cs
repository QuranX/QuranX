using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.AR;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Vectorhighlight;
using Lucene.Net.Store;

namespace QuranX
{
	public static class SearchEngine
	{
		public class SearchResult
		{
			public readonly string Type;
			public readonly string ID;
			public readonly string[] Snippets;

			public SearchResult(
				string type,
				string id,
				IEnumerable<string> snippets)
			{
				this.Type = type;
				this.ID = id;
				if (type == "Quran")
					snippets = snippets.Take(1);
				this.Snippets = snippets.ToArray();
			}
		}

		static Analyzer Analyzer;
		static Directory Index;

		static SearchEngine()
		{
			string luceneData = HttpContext.Current.Server.MapPath("~/app_data/Lucene");
			Analyzer =
				new ArabicAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
			bool needsIndexCreation = false;
			if (!System.IO.Directory.Exists(luceneData))
			{
				System.IO.Directory.CreateDirectory(luceneData);
				needsIndexCreation = true;
			}
			Index = new SimpleFSDirectory(new System.IO.DirectoryInfo(luceneData));
			ISet<string> stopWords = GenerateStopWords();
			if (needsIndexCreation)
				WriteIndexes();
		}

		public static void Initialize()
		{
			//Just ensures the class constructor is called
			SimpleFragListBuilder.MARGIN = 32;
		}

		public static IEnumerable<SearchResult> Search(
			string queryString,
			out int totalResults,
			int maxResults = 100)
		{
#if DEBUG
			maxResults = 2000;
#endif
			totalResults = 0;
			if (string.IsNullOrEmpty(queryString))
				return new List<SearchResult>();

			var indexReader = DirectoryReader.Open(
				directory: Index,
				readOnly: true);
			var indexSearcher = new IndexSearcher(indexReader);
			var queryParser = new QueryParser(
				Lucene.Net.Util.Version.LUCENE_30,
				"Body",
				Analyzer
			);
			queryParser.AllowLeadingWildcard = true;
			queryParser.DefaultOperator = QueryParser.Operator.AND;
			var query = queryParser.Parse(queryString);

			var resultsCollector = TopScoreDocCollector.Create(
				numHits: maxResults,
				docsScoredInOrder: true
			);
			indexSearcher.Search(
				query: query,
				results: resultsCollector
			);
			totalResults = resultsCollector.TotalHits;
			var result = new List<SearchResult>();

			var fvh = new FastVectorHighlighter();
			var fq = fvh.GetFieldQuery(query);
			foreach (var scoreDoc in resultsCollector.TopDocs().ScoreDocs)
			{
				string[] fragments = fvh.GetBestFragments(fq, indexSearcher.IndexReader, scoreDoc.Doc, "Body", 100, 5);
				var doc = indexSearcher.Doc(scoreDoc.Doc);
				var searchResult = new SearchResult(
					type: doc.Get("Type"),
					id: doc.Get("ID"),
					snippets: fragments
				);
				result.Add(searchResult);
			}
			return result;
		}

		static void WriteIndexes()
		{
			//Note that when this site is run for the first
			//time it is necessary to create the lucene full text index
			//for searching, and it may take some time
			System.Diagnostics.Debugger.Break();
			var maxFieldLength = new IndexWriter.MaxFieldLength(int.MaxValue);
			using (var indexWriter = new IndexWriter(Index, Analyzer, maxFieldLength))
			{
				IndexVerses(indexWriter);
				IndexTafsirs(indexWriter);
				IndexHadiths(indexWriter);
				indexWriter.Commit();
			}
		}

		static void IndexVerses(IndexWriter indexWriter)
		{
			foreach (var chapter in SharedData.Document.QuranDocument.Chapters)
			{
				foreach (var verse in chapter.Verses)
				{
					var doc = new Document();
					string body = string.Join(
						separator: " ",
						values: verse.Translations
									.Select(x => x.Text)
					)
						+ " "
						+ ArabicHelper.Standardize(verse.ArabicText);

					doc.Add(CreateField(name: "Type", value: "Quran"));
					doc.Add(CreateField(
						name: "ID",
						value: string.Format("{0}.{1}", chapter.Index, verse.Index)
						)
					);
					doc.Add(CreateField(name: "Body", value: body));
					indexWriter.AddDocument(doc);
				}
			}
		}

		static void IndexTafsirs(IndexWriter indexWriter)
		{
			foreach (var tafsir in SharedData.Document.TafsirDocument.Tafsirs)
			{
				foreach (var commentary in tafsir.Comments)
				{
					var doc = new Document();
					string body = string.Join(
						separator: " ",
						values: commentary.Text.Select(x => x)
					);
					doc.Add(CreateField(name: "Type", value: "Tafsir"));
					doc.Add(CreateField(
						name: "ID",
						value: string.Format(
							"{0}/{1}", tafsir.Code, commentary.VerseReference)
						)
					);
					doc.Add(CreateField(name: "Group", value: tafsir.Code));
					doc.Add(CreateField(name: "Body", value: body));
					indexWriter.AddDocument(doc);
				}
			}
		}

		static void IndexHadiths(IndexWriter indexWriter)
		{
			foreach (var collection in SharedData.Document.HadithDocument.Collections)
			{
				foreach (var hadith in collection.Hadiths)
				{
					string body =
						string.Join(
							separator: " ",
							values: hadith.EnglishText.Select(x => x)
						)
						+ string.Join(
							separator: " ",
							values: hadith.ArabicText.Select(x => x)
						);

					//Add indexes
					var referencesBuilder = new StringBuilder();
					referencesBuilder.AppendLine();
					foreach (var reference in hadith.References)
						referencesBuilder.AppendLine(reference + "");
					body += referencesBuilder.ToString();

					var doc = new Document();
					var primaryReference = hadith.PrimaryReference ?? hadith.References.First();
					doc.Add(CreateField(name: "ID", value: string.Format(
						"{0}/{1}/{2}", collection.Code, primaryReference.Code, primaryReference
						))
					);
					doc.Add(CreateField(name: "Type", value: "Hadith"));
					doc.Add(CreateField(name: "Group", value: collection.Code));
					doc.Add(CreateField(
						name: "Body",
						value: body
							.Replace("\r", "")
							.Replace("\n", " ")
						)
					);
					indexWriter.AddDocument(doc);
				}
			}
		}

		static Field CreateField(string name, string value)
		{
			return new Field(
				name: name,
				value: value,
				store: Field.Store.YES,
				index: Field.Index.ANALYZED,
				termVector: Field.TermVector.WITH_POSITIONS_OFFSETS
				);
		}

		static ISet<string> GenerateStopWords()
		{
			var result = new HashSet<string>(new string[] {
				"a", "am", "an", "and", "as", "at", "but",
				"by", "do", "i", "if", "in", "is", "it",
				"its", "me", "my", "no", "or",
				"so", "than", "that", "they", "us", "was",
				"when", "who", "whom", "why"
				});
			return result;
		}

	}
}