# vida

**Numa caixa de cinco por cinco cabem 33.554.432 desenhos. Quantos deles o jogo
da vida não mexe?**

Setecentos e vinte e cinco. É 0,00216% de tudo que cabe na caixa.

O jogo da vida de Conway costuma aparecer como animação: a gente põe um padrão,
aperta o play e olha. Aqui ele aparece como **domínio finito**. Uma caixa de
largura por altura tem um número exato de padrões, cabe num `ulong`, e dá para
passar por todos — sem amostra, sem sorteio, sem "rodei umas mil vezes".

A regra é a de sempre, B3/S23: nasce com três vizinhos, sobrevive com dois ou
três. E a caixa **não dá a volta**: fora dela é sempre morto, nunca um toro.

## O censo das caixas

| caixa | padrões | estáticas | período 2 | sem predecessor | formas estáticas |
|:-----|--------:|----------:|----------:|----------------:|-----------------:|
| 3×3 | 512 | 12 | 2 | 315 | 9 |
| 4×3 | 4.096 | 42 | 4 | 2.927 | 28 |
| 4×4 | 65.536 | 158 | 24 | 53.877 | 115 |
| 5×4 | 1.048.576 | 321 | 46 | 906.597 | 199 |
| 5×5 | 33.554.432 | 725 | 88 | 30.197.870 | 431 |

"Estáticas" conta padrões; "formas" conta padrões a menos de translação — o
bloco no canto e o bloco no meio são a mesma forma.

"Sem predecessor" são os padrões que **nenhum** padrão da caixa produz. É o
número que mais cresce: 61,5% na caixa 3×3, 82,2% na 4×4, **90,0%** na 5×5.
Quase tudo que cabe na caixa é um desenho que a caixa não sabe desenhar.

Uma ressalva que o número merece: isso **não** é a contagem dos jardins do Éden
do jogo da vida. Um jardim do Éden de verdade é um padrão que ninguém produz no
plano infinito, e aqui o predecessor só pode estar dentro da caixa. O número é
exato, mas é sobre a caixa, não sobre o plano.

## Para onde cada padrão vai

A caixa é finita, então todo padrão acaba caindo num ciclo. O que muda é quanto
tempo leva para chegar lá (o transiente) e em que ciclo cai (o atrator).

| caixa | padrões | atratores | maior período | maior transiente |
|:-----|--------:|----------:|--------------:|-----------------:|
| 3×3 | 512 | 13 | 2 | 6 |
| 4×3 | 4.096 | 44 | 2 | 10 |
| 4×4 | 65.536 | 174 | 3 | 14 |
| 5×4 | 1.048.576 | 351 | 18 | 28 |

A linha que me fez conferir duas vezes é a última. A caixa 4×4 não tem ciclo
maior que **3**. Quatro células a mais, e aparece um ciclo de **18** gerações.
Não é um crescimento: é um salto.

## As vidas estáticas, por tamanho

Dentro da caixa 5×5, contadas de dois jeitos: como formas (a menos de
translação) e como formas essenciais (a menos de translação, giro e espelho).

| células vivas | formas | formas essenciais |
|--------------:|-------:|------------------:|
| 0 | 1 | 1 |
| 4 | 2 | 2 |
| 5 | 4 | 1 |
| 6 | 18 | 6 |
| 7 | 36 | 6 |
| 8 | 77 | 16 |
| 9 | 76 | 13 |
| 10 | 54 | 9 |
| 11 | 58 | 10 |
| 12 | 72 | 14 |
| 13 | 26 | 6 |
| 14 | 6 | 2 |
| 16 | 1 | 1 |

Não existe vida estática com **1, 2, 3 ou 15** células dentro da caixa. Uma,
duas ou três células sempre morrem ou crescem; quinze é o buraco interessante,
cercado por 26 formas de treze e 6 de catorze.

Com cinco células existem quatro posições e **uma forma essencial só**: o barco.
Com quatro existem duas, o bloco e a tina. Com dezesseis existe exatamente uma —
quatro blocos, um em cada canto.

## As de nome próprio

| nome | células | estática | achada pela varredura |
|:-----|--------:|:--------:|:---------------------:|
| bloco | 4 | sim | sim |
| tina | 4 | sim | sim |
| barco | 5 | sim | sim |
| cobra | 6 | sim | sim |
| colmeia | 6 | sim | sim |
| navio | 6 | sim | sim |
| pão | 7 | sim | sim |

Nenhuma delas foi plantada no código da busca: elas estão num arquivo separado,
desenhadas à mão, e a varredura tem que **encontrá-las sozinha**.

## Os dois juízes

A regra é implementada duas vezes, por caminhos que não se falam: uma por
máscara de bits com a vizinhança pré-calculada, outra por matriz de `bool` com
dois laços contando os oito vizinhos, do jeito que está escrito no papel.

| caixa | padrões conferidos | divergências |
|:-----|-------------------:|-------------:|
| 3×3 | 512 | 0 |
| 4×3 | 4.096 | 0 |
| 4×4 | 65.536 | 0 |

70.144 padrões, nenhuma divergência.

## O que as medidas me corrigiram

- **Desenhei a cobra de cabeça e ela não era estática.** Escrevi `#.#` sobre
  `###`, que é bonito e está errado: a célula do meio fica com quatro vizinhos e
  morre. A cobra é `##.#` sobre `#.##`, de seis células. O arquivo das estáticas
  de nome próprio existe justamente para isso — ele acusou na primeira rodada.
- **Achei que o maior período crescesse devagar.** De 4×4 para 5×4 ele pula de
  3 para 18. Período longo não cabe em caixa pequena por um motivo de espaço, e
  o espaço mínimo aparece de uma vez, não aos poucos.
- **Quase chamei "sem predecessor" de jardim do Éden.** São coisas diferentes: o
  predecessor pode estar fora da caixa. O número continua exato; a frase é que
  estava errada.
- **Esperava mais formas pequenas.** Com cinco células há uma forma essencial
  só, e com 1, 2, 3 ou 15 não há nenhuma. A distribuição não é uma ladeira: tem
  buraco no meio.

## Quatro achados

1. Das **33.554.432** configurações da caixa 5×5, só **725** não mudam — 0,002%
   — e elas são **431** formas distintas a menos de translação.
2. A fração de padrões que a própria caixa não consegue produzir sobe de
   **61,5%** na caixa 3×3 para **90,0%** na 5×5.
3. O maior ciclo salta de **3 gerações na caixa 4×4 para 18 na 5×4**, com só
   quatro células a mais de espaço.
4. Não existe vida estática com **1, 2, 3 ou 15 células** dentro da caixa 5×5,
   embora existam 26 formas com treze e 6 com catorze.

## Rodar

```bash
dotnet build src/Vida/Vida.csproj -c Release
dotnet run --project ferramentas/Medidor -c Release -- censo
dotnet run --project ferramentas/Medidor -c Release -- destino
dotnet run --project ferramentas/Medidor -c Release -- formas
dotnet run --project ferramentas/Medidor -c Release -- conhecidas
dotnet run --project ferramentas/Medidor -c Release -- juizes
```

São 26 testes.

```bash
dotnet test testes/Vida.Testes/Vida.Testes.csproj -c Release
```

## Licença

MIT.
