# AgrupadorCSV
Sistema escrito em C# para tratamento e agrupamento (clustering) de dados em arquivos CSV.

## O que é?
O AgrupadorCSV é um aplicativo de console escrito em C# com objetivo de ler, tratar estatisticamente e agrupar dados padronizados em arquivos CSV.

## Como funciona?
### Executável
Para iniciar o software, basta acessar o executável ClusterCSV.exe, presente dentro do diretório Debug, na pasta Bin. Ao iniciar o executável, o console permitirá ao usuário a execução de diversos comandos, descritos a seguir.
### Comandos
#### Versão inicial
A versão inicial possui 12 comandos destinados à leitura, tratamento estatístico, agrupamento e exibição de dados estatísticos. Cada um com suas peculiaridades. São eles:
- **ajuda**: Exibe a lista de comandos disponíveis.
- **agrupar**: Agrupa amostras em clusters.
- **altura**: Define altura de gráficos e imagens.
- **coeficiente**: Exibe os coeficientes de variação do dataset.
- **dendrograma**: Salva dendrograma em caminho especificado.
- **desvio**: Exibe desvio padrão de dataset carregado.
- **dissimilaridade**: Define a medida de dissimilaridade adotada.
- **largura**: Define largura de gráficos e imagens.
- **ler**: A partir de um caminho, lê dataset em CSV.
- **limpar**: Limpa a tela.
- **média**: Exibe médias de dataset carregado.
- **mediana**: Exibe medianas do dataset carregado.
- **margemX**: Define margem horizontal.
- **margemY**: Define margem vertical.
- **método**: Define o método de agrupamento.
- **moda**: Exibe modas do dataset.
- **quartil**: Exibe quartis inferiores e superiores do dataset.
- **salvar**: Salva dataset normalizado e matriz de distâncias em planilha CSV.
