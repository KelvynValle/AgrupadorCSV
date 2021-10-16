using System;
using ClusterCSV.Enumerações;
//Classe destinada ao mapeamento das amostras e suas características
namespace ClusterCSV.Dados
{
    class Amostra
    {
        int _quantidade;
        string _nome;
        double[] _caracteristicas;
        public int quantidade
        {
            get { return _quantidade; }
        }
        public double getCaracteristica(int caracteristica)
        {
            return _caracteristicas[caracteristica];
        }
        public void setCaracteristica(int caracteristica, double valor)
        {
            _caracteristicas[caracteristica] = valor;
        }
        public string nome
        {
            get { return _nome; }
            set
            {
                if (value != "" && value != null)
                {
                    _nome = value;
                }
            }
        }
        public double[] caracteristicas
        {
            get { return _caracteristicas; }
            set
            {
                if (value.Length > 0 && value != null)
                {
                    _quantidade = value.Length;
                    _caracteristicas = (double[])value.Clone();
                }
            }
        }

        //Método para cálculo da distância entre amostras
        public static double distancia(Amostra amostra1, Amostra amostra2, similaridadeDissimilaridade similaridadeDissimilaridade)
        {
            double valor = 0;
            for (int i = 0; i < amostra1.quantidade; i++)
            {
                valor += Math.Pow(amostra1.getCaracteristica(i) - amostra2.getCaracteristica(i), 2);
            }
            switch (similaridadeDissimilaridade)
            {
                case similaridadeDissimilaridade.distancia:
                    return Math.Sqrt(valor);
                case similaridadeDissimilaridade.distanciaQuadrado:
                    return valor;
                default:
                    return Math.Sqrt(valor);
            }
            
        }
    }
}
