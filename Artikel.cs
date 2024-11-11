using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_Naloga2
{
	public class Artikel
	{
		public int Id { get; set; }
		public string Naziv { get; set; }
		public decimal Cena { get; set; }
		public int Zaloga { get; set; }
		public int DobaviteljId { get; set; }
		public DateTime DatumZadnjeNabave { get; set; }

	}
}
