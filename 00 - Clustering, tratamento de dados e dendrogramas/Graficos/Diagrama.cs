using System;
using System.Collections.Generic;
using System.Drawing;

namespace ClusterCSV.Graficos
{

    //Enumeração que representa os tipos de ações que se pode realizar em desenhos virtuals
    enum acoes
    {
        linha,
        spline,
        texto
    }

    //Classe destinada ao desenho virtual de diagramas e suas conversões em bitmap
    class Diagrama
    {
        private int _largura;
        private int _altura;
        private int _margemX;
        private int _margemY;
        List<Tuple<acoes, object>> desenho = new List<Tuple<acoes, object>>();

        public int largura
        {
            get { return _largura; }
            set { _largura = value; }
        }
        public int altura
        {
            get { return _altura; }
            set { _altura = value; }
        }
        public int margemX
        {
            get { return _margemX; }
            set { _margemX = value; }
        }
        public int margemY
        {
            get { return _margemY; }
            set { _margemY = value; }
        }

        //Método destinado à desenhar linha virtual entre dois pontos
        public void desenharLinha(PointF inicio, PointF fim)
        {
            desenho.Add(Tuple.Create(acoes.linha, (object)new PointF[] { inicio, fim }));
        }

        //Método destinado à desenhar texto virtual em determinada posição
        public void desenharTexto(PointF posicao, string valor)
        {
            desenho.Add(Tuple.Create(acoes.texto, (object)(new object[] { (object)posicao, (object)valor })));
        }

        //Método destinado à desenhar spline a partir de vetor de pontos
        public void desenharSpline(PointF[] pontos)
        {
            desenho.Add(Tuple.Create(acoes.linha, (object)pontos));

        }

        //Método privado para converter pontos genéricos no valor real do desenho, utilizando sua largura, altura e margens
        //Desenhos são efetivamente realizados a partir da conversão de seus pontos virtuais nos valores reais
        private PointF converterPonto(PointF ponto)
        {
            return new PointF((ponto.X * largura / larguraMaxima()) + margemX * 0.5f, (ponto.Y * altura / alturaMaxima()) + margemY * 0.5f);
        }

        //Retorna a altura virtual máxima que o desenho pode ter
        private float alturaMaxima()
        {
            float max = 0;
            for (int i = 0; i < desenho.Count; i++)
            {
                if (desenho[i].Item1 == acoes.linha)
                {
                    PointF[] pontos = (PointF[])desenho[i].Item2;
                    float atual = pontos[0].Y > pontos[1].Y ? pontos[0].Y : pontos[1].Y;
                    max = max > atual ? max : atual;
                }
                else if (desenho[i].Item1 == acoes.spline)
                {
                    //não há splines ainda no projeto
                }
            }
            return max;
        }

        //Retorna a largura virtual máxima que o desenho pode ter
        private float larguraMaxima()
        {
            float max = 0;
            for (int i = 0; i < desenho.Count; i++)
            {
                if (desenho[i].Item1 == acoes.linha)
                {
                    PointF[] pontos = (PointF[])desenho[i].Item2;
                    float atual = pontos[0].X > pontos[1].X ? pontos[0].X : pontos[1].X;
                    max = max > atual ? max : atual;
                }
                else if (desenho[i].Item1 == acoes.spline)
                {
                    //não há splines ainda no projeto
                }
            }
            return max;
        }

        //Converte o desenho virtual em bitmap
        public Bitmap desenharDiagrama()
        {
            Bitmap diagrama = new Bitmap(_largura + margemX, _altura + margemY);
            Graphics g = Graphics.FromImage(diagrama);
            g.Clear(Color.White);
            foreach (Tuple<acoes, object> acao in desenho)
            {

                switch (acao.Item1)
                {
                    case acoes.linha:
                        PointF[] pontos_linha = (PointF[])acao.Item2;
                        g.DrawLine(new Pen(new SolidBrush(Color.Black)), converterPonto(pontos_linha[0]), converterPonto(pontos_linha[1]));
                        break;
                    case acoes.spline:
                        PointF[] pontos_spline = (PointF[])acao.Item2;
                        Array.ForEach(pontos_spline, ponto => converterPonto(ponto));
                        g.DrawCurve(new Pen(new SolidBrush(Color.Black)), pontos_spline);
                        break;
                }
            }
            //como o desenho é realizado "ao contrário" rotaciona o diagrama 180° para deixá-lo na posição correta
            //Isso deve ser feito antes de se escrever o texto para evitar que eles fiquem de cabeça para baixo
            diagrama.RotateFlip(RotateFlipType.Rotate180FlipNone);
            var textos = desenho.FindAll(x => x.Item1 == acoes.texto).ToArray();
            for (int i = 0; i < textos.Length; i++)
            {
                Object[] parametros = (Object[])textos[i].Item2;
                PointF posicao = converterPonto((PointF)parametros[0]);
                string texto = (string)parametros[1];
                posicao = new PointF((_largura + _margemX) - (posicao.X + (texto.Length / 2) * 10) + 1.0f, (_altura + _margemY) - (posicao.Y + 5) + 1.0f);
                g.DrawString(texto, new Font(new FontFamily("arial"), 10), new SolidBrush(Color.Black), posicao);
            }
            g.Dispose();
            return diagrama;
        }
    }
}
