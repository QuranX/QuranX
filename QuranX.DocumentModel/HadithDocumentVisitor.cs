using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel
{
	public class HadithDocumentVisitor
	{
		protected virtual void VisitDocument(HadithDocument document)
		{
			VisitCollections(document.Collections);
		}

		protected virtual void VisitCollections(IEnumerable<HadithCollection> collections)
		{
			foreach (var collection in collections)
				VisitCollection(collection);
		}

		protected virtual void VisitCollection(HadithCollection collection)
		{
			var groupedHadiths = collection.Hadiths
				.GroupBy(x => x.PrimaryReference[0]);
			VisitHadithGroups(groupedHadiths);
		}

		protected virtual void VisitHadithGroups(IEnumerable<IGrouping<string, Hadith>> groupedHadiths)
		{
			foreach (var group in groupedHadiths)
				VisitHadiths(group);
		}

		protected virtual void VisitHadiths(IGrouping<string, Hadith> group)
		{
			foreach (var hadith in group)
				VisitHadith(hadith);
		}

		protected virtual void VisitHadith(Hadith hadith)
		{
		}
	}
}
