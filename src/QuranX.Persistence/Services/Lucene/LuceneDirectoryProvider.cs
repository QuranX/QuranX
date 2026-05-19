using Lucene.Net.Store;
using System;

namespace QuranX.Persistence.Services.Lucene;

public interface ILuceneDirectoryProvider
{
    Directory GetDirectory();
}


public class LuceneDirectoryProvider : ILuceneDirectoryProvider
{
    private readonly ISettings _settings;
    private readonly Lazy<Directory> _directory;

    public LuceneDirectoryProvider(ISettings settings)
    {
        _settings = settings;
        _directory = new Lazy<Directory>(() => FSDirectory.Open(_settings.DataPath));
    }

    public Directory GetDirectory() => _directory.Value;
}
