using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Solution
{
    class Program
    {
        internal class Token
        {

        }

        class Number : Token
        {
            public double Symbol { get; }

            public Number(double num)
            {
                Symbol = num;
            }
        }

        class Operation : Token
        {
            public char Symbol { get; }
            public int Priority { get; }

            public Operation(char symbol)
            {
                Symbol = symbol;
                Priority = GetPriority(symbol);
            }

            private static int GetPriority(char symbol)
            {
                switch (symbol)
                {
                    case '(': return 0;
                    case ')': return 0;
                    case '+': return 1;
                    case '-': return 1;
                    case '*': return 2;
                    case '/': return 2;
                    default: return 3;
                }
            }
        }

        class Paranthesis : Token
        {
            public char Symbol { get; }
            public bool isClosing { get; }

            public Paranthesis(char symbol)
            {
                Symbol = symbol;
                isClosing = symbol == ')';
            }
        }

        class RPN
        {
            static void Main(string[] args)
            {
                Console.Write("Введите математическую операцию ");
                string input = Console.ReadLine();
                input.Replace(" ", string.Empty);
                var tokens = Tokenize(input);
                var rpn = toRPN(tokens);
                Console.WriteLine($"Результат: {Calculate(rpn)}");

            }
        }

        static List<Token> Tokenize(string input)
        {
            List<Token> tokens = new List<Token>();
            string number = string.Empty;
            foreach (var c in input)
            {
                if (char.IsDigit(c))
                {
                    number += c;
                }
                else if (c == ',' || c == '.')
                {
                    number += ",";
                }

                else if (c == '+' || c == '-' || c == '*' || c == '/')
                {
                    if (number != string.Empty)
                    {
                        tokens.Add(new Number(double.Parse(number)));
                        number = string.Empty;
                    }
                    tokens.Add(new Operation(c));
                }
                else if (c == '(' || c == ')')
                {
                    if (number != string.Empty)
                    {
                        tokens.Add(new Number(double.Parse(number)));
                        number = string.Empty;
                    }
                    tokens.Add(new Paranthesis(c));
                }
            }

            if (number != string.Empty)
            {
                tokens.Add(new Number(double.Parse(number)));
            }

            return tokens;
        }

        static List<Token> toRPN(List<Token> tokens)
        {
            List<Token> rpnOutput = new List<Token>();
            Stack<Token> operators = new Stack<Token>();
            string number = string.Empty;

            foreach (Token token in tokens)
            {
                if (operators.Count == 0 && !(token is Number))
                {
                    operators.Push(token);
                    continue;
                }

                if (token is Operation)
                {
                    if (operators.Peek() is Paranthesis)
                    {
                        operators.Push(token);
                        continue;
                    }

                    Operation first = (Operation)token;
                    Operation second = (Operation)operators.Peek();

                    if (first.Priority > second.Priority)
                    {
                        operators.Push(token);
                    }
                    else if (first.Priority <= second.Priority)
                    {
                        while (operators.Count > 0 && !(token is Paranthesis))
                        {
                            rpnOutput.Add(operators.Pop());
                        }
                        operators.Push(token);
                    }
                }
                else if (token is Paranthesis)
                {
                    if (((Paranthesis)token).isClosing)
                    {
                        while (!(operators.Peek() is Paranthesis))
                        {
                            rpnOutput.Add(operators.Pop());
                        }

                        operators.Pop();
                    }
                    else
                    {
                        operators.Push(token);
                    }
                }
                else if (token is Number)
                {
                    rpnOutput.Add(token);
                }
            }

            while (operators.Count > 0)
            {
                rpnOutput.Add(operators.Pop());
            }
            return rpnOutput;
        }
        static double TheOperation(Operation oper, double number1, double number2)
        {
            if (oper.Symbol == '+') return number1 + number2;
            else if (oper.Symbol == '-') return number2 - number1;
            else if (oper.Symbol == '*') return number1 * number2;
            else return number2 / number1;
        }
        public static double Calculate(List<Token> rpnCalc)
        {
            Stack<double> tempCalc = new Stack<double>();
            double result = 0;

            for (int i = 0; i < rpnCalc.Count; i++)
            {
                if (rpnCalc[i] is Number num)
                {
                    tempCalc.Push(num.Symbol);
                }
                else
                {
                    double number1 = tempCalc.Pop();
                    double number2 = tempCalc.Pop();

                    var oper = (Operation)rpnCalc[i];
                    result = TheOperation(oper,number1, number2);
                    tempCalc.Push(result);
                }
            }
            return tempCalc.Peek();
        }
    }
}