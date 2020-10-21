using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuranX.DocumentModel;
using System.IO;
using System.Xml.Linq;

namespace QuranX.DocumentModel.Factories
{
	public class HadithDocumentFactory
	{
		HadithDocument Document;
		string GeneratedHadithsDirectory;
		string AdditionalHadithXRefsDirectory;

		public HadithDocument Create(string generatedHadithsDirectory, string additionalHadithXRefsDirectory)
		{
			this.Document = new HadithDocument();
			this.GeneratedHadithsDirectory = generatedHadithsDirectory;
			this.AdditionalHadithXRefsDirectory = additionalHadithXRefsDirectory;
			ReadHadithCollections();
			return Document;
		}

		void ReadHadithCollections()
		{
			foreach (string hadithFilePath in Directory.GetFiles(GeneratedHadithsDirectory, "*.xml"))
			{
				var factory = new HadithCollectionFactory();
				Document.AddCollection(factory.Create(hadithFilePath, AdditionalHadithXRefsDirectory));
			}
		}

	}
}
