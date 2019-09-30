using Avaliador_Codigo_Fonte.Util;
using System;

namespace Avaliador_Codigo_Fonte
{
	class Run
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Digite o caminho do DataSet");
			string caminho = Console.ReadLine();

			caminho = caminho.Replace("\"","\\");

			Reader reader = new Reader();

			Console.WriteLine("Quantidade de pastas à serem analisadas: " + reader.ContaPastasTotal(caminho));
			Console.WriteLine("Arquivos e seus respectivos diretórios:");
			reader.NomesArquivos(caminho);
			// salvar no desktop csv
			reader.SetDataSource();
			//salvar na pasta dataset csv
			//reader.SetDataSource(caminho);
			Console.WriteLine();
			Console.WriteLine("**** AGUARDE ****");
			reader.CaminhaDiretorioArquivo(caminho);
			Console.WriteLine("Favor, checar arquivo csv presente em: " + reader.GetDataSource());
		}
	}
}
