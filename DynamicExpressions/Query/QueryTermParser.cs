using System;
using System.Collections.Generic;
using System.Text;
using DynamicExpressions.Query.Expressions;

namespace DynamicExpressions.Query
{
    public static class QueryTermParser
    {
        public static ParsedQueryTerm Parse(string query)
        {
            var lowerQuery = (query ?? throw new ArgumentException($"{nameof(query)} is required")).ToLower();

            bool peek(int index, string find)
            {
                for (var i = index; i < find.Length; i++)
                {
                    if (i > lowerQuery.Length || lowerQuery[i] != find[i = index])
                    {
                        return false;
                    }
                }

                return true;
            }

            QueryExpression parseExpression(int index)
            {
                List<(QueryTermParserToken token, object value)> tokens = new List<(QueryTermParserToken token, object value)>();

                for (var i = index; i < lowerQuery.Length; i++)
                {
                    var cur = lowerQuery[i];
                    switch (cur)
                    {
                        case '"':
                            var nextQuoteIndex = lowerQuery.IndexOf('"', i + 1);
                            // read til next quote
                            //ParseException
                            break;
                        case '(':
                            tokens.Add((QueryTermParserToken.Expression, parseExpression(i + 1)));
                            break;
                        //case ')':
                        //    break;
                        case 'a':
                            if (peek(i, "and"))
                            {
                                tokens.Add((QueryTermParserToken.And, null));
                                break;
                            }
                            else
                            {
                                goto default;
                            }
                            break;
                        case 'b':
                            if (peek(i, "between"))
                            {
                                tokens.Add((QueryTermParserToken.Between, null));
                                break;
                            }
                            else
                            {
                                goto default;
                            }

                        default:

                            break;
                    }
                }

                return null;
            }

            return new ParsedQueryTerm
            {
                Term = query,
                Expression = parseExpression(0)
            };
        }
    }
}
