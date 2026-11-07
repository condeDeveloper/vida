using Conde.Vida;
using Xunit;

namespace Conde.Vida.Testes;

/// <summary>
/// O comportamento: a regra, a caixa e as formas. Os numeros das tabelas ficam
/// no <c>MedidasTestes</c>.
/// </summary>
public class NucleoTestes
{
    [Fact]
    public void OBlocoNaoSeMexe()
    {
        var caixa = new Caixa(4, 4);
        var bloco = Juiz.Ler(caixa, Juiz.Conhecidas["bloco"]);
        Assert.True(caixa.Estatica(bloco));
        Assert.Equal(4, caixa.Vivas(bloco));
    }

    [Fact]
    public void OPiscaPiscaTemPeriodoDois()
    {
        var caixa = new Caixa(3, 3);
        var deitado = Juiz.Ler(caixa, new[] { "...", "###", "..." });
        var depe = caixa.Passo(deitado);
        Assert.NotEqual(deitado, depe);
        Assert.Equal(deitado, caixa.Passo(depe));
        Assert.Equal(Juiz.Ler(caixa, new[] { ".#.", ".#.", ".#." }), depe);
    }

    [Fact]
    public void TresCelulasEmLNaoSobrevivemIguais()
    {
        var caixa = new Caixa(3, 3);
        var ele = Juiz.Ler(caixa, new[] { "##.", "#..", "..." });
        // Com tres em L nasce a quarta e vira bloco: o L e o berco do bloco.
        Assert.Equal(Juiz.Ler(caixa, Juiz.Conhecidas["bloco"]), caixa.Passo(ele));
    }

    [Fact]
    public void ACaixaNaoDaAVolta()
    {
        // Tres celulas na coluna da esquerda. Se a caixa fosse um toro, a coluna
        // da direita contaria como vizinha e o resultado seria outro.
        var caixa = new Caixa(3, 3);
        var coluna = Juiz.Ler(caixa, new[] { "#..", "#..", "#.." });
        Assert.Equal(Juiz.Ler(caixa, new[] { "...", "##.", "..." }), caixa.Passo(coluna));
    }

    [Theory]
    [InlineData("bloco")]
    [InlineData("tina")]
    [InlineData("barco")]
    [InlineData("cobra")]
    [InlineData("colmeia")]
    [InlineData("pao")]
    [InlineData("navio")]
    public void AsEstaticasDeNomeProprioSaoEstaticasPelosDoisCaminhos(string nome)
    {
        var caixa = new Caixa(5, 5);
        var padrao = Juiz.Ler(caixa, Juiz.Conhecidas[nome]);
        Assert.True(caixa.Estatica(padrao));

        var porMatriz = Juiz.Fechar(caixa, Juiz.Passo(Juiz.Abrir(caixa, padrao)));
        Assert.Equal(padrao, porMatriz);
    }

    [Fact]
    public void EncostarJuntaOQueSoDifereDeLugar()
    {
        var caixa = new Caixa(5, 5);
        var noCanto = Juiz.Ler(caixa, new[] { "##...", "##..." });
        var noMeio = Juiz.Ler(caixa, new[] { ".....", "..##.", "..##." });
        Assert.NotEqual(noCanto, noMeio);
        Assert.Equal(caixa.Encostar(noCanto), caixa.Encostar(noMeio));
    }

    [Fact]
    public void GirarEEspelharNaoMudamAFormaEssencial()
    {
        var caixa = new Caixa(5, 5);
        var barco = Juiz.Ler(caixa, Juiz.Conhecidas["barco"]);
        var girado = caixa.Girar(barco);
        var espelhado = caixa.Espelhar(barco);
        Assert.Equal(caixa.FormaEssencial(barco), caixa.FormaEssencial(girado));
        Assert.Equal(caixa.FormaEssencial(barco), caixa.FormaEssencial(espelhado));
    }

    [Fact]
    public void GirarQuatroVezesVoltaAoComeco()
    {
        var caixa = new Caixa(5, 5);
        var pao = Juiz.Ler(caixa, Juiz.Conhecidas["pao"]);
        Assert.Equal(pao, caixa.Girar(caixa.Girar(caixa.Girar(caixa.Girar(pao)))));
        Assert.Equal(pao, caixa.Espelhar(caixa.Espelhar(pao)));
    }

    [Fact]
    public void OVazioEEstaticoEOCheioNao()
    {
        var caixa = new Caixa(3, 3);
        Assert.True(caixa.Estatica(0));
        Assert.False(caixa.Estatica(0b111_111_111));
    }

    [Fact]
    public void ACaixaNaoPassaDeSessentaEQuatroCelulas()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Caixa(9, 8));
        var cheia = new Caixa(8, 8);
        Assert.Equal(64, cheia.Celulas);
    }

    [Fact]
    public void ODesenhoVoltaIgual()
    {
        var caixa = new Caixa(4, 2);
        var cobra = Juiz.Ler(caixa, Juiz.Conhecidas["cobra"]);
        Assert.Equal("##.#\n#.##", caixa.Desenhar(cobra));
    }

    [Fact]
    public void OsDoisCaminhosConcordamNaCaixaDeTresPorTres()
    {
        var caixa = new Caixa(3, 3);
        for (ulong p = 0; p < caixa.Padroes; p++)
        {
            var porBits = caixa.Passo(p);
            var porMatriz = Juiz.Fechar(caixa, Juiz.Passo(Juiz.Abrir(caixa, p)));
            Assert.Equal(porBits, porMatriz);
        }
    }
}
