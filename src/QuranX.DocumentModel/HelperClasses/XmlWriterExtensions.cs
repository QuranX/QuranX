using QuranX.DocumentModel.HelperClasses;
using System;
using System.Xml;

namespace QuranX.DocumentModel
{
    public static class XmlWriterExtensions
    {
        public static IDisposable WriteElement(this XmlWriter writer, string name)
        {
            writer.WriteStartElement(name);
            return new DisposableAction(() => writer.WriteEndElement());
        }
    }
}
