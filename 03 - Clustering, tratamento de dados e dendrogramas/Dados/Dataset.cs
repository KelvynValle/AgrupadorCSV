using System;
using System.Collections.Generic;
using System.Linq;
using ClusterCSV.MetodosArray;
using ClusterCSV.Clusterização.Relacionamento;
using ClusterCSV.Enumerações;
namespace ClusterCSV.Dados
{
    //Classe destinada para armazenar amostras
    class Dataset
    {
        private Amostra[] amostras;
        public string[] titulos;
        public double[] media;
        public List<double>[] moda;
        public double[] coeficienteVariação;
        public double[] mediana;
        public double[] desvioPadrao;
        public double[] quartisInferiores;
        public double[] quartisSuperiores;
        public int tamanho;
        public int[] sequenciaDefinida;
        public List<Relacao> mapaRelacoes;
        Tuple<int, int, double>[] distanciaAmostralInicial;

        //Retorna string que representa o nome de cada amostra
        public string[] obterNomes()
        {
            string[] _nomes = new string[amostras.Length];
            for (int i = 0; i < amostras.Length; i++)
            {
                _nomes[i] = amostras[i].nome;
            }
            return _nomes;
        }

        //Lê arquivo CSV com formatação compatível ao software
        //Utiliza os dados lidos no CSV para alimentar arrays
        public void lerArquivo(string caminho)
        {
            var csv = CSV.CSV.lerCSV(caminho);
            amostras = csv.amostras;
            titulos = csv.titulos;
            media = getMedia();
            moda = getModa();
            desvioPadrao = getDesvioPadrao();
            coeficienteVariação = getCoeficienteVariacao();
            mediana = getMediana();
            getQuartis();
            normalizar();
            tamanho = amostras.Length;
        }

        //Método inicial utilizado para agrupar dados
        //Chama outro método de mesmo nome, recursivo, para agrupar os elementos
        public void agrupar(metodo metodo, similaridadeDissimilaridade similaridadeDissimilaridade)
        {
            var distanciaAmostral = geraMatrizDistancia(similaridadeDissimilaridade);
            distanciaAmostralInicial = (Tuple<int, int, double>[])distanciaAmostral.Clone();
            distanciaAmostral = distanciaAmostral.OrderBy(par => par.Item3).ToArray();
            List<Relacao> relacoes = new List<Relacao>();
            for (int i = 0; i < amostras.Length; i++)
            {
                relacoes.Add(new RelacaoInicial(i));
            }
            agrupar(distanciaAmostral, relacoes, metodo);
            this.mapaRelacoes = relacoes;
        }

        //Método recursivo para agrupar dados
        //Chamado por método de mesmo nome, chama a si mesmo enquanto a matriz de distância possui tamanho maior que zero
        private void agrupar(Tuple<int, int, double>[] distancias, List<Relacao> relacoes, metodo metodo)
        {
            if (distancias.Length != 0)
            {
                Relacao novaRelacao;
                if (relacoes[distancias[0].Item1] is RelacaoInicial && relacoes[distancias[0].Item2] is RelacaoInicial)
                {
                    //cria relacao no no
                    novaRelacao = new RelacaoNoNo(((RelacaoInicial)relacoes[distancias[0].Item1]), ((RelacaoInicial)relacoes[distancias[0].Item2]), distancias[0].Item3);
                }
                else if ((!(relacoes[distancias[0].Item1] is RelacaoInicial) || !(relacoes[distancias[0].Item2] is RelacaoInicial)) && (relacoes[distancias[0].Item1] is RelacaoInicial || relacoes[distancias[0].Item2] is RelacaoInicial))
                {
                    //cria relação grupo nó
                    if (!(relacoes[distancias[0].Item1] is RelacaoInicial))
                    {
                        novaRelacao = new RelacaoGrupoNo(relacoes[distancias[0].Item1], ((RelacaoInicial)relacoes[distancias[0].Item2]), distancias[0].Item3);
                    }
                    else
                    {
                        novaRelacao = new RelacaoGrupoNo(relacoes[distancias[0].Item2], ((RelacaoInicial)relacoes[distancias[0].Item1]), distancias[0].Item3);
                    }
                }
                else
                {
                    //cria relacao grupo grupo
                    novaRelacao = new RelacaoGrupoGrupo(relacoes[distancias[0].Item1], relacoes[distancias[0].Item2], distancias[0].Item3);
                }
                relacoes.Add(novaRelacao);
                int anterior1 = relacoes.IndexOf(relacoes[distancias[0].Item1]);
                int anterior2 = relacoes.IndexOf(relacoes[distancias[0].Item2]);
                distancias = distancias.RemoverZero();
                if (distancias.Length > 0)
                {
                    distancias = reescreverDistancias(distancias, novaRelacao, relacoes.Count - 1, anterior1, anterior2, metodo);
                    distancias = distancias.OrderBy(par => par.Item3).ToArray();
                    agrupar(distancias, relacoes, metodo);
                }

            }
        }

        //Gera matriz de distância entre  amostras a partir das características das mesmas
        private Tuple<int, int, double>[] geraMatrizDistancia(similaridadeDissimilaridade similaridadeDissimilaridade)
        {
            List<Tuple<int, int, double>> listaDistancias = new List<Tuple<int, int, double>>();
            for (int linha = 0; linha < amostras.Length; linha++)
            {
                for (int coluna = linha + 1; coluna < amostras.Length; coluna++)
                {
                    listaDistancias.Add(Tuple.Create(linha, coluna, Amostra.distancia(amostras[linha], amostras[coluna], similaridadeDissimilaridade)));
                }
            }
            return listaDistancias.ToArray();
        }

        //Reescreve matriz de distância amostral
        //Substitui indices de amostras e agrupamentos antivos pelos índices do novo agrupamento
        //Remove indices ambiguos de maior distância
        private Tuple<int, int, double>[] reescreverDistancias(Tuple<int, int, double>[] distancias, Relacao ultimaRelacao, int novoIndice, int indiceAnterior1, int indiceAnterior2, metodo metodo)
        {
            List<Tuple<int, int, double>> novaDistancias = new List<Tuple<int, int, double>>();
            //modifica os indices na matriz de distância, trocando os anteriores que formaram a relação pelo indice atual
            for (int i = 0; i < distancias.Length; i++)
            {
                if (distancias[i].Item1 == indiceAnterior1)
                {
                    distancias[i] = Tuple.Create(novoIndice, distancias[i].Item2, distancias[i].Item3);
                }
                else if (distancias[i].Item2 == indiceAnterior1)
                {
                    distancias[i] = Tuple.Create(distancias[i].Item1, novoIndice, distancias[i].Item3);
                }
                if (distancias[i].Item1 == indiceAnterior2)
                {
                    distancias[i] = Tuple.Create(novoIndice, distancias[i].Item2, distancias[i].Item3);
                }
                else if (distancias[i].Item2 == indiceAnterior2)
                {
                    distancias[i] = Tuple.Create(distancias[i].Item1, novoIndice, distancias[i].Item3);
                }
            }
            //Busca por relações repetidas, mas com distâncias diferentes
            if (metodo == metodo.vizinhoMaisProximo)
            {
                for (int elemento = 0; elemento < distancias.Length; elemento++)
                {
                    if (novaDistancias.FindAll(x => x.Item1 == distancias[elemento].Item1 && x.Item2 == distancias[elemento].Item2).Count > 0)
                    {
                        int indice = novaDistancias.IndexOf(novaDistancias.Find(x => x.Item1 == distancias[elemento].Item1 && x.Item2 == distancias[elemento].Item2));
                        novaDistancias[indice] = novaDistancias[indice].Item3 > distancias[elemento].Item3 ? distancias[elemento] : novaDistancias[indice];
                    }
                    else
                    {
                        novaDistancias.Add(distancias[elemento]);
                    }
                }
            }
            else if (metodo == metodo.vizinhoMaisDistante)
            {
                for (int elemento = 0; elemento < distancias.Length; elemento++)
                {
                    if (novaDistancias.FindAll(x => x.Item1 == distancias[elemento].Item1 && x.Item2 == distancias[elemento].Item2).Count > 0)
                    {
                        int indice = novaDistancias.IndexOf(novaDistancias.Find(x => x.Item1 == distancias[elemento].Item1 && x.Item2 == distancias[elemento].Item2));
                        novaDistancias[indice] = novaDistancias[indice].Item3 < distancias[elemento].Item3 ? distancias[elemento] : novaDistancias[indice];
                    }
                    else
                    {
                        novaDistancias.Add(distancias[elemento]);
                    }
                }
            }
            //Seleciona a menor distância para montar a nova matriz

            return novaDistancias.ToArray();
        }

        //Normaliza as amostras do dataset
        public void normalizar()
        {
            int quantidade = amostras[0].quantidade;
            for (int amostra = 0; amostra < amostras.Length; amostra++)
            {
                for (int caracteristica = 0; caracteristica < quantidade; caracteristica++)
                {
                    amostras[amostra].setCaracteristica(caracteristica, (amostras[amostra].getCaracteristica(caracteristica) - media[caracteristica]) / desvioPadrao[caracteristica]);
                }
            }
        }

        //Obtém a média de cada característica das amostras
        private double[] getMedia()
        {
            int quantidade = amostras[0].quantidade;
            double[] media = new double[quantidade];
            for (int amostra = 0; amostra < amostras.Length; amostra++)
            {
                for (int caracteristica = 0; caracteristica < quantidade; caracteristica++)
                {
                    media[caracteristica] += amostras[amostra].getCaracteristica(caracteristica);
                }
            }
            for (int caracteristica = 0; caracteristica < quantidade; caracteristica++)
            {
                media[caracteristica] = media[caracteristica] / amostras.Length;
            }
            return media;
        }

        //Obtém o coeficiente de variação para cada característica das amostras
        private double[] getCoeficienteVariacao()
        {
            int quantidade = amostras[0].quantidade;
            double[] coeficienteVariacao = new double[quantidade];
            for (int i = 0; i < quantidade; i++)
            {
                coeficienteVariacao[i] = (desvioPadrao[i] / media[i]) * 100;
            }
            return coeficienteVariacao;
        }

        //Obtém a mediana para cada característica das amostras
        private double[] getMediana()
        {
            int quantidade = amostras[0].quantidade;
            double[] mediana = new double[quantidade];
            for (int caracteristica = 0; caracteristica < quantidade; caracteristica++)
            {
                var ordenado = amostras.OrderBy(x => x.getCaracteristica(caracteristica)).ToArray();
                if (ordenado.Length % 2 != 0)
                {
                    mediana[caracteristica] = ordenado[(int)((ordenado.Length - 1) / 2)].getCaracteristica(caracteristica);
                }
                else
                {
                    double meio = (ordenado.Length - 1) / 2.0;
                    int inicio = (int)Math.Floor(meio);
                    int fim = inicio + 1;
                    mediana[caracteristica] = (ordenado[inicio].getCaracteristica(caracteristica) + ordenado[fim].getCaracteristica(caracteristica)) / 2.0;
                }
            }
            return mediana;
        }

        //Obtém os quartis para cada característica das amostras
        private void getQuartis()
        {
            int quantidade = amostras[0].quantidade;
            int quartilInferior = (int)(amostras.Length * 0.25);
            int quartilSuperior = (int)(amostras.Length * 0.75);
            quartisInferiores = new double[quantidade];
            quartisSuperiores = new double[quantidade];
            for (int caracteristica = 0; caracteristica < quantidade; caracteristica++)
            {
                var ordenado = amostras.OrderBy(x => x.getCaracteristica(caracteristica)).ToArray();
                quartisInferiores[caracteristica] = ordenado[quartilInferior].getCaracteristica(caracteristica);
                quartisSuperiores[caracteristica] = ordenado[quartilSuperior].getCaracteristica(caracteristica);
            }
        }

        //Obtém a moda de cada característica das amostras
        private List<double>[] getModa()
        {
            int quantidade = amostras[0].quantidade;
            int[] ocorrencias = new int[quantidade];
            List<double>[] modas = new List<double>[quantidade];
            for (int caracteristica = 0; caracteristica < quantidade; caracteristica++)
            {
                modas[caracteristica] = new List<double>();
            }
            for (int amostra = 0; amostra < amostras.Length; amostra++)
            {
                for (int caracteristica = 0; caracteristica < quantidade; caracteristica++)
                {
                    int quantidade_atual = amostras.Count(x => x.getCaracteristica(caracteristica) == amostras[amostra].getCaracteristica(caracteristica));
                    if (quantidade_atual > ocorrencias[caracteristica])
                    {
                        modas[caracteristica].Clear();
                        modas[caracteristica].Add(amostras[amostra].getCaracteristica(caracteristica));
                        ocorrencias[caracteristica] = quantidade_atual;
                    }
                    else if (quantidade_atual == ocorrencias[caracteristica])
                    {
                        if (!modas[caracteristica].Contains(amostras[amostra].getCaracteristica(caracteristica)))
                        {
                            modas[caracteristica].Add(amostras[amostra].getCaracteristica(caracteristica));
                        }
                    }
                }
            }
            return modas.ToArray();
        }

        //Obtém o desvio padrão de cada característica das amostras
        private double[] getDesvioPadrao()
        {
            int quantidade = amostras[0].quantidade;
            double[] desvio = new double[quantidade];
            for (int amostra = 0; amostra < amostras.Length; amostra++)
            {
                for (int caracteristica = 0; caracteristica < quantidade; caracteristica++)
                {
                    desvio[caracteristica] += Math.Pow(amostras[amostra].getCaracteristica(caracteristica) - media[caracteristica], 2);
                }
            }
            for (int caracteristica = 0; caracteristica < quantidade; caracteristica++)
            {
                desvio[caracteristica] = Math.Sqrt(desvio[caracteristica] / (amostras.Length - 1));
            }
            return desvio;
        }

        //Converte distâncias entre amostras em matriz de string
        private string[,] converterDistancias()
        {
            string[,] subtabela = new string[amostras.Length + 1, amostras.Length + 1];
            for (int i = 0; i < distanciaAmostralInicial.Length; i++)
            {
                subtabela[distanciaAmostralInicial[i].Item1 + 1, distanciaAmostralInicial[i].Item2 + 1] = distanciaAmostralInicial[i].Item3.ToString();
            }
            for (int i = 0; i < amostras.Length; i++)
            {
                subtabela[i + 1, 0] = amostras[i].nome;
                subtabela[0, i + 1] = amostras[i].nome;
            }
            return subtabela;
        }

        //Converte média, desvio padrão amostral, moda e mediana das variaveis em matriz de string
        private string[,] converterVariaveis()
        {
            string[,] subtabela = new string[titulos.Length + 1, titulos.Length + 1];
            for (int i = 1; i <= titulos.Length; i++)
            {
                subtabela[0, i] = titulos[i - 1];
            }
            subtabela[0, 0] = "Medidas";
            subtabela[1, 0] = "Média";
            subtabela[2, 0] = "Desvio Padrão Amostral";
            subtabela[3, 0] = "Moda";
            subtabela[4, 0] = "Mediana";
            for (int j = 1; j <= titulos.Length; j++)
            {
                subtabela[1, j] = media[j - 1].ToString();
                subtabela[2, j] = desvioPadrao[j - 1].ToString();
                subtabela[3, j] = string.Join(", ", moda[j - 1].ToArray());
                subtabela[4, j] = mediana[j - 1].ToString();
            }
            return subtabela;
        }

        //Salva o dataset normalizado em CSV
        //Caso se tenha realizado agrupamentos, acrescenta a matriz de distâncias também
        public void salvar(string caminho)
        {
            if (distanciaAmostralInicial is null)
            {
                ClusterCSV.CSV.CSV csv = new ClusterCSV.CSV.CSV(amostras, titulos);
                csv.salvarCSV(caminho);
            }
            else
            {
                Tuple<string[,], int, int> distancias;
                var subtabela = converterDistancias();
                var subtabela2 = converterVariaveis();
                distancias = Tuple.Create(subtabela, 0, amostras.Length + 2);
                var medidas = Tuple.Create(subtabela2, 0, amostras.Length * 2 + 4);
                ClusterCSV.CSV.CSV csv = new ClusterCSV.CSV.CSV(amostras, titulos);
                csv.addSubtabela(distancias);
                csv.addSubtabela(medidas);
                csv.salvarCSV(caminho);
            }

        }

        //Adiciona sequência em que os dados serão exibidos
        public void addSequencia(string caminho)
        {
            string[] labels = System.IO.File.ReadAllText(caminho).Split(';');
            sequenciaDefinida = new int[labels.Length];
            string[] nomeAmostras = new string[amostras.Length];
            for (int i = 0; i < nomeAmostras.Length; i++)
            {
                nomeAmostras[i] = amostras[i].nome.Replace("\r", "").Replace("\n", "");
            }
            for (int i = 0; i < labels.Length; i++)
            {
                sequenciaDefinida[i] = Array.IndexOf(nomeAmostras, labels[i].Replace("\r", "").Replace("\n", ""));
            }
        }
    }
}
