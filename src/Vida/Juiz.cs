namespace Conde.Vida;

/// <summary>
/// O juiz nao usa <see cref="Caixa"/>, nao usa <c>ulong</c> e nao usa mascara
/// de vizinhanca pre-calculada. Ele tem uma matriz de <c>bool</c> e conta os
/// oito vizinhos com dois lacos, do jeito que a regra e escrita no papel.
///
/// Os dois caminhos passam pelos mesmos padroes. Se divergirem uma vez, um dos
/// dois esta errado — e nao da para saber qual sem olhar.
/// </summary>
public static class Juiz
{
    /// <summary>A geracao seguinte, lida ao pe da letra numa matriz de bool.</summary>
    public static bool[,] Passo(bool[,] tabuleiro)
    {
        var altura = tabuleiro.GetLength(0);
        var largura = tabuleiro.GetLength(1);
        var proximo = new bool[altura, largura];

        for (var y = 0; y < altura; y++)
            for (var x = 0; x < largura; x++)
            {
                var vizinhos = 0;
                for (var dy = -1; dy <= 1; dy++)
                    for (var dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        var ny = y + dy;
                        var nx = x + dx;
                        if (ny < 0 || nx < 0 || ny >= altura || nx >= largura) continue;
                        if (tabuleiro[ny, nx]) vizinhos++;
                    }

                proximo[y, x] = vizinhos == 3 || (tabuleiro[y, x] && vizinhos == 2);
            }

        return proximo;
    }

    public static bool[,] Abrir(Caixa caixa, ulong padrao)
    {
        var tabuleiro = new bool[caixa.Altura, caixa.Largura];
        for (var y = 0; y < caixa.Altura; y++)
            for (var x = 0; x < caixa.Largura; x++) tabuleiro[y, x] = caixa.Viva(padrao, x, y);
        return tabuleiro;
    }

    public static ulong Fechar(Caixa caixa, bool[,] tabuleiro)
    {
        ulong padrao = 0;
        for (var y = 0; y < caixa.Altura; y++)
            for (var x = 0; x < caixa.Largura; x++)
                if (tabuleiro[y, x]) padrao |= 1UL << (y * caixa.Largura + x);
        return padrao;
    }

    /// <summary>
    /// Passa a caixa inteira pelos dois caminhos e conta as divergencias. Zero e
    /// a unica resposta aceitavel.
    /// </summary>
    public static (ulong Conferidos, long Divergencias) Conferir(int largura, int altura)
    {
        var caixa = new Caixa(largura, altura);
        long divergencias = 0;

        for (ulong p = 0; p < caixa.Padroes; p++)
        {
            var porBits = caixa.Passo(p);
            var porMatriz = Fechar(caixa, Passo(Abrir(caixa, p)));
            if (porBits != porMatriz) divergencias++;
        }

        return (caixa.Padroes, divergencias);
    }

    /// <summary>
    /// As vidas estaticas conhecidas pelo nome, desenhadas a mao. Servem de
    /// terceiro juiz: a varredura tem que achar todas, e elas tem que ser
    /// estaticas pelos dois caminhos.
    /// </summary>
    public static readonly Dictionary<string, string[]> Conhecidas = new()
    {
        ["bloco"] = new[] { "##", "##" },
        ["tina"] = new[] { ".#.", "#.#", ".#." },
        ["barco"] = new[] { "##.", "#.#", ".#." },
        ["cobra"] = new[] { "##.#", "#.##" },
        ["colmeia"] = new[] { ".##.", "#..#", ".##." },
        ["pao"] = new[] { ".##.", "#..#", ".#.#", "..#." },
        ["navio"] = new[] { "##.", "#.#", ".##" },
    };

    /// <summary>Le um desenho e devolve o padrao dentro da caixa, no canto.</summary>
    public static ulong Ler(Caixa caixa, string[] desenho)
    {
        ulong padrao = 0;
        for (var y = 0; y < desenho.Length; y++)
            for (var x = 0; x < desenho[y].Length; x++)
                if (desenho[y][x] == '#') padrao |= 1UL << (y * caixa.Largura + x);
        return padrao;
    }
}
