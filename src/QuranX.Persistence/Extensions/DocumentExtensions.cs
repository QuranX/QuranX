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
			string value = document.GetField(name)?.GetStringValue();
			return value;
		}

		public static int GetStoredValue<TObj>(
			this Document document,
			Expression<Func<TObj, int>> expression)
		{
			string name = ExpressionExtensions.GetIndexName(expression);
			string value = document.GetField(name)?.GetStringValue() ?? "0";
			return int.Parse(value);
		}

		public static Field Index<TObj>(
			this Document document,
			TObj instance,
			Expression<Func<TObj, string>> expression)
		{
			expression.GetIndexNameAndPropertyValue(instance, out string name, out string value);
			Field field = Add(document, name, value, IndexKind.Index);
			return field;
		}

		public static Document IndexArray(
			this Document document,
			string name, IEnumerable<string> values)
		{
			foreach (string value in values)
				Add(document, name, value, IndexKind.Index);
			return document;
		}

		public static Document IndexArray<TObj>(
			this Document document,
			TObj instance,
			Expression<Func<TObj, IEnumerable<string>>> expression)
		{
			expression.GetIndexNameAndPropertyValue(instance, out string name, out IEnumerable<string> values);
			return document.IndexArray(name, values);
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
			{
				Field field = Add(document, name, transform(value), IndexKind.StoreAndIndex);
			}
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
			var fieldType = new FieldType {
				IsStored = true,
				IsIndexed = true,
				IsTokenized = false,
				NumericType = NumericType.INT32,
				NumericPrecisionStep = 1 // Set precision step to 1
			};
			fieldType.Freeze();

			Field field = new Int32Field(indexName, value, fieldType);
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

		public static Document AddSearchableText(this Document document, string text, float boostValue)
		{
			if (text == null)
				throw new ArgumentNullException(nameof(text));

			Add(document, Consts.FullTextFieldName, text, IndexKind.FullText, boostValue);
			return document;
		}

		public static Document AddSearchableText(this Document document, IEnumerable<string> texts, float boostValue)
		{
			if (texts == null)
				throw new ArgumentNullException(nameof(texts));

			foreach (string text in texts)
			{
				document.AddSearchableText(text, boostValue);
			}

			return document;
		}

		public static Document AddObject<T>(this Document document, T instance)
		{
			string json = Newtonsoft.Json.JsonConvert.SerializeObject(instance);
			Add(document, Consts.SerializedObjectFieldName, json, IndexKind.Store);
			Add(document, Consts.SerializedObjectTypeFieldName, typeof(T).Name.ToUpperInvariant(), IndexKind.StoreAndIndex);
			return document;
		}

		public static T GetObject<T>(this Document document)
		{
			string json = document.Get(Consts.SerializedObjectFieldName);
			T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
			return result;
		}

		private static Field Add(Document document, string fieldName, string value, IndexKind indexKind, float boostValue = 1.0f)
		{
			if (value == null)
				return null;

			bool fullText = indexKind.HasFlag(IndexKind.FullText);
			bool store = fullText || indexKind.HasFlag(IndexKind.Store);
			bool index = fullText || indexKind.HasFlag(IndexKind.Index);

			Field field;

			if (fullText)
			{
				// For full-text fields, use TextField and enable term vectors
				var fieldType = new FieldType(TextField.TYPE_NOT_STORED) {
					IsStored = store,
					IsIndexed = index,
					IsTokenized = true,
					StoreTermVectors = true,
					StoreTermVectorPositions = true,
					StoreTermVectorOffsets = true
				};
				fieldType.Freeze();

				field = new Field(fieldName, value, fieldType) {
					Boost = boostValue
				};
			}
			else if (index)
			{
				// Indexed but not full-text (not tokenized)
				field = new StringField(fieldName, value, store ? Field.Store.YES : Field.Store.NO) {
					Boost = boostValue
				};
			}
			else
			{
				// Stored but not indexed
				field = new StoredField(fieldName, value);
			}

			document.Add(field);
			return field;
		}

	}
}
