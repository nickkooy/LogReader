using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogReader.Expressions
{
    public class ExprReader
    {
        static readonly char[] litTokens;
        
        Dictionary<string, bool> booleanValues = new Dictionary<string, bool>();
        Dictionary<string, string> literals = new Dictionary<string, string>();

        string expr = "";

        public string Expression
        {
            get { return expr; }
        }

        static ExprReader()
        {
            // Create our lit token names
            List<char> chars = new List<char>(128);
            // Get A-Z
            for (int i = 65; i < 91; ++i)
            {
                char c = (char)i;
                if (!Token.dict.ContainsKey(c) && Regex.IsMatch(c.ToString(), "[A-Z]"))
                {
                    chars.Add(c);
                }
            }
            // Get 0-9
            for (int i = 48; i < 58; ++i)
            {
                char c = (char)i;
                if (!Token.dict.ContainsKey(c) && Regex.IsMatch(c.ToString(), "[0-9]"))
                {
                    chars.Add(c);
                }
            }
            litTokens = chars.ToArray();
        }

        public void ParseExpr(string expr)
        {
            if (expr == null)
                expr = "";
            //expr = "shovel_blood & (familiar_shopkeer & (weapon_golden & (!harp) & (!crossbow)))";
            // Remove the white space
            expr = expr.Replace(" ", "");

            // Get the strings
            literals.Clear();
            booleanValues.Clear();
            string newExpr = string.Copy(expr);
            int idxAdj = 0;
            foreach (Match match in Regex.Matches(expr, "\\w+"))
            {
                string lit = GetNextLiteral(literals.Count);
                literals.Add(lit, match.Value);
                booleanValues.Add(lit, false);
                newExpr = newExpr.Remove(match.Index + idxAdj, match.Length);
                newExpr = newExpr.Insert(match.Index + idxAdj, lit);
                idxAdj += lit.Length - match.Length;
            }

            this.expr = newExpr;
        }

        string GetNextLiteral(int index)
        {
            return (index > litTokens.Length) ? Utils.Math.IntToStringFast(index, litTokens) : Utils.Math.IntToString(index, litTokens);
        }
        
        public bool EvaluateItems(List<string> items)
        {
            //We'll use ! for not, & for and, | for or and remove whitespace
            //expr = @"!((A&B)|C|D)";
            List<Token> tokens = new List<Token>();
            StringReader reader = new StringReader(expr);

            //Tokenize the expression
            Token t = null;
            do
            {
                t = new Token(reader);
                tokens.Add(t);
            } while (t.type != Token.TokenType.EXPR_END);

            //Use a minimal version of the Shunting Yard algorithm to transform the token list to polish notation
            List<Token> polishNotation = TransformToPolishNotation(tokens);

            var enumerator = polishNotation.GetEnumerator();
            enumerator.MoveNext();
            BoolExpr root = Make(ref enumerator);

            //Request boolean values for all literal operands
            //---- match items to literals and set their bools
            foreach (var kvp in literals)
            {
                var itemMatches = items.Where(item => Regex.IsMatch(item, kvp.Value)).ToArray();
                booleanValues[kvp.Key] = itemMatches.Length > 0;
            }

            //foreach (Token tok in polishNotation.Where(token => token.type == Token.TokenType.LITERAL))
            //{
            //    Console.Write("Enter boolean value for {0}: ", tok.value);
            //    string line = Console.ReadLine();
            //    booleanValues[tok.value] = Boolean.Parse(line);
            //    Console.WriteLine();
            //}

            //Eval the expression tree
            return Eval(root);
            //Console.WriteLine("Eval: {0}", Eval(root));

            //Console.ReadLine();
        }
        static List<Token> TransformToPolishNotation(List<Token> infixTokenList)
        {
            Queue<Token> outputQueue = new Queue<Token>();
            Stack<Token> stack = new Stack<Token>();

            int index = 0;
            while (infixTokenList.Count > index)
            {
                Token t = infixTokenList[index];

                switch (t.type)
                {
                    case Token.TokenType.LITERAL:
                        outputQueue.Enqueue(t);
                        break;
                    case Token.TokenType.BINARY_OP:
                    case Token.TokenType.UNARY_OP:
                    case Token.TokenType.OPEN_PAREN:
                        stack.Push(t);
                        break;
                    case Token.TokenType.CLOSE_PAREN:
                        while (stack.Peek().type != Token.TokenType.OPEN_PAREN)
                        {
                            outputQueue.Enqueue(stack.Pop());
                        }
                        stack.Pop();
                        if (stack.Count > 0 && stack.Peek().type == Token.TokenType.UNARY_OP)
                        {
                            outputQueue.Enqueue(stack.Pop());
                        }
                        break;
                    default:
                        break;
                }

                ++index;
            }
            while (stack.Count > 0)
            {
                outputQueue.Enqueue(stack.Pop());
            }

            return outputQueue.Reverse().ToList();
        }

        static BoolExpr Make(ref List<Token>.Enumerator polishNotationTokensEnumerator)
        {
            if (polishNotationTokensEnumerator.Current == null)
            {
                return BoolExpr.CreateBoolVar(null);
            }
            if (polishNotationTokensEnumerator.Current.type == Token.TokenType.LITERAL)
            {
                BoolExpr lit = BoolExpr.CreateBoolVar(polishNotationTokensEnumerator.Current.value);
                polishNotationTokensEnumerator.MoveNext();
                return lit;
            }
            else
            {
                if (polishNotationTokensEnumerator.Current.value == "NOT")
                {
                    polishNotationTokensEnumerator.MoveNext();
                    BoolExpr operand = Make(ref polishNotationTokensEnumerator);
                    return BoolExpr.CreateNot(operand);
                }
                else if (polishNotationTokensEnumerator.Current.value == "AND")
                {
                    polishNotationTokensEnumerator.MoveNext();
                    BoolExpr left = Make(ref polishNotationTokensEnumerator);
                    BoolExpr right = Make(ref polishNotationTokensEnumerator);
                    return BoolExpr.CreateAnd(left, right);
                }
                else if (polishNotationTokensEnumerator.Current.value == "OR")
                {
                    polishNotationTokensEnumerator.MoveNext();
                    BoolExpr left = Make(ref polishNotationTokensEnumerator);
                    BoolExpr right = Make(ref polishNotationTokensEnumerator);
                    return BoolExpr.CreateOr(left, right);
                }
            }
            return null;
        }
        bool Eval(BoolExpr expr)
        {
            if (expr == null)
                return false;

            if (expr.IsLeaf() && expr.Lit != null)
            {
                return booleanValues[expr.Lit];
            }

            if (expr.Op == BoolExpr.BOP.NOT)
            {
                return !Eval(expr.Left);
            }

            if (expr.Op == BoolExpr.BOP.OR)
            {
                return Eval(expr.Left) || Eval(expr.Right);
            }

            if (expr.Op == BoolExpr.BOP.AND)
            {
                return Eval(expr.Left) && Eval(expr.Right);
            }

            throw new ArgumentException();
        }
    }
}
