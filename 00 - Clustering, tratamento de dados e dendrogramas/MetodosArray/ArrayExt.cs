namespace ClusterCSV.MetodosArray
{
    //Classe para adição de métodos de extensão aos vetores
    static class ArrayExt
    {
        //Método para remoção do primeiro elemento de um vetor
        public static T[] RemoverZero<T>(this T[] origem)
        {
            T[] destino = new T[origem.Length - 1];

            for (int i = 0; i < origem.Length - 1; i++)
            {
                destino[i] = origem[i + 1];
            }
            return destino;
        }

        //Método para a remoção do ultimo elemento de um vetor
        public static T[] RemoverUltimo<T>(this T[] origem)
        {
            T[] destino = new T[origem.Length - 1];

            for (int i = 0; i < origem.Length - 1; i++)
            {
                destino[i] = origem[i];
            }
            return destino;
        }
    }
}
