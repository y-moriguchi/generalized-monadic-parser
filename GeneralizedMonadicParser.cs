/**
 * Generalized Monadic Parser
 *
 * Copyright (c) 2021 Yuichiro MORIGUCHI
 *
 * This software is released under the MIT License.
 * http://opensource.org/licenses/mit-license.php
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Morilib
{
    public static class GeneralizedMonadicParser
    {
        private static readonly string PatternReal = @"[\+\-]?(?:[0-9]+(?:\.[0-9]+)?|\.[0-9]+)(?:[eE][\+\-]?[0-9]+)?";

        /// <summary>
        /// Parser delegate.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="env">initial environment</param>
        /// <param name="position">position</param>
        /// <returns>result of parsing</returns>
        public delegate IEnumerable<Result<T>> Parser<T>(Env env, int position);

        /// <summary>
        /// Initial environment.
        /// </summary>
        public class Env
        {
            /// <summary>
            /// A string to parse.
            /// </summary>
            public string ParseString { get; private set; }

            /// <summary>
            /// A pattern of skip string.
            /// </summary>
            public Parser<string> Skip { get; private set; }

            /// <summary>
            /// A pattern which keyword follow.
            /// </summary>
            public Parser<string> Follow { get; private set; }

            /// <summary>
            /// constructs initial environment.
            /// </summary>
            /// <param name="parseString">string to parse</param>
            /// <param name="skip">pattern of skip</param>
            /// <param name="follow">pattern of follow</param>
            public Env(string parseString, Parser<string> skip, Parser<string> follow)
            {
                ParseString = parseString;
                Skip = skip;
                Follow = follow;
            }

            /// <summary>
            /// constructs initial environment.
            /// </summary>
            /// <param name="parseString">string to parse</param>
            /// <param name="skip">pattern of skip</param>
            public Env(string parseString, Parser<string> skip) : this(parseString, skip, null)
            {
            }

            /// <summary>
            /// constructs initial environment with no skip pattern.
            /// </summary>
            /// <param name="parseString">string to parse</param>
            public Env(string parseString) : this(parseString, null, null)
            {
            }
        }

        /// <summary>
        /// A result of parsing.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        public class Result<T>
        {
            /// <summary>
            /// Initial environment.
            /// </summary>
            public Env Env { get; private set; }

            /// <summary>
            /// Position of parsing.
            /// </summary>
            public int Position { get; private set; }

            /// <summary>
            /// Result value.
            /// </summary>
            public T Value { get; private set; }

            /// <summary>
            /// constructs result of parsing.
            /// </summary>
            /// <param name="env">initial environment</param>
            /// <param name="position">position</param>
            /// <param name="value">value</param>
            public Result(Env env, int position, T value)
            {
                Env = env;
                Position = position;
                Value = value;
            }
        }

        private static void CheckNull(dynamic parser, string name)
        {
            if (parser == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        /// runs the parser with given condition.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="toParse">string to parse</param>
        /// <param name="position">starting position of parsing</param>
        /// <param name="skip">skip pattern</param>
        /// <param name="follow">follow pattern</param>
        /// <returns></returns>
        public static IEnumerable<Result<T>> Run<T>(this Parser<T> parser, string toParse, int position, Parser<string> skip, Parser<string> follow)
        {
            CheckNull(parser, nameof(parser));
            CheckNull(toParse, nameof(toParse));
            if (toParse.Length < position || position < 0)
            {
                throw new ArgumentOutOfRangeException("invalid position");
            }
            return parser(new Env(toParse, skip, follow), position);
        }

        /// <summary>
        /// runs the parser with given condition.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="toParse">string to parse</param>
        /// <param name="position">starting position of parsing</param>
        /// <param name="skip">skip regex pattern</param>
        /// <param name="follow">follow regex pattern</param>
        /// <returns></returns>
        public static IEnumerable<Result<T>> Run<T>(this Parser<T> parser, string toParse, int position, string skip, string follow)
        {
            return parser.Run(toParse, position, Regex(skip), Regex(follow));
        }

        /// <summary>
        /// runs the parser with given condition.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="toParse">string to parse</param>
        /// <param name="position">starting position of parsing</param>
        /// <param name="skip">skip pattern</param>
        /// <param name="follow">follow pattern</param>
        /// <returns></returns>
        public static IEnumerable<Result<T>> Run<T>(this Parser<T> parser, string toParse, int position, Parser<string> skip)
        {
            return parser.Run(toParse, position, skip, null);
        }

        /// <summary>
        /// runs the parser with given condition.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="toParse">string to parse</param>
        /// <param name="position">starting position of parsing</param>
        /// <param name="skip">skip regex pattern</param>
        /// <returns></returns>
        public static IEnumerable<Result<T>> Run<T>(this Parser<T> parser, string toParse, int position, string skip)
        {
            return parser.Run(toParse, position, Regex(skip));
        }

        /// <summary>
        /// runs the parser with given condition.
        /// Starting position is beginning of the string.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="toParse">string to parse</param>
        /// <param name="skip">skip pattern</param>
        /// <param name="follow">follow regex pattern</param>
        /// <returns></returns>
        public static IEnumerable<Result<T>> Run<T>(this Parser<T> parser, string toParse, Parser<string> skip, Parser<string> follow)
        {
            return parser.Run(toParse, 0, skip, follow);
        }

        /// <summary>
        /// runs the parser with given condition.
        /// Starting position is beginning of the string.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="toParse">string to parse</param>
        /// <param name="skip">skip regex pattern</param>
        /// <param name="follow">follow regex pattern</param>
        /// <returns></returns>
        public static IEnumerable<Result<T>> Run<T>(this Parser<T> parser, string toParse, string skip, string follow)
        {
            return parser.Run(toParse, 0, skip, follow);
        }

        /// <summary>
        /// runs the parser with given condition.
        /// Starting position is beginning of the string.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="toParse">string to parse</param>
        /// <param name="skip">skip pattern</param>
        /// <returns></returns>
        public static IEnumerable<Result<T>> Run<T>(this Parser<T> parser, string toParse, Parser<string> skip)
        {
            return parser.Run(toParse, 0, skip);
        }

        /// <summary>
        /// runs the parser with given condition.
        /// Starting position is beginning of the string.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="toParse">string to parse</param>
        /// <param name="skip">skip regex pattern</param>
        /// <returns></returns>
        public static IEnumerable<Result<T>> Run<T>(this Parser<T> parser, string toParse, string skip)
        {
            return parser.Run(toParse, 0, skip);
        }

        /// <summary>
        /// runs the parser with given condition and no skipping pattern.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="toParse">string to parse</param>
        /// <param name="position">starting position of parsing</param>
        /// <returns></returns>
        public static IEnumerable<Result<T>> Run<T>(this Parser<T> parser, string toParse, int position)
        {
            return parser.Run(toParse, position, (Parser<string>)null);
        }

        /// <summary>
        /// runs the parser with given condition and no skipping pattern.
        /// Starting position is beginning of the string.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="toParse">string to parse</param>
        /// <returns></returns>
        public static IEnumerable<Result<T>> Run<T>(this Parser<T> parser, string toParse)
        {
            return parser.Run(toParse, 0, (Parser<string>)null);
        }

        private static int Skip(string aString, int position, Parser<string> skip)
        {
            if (skip == null)
            {
                return position;
            }
            else
            {
                var result = skip.Run(aString, position);

                return result.Any() ? result.First().Position : position;
            }
        }

        private static Parser<string> Str(string aString, Func<string, string> f)
        {
            return (env, position) =>
            {
                var pos2 = Skip(env.ParseString, position, env.Skip);
                var toParse = env.ParseString;

                if (toParse.Length >= pos2 + aString.Length && f(toParse.Substring(pos2, aString.Length)) == aString)
                {
                    return new Result<string>[] { new Result<string>(env, pos2 + aString.Length, toParse.Substring(pos2, aString.Length)) };
                }
                else
                {
                    return new Result<string>[0];
                }
            };
        }

        private static Parser<string> Key(string keyword, Func<string, string> f)
        {
            CheckNull(keyword, nameof(keyword));
            return (env, position) =>
            {
                var pos2 = Skip(env.ParseString, position, env.Skip);
                var toParse = env.ParseString;

                if (toParse.Length >= pos2 + keyword.Length && f(toParse.Substring(pos2, keyword.Length)) == keyword)
                {
                    var resultString = toParse.Substring(pos2, keyword.Length);
                    if (env.Skip == null)
                    {
                        return new Result<string>[] { new Result<string>(env, pos2 + keyword.Length, resultString) };
                    }
                    else if (toParse.Length == pos2 + keyword.Length)
                    {
                        return new Result<string>[] { new Result<string>(env, pos2 + keyword.Length, resultString) };
                    }
                    else if (!env.Skip.Run(toParse, pos2 + keyword.Length).Any() && !env.Follow.Run(toParse, pos2 + keyword.Length).Any())
                    {
                        return new Result<string>[0];
                    }
                    else
                    {
                        return new Result<string>[] { new Result<string>(env, pos2 + keyword.Length, resultString) };
                    }
                }
                else
                {
                    return new Result<string>[0];
                }
            };
        }

        /// <summary>
        /// creates a parser of matching the string.
        /// </summary>
        /// <param name="aString">expected string</param>
        /// <param name="errorMessage">error message if this does not match</param>
        /// <returns>parser of matching the string</returns>
        public static Parser<string> Str(string aString)
        {
            CheckNull(aString, nameof(aString));
            return Str(aString, x => x);
        }

        /// <summary>
        /// creates a parser of matching the keyword.
        /// </summary>
        /// <param name="aString">expected keyword</param>
        /// <param name="errorMessage">error message if this does not match</param>
        /// <returns>parser of matching the keyword</returns>
        public static Parser<string> Key(string keyword)
        {
            CheckNull(keyword, nameof(keyword));
            return Key(keyword, x => x);
        }

        /// <summary>
        /// creates a parser of matching the string with ignoring case.
        /// </summary>
        /// <param name="aString">expected string</param>
        /// <param name="errorMessage">error message if this does not match</param>
        /// <returns>parser of matching the string</returns>
        public static Parser<string> IgnoreCase(string aString)
        {
            CheckNull(aString, nameof(aString));
            return Str(aString.ToLower(), x => x.ToLower());
        }

        /// <summary>
        /// creates a parser of matching the keyword with ignoring case.
        /// </summary>
        /// <param name="aString">expected keyword</param>
        /// <param name="errorMessage">error message if this does not match</param>
        /// <returns>parser of matching the keyword</returns>
        public static Parser<string> KeyIgnoreCase(string keyword)
        {
            CheckNull(keyword, nameof(keyword));
            return Key(keyword.ToLower(), x => x.ToLower());
        }

        /// <summary>
        /// creates a parser of matching the regex.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="errorMessage">error message if this does not match</param>
        /// <returns>parser of matching the regex</returns>
        public static Parser<string> Regex(string pattern)
        {
            CheckNull(pattern, nameof(pattern));
            return (env, position) =>
            {
                var pos2 = Skip(env.ParseString, position, env.Skip);
                var toParse = env.ParseString.Substring(pos2);
                var regex = new Regex(pattern);
                var match = regex.Match(toParse);

                if (match.Success && match.Index == 0)
                {
                    return new Result<string>[] { new Result<string>(env, pos2 + match.Length, match.Value) };
                }
                else
                {
                    return new Result<string>[0];
                }
            };
        }

        /// <summary>
        /// creates a parser of matching the end of string.
        /// </summary>
        /// <param name="errorMessage">error message if this does not match</param>
        /// <returns>parser of mathing the end of string</returns>
        public static Parser<int> End()
        {
            return (env, position) =>
            {
                var pos2 = Skip(env.ParseString, position, env.Skip);

                if (pos2 >= env.ParseString.Length)
                {
                    return new Result<int>[] { new Result<int>(env, pos2, 0) };
                }
                else
                {
                    return new Result<int>[0];
                }
            };
        }

        /// <summary>
        /// creates a parser of matching the real number.
        /// </summary>
        /// <param name="errorMessage">error message if this does not match</param>
        /// <returns>parser of matching the real number</returns>
        public static Parser<double> Real()
        {
            return Regex(PatternReal).Select(x => double.Parse(x));
        }

        /// <summary>
        /// The monadic unit function of Parser.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="value">value</param>
        /// <returns>unit monad</returns>
        public static Parser<T> ToParser<T>(T value)
        {
            return (env, position) => new Result<T>[] { new Result<T>(env, position, value) };
        }

        /// <summary>
        /// maps the result value of parser with the given function.
        /// </summary>
        /// <typeparam name="T">input value</typeparam>
        /// <typeparam name="U">output value</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="f">mapping function</param>
        /// <returns></returns>
        public static Parser<U> Select<T, U>(this Parser<T> parser, Func<T, U> f)
        {
            return (env, position) =>
            {
                return parser(env, position).Select(x => new Result<U>(env, x.Position, f(x.Value)));
            };
        }

        /// <summary>
        /// Monadic bind function of parser monad.
        /// </summary>
        /// <typeparam name="T">input type</typeparam>
        /// <typeparam name="U">output type</typeparam>
        /// <param name="parser1">parser</param>
        /// <param name="m">function returns monad</param>
        /// <returns>bound monad</returns>
        public static Parser<U> SelectMany<T, U>(this Parser<T> parser1, Func<T, Parser<U>> m)
        {
            return (env, position) =>
            {
                return parser1(env, position).SelectMany(t => m(t.Value)(env, t.Position));
            };
        }

        /// <summary>
        /// Monadic bind function of parser monad.
        /// </summary>
        /// <typeparam name="T">input type</typeparam>
        /// <typeparam name="U">output type</typeparam>
        /// <typeparam name="V">mapped type</typeparam>
        /// <param name="parser1">parser</param>
        /// <param name="m">function returns monad</param>
        /// <param name="f">mapping function</param>
        /// <returns>bound monad</returns>
        public static Parser<V> SelectMany<T, U, V>(this Parser<T> parser1, Func<T, Parser<U>> m, Func<T, U, V> f)
        {
            return parser1.SelectMany(t => m(t).SelectMany(u => ToParser(f(t, u))));
        }

        /// <summary>
        /// returns the result of first parser if first parser is matched,
        /// otherwise returns the result of second parser.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser1">first parser</param>
        /// <param name="parser2">second parser</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> Choice<T>(this Parser<T> parser1, Parser<T> parser2)
        {
            return (env, position) =>
            {
                var result1 = parser1(env, position);

                return result1.Any() ? result1 : parser2(env, position);
            };
        }

        /// <summary>
        /// ambiguous matching of parser1 and parser2.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser1">first parser</param>
        /// <param name="parser2">second parser</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> Or<T>(this Parser<T> parser1, Parser<T> parser2)
        {
            return (env, position) =>
            {
                return parser1(env, position).Concat(parser2(env, position));
            };
        }

        /// <summary>
        /// returns the result of the given parser if it is matched,
        /// otherwise previous result with the default value.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="defaultValue">default value if the parser is not matched</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> Option<T>(this Parser<T> parser, T defaultValue)
        {
            CheckNull(parser, nameof(parser));
            return parser.Choice(ToParser(defaultValue));
        }

        /// <summary>
        /// returns the result of the given parser if it is matched,
        /// otherwise previous result with default(T).
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> Option<T>(this Parser<T> parser)
        {
            return parser.Option(default(T));
        }

        /// <summary>
        /// ambiguous version of Option.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="defaultValue">default value if the parser is not matched</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> OptionOr<T>(this Parser<T> parser, T defaultValue)
        {
            CheckNull(parser, nameof(parser));
            return parser.Or(ToParser(defaultValue));
        }

        /// <summary>
        /// ambiguous version of Option.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> OptionOr<T>(this Parser<T> parser)
        {
            return parser.OptionOr(default(T));
        }

        /// <summary>
        /// repeats the parser delimited the delimiter parser.
        /// The value is aggregated by aggregator function.
        /// This aggregates the values from left to right.
        /// </summary>
        /// <typeparam name="T">type of parser value</typeparam>
        /// <typeparam name="D">type of delimiter value</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="delimiter">delimiter</param>
        /// <param name="aggregator">aggregator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> Delimit<T, D>(this Parser<T> parser, Parser<D> delimiter, Func<T, D, T, T> aggregator)
        {
            return parser.SelectMany(attr => DelimitRest(attr, parser, delimiter, aggregator));
        }

        /// <summary>
        /// repeats result of the parser delimited the delimiter parser.
        /// This aggregates the values from left to right.
        /// </summary>
        /// <typeparam name="T">type of parser value</typeparam>
        /// <typeparam name="D">type of delimiter value</typeparam>
        /// <param name="attr1">first attribute</param>
        /// <param name="parser">parser</param>
        /// <param name="delimiter">delimiter</param>
        /// <param name="aggregator">aggregator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> DelimitRest<T, D>(T attr1, Parser<T> parser, Parser<D> delimiter, Func<T, D, T, T> aggregator)
        {
            return delimiter.SelectMany(d => parser.SelectMany(
                attr2 => DelimitRest(aggregator(attr1, d, attr2), parser, delimiter, aggregator))).Choice(ToParser(attr1));
        }

        /// <summary>
        /// repeats the parser delimited the delimiter parser.
        /// The value is aggregated by aggregator function.
        /// This aggregates the values from right to left.
        /// </summary>
        /// <typeparam name="T">type of parser value</typeparam>
        /// <typeparam name="D">type of delimiter value</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="delimiter">delimiter</param>
        /// <param name="aggregator">aggregator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> DelimitRight<T, D>(this Parser<T> parser, Parser<D> delimiter, Func<T, D, T, T> aggregator)
        {
            return parser.SelectMany(attr => DelimitRightRest(attr, parser, delimiter, aggregator));
        }

        /// <summary>
        /// repeats result of the parser delimited the delimiter parser.
        /// </summary>
        /// <typeparam name="T">type of parser value</typeparam>
        /// <typeparam name="D">type of delimiter value</typeparam>
        /// <param name="attr1">first attribute</param>
        /// <param name="parser">parser</param>
        /// <param name="delimiter">delimiter</param>
        /// <param name="aggregator">aggregator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> DelimitRightRest<T, D>(T attr1, Parser<T> parser, Parser<D> delimiter, Func<T, D, T, T> aggregator)
        {
            return delimiter.SelectMany(d => parser.SelectMany(
                attr2 => DelimitRightRest(attr2, parser, delimiter, aggregator)).SelectMany(
                r => ToParser(aggregator(attr1, d, r))).Choice(ToParser(attr1)));
        }

        /// <summary>
        /// repeats the parser one or more times.
        /// The values are aggregated from left to right.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="aggregator">aggragator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> OneOrMore<T>(this Parser<T> parser, Func<T, T, T> aggregator)
        {
            return Delimit(parser, Str(""), (a, d, b) => aggregator(a, b));
        }

        /// <summary>
        /// repeats the parser one or more times.
        /// The values are disposed.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="aggregator">aggragator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> OneOrMore<T>(this Parser<T> parser)
        {
            return OneOrMore(parser, (a, b) => a);
        }

        /// <summary>
        /// repeats the parser zero or more times.
        /// The values are aggregated from left to right.
        /// If the parser matched zero times, the value will be the default value.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="aggregator">aggragator function</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> ZeroOrMore<T>(this Parser<T> parser, Func<T, T, T> aggregator, T defaultValue)
        {
            return OneOrMore(parser, aggregator).Choice(Str("").Select(x => defaultValue));
        }

        /// <summary>
        /// repeats the parser zero or more times.
        /// The values are aggregated from left to right.
        /// If the parser matched zero times, the value will be default(T).
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="aggregator">aggragator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> ZeroOrMore<T>(this Parser<T> parser, Func<T, T, T> aggregator)
        {
            return ZeroOrMore(parser, aggregator, default(T));
        }

        /// <summary>
        /// repeats the parser zero or more times.
        /// The values are disposed.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="aggregator">aggragator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> ZeroOrMore<T>(this Parser<T> parser)
        {
            return ZeroOrMore(parser, (x, y) => x);
        }

        /// <summary>
        /// amgiguous version of Delimit.
        /// </summary>
        /// <typeparam name="T">type of parser value</typeparam>
        /// <typeparam name="D">type of delimiter value</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="delimiter">delimiter</param>
        /// <param name="aggregator">aggregator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> DelimitOr<T, D>(this Parser<T> parser, Parser<D> delimiter, Func<T, D, T, T> aggregator)
        {
            return parser.SelectMany(attr => DelimitRestOr(attr, parser, delimiter, aggregator));
        }

        /// <summary>
        /// amgiguous version of DelimitRest.
        /// </summary>
        /// <typeparam name="T">type of parser value</typeparam>
        /// <typeparam name="D">type of delimiter value</typeparam>
        /// <param name="attr1">first attribute</param>
        /// <param name="parser">parser</param>
        /// <param name="delimiter">delimiter</param>
        /// <param name="aggregator">aggregator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> DelimitRestOr<T, D>(T attr1, Parser<T> parser, Parser<D> delimiter, Func<T, D, T, T> aggregator)
        {
            return delimiter.SelectMany(d => parser.SelectMany(
                attr2 => DelimitRestOr(aggregator(attr1, d, attr2), parser, delimiter, aggregator))).Or(ToParser(attr1));
        }

        /// <summary>
        /// amgiguous version of DelimitRight.
        /// </summary>
        /// <typeparam name="T">type of parser value</typeparam>
        /// <typeparam name="D">type of delimiter value</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="delimiter">delimiter</param>
        /// <param name="aggregator">aggregator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> DelimitRightOr<T, D>(this Parser<T> parser, Parser<D> delimiter, Func<T, D, T, T> aggregator)
        {
            return parser.SelectMany(attr => DelimitRightRestOr(attr, parser, delimiter, aggregator));
        }
               
        /// <summary>
        /// amgiguous version of DelimitRightRest.
        /// </summary>
        /// <typeparam name="T">type of parser value</typeparam>
        /// <typeparam name="D">type of delimiter value</typeparam>
        /// <param name="attr1">first attribute</param>
        /// <param name="parser">parser</param>
        /// <param name="delimiter">delimiter</param>
        /// <param name="aggregator">aggregator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> DelimitRightRestOr<T, D>(T attr1, Parser<T> parser, Parser<D> delimiter, Func<T, D, T, T> aggregator)
        {
            return delimiter.SelectMany(d => parser.SelectMany(
                attr2 => DelimitRightRestOr(attr2, parser, delimiter, aggregator)).SelectMany(
                r => ToParser(aggregator(attr1, d, r))).Or(ToParser(attr1)));
        }

        /// <summary>
        /// amgiguous version of OneOrMore.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="aggregator">aggragator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> OneOrMoreOr<T>(this Parser<T> parser, Func<T, T, T> aggregator)
        {
            return DelimitOr(parser, Str(""), (a, d, b) => aggregator(a, b));
        }

        /// <summary>
        /// amgiguous version of OneOrMore.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="aggregator">aggragator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> OneOrMoreOr<T>(this Parser<T> parser)
        {
            return OneOrMoreOr(parser, (a, b) => a);
        }

        /// <summary>
        /// amgiguous version of ZeroOrMore.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="aggregator">aggragator function</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> ZeroOrMoreOr<T>(this Parser<T> parser, Func<T, T, T> aggregator, T defaultValue)
        {
            return OneOrMoreOr(parser, aggregator).Or(Str("").Select(x => defaultValue));
        }

        /// <summary>
        /// amgiguous version of ZeroOrMore.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="aggregator">aggragator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> ZeroOrMoreOr<T>(this Parser<T> parser, Func<T, T, T> aggregator)
        {
            return ZeroOrMoreOr(parser, aggregator, default(T));
        }

        /// <summary>
        /// amgiguous version of ZeroOrMore.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="aggregator">aggragator function</param>
        /// <returns>synchronized parser</returns>
        public static Parser<T> ZeroOrMoreOr<T>(this Parser<T> parser)
        {
            return ZeroOrMoreOr(parser, (x, y) => x);
        }

        /// <summary>
        /// matches the given parser without advancing the position.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <returns>syncronized parser</returns>
        public static Parser<T> Lookahead<T>(this Parser<T> parser)
        {
            return (env, position) =>
            {
                return parser(env, position).Select(x => new Result<T>(env, position, x.Value));
            };
        }

        /// <summary>
        /// If the parser is not matched, matches without advancing the position.
        /// If the parser is matched, this match fails.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <returns>syncronized parser</returns>
        public static Parser<T> Not<T>(this Parser<T> parser)
        {
            return (env, position) =>
            {
                return parser(env, position).Any() ? new Result<T>[0] : new Result<T>[] { new Result<T>(env, position, default(T)) };
            };
        }

        /// <summary>
        /// concatenates two parsers and returns the second value.
        /// </summary>
        /// <typeparam name="T">type of first parser</typeparam>
        /// <typeparam name="U">type of second parser</typeparam>
        /// <param name="parser1">first parser</param>
        /// <param name="parser2">second parser</param>
        /// <returns>concatenated parser</returns>
        public static Parser<U> Concat<T, U>(this Parser<T> parser1, Parser<U> parser2)
        {
            return from a in parser1
                   from b in parser2
                   select b;
        }

        /// <summary>
        /// concatenates two parsers and returns the first value.
        /// </summary>
        /// <typeparam name="T">type of first parser</typeparam>
        /// <typeparam name="U">type of second parser</typeparam>
        /// <param name="parser1">first parser</param>
        /// <param name="parser2">second parser</param>
        /// <returns>concatenated parser</returns>
        public static Parser<T> ConcatLeft<T, U>(this Parser<T> parser1, Parser<U> parser2)
        {
            return from a in parser1
                   from b in parser2
                   select a;
        }

        /// <summary>
        /// matches if the result value applied to the given predicate is true. 
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="parser">parser</param>
        /// <param name="pred">predicate to test</param>
        /// <param name="errorMessage">error message if this is not matched</param>
        /// <returns>result parser</returns>
        public static Parser<T> MatchIf<T>(this Parser<T> parser, Func<T, bool> pred)
        {
            return (env, position) =>
            {
                return parser(env, position).Where(x => pred(x.Value));
            };
        }

        /// <summary>
        /// A method which can refer a return values of the function itself.<br>
        /// This method will be used for defining a expression with recursion.
        /// </summary>
        /// <param name="func">a function which is a return value itself</param>
        /// <returns>first parser</returns>
        public static Parser<T> Letrec<T>(Func<Parser<T>, Parser<T>> func)
        {
            Parser<T> delay = null;
            Parser<T> memo = null;

            delay = (env, position) =>
            {
                if (memo == null)
                {
                    memo = func(delay);
                }
                return memo(env, position);
            };
            return delay;
        }

        /// <summary>
        /// A method which can refer a return values of the function itself.<br>
        /// This method will be used for defining a expression with recursion.
        /// </summary>
        /// <param name="func1">a function whose first argument is a return value itself</param>
        /// <param name="func2">a function whose second argument is a return value itself</param>
        /// <returns>first parser</returns>
        public static Parser<T> Letrec<T, U>(Func<Parser<T>, Parser<U>, Parser<T>> func1, Func<Parser<T>, Parser<U>, Parser<U>> func2)
        {
            Parser<T> delay1 = null;
            Parser<T> memo1 = null;
            Parser<U> delay2 = null;
            Parser<U> memo2 = null;

            delay1 = (env, position) =>
            {
                if (memo1 == null)
                {
                    memo1 = func1(delay1, delay2);
                }
                return memo1(env, position);
            };
            delay2 = (env, position) =>
            {
                if (memo2 == null)
                {
                    memo2 = func2(delay1, delay2);
                }
                return memo2(env, position);
            };
            return delay1;
        }

        /// <summary>
        /// A method which can refer a return values of the function itself.<br>
        /// This method will be used for defining a expression with recursion.
        /// </summary>
        /// <param name="func1">a function whose first argument is a return value itself</param>
        /// <param name="func2">a function whose second argument is a return value itself</param>
        /// <returns>first parser</returns>
        public static Parser<T> Letrec<T>(Func<Parser<T>, Parser<T>, Parser<T>> func1, Func<Parser<T>, Parser<T>, Parser<T>> func2)
        {
            return Letrec<T, T>(func1, func2);
        }

        /// <summary>
        /// A method which can refer a return values of the function itself.<br>
        /// This method will be used for defining a expression with recursion.
        /// </summary>
        /// <param name="func1">a function whose first argument is a return value itself</param>
        /// <param name="func2">a function whose second argument is a return value itself</param>
        /// <param name="func3">a function whose third argument is a return value itself</param>
        /// <returns>first parser</returns>
        public static Parser<T> Letrec<T, U, V>(
            Func<Parser<T>, Parser<U>, Parser<V>, Parser<T>> func1,
            Func<Parser<T>, Parser<U>, Parser<V>, Parser<U>> func2,
            Func<Parser<T>, Parser<U>, Parser<V>, Parser<V>> func3)
        {
            Parser<T> delay1 = null;
            Parser<T> memo1 = null;
            Parser<U> delay2 = null;
            Parser<U> memo2 = null;
            Parser<V> delay3 = null;
            Parser<V> memo3 = null;

            delay1 = (env, position) =>
            {
                if (memo1 == null)
                {
                    memo1 = func1(delay1, delay2, delay3);
                }
                return memo1(env, position);
            };
            delay2 = (env, position) =>
            {
                if (memo2 == null)
                {
                    memo2 = func2(delay1, delay2, delay3);
                }
                return memo2(env, position);
            };
            delay3 = (env, position) =>
            {
                if (memo3 == null)
                {
                    memo3 = func3(delay1, delay2, delay3);
                }
                return memo3(env, position);
            };
            return delay1;
        }

        /// <summary>
        /// A method which can refer a return values of the function itself.<br>
        /// This method will be used for defining a expression with recursion.
        /// </summary>
        /// <param name="func1">a function whose first argument is a return value itself</param>
        /// <param name="func2">a function whose second argument is a return value itself</param>
        /// <param name="func3">a function whose third argument is a return value itself</param>
        /// <returns>first parser</returns>
        public static Parser<T> Letrec<T>(
            Func<Parser<T>, Parser<T>, Parser<T>, Parser<T>> func1,
            Func<Parser<T>, Parser<T>, Parser<T>, Parser<T>> func2,
            Func<Parser<T>, Parser<T>, Parser<T>, Parser<T>> func3)
        {
            return Letrec<T, T, T>(func1, func2, func3);
        }
    }
}
