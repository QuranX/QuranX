using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lucene.Net.Documents;
using QuranX.Persistence.Services;

namespace QuranX.Persistence.Extensions
{
	public static class DocumentExtensions
	{
		[Flags]
		private enum IndexKind
		{
			Store = 1,
			Index = 2,
			StoreAndIndex = 3,
			FullText = 4
		}

		public static string GetStoredValue<TObj>(
			this Document document,
			Expression<Func<TObj, string>> expression)
		{
			string name = ExpressionExtensions.GetIndexName(expression);
			string value = document.GetField(name)?.StringValue;
			return value;
		}

		public static int GetStoredValue<TObj>(
			this Document document,
			Expression<Func<TObj, int>> expression)
		{
			string name = ExpressionExtensions.GetIndexName(expression);
			string value = document.GetField(name)?.StringValue ?? "0";
			return int.Parse(value);
		}

		public static Document Index<TObj>(
			this Document document,
			TObj instance,
			Expression<Func<TObj, string>> expression)
		{
			expression.GetIndexNameAndPropertyValue(instance, out string name, out string value);
			Add(document, name, value, IndexKind.Index);
			return document;
		}

		public static Document IndexArray<TObj>(
			this Document document,
			TObj instance,
			Expression<Func<TObj, IEnumerable<string>>> expression)
		{
			expression.GetIndexNameAndPropertyValue(instance, out string name, out IEnumerable<string> values);
			foreach (string value in values)
				Add(document, name, value, IndexKind.Index);
			return document;
		}

		public static Document StoreAndIndex<TObj>(
			this Document document,
			TObj instance,
			Expression<Func<TObj, string>> expression,
			Func<string, string> transform = null)
		{
			expression.GetIndexNameAndPropertyValue(instance, out string name, out string value);
			transform = transform ?? new Func<string, string>(x => x);
			if (value != null)
				Add(document, name, transform(value), IndexKind.StoreAndIndex);
			return document;
		}

		public static Document StoreAndIndex<TObj>(
			this Document document,
			TObj instance,
			Expression<Func<TObj, int>> expression)
		{
			expression.GetIndexNameAndPropertyValue(instance, out string name, out int value);
			document.StoreAndIndex(name, value);
			return document;
		}

		public static Document StoreAndIndex<TObj>(
			this Document document,
			TObj instance,
			Expression<Func<TObj, int?>> expression)
		{
			expression.GetIndexNameAndPropertyValue(instance, out string name, out int? value);
			if (value.HasValue)
				document.StoreAndIndex(name, value.Value);
			return document;
		}

		public static Document StoreAndIndex(
			this Document document,
			string indexName,
			int value)
		{
			var field =
				new NumericField(
					name: indexName,
					precisionStep: 1,
					store: Field.Store.YES,
					index: true)
				.SetIntValue(value);
			document.Add(field);
			return document;
		}

		public static Document StoreAndIndex(
			this Document document,
			string indexName,
			string value)
		{
			Add(document, indexName, value, IndexKind.StoreAndIndex);
			return document;
		}

		public static Document AddSearchableText(this Document document, string text, float boost)
		{
			if (text == null)
				throw new ArgumentNullException(nameof(text));

			document.Boost = boost;
			Add(document, Consts.FullTextFieldName, text, IndexKind.FullText);
			return document;
		}

		public static Document AddSearchableText(this Document document, IEnumerable<string> texts, float boost)
		{
			if (texts == null)
				throw new ArgumentNullException(nameof(texts));

			foreach (string text in texts)
			{
				AddSearchableText(document, text, boost);
			}

			return document;
		}

		public static Document AddObject<T>(this Document document, T instance)
		{
			string json = Newtonsoft.Json.JsonConvert.SerializeObject(instance);
			Add(document, Consts.SerializedObjectFieldName, json, IndexKind.Store);
			Add(document, Consts.SerializedObjectTypeFieldName, typeof(T).Name, IndexKind.StoreAndIndex);
			return document;
		}

		public static T GetObject<T>(this Document document)
		{
			string json = document.Get(Consts.SerializedObjectFieldName);
			T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
			return result;
		}

		private static void Add(Document document, string fieldName, string value, IndexKind indexKind)
		{
			if (value == null)
				return;

			bool fullText = indexKind.HasFlag(IndexKind.FullText);
			bool store = fullText || indexKind.HasFlag(IndexKind.Store);
			bool index = fullText || indexKind.HasFlag(IndexKind.Index);
			var field = new Field(
				name: fieldName,
				value: value,
				store: store ? Field.Store.YES : Field.Store.NO,
				index: index ? Field.Index.ANALYZED : Field.Index.NO,
				termVector: fullText ? Field.TermVector.WITH_POSITIONS_OFFSETS : Field.TermVector.NO);
			document.Add(field);
		}
	}
}
