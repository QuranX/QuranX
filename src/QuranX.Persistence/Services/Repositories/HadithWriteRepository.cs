using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IHadithWriteRepository
	{
		void Write(Hadith hadith);
	}

	public class HadithWriteRepository : IHadithWriteRepository
	{
		private readonly ILuceneIndexWriterProvider IndexWriterProvider;

		public HadithWriteRepository(ILuceneIndexWriterProvider indexWriterProvider)
		{
			IndexWriterProvider = indexWriterProvider;
		}

		public void Write(Hadith hadith)
		{
			if (hadith.Id <= 0)
				throw new ArgumentOutOfRangeException(nameof(Hadith.Id));

			float boostValue = DocumentWeights.Weights["Hadith-" + hadith.CollectionCode];
			var document = new Document();
			document.StoreAndIndex(hadith, x => x.Id);
			foreach (VerseRangeReference verseRangeReference in hadith.VerseRangeReferences)
			{
				string indexName = ExpressionExtensions.GetIndexName<Hadith, object>(x => x.VerseRangeReferences);
				document.StoreAndIndex(indexName, verseRangeReference.ToIndexValue());
			}
			document.AddSearchableText(hadith.ArabicText, boostValue);
			document.AddSearchableText(hadith.EnglishText, boostValue);
			document.StoreAndIndex(hadith, x => x.CollectionCode);
			document.StoreAndIndex(hadith, x => x.PrimaryReferenceCode);
			document.StoreAndIndex(hadith, x => x.PrimaryReferencePath);
			document.AddObject(hadith);

			IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
			indexWriter.AddDocument(document);

			WriteHadithReferences(indexWriter, hadith);
			WriteVerseReferences(indexWriter, hadith);
		}

		private void WriteHadithReferences(IndexWriter indexWriter, Hadith hadith)
		{
			foreach (HadithReference reference in hadith.References)
			{
				var doc = new Document();
				doc
					.AddObject(reference)
					.StoreAndIndex(reference, x => x.CollectionCode)
					.StoreAndIndex(reference, x => x.ReferenceCode, x => x.Replace("-", ""))
					.StoreAndIndex(reference, x => x.ReferenceValue1)
					.StoreAndIndex(reference, x => x.ReferenceValue1Suffix)
					.StoreAndIndex(reference, x => x.ReferenceValue2)
					.StoreAndIndex(reference, x => x.ReferenceValue2Suffix)
					.StoreAndIndex(reference, x => x.ReferenceValue3)
					.StoreAndIndex(reference, x => x.ReferenceValue3Suffix);
				indexWriter.AddDocument(doc);
			}
		}

		private void WriteVerseReferences(IndexWriter indexWriter, Hadith hadith)
		{
			IEnumerable<HadithVerseLink> hadithVerseLinks =
				hadith.VerseRangeReferences
				.SelectMany(x => x.ToVerseReferences())
				.Select(x => x.ToIndexValue())
				.Select(x => new HadithVerseLink(
					hadithId: hadith.Id,
					verseId: x));

			foreach (HadithVerseLink hadithVerseLink in hadithVerseLinks)
			{
				var doc = new Document();
				doc
					.StoreAndIndex(hadithVerseLink, x => x.HadithId)
					.StoreAndIndex(hadithVerseLink, x => x.VerseId)
					.AddObject(hadithVerseLink);
				indexWriter.AddDocument(doc);
			}
		}
	}
}
