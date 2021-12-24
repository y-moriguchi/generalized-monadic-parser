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
using System.Text;
using static Morilib.GeneralizedMonadicParser;

namespace Morilib.Sample
{
    /// <summary>
    /// An example of parsing natural language parsing.
    /// see Structure and Interpretation of Computer Programs (SICP) 4.3.2.
    /// </summary>
    class NaturalLanguageAnalyze
    {
        /// <summary>
        /// analyzed result by S-expression like notation.
        /// </summary>
        class Analyzed
        {
            public string Analyze { get; set; }
            public string Name { get; set; }
            public IEnumerable<Analyzed> Child { get; set; }

            public Analyzed(string analyze, string name)
            {
                Analyze = analyze;
                Name = name;
            }

            public Analyzed(string analyze, IEnumerable<Analyzed> child)
            {
                Analyze = analyze;
                Child = child;
            }

            public override string ToString()
            {
                if (Child == null)
                {
                    return "(" + Analyze + " " + Name + ")";
                }
                else
                {
                    var builder = new StringBuilder();

                    foreach (var c in Child)
                    {
                        builder.Append(" ").Append(c.ToString());
                    }
                    return "(" + Analyze + builder.ToString() + ")";
                }
            }
        }

        static Parser<Analyzed> Word(string type, string word)
        {
            return Key(word).Select(x => new Analyzed(type, word));
        }

        static Parser<Analyzed> Noun(string word)
        {
            return Word("noun", word);
        }

        static Parser<Analyzed> Verb(string word)
        {
            return Word("verb", word);
        }

        static Parser<Analyzed> Article(string word)
        {
            return Word("article", word);
        }

        static Parser<Analyzed> Preposition(string word)
        {
            return Word("prep", word);
        }

        static Parser<Analyzed> CreateParser()
        {
            Parser<Analyzed> nouns = Noun("student")
                .Choice(Noun("professor"))
                .Choice(Noun("class"))
                .Choice(Noun("cat"));

            Parser<Analyzed> verbs = Verb("studies")
                .Choice(Verb("lectures"))
                .Choice(Verb("eats"))
                .Choice(Verb("sleeps"));

            Parser<Analyzed> articles = Article("the").Choice(Article("a"));

            Parser<Analyzed> prepositions = Preposition("for")
                .Choice(Preposition("to"))
                .Choice(Preposition("in"))
                .Choice(Preposition("by"))
                .Choice(Preposition("with"));

            Parser<Analyzed> simpleNounPhrase = from a in articles
                                                from b in nouns
                                                select new Analyzed("simple-noun-phrase", new Analyzed[] { a, b });

            Parser<Analyzed> nounPhrase = null;

            Parser<Analyzed> prepositionalPhrase = from a in prepositions
                                                   from b in nounPhrase
                                                   select new Analyzed("prep-phrase", new Analyzed[] { a, b });

            nounPhrase = from a in simpleNounPhrase
                         from b in DelimitRestOr(a, prepositionalPhrase, Str(""), (x, d, y) => new Analyzed("noun-phrase", new Analyzed[] { x, y }))
                         select b;

            var verbPhrase = from a in verbs
                             from b in DelimitRestOr(a, prepositionalPhrase, Str(""), (x, d, y) => new Analyzed("verb-phrase", new Analyzed[] { x, y }))
                             select b;

            var sentence = from a in nounPhrase
                           from b in verbPhrase
                           from c in Str(".")
                           select new Analyzed("sentence", new Analyzed[] { a, b });

            return sentence;
        }

        static void Main(string[] args)
        {
            var parser = CreateParser();
            var result = parser.Run("the professor lectures to the student in the class with the cat.", " +", "\\.");

            foreach (var r in result)
            {
                Console.WriteLine(r.Value.ToString());
            }
            Console.ReadLine();
        }
    }
}
