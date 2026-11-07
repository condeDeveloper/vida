namespace Conde.Vida;

/// <summary>As tabelas do README. Todo numero de la sai daqui.</summary>
public static class Medidas
{
    /// <summary>As caixas que entram no censo, da menor para a maior.</summary>
    public static readonly (int Largura, int Altura)[] Caixas =
    {
        (3, 3), (4, 3), (4, 4), (5, 4), (5, 5),
    };

    /// <summary>As caixas da analise de destino: param antes, por memoria.</summary>
    public static readonly (int Largura, int Altura)[] CaixasDoDestino =
    {
        (3, 3), (4, 3), (4, 4), (5, 4),
    };

    public static List<Censo> Censos() =>
        Caixas.Select(c => Varredura.Varrer(c.Largura, c.Altura)).ToList();

    public static List<Destinos> Destinos() =>
        CaixasDoDestino.Select(c => Destino.Analisar(c.Largura, c.Altura)).ToList();

    public static SortedDictionary<int, int> FormasEstaticas(int largura, int altura) =>
        Varredura.EstaticasPorCelulas(largura, altura);

    public static SortedDictionary<int, int> FormasEssenciais(int lado) =>
        Varredura.EstaticasEssenciais(lado);

    /// <summary>As duas implementacoes do passo, conferidas caixa por caixa.</summary>
    public static List<(int Largura, int Altura, ulong Conferidos, long Divergencias)> Conferencias()
    {
        var linhas = new List<(int, int, ulong, long)>();
        foreach (var (largura, altura) in new[] { (3, 3), (4, 3), (4, 4) })
        {
            var (conferidos, divergencias) = Juiz.Conferir(largura, altura);
            linhas.Add((largura, altura, conferidos, divergencias));
        }
        return linhas;
    }

    /// <summary>
    /// As vidas estaticas de nome proprio: em que caixa cada uma cabe e se a
    /// varredura daquela caixa realmente a encontrou.
    /// </summary>
    public static List<(string Nome, int Celulas, bool Estatica, bool Achada)> Conhecidas(int lado)
    {
        var caixa = new Caixa(lado, lado);
        var achadas = new HashSet<ulong>();
        for (ulong p = 0; p < caixa.Padroes; p++)
            if (caixa.Estatica(p)) achadas.Add(caixa.Encostar(p));

        var linhas = new List<(string, int, bool, bool)>();
        foreach (var (nome, desenho) in Juiz.Conhecidas)
        {
            if (desenho.Length > lado || desenho[0].Length > lado)
            {
                linhas.Add((nome, 0, false, false));
                continue;
            }
            var padrao = Juiz.Ler(caixa, desenho);
            linhas.Add((nome, caixa.Vivas(padrao), caixa.Estatica(padrao),
                        achadas.Contains(caixa.Encostar(padrao))));
        }
        return linhas;
    }
}
