using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
public class CPHInline
{
    public bool Execute()
    {
        CPH.TryGetArg("user", out string user);
        CPH.TryGetArg("rawInput", out string rawInput);
        string expr = NormalizeInput(rawInput);
        if (string.IsNullOrWhiteSpace(expr))
        {
            CPH.SendMessage($"{Mention(user)} try including a math expression like \"= 365 / 7\". I can also handle paranthesis and many many functions.");
            return true;
        }
        try
        {
            double value = MathEvaluator.Evaluate(expr);
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                CPH.SendMessage($"{Mention(user)} that expression isn't a finite number.");
                return true;
            }
            CPH.SendMessage($"{Mention(user)} {expr} = {FormatNumber(value)}");
        }
        catch (MathEvalException ex)
        {
            CPH.SendMessage($"{Mention(user)} {ex.Message}");
        }
        catch (Exception)
        {
            CPH.SendMessage($"{Mention(user)} I couldn't evaluate that expression.");
        }
        return true;
    }
    static string NormalizeInput(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "";
        string s = raw.Trim();
        while (s.StartsWith("="))
            s = s.Substring(1).Trim();
        return s;
    }
    static string Mention(string user)
    {
        return string.IsNullOrWhiteSpace(user) ? "" : $"@{user}";
    }
    static string FormatNumber(double value)
    {
        if (Math.Abs(value) >= 1e12 || (Math.Abs(value) > 0 && Math.Abs(value) < 1e-6))
            return value.ToString("G10", CultureInfo.InvariantCulture);
        decimal d = Math.Round((decimal)value, 10, MidpointRounding.AwayFromZero);
        return d.ToString("0.##########", CultureInfo.InvariantCulture);
    }
}
sealed class MathEvalException : Exception
{
    public MathEvalException(string message) : base(message) { }
}
static class MathEvaluator
{
    const int MaxTokens = 256;
    const int MaxDepth = 32;
    public static double Evaluate(string expression)
    {
        var tokens = Tokenizer.Tokenize(expression);
        if (tokens.Count == 0)
            throw new MathEvalException("It was an empty expression.");
        if (tokens.Count > MaxTokens)
            throw new MathEvalException("The expression is too long.");
        var parser = new Parser(tokens);
        double result = parser.ParseExpression();
        parser.ExpectEnd();
        return result;
    }
    enum TokenKind { Number, Ident, Plus, Minus, Star, Slash, Percent, Caret, LParen, RParen, Comma }
    readonly struct Token
    {
        public readonly TokenKind Kind;
        public readonly string Text;
        public Token(TokenKind kind, string text) { Kind = kind; Text = text; }
    }
    static class Tokenizer
    {
        public static List<Token> Tokenize(string s)
        {
            var tokens = new List<Token>();
            int i = 0;
            while (i < s.Length)
            {
                char c = s[i];
                if (char.IsWhiteSpace(c)) { i++; continue; }
                switch (c)
                {
                    case '+': tokens.Add(new Token(TokenKind.Plus, "+")); i++; continue;
                    case '-': tokens.Add(new Token(TokenKind.Minus, "-")); i++; continue;
                    case '*': tokens.Add(new Token(TokenKind.Star, "*")); i++; continue;
                    case '/': tokens.Add(new Token(TokenKind.Slash, "/")); i++; continue;
                    case '%': tokens.Add(new Token(TokenKind.Percent, "%")); i++; continue;
                    case '^': tokens.Add(new Token(TokenKind.Caret, "^")); i++; continue;
                    case '(': tokens.Add(new Token(TokenKind.LParen, "(")); i++; continue;
                    case ')': tokens.Add(new Token(TokenKind.RParen, ")")); i++; continue;
                    case ',': tokens.Add(new Token(TokenKind.Comma, ",")); i++; continue;
                }
                if (c == '.' || char.IsDigit(c))
                {
                    int start = i;
                    bool sawDot = false;
                    while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.'))
                    {
                        if (s[i] == '.')
                        {
                            if (sawDot) throw new MathEvalException("There was an invalid number.");
                            sawDot = true;
                        }
                        i++;
                    }
                    if (i < s.Length && (s[i] == 'e' || s[i] == 'E'))
                    {
                        int ePos = i;
                        i++;
                        if (i < s.Length && (s[i] == '+' || s[i] == '-')) i++;
                        int expStart = i;
                        while (i < s.Length && char.IsDigit(s[i])) i++;
                        if (i == expStart)
                        {
                            i = ePos;
                        }
                    }
                    string num = s.Substring(start, i - start);
                    tokens.Add(new Token(TokenKind.Number, num));
                    continue;
                }
                if (char.IsLetter(c) || c == '_')
                {
                    int start = i;
                    i++;
                    while (i < s.Length && (char.IsLetterOrDigit(s[i]) || s[i] == '_')) i++;
                    tokens.Add(new Token(TokenKind.Ident, s.Substring(start, i - start).ToLowerInvariant()));
                    continue;
                }
                throw new MathEvalException($"There was an unexpected character '{c}'.");
            }
            return tokens;
        }
    }
    sealed class Parser
    {
        readonly List<Token> _tokens;
        int _pos;
        int _depth;
        public Parser(List<Token> tokens) { _tokens = tokens; }
        public void ExpectEnd()
        {
            if (!AtEnd)
                throw new MathEvalException("There was an unexpected extra input after the expression.");
        }
        bool AtEnd => _pos >= _tokens.Count;
        Token Peek => AtEnd ? default : _tokens[_pos];
        bool Check(TokenKind k) => !AtEnd && Peek.Kind == k;
        Token Advance()
        {
            if (AtEnd) throw new MathEvalException("There was an unexpected end of expression.");
            return _tokens[_pos++];
        }
        public double ParseExpression() => ParseAdd();
        double ParseAdd()
        {
            double left = ParseMul();
            while (Check(TokenKind.Plus) || Check(TokenKind.Minus))
            {
                Token op = Advance();
                double right = ParseMul();
                left = op.Kind == TokenKind.Plus ? left + right : left - right;
            }
            return left;
        }
        double ParseMul()
        {
            double left = ParsePow();
			double right;
            while (Check(TokenKind.Star) || Check(TokenKind.Slash) || Check(TokenKind.Percent) || StartsImplicitMul())
            {
                if (StartsImplicitMul())
                {
                    right = ParsePow();
                    left *= right;
                    continue;
                }
                Token op = Advance();
                right = ParsePow();
                if (op.Kind == TokenKind.Star)
                    left *= right;
                else if (op.Kind == TokenKind.Slash)
                {
                    if (right == 0) throw new MathEvalException("I ran into a division by zero.");
                    left /= right;
                }
                else
                {
                    if (right == 0) throw new MathEvalException("I ran into a modulo by zero.");
                    left %= right;
                }
            }
            return left;
        }
        bool StartsImplicitMul()
        {
            return Check(TokenKind.LParen) || Check(TokenKind.Ident);
        }
        double ParsePow()
        {
            double left = ParseUnary();
            if (Check(TokenKind.Caret))
            {
                Advance();
                double right = ParsePow();
                left = Math.Pow(left, right);
            }
            return left;
        }
        double ParseUnary()
        {
            if (Check(TokenKind.Plus)) { Advance(); return ParseUnary(); }
            if (Check(TokenKind.Minus)) { Advance(); return -ParseUnary(); }
            return ParsePrimary();
        }
        double ParsePrimary()
        {
            if (Check(TokenKind.Number))
            {
                string text = Advance().Text;
                if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double n))
                    throw new MathEvalException($"There is an invalid number '{text}'.");
                return n;
            }
            if (Check(TokenKind.Ident))
            {
                string name = Advance().Text;
                if (Check(TokenKind.LParen))
                    return CallFunction(name);
                return Constant(name);
            }
            if (Check(TokenKind.LParen))
            {
                Enter();
                Advance();
                double v = ParseExpression();
                if (!Check(TokenKind.RParen))
                    throw new MathEvalException("There is a missing ')'.");
                Advance();
                Leave();
                return v;
            }
            throw new MathEvalException("I expected a number, constant, function, or '('.");
        }
        double Constant(string name)
        {
            switch (name)
            {
                case "pi": return Math.PI;
                case "e": return Math.E;
                case "tau": return Math.PI * 2;
                default:
                    throw new MathEvalException($"I ran into an unknown name '{name}'.");
            }
        }
        double CallFunction(string name)
        {
            Enter();
            Advance(); // (
            var args = new List<double>();
            if (!Check(TokenKind.RParen))
            {
                args.Add(ParseExpression());
                while (Check(TokenKind.Comma))
                {
                    Advance();
                    args.Add(ParseExpression());
                }
            }
            if (!Check(TokenKind.RParen))
                throw new MathEvalException("There is a missing ')' after function arguments.");
            Advance();
            Leave();
            return ApplyFunction(name, args);
        }
        void Enter()
        {
            _depth++;
            if (_depth > MaxDepth)
                throw new MathEvalException("The expression is nested too deeply.");
        }
        void Leave() { _depth--; }
        static double ApplyFunction(string name, List<double> a)
        {
            int n = a.Count;
            switch (name)
            {
                case "abs": Require(n, 1, name); return Math.Abs(a[0]);
                case "sqrt":
                    Require(n, 1, name);
                    if (a[0] < 0) throw new MathEvalException("There was a square root of a negative number.");
                    return Math.Sqrt(a[0]);
                case "floor": Require(n, 1, name); return Math.Floor(a[0]);
                case "ceil":
                case "ceiling": Require(n, 1, name); return Math.Ceiling(a[0]);
                case "round": Require(n, 1, name); return Math.Round(a[0], MidpointRounding.AwayFromZero);
                case "sign": Require(n, 1, name); return Math.Sign(a[0]);
                case "exp": Require(n, 1, name); return Math.Exp(a[0]);
                case "ln":
                case "log":
                    if (n == 1)
                    {
                        if (a[0] <= 0) throw new MathEvalException("There was a log of a non-positive number.");
                        return Math.Log(a[0]);
                    }
                    Require(n, 2, name);
                    if (a[0] <= 0 || a[1] <= 0 || a[1] == 1)
                        throw new MathEvalException("There was an invalid log argument.");
                    return Math.Log(a[0], a[1]);
                case "log10":
                    Require(n, 1, name);
                    if (a[0] <= 0) throw new MathEvalException("There was a log of a non-positive number.");
                    return Math.Log10(a[0]);
                case "sin": Require(n, 1, name); return Math.Sin(a[0]);
                case "cos": Require(n, 1, name); return Math.Cos(a[0]);
                case "tan": Require(n, 1, name); return Math.Tan(a[0]);
                case "asin": Require(n, 1, name); return Math.Asin(a[0]);
                case "acos": Require(n, 1, name); return Math.Acos(a[0]);
                case "atan": Require(n, 1, name); return Math.Atan(a[0]);
                case "atan2": Require(n, 2, name); return Math.Atan2(a[0], a[1]);
                case "sinh": Require(n, 1, name); return Math.Sinh(a[0]);
                case "cosh": Require(n, 1, name); return Math.Cosh(a[0]);
                case "tanh": Require(n, 1, name); return Math.Tanh(a[0]);
                case "min": Require(n, 2, name); return Math.Min(a[0], a[1]);
                case "max": Require(n, 2, name); return Math.Max(a[0], a[1]);
                case "pow": Require(n, 2, name); return Math.Pow(a[0], a[1]);
                case "deg": Require(n, 1, name); return a[0] * (180.0 / Math.PI);
                case "rad": Require(n, 1, name); return a[0] * (Math.PI / 180.0);
                default:
                    throw new MathEvalException($"There was an unknown function '{name}'.");
            }
        }
        static void Require(int got, int need, string name)
        {
            if (got != need)
                throw new MathEvalException($"Unfortunately, {name}() expects {need} argument(s).");
        }
    }
}