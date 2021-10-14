using System;
using System.Collections.Generic;
using System.Linq;
using ClusterCSV.Clusterização.Relacionamento;
using System.Drawing;
namespace ClusterCSV.Graficos
{
    //Classe para desenhar dendrogramas
    class Dendrograma : Diagrama
    {
        private int[] amostrasOrdenadas;
        private string[] _amostras;
        public string[] amostras
        {
            set
            {
                _amostras = (string[])value.Clone();
            }
        }
        private List<Tuple<Relacao, float, float>> nosLigacao = new List<Tuple<Relacao, float, float>>();

        //Método para estabelecer sequência das amostras no gráfico a partir da ordem de interações entre eles
        //Problemas nesse método podem fazer com que o dendrograma saia sobreposto
        public void definirSequencia(int quantidade, List<Relacao> relacoes)
        {
            int contador = 0;
            amostrasOrdenadas = new int[quantidade];
            for (int i = 0; i < relacoes.Count; i++)
            {
                if (relacoes[i] is RelacaoNoNo)
                {
                    amostrasOrdenadas[contador] = ((RelacaoNoNo)relacoes[i]).no1.no1;
                    contador++;
                    amostrasOrdenadas[contador] = ((RelacaoNoNo)relacoes[i]).no2.no1;
                    contador++;
                }
                else if (relacoes[i] is RelacaoGrupoNo)
                {
                    amostrasOrdenadas[contador] = ((RelacaoGrupoNo)relacoes[i]).no2.no1;
                    contador++;
                }
            }
        }

        //Método para realizar o desenho virtual da localização inicial das amostras
        //Apenas escreve os nomes nas posições corretas
        public void desenharRelacao(RelacaoInicial inicio)
        {
            desenharTexto(new PointF((float)Array.IndexOf(amostrasOrdenadas, inicio.no1), 0.0f), _amostras[inicio.no1]);
            nosLigacao.Add(Tuple.Create((Relacao)inicio, (float)Array.IndexOf(amostrasOrdenadas, inicio.no1), 0.0f));
        }

        //Método para realizar o desenho virtual da relações entre dois nós iniciais
        //Liga os nomes na altura da distância amostral entre eles
        public void desenharRelacao(RelacaoNoNo relacao)
        {
            int min = new int[] { relacao.no1.no1, relacao.no2.no1 }.Min();
            int max = min < relacao.no1.no1 ? relacao.no1.no1 : relacao.no2.no1;
            min = Array.IndexOf(amostrasOrdenadas, min);
            max = Array.IndexOf(amostrasOrdenadas, max);
            desenharLinha(new PointF(min, 0), new PointF(min, (float)relacao.distancia));
            desenharLinha(new PointF(max, 0), new PointF(max, (float)relacao.distancia));
            desenharLinha(new PointF(min, (float)relacao.distancia), new PointF(max, (float)relacao.distancia));
            desenharTexto(new PointF((float)_amostras.Length - 0.8f, (float)relacao.distancia), Math.Round(relacao.distancia, 2).ToString());
            nosLigacao.Add(Tuple.Create((Relacao)relacao, (min + max) / 2.0f, (float)relacao.distancia));
        }

        //Método para desenhar a relação entre um grupo e um nó
        //Liga o centro da relação deste grupo com a localização do nó inicial na altura da distância amostral entre eles
        public void desenharRelacao(RelacaoGrupoNo grupo)
        {
            int indice1 = nosLigacao.IndexOf(nosLigacao.Find(x => x.Item1 == grupo.no1));
            int indice2 = nosLigacao.IndexOf(nosLigacao.Find(x => x.Item1 == grupo.no2));
            desenharLinha(new PointF(nosLigacao[indice1].Item2, nosLigacao[indice1].Item3), new PointF(nosLigacao[indice1].Item2, (float)grupo.distancia));
            desenharLinha(new PointF(nosLigacao[indice2].Item2, nosLigacao[indice2].Item3), new PointF(nosLigacao[indice2].Item2, (float)grupo.distancia));
            desenharLinha(new PointF(nosLigacao[indice1].Item2, (float)grupo.distancia), new PointF(nosLigacao[indice2].Item2, (float)grupo.distancia));
            desenharTexto(new PointF((float)_amostras.Length - 0.8f, (float)grupo.distancia), Math.Round(grupo.distancia, 2).ToString());
            nosLigacao.Add(Tuple.Create((Relacao)grupo, (nosLigacao[indice1].Item2 + nosLigacao[indice2].Item2) / 2.0f, (float)grupo.distancia));
        }

        //Método para desenhar a relação entre um grupo e outro grupo
        //Liga o centro da relação de cada um destes grupos na altura da distância amostral entre eles
        public void desenharRelacao(RelacaoGrupoGrupo grupo)
        {
            int indice1 = nosLigacao.IndexOf(nosLigacao.Find(x => x.Item1 == grupo.no1));
            int indice2 = nosLigacao.IndexOf(nosLigacao.Find(x => x.Item1 == grupo.no2));
            desenharLinha(new PointF(nosLigacao[indice1].Item2, nosLigacao[indice1].Item3), new PointF(nosLigacao[indice1].Item2, (float)grupo.distancia));
            desenharLinha(new PointF(nosLigacao[indice2].Item2, nosLigacao[indice2].Item3), new PointF(nosLigacao[indice2].Item2, (float)grupo.distancia));
            desenharLinha(new PointF(nosLigacao[indice1].Item2, (float)grupo.distancia), new PointF(nosLigacao[indice2].Item2, (float)grupo.distancia));
            desenharTexto(new PointF((float)_amostras.Length - 0.8f, (float)grupo.distancia), Math.Round(grupo.distancia, 2).ToString());
            nosLigacao.Add(Tuple.Create((Relacao)grupo, (nosLigacao[indice1].Item2 + nosLigacao[indice2].Item2) / 2.0f, (float)grupo.distancia));
        }

        //Método para a conversão das relações em desenho virtual
        //Lê a lista de relações e desenha o tipo de relação cabível
        //Termina retornando o desenho virtual convertido em bitmap
        public Bitmap desenharPartindoDeRelacoes(List<Relacao> relacoes, int quantidade)
        {
            definirSequencia(quantidade, relacoes);
            for (int i = 0; i < relacoes.Count; i++)
            {
                if (relacoes[i] is RelacaoInicial)
                {
                    desenharRelacao((RelacaoInicial)relacoes[i]);
                }
                else if (relacoes[i] is RelacaoNoNo)
                {
                    desenharRelacao((RelacaoNoNo)relacoes[i]);
                }
                else if (relacoes[i] is RelacaoGrupoNo)
                {
                    desenharRelacao((RelacaoGrupoNo)relacoes[i]);
                }
                else
                {
                    desenharRelacao((RelacaoGrupoGrupo)relacoes[i]);
                }
            }
            return desenharDiagrama();
        }
    }
}
