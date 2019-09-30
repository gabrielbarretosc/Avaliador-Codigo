using System;
using System.Collections.Generic;
using System.Text;

namespace Avaliador_Codigo_Fonte.Acf.DTO
{
	class PropriedadesPredicao
	{
		//public int Mes { get; set; }
		public decimal SomaDeMetodos { get; set; }
		public decimal SomaDeClasses { get; set; }
		public decimal SomaLoc { get; set; }
		public decimal SomaGodMethod { get; set; }
		public decimal SomaGodClass { get; set; }

		public PropriedadesPredicao()
		{
		}

	}
}
