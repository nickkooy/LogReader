﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReader.Expressions
{
    public class Token
    {
        public static readonly Dictionary<char, KeyValuePair<TokenType, string>> dict = new Dictionary<char, KeyValuePair<TokenType, string>>()
        {
            {
                '(', new KeyValuePair<TokenType, string>(TokenType.OPEN_PAREN, "(")
            },
            {
                ')', new KeyValuePair<TokenType, string>(TokenType.CLOSE_PAREN, ")")
            },
            {
                '!', new KeyValuePair<TokenType, string>(TokenType.UNARY_OP, "NOT")
            },
            {
                '&', new KeyValuePair<TokenType, string>(TokenType.BINARY_OP, "AND")
            },
            {
                '|', new KeyValuePair<TokenType, string>(TokenType.BINARY_OP, "OR")
            }
        };

        public enum TokenType
        {
            OPEN_PAREN,
            CLOSE_PAREN,
            UNARY_OP,
            BINARY_OP,
            LITERAL,
            EXPR_END
        }

        public TokenType type;
        public string value;

        public Token(StringReader s)
        {
            int c = s.Read();
            if (c == -1)
            {
                type = TokenType.EXPR_END;
                value = "";
                return;
            }

            char ch = (char)c;

            if (dict.ContainsKey(ch))
            {
                type = dict[ch].Key;
                value = dict[ch].Value;
            }
            else
            {
                string str = "";
                str += ch;
                while (s.Peek() != -1 && !dict.ContainsKey((char)s.Peek()))
                {
                    str += (char)s.Read();
                }
                type = TokenType.LITERAL;
                value = str;
            }
        }

        
    }
}
