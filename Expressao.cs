using System;
using System.Linq.Expressions;

namespace CAS;

public abstract class Expressao
{
    public abstract override string ToString();
    public abstract Expressao Derivar(Simbolo x);
    public abstract Expressao Simplificar();

    public static Expressao operator +(Expressao a, Expressao b) => new Soma(a, b).Simplificar();
    public static Expressao operator -(Expressao a, Expressao b) => new Subtracao(a, b).Simplificar();
    public static Expressao operator *(Expressao a, Expressao b) => new Multiplicacao(a, b).Simplificar();
    public static Expressao operator /(Expressao a, Expressao b) => new Divisao(a, b).Simplificar();

    public static implicit operator Expressao(int v) => new Numero(v);
    public static implicit operator Expressao(string s) => new Simbolo(s);   
}

public class Numero : Expressao
{
    public int valor;
    public Numero(int v) => this.valor = v;
    public override string ToString() => valor.ToString();
    public override Expressao Derivar(Simbolo x) => new Numero(0);
    public override Expressao Simplificar() => this;
}

public class Simbolo : Expressao
{
    string simbolo;
    public Simbolo(string s) => this.simbolo = s;
    public override string ToString() => simbolo;
    public override Expressao Derivar(Simbolo x) => 
        x.simbolo == simbolo 
            ? new Numero(1) 
            : new Numero(0);
    public override Expressao Simplificar() => this;
}

public class Soma : Expressao
{
    Expressao a, b; // a + b
    public Soma(Expressao x, Expressao y)
    {
        this.a = x;
        this.b = y;
    }
    public override string ToString() => $"({a.ToString()} + {b.ToString()})";
    public override Expressao Derivar(Simbolo x) => 
        new Soma(a.Derivar(x), b.Derivar(x));
    public override Expressao Simplificar(){
       if(a is Numero && b is Numero)
       {
          return new Numero((a as Numero).valor + (b as Numero).valor);
       }
       return this;
    }
}

public class Subtracao : Expressao
{
    Expressao a, b; // a - b
    public Subtracao(Expressao x, Expressao y)
    {
        this.a = x;
        this.b = y;
    }
    public override string ToString() => $"({a.ToString()} - {b.ToString()})";
    public override Expressao Derivar(Simbolo x) => 
        new Subtracao(a.Derivar(x), b.Derivar(x));
    public override Expressao Simplificar()
    {
        if(a is Numero && b is Numero)
        {
            return new Numero((a as Numero).valor - (b as Numero).valor);
        }
        return this;
    }
}

public class Multiplicacao : Expressao
{
    Expressao a, b; // a * b
    public Multiplicacao(Expressao x, Expressao y)
    {
        this.a = x;
        this.b = y;
    }
    public override string ToString() => $"({a.ToString()} * {b.ToString()})";
    public override Expressao Derivar(Simbolo x) =>
        new Soma(
            new Multiplicacao(a.Derivar(x), b),
            new Multiplicacao(a, b.Derivar(x)));
    public override Expressao Simplificar()
    {
        if (a is Numero && b is Numero)
        {
            return new Numero((a as Numero).valor * (b as Numero).valor);
        }
        return this;
    }
}

public class Divisao : Expressao
{
    Expressao a, b; // a / b
    public Divisao(Expressao x, Expressao y)
    {
        this.a = x;
        this.b = y;
    }
    public override string ToString() => $"({a.ToString()} / {b.ToString()})";
    public override Expressao Derivar(Simbolo x) =>
        new Divisao(
            new Subtracao(
                new Multiplicacao(a.Derivar(x), b), 
                new Multiplicacao(a, b.Derivar(x))),
            new Multiplicacao(b, b));
    public override Expressao Simplificar()
    {
        return this;
    }
}

// Implementação de números complexos
public class Complexo : Expressao
{
    public double Real { get; }
    public double Imaginario { get; }

    public Complexo(double real, double imaginario)
    {
        Real = real;
        Imaginario = imaginario;
    }

    public override string ToString() => $"{Real} + {Imaginario}i";

    public override Expressao Derivar(Simbolo x) => new Complexo(0, 0);

    public override Expressao Simplificar() => this;

    public static Complexo operator +(Complexo a, Complexo b) => new Complexo(a.Real + b.Real, a.Imaginario + b.Imaginario);
    public static Complexo operator -(Complexo a, Complexo b) => new Complexo(a.Real - b.Real, a.Imaginario - b.Imaginario);
    public static Complexo operator *(Complexo a, Complexo b) => new Complexo(a.Real * b.Real - a.Imaginario * b.Imaginario, a.Real * b.Imaginario + a.Imaginario * b.Real);
    public static Complexo operator /(Complexo a, Complexo b)
    {
        double denom = b.Real * b.Real + b.Imaginario * b.Imaginario;
        return new Complexo((a.Real * b.Real + a.Imaginario * b.Imaginario) / denom, (a.Imaginario * b.Real - a.Real * b.Imaginario) / denom);
    }
}

// Função para substituir símbolos em uma expressão
public static class ExpressaoExtensions
{
    public static Expressao SubstituirSimbolo(this Expressao expressao, string simbolo, Expressao substituto)
    {
        if (expressao is Simbolo s && s.ToString() == simbolo)
        {
            return substituto;
        }
        if (expressao is Soma soma)
        {
            return new Soma(soma.a.SubstituirSimbolo(simbolo, substituto), soma.b.SubstituirSimbolo(simbolo, substituto)).Simplificar();
        }
        if (expressao is Subtracao subtracao)
        {
            return new Subtracao(subtracao.a.SubstituirSimbolo(simbolo, substituto), subtracao.b.SubstituirSimbolo(simbolo, substituto)).Simplificar();
        }
        if (expressao is Multiplicacao multiplicacao)
        {
            return new Multiplicacao(multiplicacao.a.SubstituirSimbolo(simbolo, substituto), multiplicacao.b.SubstituirSimbolo(simbolo, substituto)).Simplificar();
        }
        if (expressao is Divisao divisao)
        {
            return new Divisao(divisao.a.SubstituirSimbolo(simbolo, substituto), divisao.b.SubstituirSimbolo(simbolo, substituto)).Simplificar();
        }
        return expressao;
    }
}