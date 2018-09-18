using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using QuranX.Persistence.Services.Repositories;

namespace QuranX.Persistence.Extensions
{
	public static class DocumentExtensions
	{
		[Flags]
		private enum IndexKind
		{
			Store = 1,
			Index = 2,
			FullText = Store | Index
		}

		public static Document AddIndexed(Document document, string fieldName, string value)
		{
			Add(document, fieldName, value, IndexKind.Index);
			return document;
		}

		public static Document AddIndexed(this Document document, string fieldName, int value)
		{
			return AddIndexed(document, fieldName, value.ToString());
		}

		public static Document AddFullText(this Document document, string text)
		{
			return AddFullText(document, new string[] { text });
		}

		public static Document AddFullText(this Document document, IEnumerable<string> texts)
		{
			if (texts == null)
				throw new ArgumentNullException(nameof(texts));

			foreach (string text in texts)
			{
				Add(document, Consts.FullTextFieldName, text, IndexKind.FullText);
			}

			return document;
		}

		public static Document AddObject<T>(this Document document, T instance)
		{
			string json = Newtonsoft.Json.JsonConvert.SerializeObject(instance);
			Add(document, Consts.SerializedObjectFieldName, json, IndexKind.Store);
			Add(document, Consts.SerializedObjectTypeFieldName, typeof(T).Name, IndexKind.FullText);
			return document;
		}

		private static void Add(Document document, string fieldName, string value, IndexKind indexKind)
		{
			var field = new Field(
				name: fieldName,
				value: value,
				store: indexKind.HasFlag(IndexKind.Store) ? Field.Store.YES : Field.Store.NO,
				index: indexKind.HasFlag(IndexKind.Index) ? Field.Index.ANALYZED : Field.Index.NO,
				termVector: indexKind.HasFlag(IndexKind.FullText) ? Field.TermVector.WITH_POSITIONS_OFFSETS : Field.TermVector.NO);
			document.Add(field);
		}
	}
}
