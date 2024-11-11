using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml;

namespace RIS_Naloga2
{
	internal class AdvancedXmlSchemaValidator
	{
		private readonly Dictionary<string, string> _schemaFiles;
		private readonly List<string> _validationErrors;
		private bool _isValid;

		public AdvancedXmlSchemaValidator(Dictionary<string, string> schemaFiles)
		{
			_schemaFiles = schemaFiles;
			_validationErrors = new List<string>();
			_isValid = true;
		}

		public (bool isValid, List<string> errors) ValidateXml(string xmlContent)
		{
			_validationErrors.Clear();
			_isValid = true;

			try
			{
				var settings = new XmlReaderSettings
				{
					ValidationType = ValidationType.Schema,
					ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation |
									XmlSchemaValidationFlags.ProcessInlineSchema |
									XmlSchemaValidationFlags.ReportValidationWarnings
				};

				settings.ValidationEventHandler += ValidationEventHandler;

				// Load all schemas
				var schemas = new XmlSchemaSet();
				foreach (var schemaFile in _schemaFiles)
				{
					using var schemaReader = XmlReader.Create(schemaFile.Value);
					var schema = XmlSchema.Read(schemaReader, ValidationEventHandler);
					schemas.Add(schema);
				}

				settings.Schemas = schemas;

				using var stringReader = new StringReader(xmlContent);
				using var xmlReader = XmlReader.Create(stringReader, settings);
				while (xmlReader.Read()) { }
			}
			catch (Exception ex)
			{
				_isValid = false;
				_validationErrors.Add($"Fatal error: {ex.Message}");
			}

			return (_isValid, _validationErrors);
		}

		private void ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			_isValid = false;
			_validationErrors.Add($"{e.Severity}: {e.Message}");
		}

		public static string GenerateValidXml()
		{
			var xml = new StringBuilder();
			xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			xml.AppendLine("<art:artikli xmlns:art=\"http://www.example.com/articles\"");
			xml.AppendLine("            xmlns:base=\"http://www.example.com/base-types\"");
			xml.AppendLine("            xmlns:contact=\"http://www.example.com/contact-types\"");
			xml.AppendLine("            version=\"1.0\">");
			xml.AppendLine("  <art:artikel dobavljiv=\"true\" tip=\"standard\">");
			xml.AppendLine("    <art:articleId>1</art:articleId>");
			xml.AppendLine("    <art:ime>Test Artikel</art:ime>");
			xml.AppendLine("    <art:cena>25.99</art:cena>");
			xml.AppendLine("    <art:zaloga>100</art:zaloga>");
			xml.AppendLine("    <art:dobaviteljInfo davcnaStevilka=\"12345678\">");
			xml.AppendLine("      <art:dobaviteljId>2</art:dobaviteljId>");
			xml.AppendLine("      <art:kontakt type=\"primary\" active=\"true\">test@example.com</art:kontakt>");
			xml.AppendLine("      <art:naslov addressType=\"both\">");
			xml.AppendLine("        <contact:street>Slovenska cesta 1</contact:street>");
			xml.AppendLine("        <contact:city>Ljubljana</contact:city>");
			xml.AppendLine("        <contact:postalCode>1000</contact:postalCode>");
			xml.AppendLine("        <contact:country>Slovenija</contact:country>");
			xml.AppendLine("        <contact:additionalInfo>Poslovna stavba</contact:additionalInfo>");
			xml.AppendLine("        <contact:coordinates>");
			xml.AppendLine("          <contact:latitude>46.0569</contact:latitude>");
			xml.AppendLine("          <contact:longitude>14.5058</contact:longitude>");
			xml.AppendLine("        </contact:coordinates>");
			xml.AppendLine("      </art:naslov>");
			xml.AppendLine("    </art:dobaviteljInfo>");
			xml.AppendLine("    <art:datumZadnjeNabave>2024-11-11T10:00:00</art:datumZadnjeNabave>");
			xml.AppendLine("  </art:artikel>");
			xml.AppendLine("</art:artikli>");

			return xml.ToString();
		}

		public static string GenerateInvalidXml()
		{
			var xml = new StringBuilder();
			xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			xml.AppendLine("<art:artikli xmlns:art=\"http://www.example.com/articles\"");
			xml.AppendLine("            xmlns:base=\"http://www.example.com/base-types\"");
			xml.AppendLine("            xmlns:contact=\"http://www.example.com/contact-types\">");
			xml.AppendLine("  <art:artikel dobavljiv=\"true\" tip=\"\">");  // Invalid tip
			xml.AppendLine("    <art:barcode>invalid-id</art:barcode>");  // Testing substitution group
			xml.AppendLine("    <art:ime></art:ime>");  // Empty name
			xml.AppendLine("    <art:cena>-25.99</art:cena>");  // Negative price
			xml.AppendLine("    <art:zaloga>-10</art:zaloga>");  // Negative stock
			xml.AppendLine("    <art:dobaviteljInfo davcnaStevilka=\"123\">");  // Invalid tax number
			xml.AppendLine("      <art:dobaviteljId>2</art:dobaviteljId>");
			xml.AppendLine("      <art:kontakt type=\"invalid\" active=\"true\">invalid-email</art:kontakt>");
			xml.AppendLine("      <art:naslov addressType=\"invalid\">");
			xml.AppendLine("        <contact:street></contact:street>");
			xml.AppendLine("        <contact:city></contact:city>");
			xml.AppendLine("        <contact:postalCode>0000</contact:postalCode>");  // Invalid postal code
			xml.AppendLine("        <contact:country></contact:country>");
			xml.AppendLine("        <contact:additionalInfo>Poslovna stavba</contact:additionalInfo>");
			xml.AppendLine("        <contact:coordinates>");
			xml.AppendLine("          <contact:latitude>46.0569</contact:latitude>");
			xml.AppendLine("          <contact:longitude>14.5058</contact:longitude>");
			xml.AppendLine("        </contact:coordinates>");
			xml.AppendLine("      </art:naslov>");
			xml.AppendLine("    </art:dobaviteljInfo>");
			xml.AppendLine("    <art:datumZadnjeNabave>2024-11-11T10:00:00</art:datumZadnjeNabave>");
			xml.AppendLine("  </art:artikel>");
			xml.AppendLine("</art:artikli>");

			return xml.ToString();
		}
	}
}
