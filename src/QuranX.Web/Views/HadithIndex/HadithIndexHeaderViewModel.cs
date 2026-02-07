using QuranX.Persistence.Models;
using System.Collections.Generic;

namespace QuranX.Web.Views.HadithIndex;

public class HadithIndexHeaderViewModel
{
    public readonly string SelectedReferenceCode;
    public readonly string UrlSoFar;
    public readonly HadithCollection Collection;
    public readonly IEnumerable<string> ReferencePartNamesAndValues;

    public HadithIndexHeaderViewModel(
        string selectedReferenceCode,
        string urlSoFar,
        HadithCollection collection,
        IEnumerable<string> referencePartNamesAndValues)
    {
        SelectedReferenceCode = selectedReferenceCode;
        UrlSoFar = urlSoFar;
        Collection = collection;
        ReferencePartNamesAndValues = referencePartNamesAndValues;
    }
}