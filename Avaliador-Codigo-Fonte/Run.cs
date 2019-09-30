using Avaliador_Codigo_Fonte.Util;
using System;

namespace Avaliador_Codigo_Fonte
{
	class Run
	{
		static void Main(string[] args)
		{
			string caminho = "C:\\Users\\Gabriel Barreto\\Downloads\\Dataset\\Dataset";

			Reader reader = new Reader();

			Console.WriteLine(reader.ContaPastasTotal(caminho));
			//reader.NomesArquivos(caminho);
			reader.CaminhaDiretorioArquivo(caminho);
		}
	}
}
