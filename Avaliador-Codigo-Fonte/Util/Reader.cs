﻿using Avaliador_Codigo_Fonte.Acf.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Avaliador_Codigo_Fonte.Util
{
	class Reader
	{
		#region Atributos do reader, caminho do arquivo csv, titulo do texto csv, regexs.
		string caminhoArquivoCSV = @"C:\\Users\\Gabriel Barreto\\Downloads\\Dataset\\Evolucao.csv";
		static readonly string tituloTexto = "MÊS,LOC,CLASSES,MÉTODOS"; //,CLASSE DEUS, METODO DEUS";
		private readonly Regex rxClasses = new Regex("(.*class) * [A-Z].*[{]");
		private readonly Regex rxMetodos = new Regex("(^.*(public|private|protected|.*))*\\s(List|void|int|Integer|double|Double|String|string|char|long|Long|boolean|short|float|byte|TPFactorizedValue|MessageKeyData|Typeface|(public|private|protected|.*) File |CharSequence|TLObject|TLRPC.PhotoSize|Bitmap).*([A-z0-9a-z]*[(].*[)]*[{])");
		private readonly Regex rxLoc = new Regex("(\\S)");
		private readonly Regex rxComentado = new Regex("(^(\\s*[/][*]))");
		#endregion

		#region Contador de Pastas
		public String ContaPastasTotal(string pCaminho)
		{
			return Directory.GetDirectories(pCaminho, "**", SearchOption.TopDirectoryOnly).Count().ToString();
		}
		#endregion

		#region Nomes dos Arquivos
		public void NomesArquivos(string pCaminho)
		{
			DirectoryInfo dir = new DirectoryInfo(@pCaminho);
			var files = dir.GetFiles("*.java", SearchOption.AllDirectories).ToList();
			foreach (var file in files.OrderBy(x => x.Directory.Name, new NaturalSortComparer<string>()))
			{
				Console.WriteLine(file.FullName);
			}
		}
		#endregion

		#region Leitor do Arquivo e suas Metricas
		public void CaminhaDiretorioArquivo(string pCaminho)
		{
			// Para contar o mês.
			int mesVigente = 0;
			Properties proprieties = new Properties();
			List<Properties> propriedadesProCSV = new List<Properties>();
			DirectoryInfo dir = new DirectoryInfo(@pCaminho);
			List<String> totalLinhas = new List<String>();
			// Criar o arquivo e seu nome.	
			StreamWriter arquivo = new StreamWriter(caminhoArquivoCSV);

			arquivo.WriteLine(tituloTexto);
			// ordernar por meses
			var files = dir.GetFiles("*.java", SearchOption.AllDirectories).OrderBy(x => x.Directory.Name, new NaturalSortComparer<string>()).ToList();
			// agrupar por meses
			var filesAgrupados = files.GroupBy(x => x.Directory.FullName).Select(y => new
			{
				y.Key,
				FileName = y.Select(a => new
				{
					a.FullName
				})
			}).ToList();

			foreach (var file in filesAgrupados)
			{
				// resetar tudo.
				string linha = "";
				int linhasEmBranco = 0;
				int numeroDeMetodos = 0;
				int numeroDeClasses = 0;
				int loc = 0;
				int godMetodo = 0;
				int godClass = 0;
				totalLinhas.Clear();

				// percorrer na lista agrupada.
				foreach (var fileInfo in file.FileName)
				{
					using (StreamReader texto = new StreamReader(fileInfo.FullName))
						while ((linha = texto.ReadLine()) != null)
						{
							if (!rxLoc.IsMatch(linha))
								linhasEmBranco++;

							if (rxMetodos.IsMatch(linha) && !rxComentado.IsMatch(linha)) {
								godMetodo =+ ChecarGodMetodo();
								numeroDeMetodos++;
							}
							if (rxClasses.IsMatch(linha) && !rxComentado.IsMatch(linha)) {
								godClass =+ ChecarGodClass();
								numeroDeClasses++;
							}
							totalLinhas.Add(linha);

							loc = totalLinhas.Count() - linhasEmBranco;
						}
				}
				mesVigente++; //ao sair da pasta contar como mês vigente.
				arquivo.WriteLine(mesVigente.ToString() + ',' + loc.ToString() + ',' + numeroDeClasses.ToString() + ',' + numeroDeMetodos.ToString());

				#region Propriedades comentadas			
				// propriedades à serem inseridas no arquivo csv -- poderia ter sido direto, mas preferi assim.
				//proprieties.Mes = mesVigente;
				//proprieties.Loc = loc;
				//proprieties.NumeroDeClasses = numeroDeClasses;
				//proprieties.NumeroDeMetodos = numeroDeMetodos;
				//propriedadesProCSV.Add(proprieties);
				#endregion
			}
			//GerarCSV(propriedadesProCSV); * GERAR CSV *
			arquivo.Close();

			#region Predição csv
			// gerar predição
			{
				var lPredicao = LeitorCSV(caminhoArquivoCSV);

				using (StreamWriter csvComPredicao = new StreamWriter(@caminhoArquivoCSV, true))
				{
					csvComPredicao.WriteLine(GerarPredicao(lPredicao));
				}

				arquivo.Close();
			}
			#endregion
		}
		public string GerarPredicao(List<string[]> pColunas)
		{
			int predicaoLoc = 0;
			int predicaoClasse = 0;
			int predicaoMetodo = 0;
			var lista = pColunas;

			PropriedadesPredicao propriedadesPredicao = new PropriedadesPredicao();
			List<PropriedadesPredicao> lPropriedadesPredicao = new List<PropriedadesPredicao>();

			foreach (var mes in pColunas)
			{
				// Posição 1: LOC - Posição 2: CLASSE - Posição 3: - METODOS
				propriedadesPredicao.SomaLoc = Convert.ToDecimal(mes[1]) / 30;
				propriedadesPredicao.SomaDeClasses = Convert.ToDecimal(mes[2]) / 30;
				propriedadesPredicao.SomaDeMetodos = Convert.ToDecimal(mes[3]) / 30;

				lPropriedadesPredicao.Add(new PropriedadesPredicao()
				{
					SomaLoc = propriedadesPredicao.SomaLoc,
					SomaDeClasses = propriedadesPredicao.SomaDeClasses,
					SomaDeMetodos = propriedadesPredicao.SomaDeMetodos
				});
			}
			predicaoLoc = Convert.ToInt32(lPropriedadesPredicao.Sum(x => x.SomaLoc));
			predicaoClasse = Convert.ToInt32(lPropriedadesPredicao.Sum(x => x.SomaDeClasses));
			predicaoMetodo = Convert.ToInt32(lPropriedadesPredicao.Sum(x => x.SomaDeMetodos));

			return "28" + ',' + predicaoLoc.ToString() + ',' + predicaoClasse.ToString() + ',' + predicaoMetodo.ToString();
		}


		public int ChecarGodClass()
		{

			return 0;
		}

		public int ChecarGodMetodo()
		{
			return 0;
		}
		#endregion

		#region CSV - GERADOR E LEITOR
		public void GerarCSV(List<Properties> pPropriedadesCSV, string pCaminhoCsv)
		{
			StreamWriter arquivo = new StreamWriter(pCaminhoCsv);

			arquivo.WriteLine(tituloTexto);

			foreach (var propriedade in pPropriedadesCSV)
				arquivo.WriteLine(propriedade.ToString());

			arquivo.Close();
		}

		public List<string[]> LeitorCSV(string pCaminho)
		{
			StreamReader stream = new StreamReader(pCaminho);
			List<string[]> listaLinhaCsv = new List<string[]>();
			string linha = "";
			string[] coluna = null;
			while ((linha = stream.ReadLine()) != null)
			{
				if (linha.Contains(tituloTexto))
					continue;

				coluna = linha.Split(',');
				//Console.WriteLine(coluna[0] + " - " + coluna[1]
				//	  + " - " + coluna[2] + " - " + coluna[3]);

				listaLinhaCsv.Add(coluna);
			}
			stream.Close();

			return listaLinhaCsv;
		}
		#endregion
	}
}
