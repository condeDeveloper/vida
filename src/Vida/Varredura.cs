namespace Conde.Vida;

/// <summary>O que a varredura de uma caixa inteira encontrou.</summary>
public readonly record struct Censo(
    int Largura, int Altura, ulong Padroes, long Estaticas, long Periodo2,
    long SemPredecessor, long FormasEstaticas);

/// <summary>
/// A varredura: TODO padrao que cabe na caixa, um por um. Nao ha amostra nem
/// sorteio — a caixa 5x5 tem 33.554.432 padroes e os 33.554.432 passam.
/// </summary>
public static class Varredura
{
    /// <summary>
    /// Conta as vidas estaticas, os osciladores de periodo 2 e os padroes que
    /// nenhum outro padrao da caixa produz — os jardins do Eden DA CAIXA.
    ///
    /// Cuidado com a palavra: um jardim do Eden de verdade e um padrao que
    /// ninguem produz no plano infinito. Aqui o predecessor so pode estar dentro
    /// da caixa, entao o numero conta outra coisa, mais modesta e exata.
    /// </summary>
    public static Censo Varrer(int largura, int altura)
    {
        var caixa = new Caixa(largura, altura);
        var total = caixa.Padroes;

        var produzido = new bool[total];
        long estaticas = 0, periodo2 = 0;
        var formas = new HashSet<ulong>();

        for (ulong p = 0; p < total; p++)
        {
            var q = caixa.Passo(p);
            produzido[q] = true;

            if (q == p)
            {
                estaticas++;
                formas.Add(caixa.Encostar(p));
                continue;
            }
            if (caixa.Passo(q) == p) periodo2++;
        }

        long semPredecessor = 0;
        for (ulong p = 0; p < total; p++)
            if (!produzido[p]) semPredecessor++;

        return new Censo(largura, altura, total, estaticas, periodo2,
                         semPredecessor, formas.Count);
    }

    /// <summary>
    /// As vidas estaticas agrupadas por quantas celulas vivas tem, contadas
    /// como FORMAS: dois padroes que so diferem por translacao contam uma vez.
    /// </summary>
    public static SortedDictionary<int, int> EstaticasPorCelulas(int largura, int altura)
    {
        var caixa = new Caixa(largura, altura);
        var porTamanho = new Dictionary<int, HashSet<ulong>>();

        for (ulong p = 0; p < caixa.Padroes; p++)
        {
            if (!caixa.Estatica(p)) continue;
            var vivas = caixa.Vivas(p);
            if (!porTamanho.TryGetValue(vivas, out var conjunto))
                porTamanho[vivas] = conjunto = new HashSet<ulong>();
            conjunto.Add(caixa.Encostar(p));
        }

        var resposta = new SortedDictionary<int, int>();
        foreach (var par in porTamanho) resposta[par.Key] = par.Value.Count;
        return resposta;
    }

    /// <summary>
    /// As vidas estaticas contadas como FORMAS ESSENCIAIS: girar e espelhar nao
    /// produz forma nova. So faz sentido em caixa quadrada.
    /// </summary>
    public static SortedDictionary<int, int> EstaticasEssenciais(int lado)
    {
        var caixa = new Caixa(lado, lado);
        var porTamanho = new Dictionary<int, HashSet<ulong>>();

        for (ulong p = 0; p < caixa.Padroes; p++)
        {
            if (!caixa.Estatica(p)) continue;
            var vivas = caixa.Vivas(p);
            if (!porTamanho.TryGetValue(vivas, out var conjunto))
                porTamanho[vivas] = conjunto = new HashSet<ulong>();
            conjunto.Add(caixa.FormaEssencial(p));
        }

        var resposta = new SortedDictionary<int, int>();
        foreach (var par in porTamanho) resposta[par.Key] = par.Value.Count;
        return resposta;
    }
}

/// <summary>O que a analise de destino de uma caixa encontrou.</summary>
public readonly record struct Destinos(
    int Largura, int Altura, ulong Padroes, int Atratores, int MaiorPeriodo,
    int MaiorTransiente, ulong PadraoMaisDemorado);

/// <summary>
/// Para onde cada padrao vai. Como a caixa e finita, todo padrao acaba caindo
/// num ciclo: o que varia e quanto tempo ele leva (o transiente) e em que ciclo
/// ele cai (o atrator).
/// </summary>
public static class Destino
{
    public static Destinos Analisar(int largura, int altura)
    {
        var caixa = new Caixa(largura, altura);
        var total = caixa.Padroes;

        var proximo = new uint[total];
        for (ulong p = 0; p < total; p++) proximo[p] = (uint)caixa.Passo(p);

        // 0 = nao visitado, 1 = na pilha desta descida, 2 = pronto.
        var estado = new byte[total];
        var transiente = new int[total];
        var periodoDe = new int[total];
        var ciclos = new HashSet<uint>();
        var maiorPeriodo = 0;
        var maiorTransiente = 0;
        uint maisDemorado = 0;

        var caminho = new List<uint>();
        for (uint inicio = 0; inicio < total; inicio++)
        {
            if (estado[inicio] != 0) continue;

            caminho.Clear();
            var atual = inicio;
            while (estado[atual] == 0)
            {
                estado[atual] = 1;
                caminho.Add(atual);
                atual = proximo[atual];
            }

            int transienteDoFim, periodoDoFim;
            if (estado[atual] == 1)
            {
                // Fechou um ciclo dentro deste caminho.
                var inicioDoCiclo = caminho.IndexOf(atual);
                var periodo = caminho.Count - inicioDoCiclo;
                if (periodo > maiorPeriodo) maiorPeriodo = periodo;

                var menor = uint.MaxValue;
                for (var i = inicioDoCiclo; i < caminho.Count; i++)
                    if (caminho[i] < menor) menor = caminho[i];
                ciclos.Add(menor);

                for (var i = inicioDoCiclo; i < caminho.Count; i++)
                {
                    estado[caminho[i]] = 2;
                    transiente[caminho[i]] = 0;
                    periodoDe[caminho[i]] = periodo;
                }
                caminho.RemoveRange(inicioDoCiclo, caminho.Count - inicioDoCiclo);
                transienteDoFim = 0;
                periodoDoFim = periodo;
            }
            else
            {
                transienteDoFim = transiente[atual];
                periodoDoFim = periodoDe[atual];
            }

            for (var i = caminho.Count - 1; i >= 0; i--)
            {
                transienteDoFim++;
                var v = caminho[i];
                estado[v] = 2;
                transiente[v] = transienteDoFim;
                periodoDe[v] = periodoDoFim;
                if (transienteDoFim > maiorTransiente)
                {
                    maiorTransiente = transienteDoFim;
                    maisDemorado = v;
                }
            }
        }

        return new Destinos(largura, altura, total, ciclos.Count, maiorPeriodo,
                            maiorTransiente, maisDemorado);
    }
}
