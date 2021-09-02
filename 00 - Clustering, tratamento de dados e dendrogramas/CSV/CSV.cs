using System;
using System.Collections.Generic;
using ClusterCSV.MetodosArray;
namespace ClusterCSV.CSV
{
    class CSV
    {
        public string[] titulos;
        public Amostra[] amostras;
        private List<Tuple<string[,], int, int>> subtabelas = new List<Tuple<string[,], int, int>>();
     
        public CSV(Amostra[] amostras, string[] titulos)
        {
            this.amostras = amostras;
            this.titulos = titulos;
        }

        //adiciona subtabela ao arquivo CSV
        public void addSubtabela(Tuple<string[,], int, int> subtabela)
        {
            this.subtabelas.Add(subtabela);
        }

        //Lê arquivo CSV, alimentando amostras e títulos
        //O arquivo deve estar no formato:
        //Primeira linha: títulos das colunas
        //Primeira coluna: nome da amostra
        public static CSV lerCSV(string caminho)
        {
            List<Amostra> tabela = new List<Amostra>();
            string[] titulos;
            using (System.IO.StreamReader arquivo = new System.IO.StreamReader(caminho))
            {
                string linha;
                linha = arquivo.ReadLine();
                titulos = linha.Split(';');
                titulos = titulos.RemoverZero();
                while ((linha = arquivo.ReadLine()) != null)
                {
                    string[] celulas = linha.Split(';');
                    Amostra amostra = new Amostra();
                    amostra.nome = celulas[0];
                    double[] valores = new double[celulas.Length - 1];
                    for (int i = 1; i < celulas.Length; i++)
                    {
                        valores[i - 1] = double.Parse(celulas[i]);
                    }
                    amostra.caracteristicas = valores;
                    tabela.Add(amostra);
                }
                arquivo.Close();
            }
            return new CSV(tabela.ToArray(), titulos);
        }

        //Gera matriz string CSV
        private string[,] tabela()
        {
            int linhas, colunas;
            colunas = amostras[0].quantidade + 1;
            linhas = amostras.Length + 1;
            foreach(Tuple<string[,], int, int> subtabela in subtabelas)
            {
                linhas = linhas > subtabela.Item2 + subtabela.Item1.GetLength(0) ? linhas : subtabela.Item2 + subtabela.Item1.GetLength(0);
                colunas = colunas > subtabela.Item3 + subtabela.Item1.GetLength(1) ? colunas : subtabela.Item3 + subtabela.Item1.GetLength(1);
            }
            string[,] valores = new string[linhas + 1, colunas + 1];
            
            //adiciona os títulos
            for(int i = 1; i <= titulos.Length; i++) {
                valores[0, i] = titulos[i - 1];
            }

            //adiciona as amostras
            for(int amostra = 0; amostra < amostras.Length; amostra++)
            {
                valores[amostra + 1, 0] = amostras[amostra].nome;
                for (int i = 1; i <= amostras[amostra].caracteristicas.Length; i++)
                {
                    valores[amostra + 1, i] = amostras[amostra].caracteristicas[i - 1].ToString();
                }
            }

            //adiciona subtabelas
            foreach (Tuple<string[,], int, int> subtabela in subtabelas)
            {
                for(int linha  = 0; linha < subtabela.Item1.GetLength(0); linha++)
                {
                    for (int coluna = 0; coluna < subtabela.Item1.GetLength(1); coluna++)
                    {
                        valores[linha + subtabela.Item2, coluna + subtabela.Item3] = subtabela.Item1[linha, coluna];
                    }
                }
            }

                return valores;
        }

        //Salva em arquivo CSV
        public void salvarCSV(string caminho)
        {
            string arquivo = "";
            var tabelaCSV = tabela();
            for(int linha = 0; linha < tabelaCSV.GetLength(0); linha++)
            {
                for(int coluna = 0; coluna < tabelaCSV.GetLength(1); coluna++)
                {
                    arquivo = arquivo + tabelaCSV[linha, coluna] + ";";
                }
                arquivo = arquivo + "\n";
            }
            System.IO.File.WriteAllText(caminho, arquivo);
        }


    }
}
