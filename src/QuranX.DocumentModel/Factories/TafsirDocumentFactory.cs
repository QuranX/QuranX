using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuranX.DocumentModel;
using System.IO;

namespace QuranX.DocumentModel.Factories
{
	public class TafsirDocumentFactory
	{
		TafsirDocument Document;
		string GeneratedTafsirsDirectory;

		public TafsirDocument Create(string generatedTafsirsDirectory)
		{
			Document = new TafsirDocument();
			GeneratedTafsirsDirectory = generatedTafsirsDirectory;
			ReadTafsirs();
			return Document;
		}

		void ReadTafsirs()
		{
			foreach (string tafsirFilePath in Directory.GetFiles(GeneratedTafsirsDirectory, "*.xml"))
			{
				var tafsir = new TafsirFactory().Create(tafsirFilePath);
				Document.AddTafsir(tafsir);
			}
		}

	}
}
