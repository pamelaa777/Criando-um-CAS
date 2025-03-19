using CAS;
using System.Numerics;

Expressao a = 10;
Expressao b = "b";

Expressao soma = a + b;
Expressao c = 50;

Console.WriteLine(a + c);

// Exemplo de uso de números complexos
Complexo c1 = new Complexo(1, 2);
Complexo c2 = new Complexo(3, 4);
Console.WriteLine($"Soma de complexos: {c1 + c2}");
Console.WriteLine($"Subtração de complexos: {c1 - c2}");
Console.WriteLine($"Multiplicação de complexos: {c1 * c2}");
Console.WriteLine($"Divisão de complexos: {c1 / c2}");

// Exemplo de substituição de símbolo
Expressao expressao = new Soma(new Simbolo("x"), new Numero(2));
Expressao novaExpressao = expressao.SubstituirSimbolo("x", new Numero(5));
Console.WriteLine($"Expressão após substituição: {novaExpressao}");

// Exemplo de simplificação de expressão
Expressao expressaoSimplificada = new Soma(new Numero(2), new Numero(3)).Simplificar();
Console.WriteLine($"Expressão simplificada: {expressaoSimplificada}");