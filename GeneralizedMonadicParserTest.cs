/**
 * Generalized Monadic Parser
 *
 * Copyright (c) 2021 Yuichiro MORIGUCHI
 *
 * This software is released under the MIT License.
 * http://opensource.org/licenses/mit-license.php
 **/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using static Morilib.GeneralizedMonadicParser;

namespace Morilib
{
    [TestClass]
    public class GeneralizedMonadicParserTest
    {
        private void Match<T>(Parser<T> expr, string toParse, Parser<string> skip, Parser<string> follow, int position,
            IEnumerable<int> positionExpected, IEnumerable<T> valueExpected)
        {
            var config = new Env(toParse, skip, follow);
            var result = expr(config, position);

            Assert.AreEqual(positionExpected.Count(), result.Count());
            Assert.AreEqual(valueExpected.Count(), result.Count());
            for (int i = 0; i < result.Count(); i++)
            {
                Assert.AreEqual(positionExpected.ElementAt(i), result.ElementAt(i).Position);
                Assert.AreEqual(valueExpected.ElementAt(i), result.ElementAt(i).Value);
            }
        }

        private void Match<T>(Parser<T> expr, string toParse, Parser<string> skip, Parser<string> follow, int position, int positionExpected, T valueExpected)
        {
            var config = new Env(toParse, skip, follow);
            var result = expr(config, position);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(positionExpected, result.First().Position);
            Assert.AreEqual(valueExpected, result.First().Value);
        }

        private void Match<T>(Parser<T> expr, string toParse, Parser<string> skip, int position,
            IEnumerable<int> positionExpected, IEnumerable<T> valueExpected)
        {
            var config = new Env(toParse, skip);
            var result = expr(config, position);

            Assert.AreEqual(positionExpected.Count(), result.Count());
            Assert.AreEqual(valueExpected.Count(), result.Count());
            for (int i = 0; i < result.Count(); i++)
            {
                Assert.AreEqual(positionExpected.ElementAt(i), result.ElementAt(i).Position);
                Assert.AreEqual(valueExpected.ElementAt(i), result.ElementAt(i).Value);
            }
        }

        private void Match<T>(Parser<T> expr, string toParse, Parser<string> skip, int position, int positionExpected, T valueExpected)
        {
            var config = new Env(toParse, skip);
            var result = expr(config, position);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(positionExpected, result.First().Position);
            Assert.AreEqual(valueExpected, result.First().Value);
        }

        private void NoMatch<T>(Parser<T> expr, string toParse, Parser<string> skip, Parser<string> follow, int position)
        {
            var config = new Env(toParse, skip, follow);
            var result = expr(config, position);

            Assert.IsFalse(result.Any());
        }

        private void NoMatch<T>(Parser<T> expr, string toParse, Parser<string> skip, int position)
        {
            var config = new Env(toParse, skip);
            var result = expr(config, position);

            Assert.IsFalse(result.Any());
        }

        private void Match<T>(Parser<T> expr, string toParse, string skip, string follow, int position,
            IEnumerable<int> positionExpected, IEnumerable<T> valueExpected)
        {
            Match(expr, toParse, Regex(skip), Regex(follow), position, positionExpected, valueExpected);
        }

        private void Match<T>(Parser<T> expr, string toParse, string skip, int position,
            IEnumerable<int> positionExpected, IEnumerable<T> valueExpected)
        {
            Match(expr, toParse, Regex(skip), position, positionExpected, valueExpected);
        }

        private void Match<T>(Parser<T> expr, string toParse, string skip, string follow, int position, int positionExpected, T valueExpected)
        {
            Match(expr, toParse, Regex(skip), Regex(follow), position, positionExpected, valueExpected);
        }

        private void Match<T>(Parser<T> expr, string toParse, string skip, int position, int positionExpected, T valueExpected)
        {
            Match(expr, toParse, Regex(skip), position, positionExpected, valueExpected);
        }

        private void NoMatch<T>(Parser<T> expr, string toParse, string skip, string follow, int position)
        {
            NoMatch(expr, toParse, Regex(skip), Regex(follow), position);
        }

        private void NoMatch<T>(Parser<T> expr, string toParse, string skip, int position)
        {
            NoMatch(expr, toParse, Regex(skip), position);
        }

        private void Match<T>(Parser<T> expr, string toParse, int position,
            IEnumerable<int> positionExpected, IEnumerable<T> valueExpected)
        {
            var config = new Env(toParse);
            var result = expr(config, position);

            Assert.AreEqual(positionExpected.Count(), result.Count());
            Assert.AreEqual(valueExpected.Count(), result.Count());
            for (int i = 0; i < result.Count(); i++)
            {
                Assert.AreEqual(positionExpected.ElementAt(i), result.ElementAt(i).Position);
                Assert.AreEqual(valueExpected.ElementAt(i), result.ElementAt(i).Value);
            }
        }

        private void Match<T>(Parser<T> expr, string toParse, int position, int positionExpected, T valueExpected)
        {
            var config = new Env(toParse);
            var result = expr(config, position);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(positionExpected, result.First().Position);
            Assert.AreEqual(valueExpected, result.First().Value);
        }

        private void NoMatch<T>(Parser<T> expr, string toParse, int position)
        {
            var config = new Env(toParse);
            var result = expr(config, position);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void StrTest()
        {
            var expr1 = Str("765");
            var skip2 = Letrec<string>(x => from a in Str("#|")
                                            from b in Str("|#").Not().Concat(x.Choice(Regex("."))).ZeroOrMore()
                                            from c in Str("|#")
                                            select b);

            Match(expr1, "000765", 3, 6, "765");
            NoMatch(expr1, "000666", 3);
            Match(expr1, "000   765", " +", 3, 9, "765");
            Match(expr1, "000#|aaa#|aaa|#aaa|#765", skip2, 3, new int[] { 23 }, new string[] { "765" });
        }

        [TestMethod]
        public void KeyTest()
        {
            var expr1 = Key("765");

            Match(expr1, "000765", " +", "[\\);]", 3, 6, "765");
            NoMatch(expr1, "000666", " +", "[\\);]", 3);
            Match(expr1, "000   765  346", " +", "[\\);]", 3, 9, "765");
            NoMatch(expr1, "000   765pro", " +", "[\\);]", 3);
            Match(expr1, "000   765; 346", " +", "[\\);]", 3, 9, "765");
            Match(expr1, "000   765) 346", " +", "[\\);]", 3, 9, "765");
        }

        [TestMethod]
        public void IgnoreCaseTest()
        {
            var expr1 = IgnoreCase("Abc");

            Match(expr1, "000Abc", 3, 6, "Abc");
            Match(expr1, "000ABC", 3, 6, "ABC");
            Match(expr1, "000abc", 3, 6, "abc");
            NoMatch(expr1, "000666", 3);
            Match(expr1, "000   aBC", " +", 3, 9, "aBC");
        }

        [TestMethod]
        public void KeyIgnoreCaseTest()
        {
            var expr1 = KeyIgnoreCase("Abc");

            Match(expr1, "000Abc", " +", "[\\);]", 3, 6, "Abc");
            Match(expr1, "000ABC", " +", "[\\);]", 3, 6, "ABC");
            NoMatch(expr1, "000666", " +", "[\\);]", 3);
            Match(expr1, "000   ABC  346", " +", "[\\);]", 3, 9, "ABC");
            NoMatch(expr1, "000   ABCpro", " +", "[\\);]", 3);
            Match(expr1, "000   ABC; 346", " +", "[\\);]", 3, 9, "ABC");
            Match(expr1, "000   ABC) 346", " +", "[\\);]", 3, 9, "ABC");
        }

        [TestMethod]
        public void RegexTest()
        {
            var expr1 = Regex("8?765");

            Match(expr1, "000765", 3, 6, "765");
            Match(expr1, "0008765", 3, 7, "8765");
            NoMatch(expr1, "000666", 3);
            Match(expr1, "000   765", " +", 3, 9, "765");
        }

        [TestMethod]
        public void EndTest()
        {
            var expr1 = End();

            Match(expr1, "000", 3, 3, 0);
            NoMatch(expr1, "000666", 3);
            Match(expr1, "000   ", " +", 3, 6, 0);
        }

        [TestMethod]
        public void RealTest()
        {
            var expr1 = Real();

            Match(expr1, "765", 0, 3, 765.0);
        }

        [TestMethod]
        public void SelectTest()
        {
            var expr1 = Str("76").Or(Str("765")).Select(x => int.Parse(x));

            Match(expr1, "000765", 3, new int[] { 5, 6 }, new int[] { 76, 765 });
            Match(expr1, "000766", 3, new int[] { 5 }, new int[] { 76 });
            NoMatch(expr1, "000666", 3);
        }

        [TestMethod]
        public void SelectManyMonadRule1Test()
        {
            Func<int, Parser<int>> f1 = x => Str((x + 1).ToString()).Or(Str((x / 10).ToString())).Select(y => int.Parse(y) + 1);
            var expr1 = ToParser(765).SelectMany(f1);
            var expr2 = f1(765);

            Match(expr1, "766", 0, new int[] { 3, 2 }, new int[] { 767, 77 });
            Match(expr2, "766", 0, new int[] { 3, 2 }, new int[] { 767, 77 });
            NoMatch(expr1, "666", 0);
            NoMatch(expr2, "666", 0);
        }

        [TestMethod]
        public void SelectManyMonadRule2Test()
        {
            var expr1 = Str("765").Or(Str("76")).Select(y => int.Parse(y) + 1);
            var expr2 = expr1.SelectMany(ToParser);

            Match(expr1, "765", 0, new int[] { 3, 2 }, new int[] { 766, 77 });
            Match(expr2, "765", 0, new int[] { 3, 2 }, new int[] { 766, 77 });
            NoMatch(expr1, "666", 0);
            NoMatch(expr2, "666", 0);
        }

        [TestMethod]
        public void SelectManyMonadRule3Test()
        {
            Func<int, Parser<int>> f1 = x => Str((x + 1).ToString()).Or(Str((x / 10).ToString())).Select(y => int.Parse(y) + 2);
            Func<int, Parser<int>> f2 = x => Str((x - 1).ToString()).Or(Str((x / 10).ToString())).Select(y => int.Parse(y) - 2);
            var expr1 = ToParser(765).SelectMany(f1).SelectMany(f2);
            var expr2 = ToParser(765).SelectMany(x => f1(x).SelectMany(f2));

            Match(expr1, "766767", 0, new int[] { 6, 5 }, new int[] { 765, 74 });
            Match(expr2, "766767", 0, new int[] { 6, 5 }, new int[] { 765, 74 });
            NoMatch(expr1, "666767", 0);
            NoMatch(expr2, "666767", 0);
            NoMatch(expr1, "766666", 0);
            NoMatch(expr2, "766666", 0);
        }

        [TestMethod]
        public void SelectMany2Test()
        {
            var expr1 = from x in Str("7")
                        from y in Str("6")
                        from z in Str("5").Or(Str(""))
                        select x + y + z;

            Match(expr1, "765", 0, new int[] { 3, 2 }, new string[] { "765", "76" });
            Match(expr1, "7  6  5", " +", 0, new int[] { 7, 6 }, new string[] { "765", "76" });
        }

        [TestMethod]
        public void OneOrMoreTest()
        {
            var expr1 = from x in ToParser(0)
                        from y in Regex("[0-9]").Select(a => int.Parse(a)).OneOrMore((a, b) => a + b)
                        select y;

            Match(expr1, "7", 0, 1, 7);
            Match(expr1, "765", 0, 3, 18);
            NoMatch(expr1, "", 0);
        }

        [TestMethod]
        public void ZeroOrMoreTest()
        {
            var expr1 = from x in ToParser(0)
                        from y in Regex("[0-9]").Select(a => int.Parse(a)).ZeroOrMore((a, b) => a + b, 876)
                        select y;

            Match(expr1, "7", 0, 1, 7);
            Match(expr1, "765", 0, 3, 18);
            Match(expr1, "", 0, 0, 876);
        }

        [TestMethod]
        public void OptionTest()
        {
            var expr1 = Str("765").Option();

            Match(expr1, "765", 0, 3, "765");
            Match(expr1, "765765765876", 0, 3, "765");
            Match(expr1, "876", 0, 0, default(string));
        }

        [TestMethod]
        public void OneOrMoreOrTest()
        {
            var expr1 = from x in ToParser(0)
                        from y in Regex("[0-9]").Select(a => int.Parse(a)).OneOrMoreOr((a, b) => a + b)
                        select y;

            Match(expr1, "7", 0, 1, 7);
            Match(expr1, "765", 0, new int[] { 3, 2, 1 }, new int[] { 18, 13, 7 });
            NoMatch(expr1, "", 0);
        }

        [TestMethod]
        public void ZeroOrMoreOrTest()
        {
            var expr1 = from x in ToParser(0)
                        from y in Regex("[0-9]").Select(a => int.Parse(a)).ZeroOrMoreOr((a, b) => a + b, 876)
                        select y;

            Match(expr1, "7", 0, new int[] { 1, 0 }, new int[] { 7, 876 });
            Match(expr1, "765", 0, new int[] { 3, 2, 1, 0 }, new int[] { 18, 13, 7, 876 });
            Match(expr1, "", 0, 0, 876);
        }

        [TestMethod]
        public void OptionOrTest()
        {
            var expr1 = Str("765").OptionOr();

            Match(expr1, "765", 0, new int[] { 3, 0 }, new string[] { "765", default(string) });
            Match(expr1, "765765765876", 0, new int[] { 3, 0 }, new string[] { "765", default(string) });
            Match(expr1, "876", 0, 0, default(string));
        }

        [TestMethod]
        public void LookaheadTest()
        {
            var expr1 = from a in Str("765").Lookahead()
                        from b in Regex("[0-9]+")
                        select b;

            Match(expr1, "765", 0, 3, "765");
            NoMatch(expr1, "666", 0);
        }

        [TestMethod]
        public void NotTest()
        {
            var expr1 = from a in Str("666").Not()
                        from b in Regex("[0-9]+")
                        select b;

            Match(expr1, "765", 0, 3, "765");
            NoMatch(expr1, "666", 0);
        }

        [TestMethod]
        public void MatchIfTest()
        {
            var expr1 = Regex("[0-9]").OneOrMoreOr((a, b) => a + b).MatchIf(x => x == "765");

            Match(expr1, "765", 0, 3, "765");
            NoMatch(expr1, "666", 0);
            NoMatch(expr1, "aaa", 0);
        }

        [TestMethod]
        public void Letrec1Test()
        {
            var expr1 = Letrec<string>(x => from a in Str("(")
                                            from b in x.Choice(Str(""))
                                            from c in Str(")")
                                            select a + b + c);

            Match(expr1, "((()))", 0, 6, "((()))");
            NoMatch(expr1, "((())", 0);
        }

        [TestMethod]
        public void Letrec2Test()
        {
            var expr1 = Letrec<string, string>((x, y) => from a in Str("(")
                                                         from b in y.Choice(Str(""))
                                                         from c in Str(")")
                                                         select a + b + c,
                                               (x, y) => from a in Str("[")
                                                         from b in x
                                                         from c in Str("]")
                                                         select a + b + c);

            Match(expr1, "([([()])])", 0, 10, "([([()])])");
            NoMatch(expr1, "([([()])]", 0);
        }
    }
}
