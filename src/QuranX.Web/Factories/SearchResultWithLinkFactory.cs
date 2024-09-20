using System;
using Lucene.Net.Documents;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Web.Views.Search;

namespace QuranX.Web.Services
{
	public interface ISearchResultWithLinkFactory
	{
		SearchResultWithLink Create(SearchResult searchResult);
	}


	public class SearchResultWithLinkMapper : ISearchResultWithLinkFactory
	{
		private readonly ICommentatorRepository CommentatorRepository;
		private readonly IHadithCollectionRepository HadithCollectionRepository;

		public SearchResultWithLinkMapper(
			ICommentatorRepository commentatorRepository,
			IHadithCollectionRepository hadithCollectionRepository)
		{
			CommentatorRepository = commentatorRepository;
			HadithCollectionRepository = hadithCollectionRepository;
		}

		public SearchResultWithLink Create(SearchResult searchResult)
		{
			GetUrl(searchResult, out string url, out string caption);
			var result = new SearchResultWithLink(
				url: url,
				caption: caption,
				snippets: searchResult.Snippets);
			return result;
		}

		public void GetUrl(SearchResult searchResult, out string url, out string caption)
		{
			switch (searchResult.Type)
			{
				case nameof(Verse):
					GetQuranUrl(searchResult.Document, out url, out caption);
					break;

				case nameof(Commentary):
					GetCommentaryUrl(searchResult.Document, out url, out caption);
					break;

				case nameof(Hadith):
					GetHadithUrl(searchResult.Document, out url, out caption);
					break;

				default:
					throw new NotImplementedException(searchResult.Type);
			}
		}

		private void GetQuranUrl(Document document, out string url, out string caption)
		{
			int chapterNumber = document.GetStoredValue<Verse>(x => x.ChapterNumber);
			int verseNumber = document.GetStoredValue<Verse>(x => x.VerseNumber);
			caption = $"Quran {chapterNumber}.{verseNumber}";
			url = $"/{chapterNumber}.{verseNumber}";
		}

		private void GetCommentaryUrl(Document document, out string url, out string caption)
		{
			int chapterNumber = document.GetStoredValue<Commentary>(x => x.ChapterNumber);
			int verseNumber = document.GetStoredValue<Commentary>(x => x.FirstVerseNumber);
			string commentatorCode = document.GetStoredValue<Commentary>(x => x.CommentatorCode);
			CommentatorRepository.TryGet(commentatorCode, out Commentator commentator);
			string commentatorDescription = commentator?.Description;
			url = $"/Tafsir/{commentatorCode}/{chapterNumber}.{verseNumber}";
			caption = $"Commentary by {commentatorDescription} for {chapterNumber}.{verseNumber}";
		}

		private void GetHadithUrl(Document document, out string url, out string caption)
		{
			string collectionCode = document.GetStoredValue<Hadith>(x => x.CollectionCode);
			string primaryReferenceCode = document.GetStoredValue<Hadith>(x => x.PrimaryReferenceCode);
			string primaryReferencePath = document.GetStoredValue<Hadith>(x => x.PrimaryReferencePath);

			HadithCollection collection = HadithCollectionRepository.Get(collectionCode);
			url = $"/Hadith/{collectionCode}/{primaryReferenceCode}/{primaryReferencePath}";
			caption = $"{collection.Name} {primaryReferencePath.Replace("/", ", ").Replace("-", " ")}";
		}
	}
}