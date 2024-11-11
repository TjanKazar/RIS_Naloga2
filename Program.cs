using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using RIS_Naloga2;
using System.Text;
using System.Xml.Schema;
using System.Xml;




class Program
{
	static string currentDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..");
	static string artikliFileName = "artikli.xml";
	static string artikelShemaPath = Path.Combine(currentDir, "ArtikelSchema.xsd");
	static string dobaviteljShemaPath = Path.Combine(currentDir, "DobaviteljSchema.xsd");

	static string dobaviteljiFileName = "dobavitelji.xml";

	static void Main(string[] args)
	{
		List<Artikel> artikli = new List<Artikel>()
		{
			new Artikel { Id = 1, Naziv = "Artikel 1", Cena = 10.5m, Zaloga = 100, DobaviteljId = 1, DatumZadnjeNabave = DateTime.Now },
			new Artikel { Id = 2, Naziv = "Artikel 2", Cena = 20.0m, Zaloga = 200, DobaviteljId = 2, DatumZadnjeNabave = DateTime.Now },
			new Artikel { Id = 3, Naziv = "Artikel 3", Cena = 30.0m, Zaloga = 50, DobaviteljId = 3, DatumZadnjeNabave = DateTime.Now },
			new Artikel { Id = 4, Naziv = "Artikel 4", Cena = 69.5m, Zaloga = 150, DobaviteljId = 4, DatumZadnjeNabave = DateTime.Now },
			new Artikel { Id = 5, Naziv = "Artikel 5", Cena = 15.0m, Zaloga = 80, DobaviteljId = 5, DatumZadnjeNabave = DateTime.Now }
		};

		List<Dobavitelj> dobavitelji = new List<Dobavitelj>()
		{
			new Dobavitelj { Id = 1, Naziv = "Dobavitelj 1", Naslov = "Naslov 1", DavcnaStevilka = "12345678", Kontakt = "kontakt1@dobavitelj.com", Opis = "Opis dobavitelja 1" },
			new Dobavitelj { Id = 2, Naziv = "Dobavitelj 2", Naslov = "Naslov 2", DavcnaStevilka = "23456789", Kontakt = "kontakt2@dobavitelj.com", Opis = "Opis dobavitelja 2" },
			new Dobavitelj { Id = 3, Naziv = "Dobavitelj 3", Naslov = "Naslov 3", DavcnaStevilka = "34567890", Kontakt = "kontakt3@dobavitelj.com", Opis = "Opis dobavitelja 3" },
			new Dobavitelj { Id = 4, Naziv = "Dobavitelj 4", Naslov = "Naslov 4", DavcnaStevilka = "45678901", Kontakt = "kontakt4@dobavitelj.com", Opis = "Opis dobavitelja 4" },
			new Dobavitelj { Id = 5, Naziv = "Dobavitelj 5", Naslov = "Naslov 5", DavcnaStevilka = "56789012", Kontakt = "kontakt5@dobavitelj.com", Opis = "Opis dobavitelja 5" }
		};

		ShraniArtikle(artikli);
		ShraniDobavitelje(dobavitelji);
		Console.WriteLine("Podatki so bili shranjeni v XML datoteke.");

		List<Artikel> prebrani_artikli = PreberiArtikle();
		List<Dobavitelj> prebrani_dobavitelji = PreberiDobavitelje();
		Console.WriteLine("Podatki so bili prebrani iz XML datotek.");

		if (prebrani_artikli.Count > 0)
		{
			UrediArtikel(prebrani_artikli[0].Id, "Spremenjeni artikel", 25.0m, 75, 2, DateTime.Now);
		}
		if (prebrani_dobavitelji.Count > 0)
		{
			UrediDobavitelja(prebrani_dobavitelji[0].Id, "Spremenjeni dobavitelj", "Novi naslov", "87654321", "novi@kontakt.com", "Novi opis");
		}
		Console.WriteLine("Podatki so bili urejeni v XML datotekah.");

		prebrani_artikli = PreberiArtikle();
		prebrani_dobavitelji = PreberiDobavitelje();
		Console.WriteLine("Spremenjeni podatki so bili ponovno prebrani iz XML datotek.");

		if (prebrani_artikli.Count > 0)
		{
			var a = prebrani_artikli[0];
			Console.WriteLine($"Prvi artikel: Id={a.Id}, Naziv={a.Naziv}, Cena={a.Cena}, Zaloga={a.Zaloga}, DobaviteljId={a.DobaviteljId}, DatumZadnjeNabave={a.DatumZadnjeNabave}");
		}
		if (prebrani_dobavitelji.Count > 0)
		{
			var d = prebrani_dobavitelji[0];
			Console.WriteLine($"Prvi dobavitelj: Id={d.Id}, Naziv={d.Naziv}, Naslov={d.Naslov}, DavcnaStevilka={d.DavcnaStevilka}, Kontakt={d.Kontakt}, Opis={d.Opis}");
		}


		// 3. naloga testi

		var novArtikel = new Artikel
		{
			Id = 67,
			Naziv = "Nov Artikel",
			Cena = 1m,
			Zaloga = 100,
			DobaviteljId = 1,
			DatumZadnjeNabave = DateTime.Now
		};
		ProgramExtensions.DodajArtikel(novArtikel);


		// 4. naloga testi

		var validator = new XmlSchemaValidator(artikelShemaPath);

		Console.WriteLine("Validating valid XML...");
		var validXml = XmlSchemaValidator.GenerateTestXml();
		var (isValid, errors) = validator.ValidateXml(validXml);

		if (isValid)
		{
			Console.WriteLine("XML is valid according to the schema.");
		}
		else
		{
			Console.WriteLine("XML validation errors:");
			foreach (var error in errors)
			{
				Console.WriteLine($"- {error}");
			}
		}

		Console.WriteLine("\nValidating invalid XML...");
		var invalidXml = XmlSchemaValidator.GenerateInvalidTestXml();
		(isValid, errors) = validator.ValidateXml(invalidXml);

		if (isValid)
		{
			Console.WriteLine("XML is valid according to the schema.");
		}
		else
		{
			Console.WriteLine("XML validation errors:");
			foreach (var error in errors)
			{
				Console.WriteLine($"- {error}");
			}
		}
	
}

	static void ShraniArtikle(List<Artikel> artikli)
	{
		XDocument doc = new XDocument(
			new XElement("Artikli",
				artikli.Select(a =>
					new XElement("Artikel",
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
		doc.Save(Path.Combine(currentDir, artikliFileName));
	}

	static void ShraniDobavitelje(List<Dobavitelj> dobavitelji)
	{
		XDocument doc = new XDocument(
			new XElement("Dobavitelji",
				dobavitelji.Select(d =>
					new XElement("Dobavitelj",
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
		doc.Save(Path.Combine(currentDir, dobaviteljiFileName));
	}

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

	static void UrediArtikel(int id, string naziv, decimal cena, int zaloga, int dobaviteljId, DateTime datumZadnjeNabave)
	{
		XDocument doc = XDocument.Load(Path.Combine(currentDir, artikliFileName));
		var artikel = doc.Root.Elements("Artikel").FirstOrDefault(a => int.Parse(a.Element("Id").Value) == id);
		if (artikel != null)
		{
			artikel.Element("Naziv").Value = naziv;
			artikel.Element("Cena").Value = cena.ToString();
			artikel.Element("Zaloga").Value = zaloga.ToString();
			artikel.Element("DobaviteljId").Value = dobaviteljId.ToString();
			artikel.Element("DatumZadnjeNabave").Value = datumZadnjeNabave.ToString("yyyy-MM-dd HH:mm:ss");
			doc.Save(Path.Combine(currentDir, artikliFileName));
		}
	}

	static void UrediDobavitelja(int id, string naziv, string naslov, string davcnaStevilka, string kontakt, string opis)
	{
		XDocument doc = XDocument.Load(Path.Combine(currentDir, dobaviteljiFileName));
		var dobavitelj = doc.Root.Elements("Dobavitelj").FirstOrDefault(d => int.Parse(d.Element("Id").Value) == id);
		if (dobavitelj != null)
		{
			dobavitelj.Element("Naziv").Value = naziv;
			dobavitelj.Element("Naslov").Value = naslov;
			dobavitelj.Element("DavcnaStevilka").Value = davcnaStevilka;
			dobavitelj.Element("Kontakt").Value = kontakt;
			dobavitelj.Element("Opis").Value = opis;
			doc.Save(Path.Combine(currentDir, dobaviteljiFileName));
		}
	}
}