using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel;

public class Chapter
{
    readonly Dictionary<int, Verse> _Verses;
    public readonly int Number;
    public readonly string EnglishName;
    public readonly string ArabicName;

    public Chapter(int number, string englishName, string arabicName)
    {
        _Verses = new Dictionary<int, Verse>();
        Number = number;
        EnglishName = englishName;
        ArabicName = arabicName;
    }

    public string FullName
    {
        get
        {
            return string.Format(
                    "{0} - {1}",
                    EnglishName,
                    ArabicName
                );
        }
    }
    public IEnumerable<Verse> Verses
    {
        get
        {
            return _Verses
                .Select(x => x.Value)
                .OrderBy(x => x.Index);
        }
    }

    public int VerseCount
    {
        get { return _Verses.Count; }
    }

    public Verse this[int index]
    {
        get
        {
            return _Verses[index];
        }
    }

    public void AddVerse(Verse verse)
    {
        _Verses.Add(verse.Index, verse);
    }

}
