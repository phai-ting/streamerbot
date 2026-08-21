using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

public class CPHInline
{
    public bool Execute()
    {
        string calculatorResponse = "";
        CPH.TryGetArg("user", out string user);
        CPH.TryGetArg("rawInput", out string rawInput);
        string expr = NormalizeInput(rawInput);
        if (string.IsNullOrWhiteSpace(expr))
        {
            CPH.SetArgument("calculatorResponse",$"Try including a math expression like \"= 365 / 7\". I can also handle paranthesis, many functions, and unit conversions like \"= 65f\".");
            return true;
        }
        if (UnitConverter.TryHandle(expr, out string conversionMessage))
        {
           CPH.SetArgument("calculatorResponse",$"{conversionMessage}");
            return true;
        }
        try
        {
            double value = MathEvaluator.Evaluate(expr);
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                CPH.SetArgument("calculatorResponse",$"That expression didn't come out to a real number.");
                return true;
            }
            calculatorResponse =$"{expr} = {FormatNumber(value)}";
        }
        catch (MathEvalException ex)
        {
            calculatorResponse = ex.Message;
        }
        catch (Exception)
        {
             calculatorResponse = "I couldn't evaluate that expression.";
        }
        CPH.SetArgument("calculatorResponse", calculatorResponse);
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
            throw new MathEvalException("I need an expression to work with.");
        if (tokens.Count > MaxTokens)
            throw new MathEvalException("That's a bit too long for me to handle.");
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
                            if (sawDot) throw new MathEvalException("That number doesn't look quite right.");
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
                throw new MathEvalException($"I'm not sure what to do with '{c}'.");
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
                throw new MathEvalException("There was some extra stuff at the end that I didn't know what to do with.");
        }
        bool AtEnd => _pos >= _tokens.Count;
        Token Peek => AtEnd ? default : _tokens[_pos];
        bool Check(TokenKind k) => !AtEnd && Peek.Kind == k;
        Token Advance()
        {
            if (AtEnd) throw new MathEvalException("It looks like that expression got cut off.");
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
                    if (right == 0) throw new MathEvalException("I can't divide by zero.");
                    left /= right;
                }
                else
                {
                    if (right == 0) throw new MathEvalException("I can't do a remainder with zero.");
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
                    throw new MathEvalException($"I'm not sure how to read the number '{text}'.");
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
                    throw new MathEvalException("Looks like a closing ')' is missing.");
                Advance();
                Leave();
                return v;
            }
            throw new MathEvalException("I was looking for a number, something like pi, a function, or a '('.");
        }
        double Constant(string name)
        {
            switch (name)
            {
                case "pi": return Math.PI;
                case "e": return Math.E;
                case "tau": return Math.PI * 2;
                default:
                    throw new MathEvalException($"I'm not sure what '{name}' is.");
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
                throw new MathEvalException("Looks like a closing ')' is missing after the function.");
            Advance();
            Leave();
            return ApplyFunction(name, args);
        }
        void Enter()
        {
            _depth++;
            if (_depth > MaxDepth)
                throw new MathEvalException("That's nested a little too deep for me.");
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
                    if (a[0] < 0) throw new MathEvalException("I can't take the square root of a negative number.");
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
                        if (a[0] <= 0) throw new MathEvalException("I can only take the log of a positive number.");
                        return Math.Log(a[0]);
                    }
                    Require(n, 2, name);
                    if (a[0] <= 0 || a[1] <= 0 || a[1] == 1)
                        throw new MathEvalException("Those log values don't work. Both need to be positive, and the base can't be 1.");
                    return Math.Log(a[0], a[1]);
                case "log10":
                    Require(n, 1, name);
                    if (a[0] <= 0) throw new MathEvalException("I can only take the log of a positive number.");
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
                    throw new MathEvalException($"I don't know a function called '{name}'.");
            }
        }
        static void Require(int got, int need, string name)
        {
            if (got != need)
            {
                string needed = need == 1 ? "1 value" : $"{need} values";
                throw new MathEvalException($"Unfortunately, {name}() needs {needed}.");
            }
        }
    }
}

static class UnitConverter
{
    const string Length = "length";
    const string Mass = "mass";
    const string Temperature = "temperature";
    const string Volume = "volume";
    const string Time = "time";
    static readonly Dictionary<string, string> AliasToId = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    static readonly Dictionary<string, UnitDef> Units = new Dictionary<string, UnitDef>(StringComparer.OrdinalIgnoreCase);
    static UnitConverter()
    {
        Add("mm", Length, "mm", "in", 0.001, "millimeter", "millimeters", "millimetre", "millimetres");
        Add("cm", Length, "cm", "in", 0.01, "centimeter", "centimeters", "centimetre", "centimetres");
        Add("m", Length, "m", "ft", 1, "meter", "meters", "metre", "metres");
        Add("km", Length, "km", "mi", 1000, "kilometer", "kilometers", "kilometre", "kilometres");
        Add("in", Length, "in", "cm", 0.0254, "inch", "inches");
        Add("ft", Length, "ft", "m", 0.3048, "foot", "feet");
        Add("yd", Length, "yd", "m", 0.9144, "yard", "yards");
        Add("mi", Length, "mi", "km", 1609.344, "mile", "miles");
        Add("mg", Mass, "mg", "oz", 0.000001, "milligram", "milligrams", "milligramme", "milligrammes");
        Add("g", Mass, "g", "oz", 0.001, "gram", "grams", "gramme", "grammes");
        Add("kg", Mass, "kg", "lb", 1, "kilogram", "kilograms", "kilogramme", "kilogrammes", "kilo", "kilos");
        Add("oz", Mass, "oz", "g", 0.028349523125, "ounce", "ounces");
        Add("lb", Mass, "lb", "kg", 0.45359237, "lbs", "pound", "pounds");
        Add("st", Mass, "st", "kg", 6.35029318, "stone", "stones");
        Add("c", Temperature, "C", "f", 0, "celsius", "centigrade");
        Add("f", Temperature, "F", "c", 0, "fahrenheit");
        Add("k", Temperature, "K", "c", 0, "kelvin");
        Add("ml", Volume, "ml", "floz", 0.001, "milliliter", "milliliters", "millilitre", "millilitres");
        Add("l", Volume, "L", "gal", 1, "liter", "liters", "litre", "litres");
        Add("tsp", Volume, "tsp", "ml", 0.00492892159375, "teaspoon", "teaspoons");
        Add("tbsp", Volume, "tbsp", "ml", 0.01478676478125, "tablespoon", "tablespoons");
        Add("cup", Volume, "cup", "ml", 0.2365882365, "cups");
        Add("pt", Volume, "pint", "ml", 0.473176473, "pint", "pints");
        Add("qt", Volume, "quart", "ml", 0.946352946, "quart", "quarts");
        Add("floz", Volume, "fl oz", "ml", 0.0295735295625, "fluidounce", "fluidounces");
        Add("gal", Volume, "gal", "l", 3.785411784, "gallon", "gallons");
        Add("sec", Time, "sec", "min", 1, "secs", "second", "seconds", "s");
        Add("min", Time, "min", "hr", 60, "mins", "minute", "minutes");
        Add("hr", Time, "hr", "min", 3600, "hrs", "hour", "hours", "h");
        Add("day", Time, "day", "hr", 86400, "days", "d");
        Add("wk", Time, "wk", "day", 604800, "week", "weeks", "wks");
        Add("yr", Time, "yr", "day", 31536000, "year", "years", "yrs", "y");
    }
    public static bool TryHandle(string input, out string message)
    {
        message = null;
        if (string.IsNullOrWhiteSpace(input))
            return false;
        int i = 0;
        SkipWs(input, ref i);
        if (!TryParseNumber(input, ref i, out double value))
            return false;
        SkipWs(input, ref i);
        if (!TryParseUnitToken(input, ref i, out string fromRaw))
            return false;
        TryCombineFluidOunce(input, ref i, ref fromRaw);
        SkipWs(input, ref i);
        string toRaw = null;
        bool explicitTo = false;
        if (i < input.Length)
        {
            if (!TryMatchTo(input, ref i))
                return false;
            explicitTo = true;
            SkipWs(input, ref i);
            if (!TryParseUnitToken(input, ref i, out toRaw))
            {
                message = "I need a unit to convert to. Try something like \"= 5 ft to m\" or just \"= 65f\".";
                return true;
            }
            TryCombineFluidOunce(input, ref i, ref toRaw);
            SkipWs(input, ref i);
            if (i < input.Length)
            {
                message = "There was some extra stuff after the conversion that I didn't know what to do with.";
                return true;
            }
        }
        if (!TryResolve(fromRaw, out UnitDef fromUnit))
        {
            if (!explicitTo)
                return false;
            message = $"I'm not sure what unit '{fromRaw}' is.";
            return true;
        }
        UnitDef toUnit;
        if (!explicitTo)
            toUnit = Units[fromUnit.DefaultTarget];
        else if (!TryResolve(toRaw, out toUnit))
        {
            message = $"I'm not sure what unit '{toRaw}' is.";
            return true;
        }
        if (fromUnit.Dimension != toUnit.Dimension)
        {
            message = $"I don't know how to convert {fromUnit.Display} to {toUnit.Display}.";
            return true;
        }
        double result = ConvertValue(fromUnit, toUnit, value);
        if (double.IsNaN(result) || double.IsInfinity(result))
        {
            message = "That conversion didn't come out to a real number.";
            return true;
        }
        message = $"{FormatNumber(value)} {fromUnit.Display} = {FormatNumber(result)} {toUnit.Display}";
        return true;
    }
    static double ConvertValue(UnitDef from, UnitDef to, double value)
    {
        if (from.Dimension == Temperature)
            return FromKelvin(to.Id, ToKelvin(from.Id, value));
        return value * from.ToCanonical / to.ToCanonical;
    }
    static double ToKelvin(string id, double value)
    {
        switch (id)
        {
            case "c": return value + 273.15;
            case "f": return (value - 32) * 5.0 / 9.0 + 273.15;
            default: return value;
        }
    }
    static double FromKelvin(string id, double kelvin)
    {
        switch (id)
        {
            case "c": return kelvin - 273.15;
            case "f": return (kelvin - 273.15) * 9.0 / 5.0 + 32;
            default: return kelvin;
        }
    }
    static bool TryResolve(string raw, out UnitDef unit)
    {
        unit = null;
        if (string.IsNullOrWhiteSpace(raw))
            return false;
        string key = raw.Trim().TrimStart('°');
        if (!AliasToId.TryGetValue(key, out string id))
            return false;
        return Units.TryGetValue(id, out unit);
    }
    static void Add(string id, string dimension, string display, string defaultTarget, double toCanonical, params string[] aliases)
    {
        Units[id] = new UnitDef(id, dimension, display, defaultTarget, toCanonical);
        AliasToId[id] = id;
        foreach (string alias in aliases)
            AliasToId[alias] = id;
    }
    static void SkipWs(string s, ref int i)
    {
        while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
    }
    static bool TryParseNumber(string s, ref int i, out double value)
    {
        value = 0;
        int start = i;
        if (i < s.Length && (s[i] == '+' || s[i] == '-')) i++;
        bool sawDot = false;
        bool sawDigit = false;
        while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.'))
        {
            if (s[i] == '.')
            {
                if (sawDot) { i = start; return false; }
                sawDot = true;
            }
            else
                sawDigit = true;
            i++;
        }
        if (!sawDigit)
        {
            i = start;
            return false;
        }
        if (i < s.Length && (s[i] == 'e' || s[i] == 'E'))
        {
            int ePos = i;
            i++;
            if (i < s.Length && (s[i] == '+' || s[i] == '-')) i++;
            int expStart = i;
            while (i < s.Length && char.IsDigit(s[i])) i++;
            if (i == expStart)
                i = ePos;
        }
        string num = s.Substring(start, i - start);
        return double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
    static bool TryParseUnitToken(string s, ref int i, out string raw)
    {
        raw = null;
        int start = i;
        if (i < s.Length && s[i] == '°') i++;
        if (i >= s.Length || !char.IsLetter(s[i]))
        {
            i = start;
            return false;
        }
        while (i < s.Length && char.IsLetter(s[i])) i++;
        raw = s.Substring(start, i - start);
        return true;
    }
    static void TryCombineFluidOunce(string s, ref int i, ref string unitRaw)
    {
        if (!string.Equals(unitRaw, "fl", StringComparison.OrdinalIgnoreCase))
            return;
        int save = i;
        SkipWs(s, ref i);
        if (TryParseUnitToken(s, ref i, out string second) && string.Equals(second, "oz", StringComparison.OrdinalIgnoreCase))
            unitRaw = "floz";
        else
            i = save;
    }
    static bool TryMatchTo(string s, ref int i)
    {
        if (i + 2 > s.Length)
            return false;
        if ((s[i] != 't' && s[i] != 'T') || (s[i + 1] != 'o' && s[i + 1] != 'O'))
            return false;
        if (i + 2 < s.Length && char.IsLetter(s[i + 2]))
            return false;
        i += 2;
        return true;
    }
    static string FormatNumber(double value)
    {
        decimal d = Math.Round((decimal)value, 3, MidpointRounding.AwayFromZero);
        return d.ToString("0.###", CultureInfo.InvariantCulture);
    }
    sealed class UnitDef
    {
        public readonly string Id;
        public readonly string Dimension;
        public readonly string Display;
        public readonly string DefaultTarget;
        public readonly double ToCanonical;
        public UnitDef(string id, string dimension, string display, string defaultTarget, double toCanonical)
        {
            Id = id;
            Dimension = dimension;
            Display = display;
            DefaultTarget = defaultTarget;
            ToCanonical = toCanonical;
        }
    }
}