using System;
using System.Collections.Generic;
using System.Text;

namespace Avaliador_Codigo_Fonte.Acf.DTO
{
	class Properties
	{
		public int Mes { get; set; }
		public int NumeroDeMetodos { get; set; }
		public int NumeroDeClasses { get; set; }
		public int Loc { get; set; }

		public Properties()
		{
		}

		public override String ToString()
		{
			return Mes.ToString() + ',' + Loc.ToString() + ',' + NumeroDeClasses.ToString() + ',' + NumeroDeMetodos.ToString() + '\n';
			
			// + ',' + godClass + ',' + godMethod + '\n';
		}

	}
}
