using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using System.Xml.Schema;
using RIS_Naloga2;
using System.Text;

class XmlValidator
{
	private static string currentDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..");
	private static string artikliFileName = "artikli.xml";
	private static string dobaviteljiFileName = "dobavitelji.xml";
	private static string artikliDTDFileName = "artikli.dtd";
	private static string dobaviteljiDTDFileName = "dobavitelji.dtd";
	private static bool isValid = true;
	private static string validationError = string.Empty;

	private static string GetDTDContent(string dtdFileName)
	{
		string dtdPath = Path.Combine(currentDir, dtdFileName);
		if (!File.Exists(dtdPath))
		{
			throw new FileNotFoundException($"DTD datoteka {dtdFileName} ne obstaja.");
		}
		return File.ReadAllText(dtdPath);
	}

	public static void ValidateXmlAgainstDTD(string xmlContent, string documentType)
	{
		isValid = true;
		validationError = string.Empty;

		XmlReaderSettings settings = new XmlReaderSettings
		{
			DtdProcessing = DtdProcessing.Parse,
			ValidationType = ValidationType.DTD,
		};

		using (StringReader stringReader = new StringReader(xmlContent))
		using (XmlReader reader = XmlReader.Create(stringReader, settings))
		{
			try
			{
				while (reader.Read()) { }
			}
			catch (XmlException ex)
			{
				isValid = false;
				validationError = $"XML napaka: {ex.Message}";
			}
		}
	}

	public static (bool isValid, string error) IsValidArtikel(Artikel artikel)
	{
		if (artikel == null) return (false, "Artikel ne sme biti prazen.");
		if (string.IsNullOrWhiteSpace(artikel.Naziv)) return (false, "Naziv artikla je obvezen.");
		if (artikel.Cena <= 0) return (false, "Cena mora biti večja od 0.");
		if (artikel.Zaloga < 0) return (false, "Zaloga ne sme biti negativna.");
		if (artikel.DobaviteljId <= 0) return (false, "Neveljaven ID dobavitelja.");
		return (true, string.Empty);
	}

	public static (bool isValid, string error) IsValidDobavitelj(Dobavitelj dobavitelj)
	{
		if (dobavitelj == null) return (false, "Dobavitelj ne sme biti prazen.");
		if (string.IsNullOrWhiteSpace(dobavitelj.Naziv)) return (false, "Naziv dobavitelja je obvezen.");
		if (string.IsNullOrWhiteSpace(dobavitelj.DavcnaStevilka)) return (false, "Davčna številka je obvezna.");
		if (dobavitelj.DavcnaStevilka.Length != 8) return (false, "Davčna številka mora vsebovati 8 številk.");
		if (!dobavitelj.DavcnaStevilka.All(char.IsDigit)) return (false, "Davčna številka mora vsebovati samo številke.");
		if (string.IsNullOrWhiteSpace(dobavitelj.Kontakt)) return (false, "Kontaktni podatki so obvezni.");
		return (true, string.Empty);
	}

	public static string GetUpdatedXmlContentArtikel(List<Artikel> artikli)
	{
		var xmlDoc = new StringBuilder();
		xmlDoc.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
		xmlDoc.AppendLine(GetDTDContent(artikliDTDFileName));
		xmlDoc.AppendLine("<Artikli>");

		foreach (var a in artikli)
		{
			xmlDoc.AppendLine($@"  <Artikel artikelId=""A{a.Id}"" status=""aktiven"">");
			xmlDoc.AppendLine($"    <Id>{a.Id}</Id>");
			xmlDoc.AppendLine($"    <Naziv>{a.Naziv}</Naziv>");
			xmlDoc.AppendLine($"    <Cena>{a.Cena}</Cena>");
			xmlDoc.AppendLine($"    <Zaloga>{a.Zaloga}</Zaloga>");
			xmlDoc.AppendLine($"    <DobaviteljId>{a.DobaviteljId}</DobaviteljId>");
			xmlDoc.AppendLine($"    <DatumZadnjeNabave>{a.DatumZadnjeNabave:yyyy-MM-dd HH:mm:ss}</DatumZadnjeNabave>");
			xmlDoc.AppendLine("  </Artikel>");
		}

		xmlDoc.AppendLine("</Artikli>");
		return xmlDoc.ToString();
	}

	public static string GetUpdatedXmlContentDobavitelj(List<Dobavitelj> dobavitelji)
	{
		var xmlDoc = new StringBuilder();
		xmlDoc.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
		xmlDoc.AppendLine(GetDTDContent(dobaviteljiDTDFileName));
		xmlDoc.AppendLine("<Dobavitelji>");

		foreach (var d in dobavitelji)
		{
			xmlDoc.AppendLine($@"  <Dobavitelj dobaviteljId=""D{d.Id}"" status=""aktiven"">");
			xmlDoc.AppendLine($"    <Id>{d.Id}</Id>");
			xmlDoc.AppendLine($"    <Naziv>{d.Naziv}</Naziv>");
			xmlDoc.AppendLine($"    <Naslov>{d.Naslov}</Naslov>");
			xmlDoc.AppendLine($"    <DavcnaStevilka>{d.DavcnaStevilka}</DavcnaStevilka>");
			xmlDoc.AppendLine($"    <Kontakt>{d.Kontakt}</Kontakt>");
			xmlDoc.AppendLine($"    <Opis>{d.Opis}</Opis>");
			xmlDoc.AppendLine("  </Dobavitelj>");
		}

		xmlDoc.AppendLine("</Dobavitelji>");
		return xmlDoc.ToString();
	}
}

public static class ProgramExtensions
{
	public static string currentDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..");
	public static string artikliFileName = "artikli.xml";
	public static string dobaviteljiFileName = "dobavitelji.xml";
	public static bool isValid = true;
	public static string validationError = string.Empty;
	static List<Artikel> PreberiArtikle()
	{
		XDocument doc = XDocument.Load(Path.Combine(currentDir, artikliFileName));
		return doc.Root.Elements("Artikel").Select(a => new Artikel
		{
			Id = int.Parse(a.Element("Id").Value),
			Naziv = a.Element("Naziv").Value,
			Cena = decimal.Parse(a.Element("Cena").Value),
			Zaloga = int.Parse(a.Element("Zaloga").Value),
			DobaviteljId = int.Parse(a.Element("DobaviteljId").Value),
			DatumZadnjeNabave = DateTime.Parse(a.Element("DatumZadnjeNabave").Value)
		}).ToList();
	}

	static List<Dobavitelj> PreberiDobavitelje()
	{
		XDocument doc = XDocument.Load(Path.Combine(currentDir, dobaviteljiFileName));
		return doc.Root.Elements("Dobavitelj").Select(d => new Dobavitelj
		{
			Id = int.Parse(d.Element("Id").Value),
			Naziv = d.Element("Naziv").Value,
			Naslov = d.Element("Naslov").Value,
			DavcnaStevilka = d.Element("DavcnaStevilka").Value,
			Kontakt = d.Element("Kontakt").Value,
			Opis = d.Element("Opis").Value
		}).ToList();
	}
	public static void DodajArtikel(Artikel artikel)
	{
		var (isValid, error) = XmlValidator.IsValidArtikel(artikel);
		if (!isValid)
		{
			throw new ArgumentException($"Neveljaven artikel: {error}");
		}

		var artikli = PreberiArtikle();

		// Preveri, če ID že obstaja
		if (artikli.Any(a => a.Id == artikel.Id))
		{
			throw new ArgumentException("Artikel s tem ID-jem že obstaja.");
		}

		artikli.Add(artikel);

		// Ustvari nov XML dokument z artikli
		var xmlDoc = new XDocument(
			new XElement("Artikli",
				artikli.Select(a =>
					new XElement("Artikel",
						new XAttribute("artikelId", $"A{a.Id}"),
						new XAttribute("status", "aktiven"),
						new XElement("Id", a.Id),
						new XElement("Naziv", a.Naziv),
						new XElement("Cena", a.Cena),
						new XElement("Zaloga", a.Zaloga),
						new XElement("DobaviteljId", a.DobaviteljId),
						new XElement("DatumZadnjeNabave", a.DatumZadnjeNabave.ToString("yyyy-MM-dd HH:mm:ss"))
					)
				)
			)
		);

		// Validacija pred shranjevanjem
		XmlValidator.ValidateXmlAgainstDTD(xmlDoc.ToString(), "Artikli");
		if (!isValid)
		{
			throw new XmlException(validationError);
		}

		xmlDoc.Save(Path.Combine(currentDir, artikliFileName));
	}

	public static void DodajDobavitelja(Dobavitelj dobavitelj)
	{
		var (isValid, error) = XmlValidator.IsValidDobavitelj(dobavitelj);
		if (!isValid)
		{
			throw new ArgumentException($"Neveljaven dobavitelj: {error}");
		}

		var dobavitelji = PreberiDobavitelje();

		// Preveri, če ID že obstaja
		if (dobavitelji.Any(d => d.Id == dobavitelj.Id))
		{
			throw new ArgumentException("Dobavitelj s tem ID-jem že obstaja.");
		}

		dobavitelji.Add(dobavitelj);

		// Ustvari nov XML dokument z dobavitelji
		var xmlDoc = new XDocument(
			new XElement("Dobavitelji",
				dobavitelji.Select(d =>
					new XElement("Dobavitelj",
						new XAttribute("dobaviteljId", $"D{d.Id}"),
						new XAttribute("status", "aktiven"),
						new XElement("Id", d.Id),
						new XElement("Naziv", d.Naziv),
						new XElement("Naslov", d.Naslov),
						new XElement("DavcnaStevilka", d.DavcnaStevilka),
						new XElement("Kontakt", d.Kontakt),
						new XElement("Opis", d.Opis)
					)
				)
			)
		);

		// Validacija pred shranjevanjem
		XmlValidator.ValidateXmlAgainstDTD(xmlDoc.ToString(), "Dobavitelji");
		if (!isValid)
		{
			throw new XmlException(validationError);
		}

		xmlDoc.Save(Path.Combine(currentDir, dobaviteljiFileName));
	}
}