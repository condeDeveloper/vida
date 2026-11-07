using Conde.Vida;

var qual = args.Length > 0 ? args[0] : "tudo";

if (qual is "censo" or "tudo")
{
    Console.WriteLine("caixa | padroes | estaticas | periodo 2 | sem predecessor | formas estaticas");
    foreach (var c in Medidas.Censos())
        Console.WriteLine($"{c.Largura}x{c.Altura}   | {c.Padroes,9} | {c.Estaticas,9} | {c.Periodo2,9} | {c.SemPredecessor,15} | {c.FormasEstaticas,16}");
    Console.WriteLine();
}

if (qual is "destino" or "tudo")
{
    Console.WriteLine("caixa | padroes | atratores | maior periodo | maior transiente");
    foreach (var d in Medidas.Destinos())
        Console.WriteLine($"{d.Largura}x{d.Altura}   | {d.Padroes,9} | {d.Atratores,9} | {d.MaiorPeriodo,13} | {d.MaiorTransiente,16}");
    Console.WriteLine();
}

if (qual is "formas" or "tudo")
{
    Console.WriteLine("celulas vivas | formas estaticas na caixa 5x5 | formas essenciais");
    var porTranslacao = Medidas.FormasEstaticas(5, 5);
    var essenciais = Medidas.FormasEssenciais(5);
    foreach (var par in porTranslacao)
        Console.WriteLine($"{par.Key,13} | {par.Value,29} | {essenciais[par.Key],17}");
    Console.WriteLine();
}

if (qual is "conhecidas" or "tudo")
{
    Console.WriteLine("nome | celulas | estatica | achada pela varredura 5x5");
    foreach (var (nome, celulas, estatica, achada) in Medidas.Conhecidas(5))
        Console.WriteLine($"{nome,-8} | {celulas,7} | {estatica,8} | {achada,25}");
    Console.WriteLine();
}

if (qual is "juizes" or "tudo")
{
    Console.WriteLine("caixa | padroes conferidos | divergencias");
    foreach (var (largura, altura, conferidos, divergencias) in Medidas.Conferencias())
        Console.WriteLine($"{largura}x{altura}   | {conferidos,18} | {divergencias,12}");
    Console.WriteLine();
}
