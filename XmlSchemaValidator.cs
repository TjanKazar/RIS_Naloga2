using System.Text;
using System.Xml.Schema;
using System.Xml;

public class XmlSchemaValidator
{
	private readonly string _schemaPath;
	private readonly List<string> _validationErrors;
	private bool _isValid;

	public XmlSchemaValidator(string schemaPath)
	{
		_schemaPath = schemaPath;
		_validationErrors = new List<string>();
		_isValid = true;
	}

	public (bool isValid, List<string> errors) ValidateXml(string xmlContent)
	{
		_validationErrors.Clear();
		_isValid = true;

		try
		{
			// Create XML Schema settings
			var settings = new XmlReaderSettings();
			settings.ValidationType = ValidationType.Schema;
			settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
			settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

			// Add event handler for validation errors
			settings.ValidationEventHandler += ValidationEventHandler;

			// Load XSD schema
			using (var schemaReader = XmlReader.Create(_schemaPath))
			{
				var schema = XmlSchema.Read(schemaReader, ValidationEventHandler);
				settings.Schemas.Add(schema);
			}

			// Validate XML
			using (var stringReader = new StringReader(xmlContent))
			using (var xmlReader = XmlReader.Create(stringReader, settings))
			{
				while (xmlReader.Read()) { }
			}
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

	public static string GenerateTestXml()
	{
		var xmlBuilder = new StringBuilder();
		xmlBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
		xmlBuilder.AppendLine("<art:artikli xmlns:art=\"http://www.example.com/articles\">");
		xmlBuilder.AppendLine("  <art:artikel dobavljiv=\"true\" tip=\"standard\">");
		xmlBuilder.AppendLine("    <art:id>1</art:id>");
		xmlBuilder.AppendLine("    <art:ime>Test Artikel</art:ime>");
		xmlBuilder.AppendLine("    <art:cena>25.99</art:cena>");
		xmlBuilder.AppendLine("    <art:zaloga>100</art:zaloga>");
		xmlBuilder.AppendLine("    <art:dobaviteljId>2</art:dobaviteljId>");
		xmlBuilder.AppendLine("    <art:datumZadnjeNabave>2024-11-11T10:00:00</art:datumZadnjeNabave>");
		xmlBuilder.AppendLine("  </art:artikel>");
		xmlBuilder.AppendLine("</art:artikli>");

		return xmlBuilder.ToString();
	}

	public static string GenerateInvalidTestXml()
	{
		var xmlBuilder = new StringBuilder();
		xmlBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
		xmlBuilder.AppendLine("<art:artikli xmlns:art=\"http://www.example.com/articles\">");
		xmlBuilder.AppendLine("  <art:artikel dobavljiv=\"true\" tip=\"\">");  // Invalid tip (empty string)
		xmlBuilder.AppendLine("    <art:id>1</art:id>");
		xmlBuilder.AppendLine("    <art:ime></art:ime>");  // Invalid ime (empty)
		xmlBuilder.AppendLine("    <art:cena>-25.99</art:cena>");  // Invalid cena (negative)
		xmlBuilder.AppendLine("    <art:zaloga>-10</art:zaloga>");  // Invalid zaloga (negative)
		xmlBuilder.AppendLine("    <art:dobaviteljId>2</art:dobaviteljId>");
		xmlBuilder.AppendLine("    <art:datumZadnjeNabave>invalid-date</art:datumZadnjeNabave>");  // Invalid date format
		xmlBuilder.AppendLine("  </art:artikel>");
		xmlBuilder.AppendLine("</art:artikli>");

		return xmlBuilder.ToString();
	}
}