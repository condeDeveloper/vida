using Conde.Vida;
using Xunit;

namespace Conde.Vida.Testes;

/// <summary>Os numeros das tabelas do README, presos. Todos saem do medidor.</summary>
public class MedidasTestes
{
    [Fact]
    public void ATabelaDoCenso()
    {
        var linhas = Medidas.Censos();
        Assert.Equal(5, linhas.Count);

        Assert.Equal(new Censo(3, 3, 512, 12, 2, 315, 9), linhas[0]);
        Assert.Equal(new Censo(4, 3, 4096, 42, 4, 2927, 28), linhas[1]);
        Assert.Equal(new Censo(4, 4, 65536, 158, 24, 53877, 115), linhas[2]);
        Assert.Equal(new Censo(5, 4, 1048576, 321, 46, 906597, 199), linhas[3]);
        Assert.Equal(new Censo(5, 5, 33554432, 725, 88, 30197870, 431), linhas[4]);
    }

    [Fact]
    public void QuaseTudoNaoTemPredecessorDentroDaCaixa()
    {
        // A fracao de padroes que nenhum outro padrao da caixa produz cresce com
        // o tamanho da caixa, e ja passa de 85% na 5x4.
        foreach (var c in Medidas.Censos())
        {
            var fracao = (double)c.SemPredecessor / c.Padroes;
            Assert.True(fracao > 0.6);
        }
        var cinco = Medidas.Censos()[4];
        Assert.Equal(90.0, Math.Round(100.0 * cinco.SemPredecessor / cinco.Padroes, 1));
    }

    [Fact]
    public void ATabelaDosDestinos()
    {
        var linhas = Medidas.Destinos();
        Assert.Equal(4, linhas.Count);

        Assert.Equal((3, 3, 512UL, 13, 2, 6), Corte(linhas[0]));
        Assert.Equal((4, 3, 4096UL, 44, 2, 10), Corte(linhas[1]));
        Assert.Equal((4, 4, 65536UL, 174, 3, 14), Corte(linhas[2]));
        Assert.Equal((5, 4, 1048576UL, 351, 18, 28), Corte(linhas[3]));
    }

    private static (int, int, ulong, int, int, int) Corte(Destinos d) =>
        (d.Largura, d.Altura, d.Padroes, d.Atratores, d.MaiorPeriodo, d.MaiorTransiente);

    [Fact]
    public void OPeriodoDezoitoCabeNumaCaixaDeCincoPorQuatro()
    {
        // O achado que me surpreendeu: numa caixa de vinte celulas existe um
        // ciclo de dezoito geracoes, enquanto a caixa de 4x4, so quatro celulas
        // menor, nao passa de tres.
        var cinco = Medidas.Destinos()[3];
        var quatro = Medidas.Destinos()[2];
        Assert.Equal(18, cinco.MaiorPeriodo);
        Assert.Equal(3, quatro.MaiorPeriodo);
    }

    [Fact]
    public void ATabelaDasFormasEstaticas()
    {
        var porTranslacao = Medidas.FormasEstaticas(5, 5);
        var essenciais = Medidas.FormasEssenciais(5);

        var esperado = new (int Celulas, int Translacao, int Essencial)[]
        {
            (0, 1, 1), (4, 2, 2), (5, 4, 1), (6, 18, 6), (7, 36, 6), (8, 77, 16),
            (9, 76, 13), (10, 54, 9), (11, 58, 10), (12, 72, 14), (13, 26, 6),
            (14, 6, 2), (16, 1, 1),
        };

        Assert.Equal(esperado.Length, porTranslacao.Count);
        foreach (var (celulas, translacao, essencial) in esperado)
        {
            Assert.Equal(translacao, porTranslacao[celulas]);
            Assert.Equal(essencial, essenciais[celulas]);
        }

        // Nao existe vida estatica de 1, 2, 3 ou 15 celulas dentro da caixa 5x5.
        foreach (var vazio in new[] { 1, 2, 3, 15 })
            Assert.False(porTranslacao.ContainsKey(vazio));

        // E as formas somadas tem que dar as 431 do censo.
        Assert.Equal(431, porTranslacao.Values.Sum());
    }

    [Fact]
    public void ComCincoCelulasSoExisteUmaFormaEssencial()
    {
        // Quatro posicoes diferentes na caixa, uma forma so: o barco. Com quatro
        // celulas sao duas, o bloco e a tina.
        var essenciais = Medidas.FormasEssenciais(5);
        Assert.Equal(1, essenciais[5]);
        Assert.Equal(2, essenciais[4]);
    }

    [Fact]
    public void AsEstaticasDeNomeProprioForamTodasEncontradas()
    {
        var linhas = Medidas.Conhecidas(5);
        Assert.Equal(7, linhas.Count);
        Assert.All(linhas, l => Assert.True(l.Estatica));
        Assert.All(linhas, l => Assert.True(l.Achada));

        Assert.Equal(("bloco", 4, true, true), linhas[0]);
        Assert.Equal(("cobra", 6, true, true), linhas[3]);
        Assert.Equal(("pao", 7, true, true), linhas[5]);
    }

    [Fact]
    public void OsDoisJuizesNaoDivergemUmaVez()
    {
        var linhas = Medidas.Conferencias();
        Assert.Equal(3, linhas.Count);
        Assert.Equal((3, 3, 512UL, 0L), linhas[0]);
        Assert.Equal((4, 3, 4096UL, 0L), linhas[1]);
        Assert.Equal((4, 4, 65536UL, 0L), linhas[2]);
        Assert.Equal(70144L, linhas.Sum(l => (long)l.Conferidos));
    }
}
