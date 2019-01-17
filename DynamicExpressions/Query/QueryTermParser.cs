using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicExpressions.Query.Expressions;

namespace DynamicExpressions.Query
{
    public static class QueryTermParser
    {
        public static ParsedQueryTerm Parse(string query)
        {
            var lowerQuery = (query ?? throw new ArgumentException($"{nameof(query)} is required")).ToLower();

            (int endPosition, QueryExpression query) parseExpression(int position)
            {
                List<(QueryTermParserToken token, QueryExpression exp)> compoundExpressions = new List<(QueryTermParserToken token, QueryExpression exp)>();
                List<(QueryTermParserToken token, object value)> tokens = new List<(QueryTermParserToken token, object value)>();
                StringBuilder currentTerm = new StringBuilder();
                var lastCompoundPositionIndex = position;

                void addCurrentTermIfNeeded()
                {
                    var term = currentTerm.ToString().Trim();
                    if (term.Length > 0)
                    {
                        tokens.Add((QueryTermParserToken.Literal, term));
                        currentTerm = new StringBuilder();
                    }
                }

                bool expressionTerminated = false;
                bool inQuote = false;
                var index = position;
                for (; index < lowerQuery.Length && !expressionTerminated; index++)
                {
                    bool peek(string find)
                    {
                        for (var i = 0; i < find.Length; i++)
                        {
                            if (index + i >= lowerQuery.Length || lowerQuery[index + i] != find[i])
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                    bool matchedTokenAndAdvancedIndex(QueryTermParserToken token, string symbol = null)
                    {
                        var term = symbol ?? token.ToString().ToLower();
                        if (peek(term))
                        {
                            addCurrentTermIfNeeded();
                            tokens.Add((token, null));
                            index += term.Length - 1;

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    var cur = lowerQuery[index];
                    switch (cur)
                    {
                        case '"':
                            inQuote = !inQuote;
                            break;
                        case '(':
                            if (index == 0 || index != position)
                            {
                                var nestedExp = parseExpression(index + 1);
                                tokens.Add((QueryTermParserToken.Expression, nestedExp.query));
                                index = nestedExp.endPosition - 1;
                            }
                            break;
                        case ')':
                            expressionTerminated = true;
                            break;
                        default:
                            var knownToken = false;
                            if (!inQuote)
                            {
                                if (tokens.Count == 0)
                                {
                                    knownToken = matchedTokenAndAdvancedIndex(QueryTermParserToken.Between)
                                        || matchedTokenAndAdvancedIndex(QueryTermParserToken.Equals, "=")
                                        || matchedTokenAndAdvancedIndex(QueryTermParserToken.LessThanEquals, "<=")
                                        || matchedTokenAndAdvancedIndex(QueryTermParserToken.GreaterThanEquals, ">=")
                                        || matchedTokenAndAdvancedIndex(QueryTermParserToken.LessThan, "<")
                                        || matchedTokenAndAdvancedIndex(QueryTermParserToken.GreaterThan, ">");
                                }
                                else if (tokens.Count == 2 && tokens[1].token == QueryTermParserToken.Between)
                                {
                                    knownToken = matchedTokenAndAdvancedIndex(QueryTermParserToken.And);
                                }
                                else if (peek("and") || peek("or"))
                                {
                                    knownToken = matchedTokenAndAdvancedIndex(peek("and") ? QueryTermParserToken.And : QueryTermParserToken.Or);
                                    var compoundToken = tokens.Last().token;
                                    var compoundTokenLength = compoundToken.ToString().Length;

                                    var exp = TokensToExpression(tokens.Take(tokens.Count - 1).ToList(), query.Substring(lastCompoundPositionIndex, index - compoundTokenLength - lastCompoundPositionIndex));
                                    compoundExpressions.Add((compoundToken, exp));

                                    tokens = new List<(QueryTermParserToken token, object value)>();
                                    lastCompoundPositionIndex = index + compoundTokenLength - 1;
                                }
                            }

                            if (!knownToken)
                            {
                                currentTerm.Append(query[index]);
                            }
                            break;
                    }
                }

                addCurrentTermIfNeeded();
                if (inQuote)
                {
                    throw new ParseException("Expression contained an opening quote symbol without a closing quote symbol");
                }

                if (compoundExpressions.Any())
                {
                    var finalExp = TokensToExpression(tokens, query.Substring(lastCompoundPositionIndex, index - lastCompoundPositionIndex - (expressionTerminated ? 1 : 0)));

                    tokens = new List<(QueryTermParserToken token, object value)>();
                    foreach (var (token, exp) in compoundExpressions)
                    {
                        tokens.Add((QueryTermParserToken.Expression, exp));
                        tokens.Add((token, null));
                    }
                    tokens.Add((QueryTermParserToken.Expression, finalExp));
                }

                return (index, TokensToExpression(tokens, query.Substring(position, index - position - (expressionTerminated ? 1 : 0))));
            }

            return new ParsedQueryTerm
            {
                Term = query,
                Expression = parseExpression(0).query
            };
        }

        private static QueryExpression TokensToExpression(List<(QueryTermParserToken token, object value)> tokens, string term)
        {
            if (tokens.Count == 1 && tokens[0].token == QueryTermParserToken.Literal)
            {
                return new TermExpression { Value = term };
            }
            else if (tokens.Count == 1 && tokens[0].token == QueryTermParserToken.Expression)
            {
                return tokens[0].value as QueryExpression;
            }
            else if (tokens[1].token == QueryTermParserToken.And || tokens[1].token == QueryTermParserToken.Or)
            {
                var operations = tokens.Where((t, i) => i % 2 == 1).Select(t => t.token).ToList();

                if (operations.Count > 1 && operations.Skip(1).All(o => o != operations[0]))
                {
                    throw new ParseException($"The term \"{term}\" contains a mixture of \"And\" and \"Or\": try adding parentheses to clarify the expression");
                }
                else
                {
                    return new CompoundExpression
                    {
                        Operation = (CompoundOperation)Enum.Parse(typeof(CompoundOperation), tokens[1].token.ToString()),
                        Expressions = tokens.Where((t, i) => i % 2 == 0).Select(t => t.value as QueryExpression).ToList()
                    };
                }
            }
            else if (tokens[1].token == QueryTermParserToken.Between)
            {
                if (tokens.Count != 5 || tokens[0].token != QueryTermParserToken.Literal || tokens[2].token != QueryTermParserToken.Literal || tokens[4].token != QueryTermParserToken.Literal
                    || tokens[3].token != QueryTermParserToken.And)
                {
                    throw new ParseException($"The term \"{term}\" does not appear to be a valid \"between\" expression");
                }
                else
                {
                    return new BetweenExpression
                    {
                        Field = (string)tokens[0].value,
                        ValueFrom = (string)tokens[2].value,
                        ValueTo = (string)tokens[4].value
                    };
                }
            }
            else if (tokens.Count != 3)
            {
                throw new ParseException($"The term \"{term}\" could not be parsed");
            }
            else
            {
                return new ComparisonExpression
                {
                    Field = (string)tokens[0].value,
                    Operation = (ComparisonOperation)Enum.Parse(typeof(ComparisonOperation), tokens[1].token.ToString()),
                    Value = (string)tokens[2].value
                };
            }
        }
    }
}
