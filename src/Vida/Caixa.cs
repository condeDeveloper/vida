namespace Conde.Vida;

/// <summary>
/// Uma caixa de largura por altura, com no maximo 64 celulas. Um padrao e um
/// <c>ulong</c>: o bit y*largura + x vale 1 quando a celula esta viva. Fora da
/// caixa e sempre morto — a caixa nao e um toro, e a borda nao da a volta.
///
/// Guardar o padrao num inteiro de 64 bits e o que torna a varredura possivel:
/// a caixa 5x5 tem 33.554.432 padroes, e cada um precisa caber num registrador.
/// </summary>
public sealed class Caixa
{
    private readonly ulong[] vizinhanca;

    public Caixa(int largura, int altura)
    {
        if (largura < 1 || altura < 1 || largura * altura > 64)
            throw new ArgumentOutOfRangeException(nameof(largura), "a caixa nao passa de 64 celulas");

        Largura = largura;
        Altura = altura;
        Celulas = largura * altura;
        Padroes = Celulas == 64 ? ulong.MaxValue : (1UL << Celulas);

        vizinhanca = new ulong[Celulas];
        for (var y = 0; y < altura; y++)
            for (var x = 0; x < largura; x++)
            {
                ulong m = 0;
                for (var dy = -1; dy <= 1; dy++)
                    for (var dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        var nx = x + dx;
                        var ny = y + dy;
                        if (nx < 0 || ny < 0 || nx >= largura || ny >= altura) continue;
                        m |= 1UL << (ny * largura + nx);
                    }
                vizinhanca[y * largura + x] = m;
            }
    }

    public int Largura { get; }
    public int Altura { get; }
    public int Celulas { get; }

    /// <summary>Quantos padroes cabem na caixa: 2 elevado ao numero de celulas.</summary>
    public ulong Padroes { get; }

    public bool Viva(ulong padrao, int x, int y) => (padrao & (1UL << (y * Largura + x))) != 0;

    public int Vivas(ulong padrao) => System.Numerics.BitOperations.PopCount(padrao);

    /// <summary>
    /// A geracao seguinte, pela regra B3/S23: nasce com tres vizinhos, sobrevive
    /// com dois ou tres.
    /// </summary>
    public ulong Passo(ulong padrao)
    {
        ulong proximo = 0;
        for (var i = 0; i < Celulas; i++)
        {
            var vizinhos = System.Numerics.BitOperations.PopCount(padrao & vizinhanca[i]);
            var viva = (padrao & (1UL << i)) != 0;
            if (vizinhos == 3 || (viva && vizinhos == 2)) proximo |= 1UL << i;
        }
        return proximo;
    }

    /// <summary>Um padrao que nao muda de uma geracao para a outra.</summary>
    public bool Estatica(ulong padrao) => Passo(padrao) == padrao;

    /// <summary>
    /// A forma do padrao encostada no canto: desloca para a esquerda e para
    /// cima ate tocar as bordas. Dois padroes iguais a menos de translacao tem a
    /// mesma forma.
    /// </summary>
    public ulong Encostar(ulong padrao)
    {
        if (padrao == 0) return 0;

        var minX = Largura;
        var minY = Altura;
        for (var y = 0; y < Altura; y++)
            for (var x = 0; x < Largura; x++)
                if (Viva(padrao, x, y))
                {
                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                }

        ulong encostado = 0;
        for (var y = 0; y < Altura; y++)
            for (var x = 0; x < Largura; x++)
                if (Viva(padrao, x, y))
                    encostado |= 1UL << ((y - minY) * Largura + (x - minX));
        return encostado;
    }

    /// <summary>
    /// A menor das oito formas que as simetrias do quadrado produzem — quatro
    /// rotacoes e o espelho de cada uma, todas encostadas no canto. Padroes que
    /// so diferem por girar ou espelhar tem a mesma forma essencial.
    /// </summary>
    public ulong FormaEssencial(ulong padrao)
    {
        var melhor = ulong.MaxValue;
        var atual = padrao;
        for (var espelho = 0; espelho < 2; espelho++)
        {
            for (var giro = 0; giro < 4; giro++)
            {
                var encostado = Encostar(atual);
                if (encostado < melhor) melhor = encostado;
                atual = Girar(atual);
            }
            atual = Espelhar(atual);
        }
        return melhor;
    }

    /// <summary>
    /// Gira um quarto de volta. So faz sentido em caixa quadrada: numa caixa
    /// torta o giro trocaria largura por altura e o padrao nao caberia.
    /// </summary>
    public ulong Girar(ulong padrao)
    {
        if (Largura != Altura) return padrao;
        ulong girado = 0;
        for (var y = 0; y < Altura; y++)
            for (var x = 0; x < Largura; x++)
                if (Viva(padrao, x, y))
                    girado |= 1UL << (x * Largura + (Altura - 1 - y));
        return girado;
    }

    public ulong Espelhar(ulong padrao)
    {
        ulong espelhado = 0;
        for (var y = 0; y < Altura; y++)
            for (var x = 0; x < Largura; x++)
                if (Viva(padrao, x, y))
                    espelhado |= 1UL << (y * Largura + (Largura - 1 - x));
        return espelhado;
    }

    /// <summary>A caixa desenhada, para mensagem de teste.</summary>
    public string Desenhar(ulong padrao)
    {
        var linhas = new List<string>(Altura);
        for (var y = 0; y < Altura; y++)
        {
            var linha = new char[Largura];
            for (var x = 0; x < Largura; x++) linha[x] = Viva(padrao, x, y) ? '#' : '.';
            linhas.Add(new string(linha));
        }
        return string.Join("\n", linhas);
    }
}
