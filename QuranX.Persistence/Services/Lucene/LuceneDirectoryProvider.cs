using System;
using Lucene.Net.Store;

namespace QuranX.Persistence.Services.Lucene
{
	public interface ILuceneDirectoryProvider
	{
		Directory GetDirectory();
	}


	public class LuceneDirectoryProvider : ILuceneDirectoryProvider
	{
		private static object _syncRoot = new Object();
		private static ISettings _settings;
		private static Lazy<Directory> _directory = new Lazy<Directory>(() => FSDirectory.Open(_settings.DataPath));

		public LuceneDirectoryProvider(ISettings settings)
		{
			if (_settings == null)
			{
				lock(_syncRoot)
				{
					_settings = _settings ?? settings;
				}
			}
		}

		public Directory GetDirectory()
		{
			return _directory.Value;
		}
	}
}