
using System;
using System.Globalization;
using System.Linq.Expressions;

namespace MathAnimator.MathCore
{

    public static class MathParser
    {
        public static bool UseDegrees { get; set; } = false;

        public static class RandomWrapper
        {
            private static readonly Random rng = new Random();

            public static double Range(double min, double max)
            {
                return min + rng.NextDouble() * (max - min);
            }
        }


        private static Expression ParseRandom(ref string expr, ParameterExpression x, ParameterExpression a, ParameterExpression b, ParameterExpression c)
        {
            expr = expr[6..];

            if (!expr.StartsWith("("))
                throw new Exception("random() muss Klammern enthalten!");

            expr = expr[1..];

            if (expr.StartsWith(")"))
            {
                expr = expr[1..];
                return Expression.Call(
                    typeof(RandomWrapper).GetMethod("Range")!,
                    Expression.Constant(1.0),
                    Expression.Constant(1.0)
                );
            }

            Expression minExpr = ParseAddSubtract(ref expr, x, a, b, c);

            if (!expr.StartsWith(","))
            {
                while (expr.Length > 0 && expr[0] != ')')
                    expr = expr[1..];
                expr = expr[1..];

                return Expression.Call(
                    typeof(RandomWrapper).GetMethod("Range")!,
                    Expression.Constant(1.0),
                    Expression.Constant(1.0)
                );
            }

            expr = expr[1..];

            Expression maxExpr = ParseAddSubtract(ref expr, x, a, b, c);

            if (!expr.StartsWith(")"))
                throw new Exception("random(z1, z2) muss ')' enthalten!");
            expr = expr[1..];

            return Expression.Call(
                typeof(RandomWrapper).GetMethod("Range")!,
                minExpr,
                maxExpr
            );
        }

        public static Func<double, double, double, double, double> Parse(string formula)
        {
            var xParam = Expression.Parameter(typeof(double), "x");
            var aParam = Expression.Parameter(typeof(double), "a");
            var bParam = Expression.Parameter(typeof(double), "b");
            var cParam = Expression.Parameter(typeof(double), "c");

            Expression body = ParseExpression(formula.Replace(" ", ""), xParam, aParam, bParam, cParam);

            return Expression.Lambda<Func<double, double, double, double, double>>(body, xParam, aParam, bParam, cParam).Compile();
        }


        private static Expression ParseExpression(string expr, ParameterExpression x, ParameterExpression a, ParameterExpression b, ParameterExpression c)
        {
            return ParseAddSubtract(ref expr, x, a, b, c);
        }


        private static Expression ParseAddSubtract(ref string expr, ParameterExpression x, ParameterExpression a, ParameterExpression b, ParameterExpression c)
        {
            var left = ParseMultiplyDivide(ref expr, x, a, b, c);

            while (expr.Length > 0)
            {
                if (expr[0] == '+')
                {
                    expr = expr[1..];
                    left = Expression.Add(left, ParseMultiplyDivide(ref expr, x, a, b, c));
                }
                else if (expr[0] == '-')
                {
                    expr = expr[1..];
                    left = Expression.Subtract(left, ParseMultiplyDivide(ref expr, x, a, b, c));
                }
                else break;
            }

            return left;
        }


        private static Expression ParseMultiplyDivide(ref string expr, ParameterExpression x, ParameterExpression a, ParameterExpression b, ParameterExpression c)
        {
            var left = ParsePower(ref expr, x, a, b, c);

            while (expr.Length > 0)
            {
                if (expr[0] == '*')
                {
                    expr = expr[1..];
                    left = Expression.Multiply(left, ParsePower(ref expr, x, a, b, c));
                }
                else if (expr[0] == '/')
                {
                    expr = expr[1..];
                    left = Expression.Divide(left, ParsePower(ref expr, x, a, b, c));
                }
                else break;
            }

            return left;
        }


        private static Expression ParsePower(ref string expr, ParameterExpression x, ParameterExpression a, ParameterExpression b, ParameterExpression c)
        {
            var left = ParseAtom(ref expr, x, a, b, c);

            while (expr.Length > 0 && expr[0] == '^')
            {
                expr = expr[1..];
                left = Expression.Power(left, ParseAtom(ref expr, x, a, b, c));
            }
            return left;
        }

        private static Expression ParseAtom(ref string expr, ParameterExpression x, ParameterExpression a, ParameterExpression b, ParameterExpression c)
        {


            if (expr.StartsWith("pi"))
            {
                expr = expr[2..];
                return Expression.Constant(Math.PI);
            }

            if (expr.StartsWith("π"))
            {
                expr = expr[1..];
                return Expression.Constant(Math.PI);
            }


            if (expr.StartsWith("tau"))
            {
                expr = expr[3..];
                return Expression.Constant(Math.PI * 2.0);
            }

            if (expr.StartsWith("e"))
            {
                expr = expr[1..];
                return Expression.Constant(Math.E);
            }

            if (expr.StartsWith("-"))
            {
                expr = expr[1..];
                return Expression.Negate(ParseAtom(ref expr, x, a, b, c));
            }

            if (expr.StartsWith("sinh")) return ParseFunction(ref expr, x, a, b, c, "sinh");
            if (expr.StartsWith("cosh")) return ParseFunction(ref expr, x, a, b, c, "cosh");
            if (expr.StartsWith("tanh")) return ParseFunction(ref expr, x, a, b, c, "tanh");
            if (expr.StartsWith("asin")) return ParseFunction(ref expr, x, a, b, c, "asin");
            if (expr.StartsWith("acos")) return ParseFunction(ref expr, x, a, b, c, "acos");
            if (expr.StartsWith("atan")) return ParseFunction(ref expr, x, a, b, c, "atan");
            if (expr.StartsWith("atan2")) return ParseTwoArgs(ref expr, x, a, b, c, "atan2");
            if (expr.StartsWith("min")) return ParseTwoArgs(ref expr, x, a, b, c, "min");
            if (expr.StartsWith("max")) return ParseTwoArgs(ref expr, x, a, b, c, "max");
            if (expr.StartsWith("mod")) return ParseTwoArgs(ref expr, x, a, b, c, "mod");
            if (expr.StartsWith("floor")) return ParseFunction(ref expr, x, a, b, c, "floor");
            if (expr.StartsWith("ceil")) return ParseFunction(ref expr, x, a, b, c, "ceil");
            if (expr.StartsWith("round")) return ParseFunction(ref expr, x, a, b, c, "round");
            if (expr.StartsWith("abs")) return ParseFunction(ref expr, x, a, b, c, "abs");
            if (expr.StartsWith("exp")) return ParseFunction(ref expr, x, a, b, c, "exp");
            if (expr.StartsWith("random")) return ParseRandom(ref expr, x, a, b, c);
            if (expr.StartsWith("sqrt")) return ParseFunction(ref expr, x, a, b, c, "sqrt");
            if (expr.StartsWith("cbrt")) return ParseFunction(ref expr, x, a, b, c, "cbrt");
            if (expr.StartsWith("sin")) return ParseFunction(ref expr, x, a, b, c, "sin");
            if (expr.StartsWith("cos")) return ParseFunction(ref expr, x, a, b, c, "cos");
            if (expr.StartsWith("tan")) return ParseFunction(ref expr, x, a, b, c, "tan");
            if (expr.StartsWith("ln")) return ParseFunction(ref expr, x, a, b, c, "ln");
            if (expr.StartsWith("log10")) return ParseFunction(ref expr, x, a, b, c, "log10");

            if (expr.StartsWith("log"))
            {
                expr = expr[3..];
                expr = expr[1..];

                Expression baseA = ParseAddSubtract(ref expr, x, a, b, c);
                expr = expr[1..];
                Expression valueB = ParseAddSubtract(ref expr, x, a, b, c);
                expr = expr[1..];

                return Expression.Call(typeof(Math).GetMethod("Log", new[] { typeof(double), typeof(double) })!,
                    valueB, baseA);
            }

            if (expr.StartsWith("a"))
            {
                expr = expr[1..]; return a;
            }
            if (expr.StartsWith("b"))
            {
                expr = expr[1..]; return b;
            }
            if (expr.StartsWith("c"))
            {
                expr = expr[1..]; return c;
            }
            if (expr.StartsWith("x"))
            {
                expr = expr[1..]; return x;
            }

            if (expr.StartsWith("t"))
            {
                expr = expr[1..];
                return x;
            }

            if (char.IsDigit(expr[0]))
            {
                int i = 0;
                while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.')) i++;
                {
                    double value = double.Parse(expr[..i], CultureInfo.InvariantCulture);
                    expr = expr[i..];
                    return Expression.Constant(value);
                }
            }

            if (expr.StartsWith("("))
            {
                expr = expr[1..];
                Expression inside = ParseAddSubtract(ref expr, x, a, b, c);
                expr = expr[1..];
                return inside;
            }

            throw new Exception("Unbekannter Ausdruck: " + expr);
        }



        private static Expression ParseTwoArgs(ref string expr, ParameterExpression x, ParameterExpression a, ParameterExpression b, ParameterExpression c, string func)
        {
            expr = expr[func.Length..];
            expr = expr[1..];

            Expression p1 = ParseAddSubtract(ref expr, x, a, b, c);

            if (!expr.StartsWith(","))
                throw new Exception($"{func}() benötigt zwei Argumente!");

            expr = expr[1..];

            Expression p2 = ParseAddSubtract(ref expr, x, a, b, c);

            if (!expr.StartsWith(")"))
                throw new Exception($"{func}() muss mit ) enden!");

            expr = expr[1..];

            return func switch
            {
                "atan2" => Expression.Call(typeof(Math).GetMethod("Atan2")!, p1, p2),
                "min" => Expression.Call(typeof(Math).GetMethod("Min", new[] { typeof(double), typeof(double) })!, p1, p2),
                "max" => Expression.Call(typeof(Math).GetMethod("Max", new[] { typeof(double), typeof(double) })!, p1, p2),
                "mod" => Expression.Modulo(p1, p2),

                _ => throw new Exception("Unbekannte Funktion: " + func)
            };
        }

        private static Expression ParseFunction(ref string expr, ParameterExpression x, ParameterExpression a, ParameterExpression b, ParameterExpression c, string func)
        {
            expr = expr[func.Length..];
            expr = expr[1..];

            Expression inside = ParseAddSubtract(ref expr, x, a, b, c);
            expr = expr[1..];

            return func switch
            {

                "sin" => Expression.Call(typeof(Math).GetMethod("Sin")!, MathParser.UseDegrees ? Expression.Multiply(inside, Expression.Constant(Math.PI / 180)) : inside),
                "cos" => Expression.Call(typeof(Math).GetMethod("Cos")!, MathParser.UseDegrees ? Expression.Multiply(inside, Expression.Constant(Math.PI / 180)) : inside),
                "tan" => Expression.Call(typeof(Math).GetMethod("Tan")!, MathParser.UseDegrees ? Expression.Multiply(inside, Expression.Constant(Math.PI / 180)) : inside),
                "ln" => Expression.Call(typeof(Math).GetMethod("Log", new[] { typeof(double) })!, inside),
                "log10" => Expression.Call(typeof(Math).GetMethod("Log10")!, inside),
                "sqrt" => Expression.Call(typeof(Math).GetMethod("Sqrt")!, inside),
                "cbrt" => Expression.Call(typeof(Math).GetMethod("Cbrt")!, inside),
                "abs" => Expression.Call(typeof(Math).GetMethod("Abs", new[] { typeof(double) })!, inside),
                "exp" => Expression.Call(typeof(Math).GetMethod("Exp")!, inside),
                "round" => Expression.Call(typeof(Math).GetMethod("Round", new[] { typeof(double) })!, inside),
                "sinh" => Expression.Call(typeof(Math).GetMethod("Sinh")!, inside),
                "cosh" => Expression.Call(typeof(Math).GetMethod("Cosh")!, inside),
                "tanh" => Expression.Call(typeof(Math).GetMethod("Tanh")!, inside),
                "asin" => MathParser.UseDegrees ? Expression.Multiply(Expression.Call(typeof(Math).GetMethod("Asin")!, inside), Expression.Constant(180.0 / Math.PI)) : Expression.Call(typeof(Math).GetMethod("Asin")!, inside),
                "acos" => MathParser.UseDegrees ? Expression.Multiply(Expression.Call(typeof(Math).GetMethod("Acos")!, inside), Expression.Constant(180.0 / Math.PI)) : Expression.Call(typeof(Math).GetMethod("Acos")!, inside),
                "atan" => MathParser.UseDegrees ? Expression.Multiply(Expression.Call(typeof(Math).GetMethod("Atan")!, inside), Expression.Constant(180.0 / Math.PI)) : Expression.Call(typeof(Math).GetMethod("Atan")!, inside),
                "floor" => Expression.Call(typeof(Math).GetMethod("Floor", new[] { typeof(double) })!, inside),
                "ceil" => Expression.Call(typeof(Math).GetMethod("Ceiling", new[] { typeof(double) })!, inside),
                _ => throw new Exception("Unbekannte Funktion: " + func)
            };
        }
    }
}
