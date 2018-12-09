using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Models
{
	public class VerseAnalysisWordPart
	{
		public string Root { get; }
		public string Type { get; }
		public string Form { get; }
		public string Description { get; }
		public IReadOnlyList<string> Decorators { get; }

		public VerseAnalysisWordPart(
			string root,
			string type,
			string form,
			string description,
			IEnumerable<string> decorators)
		{
			if (decorators == null)
				throw new ArgumentNullException(nameof(decorators));

			Root = root;
			Type = type;
			Form = form;
			Description = description;
			Decorators = decorators.ToList().AsReadOnly();
		}
	}
}
