using System;
using ClusterCSV.Graficos;
using ClusterCSV.Dados;
using ClusterCSV.Enumerações;
namespace ClusterCSV
{
    class Program
    {
        static void Main(string[] args)
        {
            int altura = 300;
            int largura = 300;
            int margemX = 50;
            int margemY = 50;
            metodo metodo = metodo.vizinhoMaisProximo;
            similaridadeDissimilaridade similaridadeDissimilaridade = similaridadeDissimilaridade.distancia;
            Dataset dataset = new Dataset();
            while (true)
            {
                Console.WriteLine("Digite um comando.\nPara ver a lista de comandos, digite \"ajuda\".");
                switch (Console.ReadLine().ToLower())
                {
                    case "ajuda":
                        //Exibe todos os comandos
                        Console.WriteLine("ajuda - Exibe a lista de comandos disponíveis\n" +
                            "agrupar - Agrupa amostras em clusters\n" +
                            "altura - Define altura de gráficos e imagens.\n" +
                            "dendrograma - Salva dendrograma em caminho especificado\n" +
                            "desvio - Exibe desvio padrão de dataset carregado\n" +
                            "dissimilaridade - Define a medida de dissimilaridade adotada\n" +
                            "largura - Define largura de gráficos e imagens.\n" +
                            "ler - A partir de um caminho, lê dataset em CSV\n" +
                            "limpar - Limpa a tela.\n" +
                            "média - Exibe média de dataset carregado.\n" +
                            "margemX - Define margem horizontal.\n" +
                            "margemY - Define margem vertical.\n" +
                            "método - Define o método de agrupamento.\n" +
                            "salvar - Salva dataset normalizado.\n" +
                            "");
                        break;
                    case "agrupar":
                        //Chama funções de agrupamento
                        try
                        {
                            dataset.agrupar(metodo, similaridadeDissimilaridade);
                            Console.WriteLine("Agrupamento definido!");
                        }
                        catch
                        {
                            Console.WriteLine("Erro ao tentar agrupar o dataset. Verifique os valores.");
                        }
                        break;
                    case "altura":
                        //Permite que o usuário modifique a altura de saída dos diagramas
                        //Altura em pixels
                        Console.WriteLine("Digite a altura de saída de gráficos e imagens:");
                        string _altura = Console.ReadLine();
                        if (!int.TryParse(_altura, out _))
                        {
                            Console.WriteLine("Valor inválido.");
                        }
                        else
                        {
                            altura = int.Parse(_altura);
                            Console.WriteLine("Altura definida.");
                        }
                        break;
                    case "dendrograma":
                        //Desenha e salva dendrograma
                        //Salvo em bmp
                        try
                        {
                            Console.WriteLine("Escreva o caminho para salvar o dendrogama: (exemplo: c:\\pasta\\arquivo.bmp)");
                            string caminho = Console.ReadLine();
                            if (!caminho.Contains("bmp"))
                            {
                                Console.WriteLine("Caminho inválido.");
                                break;
                            }
                            Dendrograma dendrograma = new Dendrograma();
                            dendrograma.altura = altura;
                            dendrograma.largura = largura;
                            dendrograma.margemX = margemX;
                            dendrograma.margemY = margemY;
                            dendrograma.amostras = dataset.obterNomes();
                            var diagrama = dendrograma.desenharPartindoDeRelacoes(dataset.mapaRelacoes, dataset.tamanho);
                            diagrama.Save(caminho);
                            Console.WriteLine("Arquivo salvo.");
                        }
                        catch
                        {
                            Console.WriteLine("Erro. Verifique se o caminho é válido.");
                        }
                        break;
                    case "desvio":
                        //Exibe desvio padrão amostral do dataset
                        if (dataset is null)
                        {
                            Console.WriteLine("Andes de verificar o desvio padrão, carregue um dataset.");
                            break;
                        }
                        Console.WriteLine("Desvio:\n");
                        for (int desvio = 0; desvio < dataset.desvioPadrao.Length; desvio++)
                        {
                            Console.WriteLine(dataset.titulos[desvio] + ": " + dataset.desvioPadrao[desvio].ToString());
                        }
                        break;
                    case "dissimilaridade":
                        //define qual será a medida de dissimilaridade escolhida para avaliar as amostras
                        //Quanto maior for a medida de dissimilaridade, menor será a semelhança entre os elementos
                        Console.WriteLine("Escolha uma medida de dissimilaridade:\n1 - Distância euclidiana\n2 - Distância euclidiana ao quadrado");
                        string medida_escolhida = Console.ReadLine();
                        switch (medida_escolhida)
                        {
                            case "1":
                                similaridadeDissimilaridade = similaridadeDissimilaridade.distancia;
                                Console.WriteLine("Medida de dissimilaridade definida com sucesso!");
                                break;
                            case "2":
                                similaridadeDissimilaridade = similaridadeDissimilaridade.distanciaQuadrado;
                                Console.WriteLine("Medida de dissimilaridade definida com sucesso!");
                                break;
                            default:
                                Console.WriteLine("Erro. Digite uma das opções numéricas dadas.");
                                break;
                        }
                        break;
                    case "largura":
                        //Permite que o usuário modifique a largura de saída do diagrama
                        //Largura em pixels
                        Console.WriteLine("Digite a largura de saída de gráficos e imagens:");
                        string _largura = Console.ReadLine();
                        if (!int.TryParse(_largura, out _))
                        {
                            Console.WriteLine("Valor inválido.");
                        }
                        else
                        {
                            largura = int.Parse(_largura);
                            Console.WriteLine("Largura definida.");
                        }
                        break;
                    case "ler":
                        //Permite que o sistema leia arquivo fornecido por usuário
                        //Arquivo em CSV
                        Console.WriteLine("Digite o caminho do arquivo. Exemplo: C:\\Arquivo.csv");
                        var arquivo = Console.ReadLine();
                        if (!arquivo.Contains(".csv"))
                        {
                            Console.WriteLine("Arquivo inválido. Utilize uma tabela CSV.");
                            break;
                        }
                        try
                        {
                            dataset.lerArquivo(arquivo);
                            Console.WriteLine("Arquivo carregado com sucesso!");
                        }
                        catch
                        {
                            Console.WriteLine("Erro na leitura da tabela. Verifique a consistência dos dados.\n" +
                                "Certifique-se de que a tabela está no padrão:\n" +
                                "Primeira coluna: identificadores de amostra (texto)\n" +
                                "Primeira linha: identificadores de coluna (texto)\n" +
                                "Demais celulas: valores numéricos.");
                        }
                        break;
                    case "limpar":
                        //Permite que o usuário limpe o console
                        Console.Clear();
                        break;
                    case "média":
                        //Exibe médias do dataset
                        if (dataset is null)
                        {
                            Console.WriteLine("Andes de verificar a média, carregue um dataset.");
                            break;
                        }
                        Console.WriteLine("Média:\n");
                        for (int media = 0; media < dataset.media.Length; media++)
                        {
                            Console.WriteLine(dataset.titulos[media] + ": " + dataset.media[media].ToString());
                        }
                        break;
                    case "margemx":
                        //Permite que o usuário modifique a margem horizontal do diagrama
                        //Valor em pixels
                        Console.WriteLine("Digite a margem horizontal de saída de gráficos e imagens:");
                        string _margemX = Console.ReadLine();
                        if (!int.TryParse(_margemX, out _))
                        {
                            Console.WriteLine("Valor inválido.");
                        }
                        else
                        {
                            margemX = int.Parse(_margemX);
                            Console.WriteLine("Margem horizontal definida.");
                        }
                        break;
                    case "margemy":
                        //Permite que o usuário modifique a marvem vertical do diagrama
                        //Valor em pixels
                        Console.WriteLine("Digite a margem vertical de saída de gráficos e imagens:");
                        string _margemY = Console.ReadLine();
                        if (!int.TryParse(_margemY, out _))
                        {
                            Console.WriteLine("Valor inválido.");
                        }
                        else
                        {
                            margemY = int.Parse(_margemY);
                            Console.WriteLine("Margem vertical definida.");
                        }
                        break;
                    case "método":
                        //Permite que o usuário selecione o método de agrupamento
                        Console.WriteLine("Defina o método de agrupamento. \nDigite 1 para vizinho mais próximo\nDigite 2 para vizinho mais distante.");
                        string metodo_escolhido = Console.ReadLine();
                        switch (metodo_escolhido)
                        {
                            case "1":
                                metodo = metodo.vizinhoMaisProximo;
                                Console.WriteLine("Método definido com sucesso!");
                                break;
                            case "2":
                                metodo = metodo.vizinhoMaisDistante;
                                Console.WriteLine("Método definido com sucesso!");
                                break;
                            default:
                                Console.WriteLine("Erro. Digite uma das opções numéricas dadas.");
                                break;
                        }
                        break;
                    case "salvar":
                        //Permite que o usuário salve nova tabela CSV
                        //Dataset normalizado e com atributos modificados
                        Console.WriteLine("Digite o caminho do arquivo. Exemplo: C:\\Arquivo.csv");
                        var arquivoCSV = Console.ReadLine();
                        if (!arquivoCSV.Contains(".csv"))
                        {
                            Console.WriteLine("Arquivo inválido. Utilize uma tabela CSV.");
                            break;
                        }
                        try
                        {
                            dataset.salvar(arquivoCSV);
                            Console.WriteLine("Arquivo salvo com sucesso!");
                        }
                        catch
                        {
                            Console.WriteLine("Erro ao salvar tabela. Verifique a consistência dos dados.\n" +
                                "Certifique-se de que o caminho está correto.");
                        }
                        break;
                }
            }

        }
    }
}
