using Lucene.Net.Store;

namespace QuranX.Persistence.Services
{
	public interface ILuceneDirectoryProvider
	{
		Directory GetDirectory();
	}

	public class LuceneDirectoryProvider : ILuceneDirectoryProvider
	{
		private readonly Directory _directory;

		public LuceneDirectoryProvider(ISettings settings)
		{
			System.IO.Directory.CreateDirectory(settings.DataPath);
			_directory = FSDirectory.Open(settings.DataPath);
		}

		public Directory GetDirectory()
		{
			return _directory;
		}
	}
}
