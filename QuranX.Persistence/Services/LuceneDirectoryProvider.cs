using System;
using Lucene.Net.Store;

namespace QuranX.Persistence.Services
{
	public interface ILuceneDirectoryProvider
	{
		Directory GetDirectory();
	}

	public class LuceneDirectoryProvider : ILuceneDirectoryProvider
	{
		private readonly Lazy<Directory> Directory;

		public LuceneDirectoryProvider(ISettings settings)
		{
			System.IO.Directory.CreateDirectory(settings.DataPath);
			Directory = new Lazy<Directory>(() => FSDirectory.Open(settings.DataPath));
		}

		public Directory GetDirectory()
		{
			return Directory.Value;
		}
	}
}
