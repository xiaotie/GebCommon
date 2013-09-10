using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Geb.Utils.Collections
{
	[XmlRoot("dictionary")]
	public class SerializableDictionary<K, V> : Dictionary<K, V>, IXmlSerializable
	{
		#region IXmlSerializable Membres

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			XmlSerializer keySer = new XmlSerializer(typeof(K));
			XmlSerializer valueSer = new XmlSerializer(typeof(V));

			reader.Read();
			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				reader.ReadStartElement("item");

				reader.ReadStartElement("key");
				K key = (K)keySer.Deserialize(reader);
				reader.ReadEndElement();

				reader.ReadStartElement("value");
				V value = (V)valueSer.Deserialize(reader);
				reader.ReadEndElement();

				Add(key, value);

				reader.ReadEndElement();
				reader.MoveToContent();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			XmlSerializer keySer = new System.Xml.Serialization.XmlSerializer(typeof(K));
			XmlSerializer valueSer = new System.Xml.Serialization.XmlSerializer(typeof(V));
			foreach (K key in Keys)
			{
				writer.WriteStartElement("item");

				writer.WriteStartElement("key");
				keySer.Serialize(writer, key);
				writer.WriteEndElement();

				writer.WriteStartElement("value");
				V value = this[key];
				valueSer.Serialize(writer, value);
				writer.WriteEndElement();

				writer.WriteEndElement();
			}
		}

		#endregion
	}
}
