namespace Serpent.Common.Xml
{
    using System.Xml;

    public static class XmlTextReaderExtensions
    {
        public static string ReadContent(this XmlTextReader xmlTextReader)
        {
            if (xmlTextReader.EOF || xmlTextReader.IsEmptyElement)
            {
                return null;
            }

            xmlTextReader.Read();

            if (xmlTextReader.NodeType != XmlNodeType.CDATA && xmlTextReader.NodeType != XmlNodeType.Text)
            {
                return null;
            }

            return xmlTextReader.Value;
        }
    }
}