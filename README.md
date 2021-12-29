# Generalized Monadic Parser
Generalized Monadic Parser is a parser combinator which can match ambiguously.  
Generalized Monadic Parser can use to parsing natural language analysis.

## Parser delegate
Parser delegate plays a role of generalized monadic parser.
Arguments of Parser delegate are environment (Env class) and position (int).  
And Parser delegate return the list of result (Result class).

```csharp
public delegate IEnumerable<Result<T>> Parser<T>(Env env, int position);
```

Parser delegate has Run extension methods to pass input string and skip pattern.  
Some overloads are available.

```csharp
        // input string, position, Skip and follow pattern by Parser
        public static Result<T> Run<T>(this Parser<T> parser,
            string toParse, int position, Parser<string> skip, Parser<string> follow) { /* ... */ }

        // some overloads available.
```

## Env class
Env class has infomation which does not change in parsing.  
The information consists of input string to parser, Skip pattern and Follow pattern (Parser delegate).

```csharp
        public class Env
        {
            public string ParseString { get; private set; }
            public Parser<string> Skip { get; private set; }
            public Parser<string> Follow { get; private set; }

            public Env(string parseString, Parser<string> skip, Parser<string. follow)
            {
                ParseString = parseString;
                Skip = skip;
                Follow = follow;
            }

            // some overloads available.
        }
```

## Result class
Result class has three properties that are Env class, position, value and error message.

```csharp
        public class Result<T>
        {
            public Env Env { get; private set; }
            public int Position { get; private set; }
            public T Value { get; private set; }

            public Result(Env env, int position, T value)
            {
                Env = env;
                Position = position;
                Value = value;
            }
        }
```

## Str method
Str method matches the given string and input string are matched,
otherwise returns with error message.  

```csharp
var res1 = GeneralizedMonadicParser.Str("765");

//res1.Run("765");   // Match
//res1.Run("666");   // No Match
```

## IgnoreCase method
IgnoreCase method matches the given string and input string are matched without case.

```csharp
var res1 = GeneralizedMonadicParser.IgnoreCase("765pro");

//res1.Run("765pro");   // Match
//res1.Run("765PRO");   // Match
```

## Regex method
Regex method matches the given regex matches the given string.

```csharp
var res1 = GeneralizedMonadicParser.Regex("[1-8]+");

//res1.Run("765");   // Match
//res1.Run("aaa");   // No Match
```

## End method
End method matches end of input string.

```csharp
var res1 = GeneralizedMonadicParser.End();

//res1.Run("");      // Match
//res1.Run("666");   // No Match
```

## Real method
Real method matches any float value and the value of parsing is the matched double value.

```csharp
var res1 = GeneralizedMonadicParser.Real();

//res1.Run("76.5");   // Match, value: [76.5]
//res1.Run("aaaa");   // No Match
```

## ToParser method
ToParser method returns a singleton list of the given value itself.  
ToParser method is monadic Unit function.

```csharp
var res1 = GeneralizedMonadicParser.ToParser(765);

//res1.Run("");   // Match, value: [765]
```

## Select method
Select method maps result value of the given Parser delegate.

```csharp
var res1 = GeneralizedMonadicParser.Real().Select(x => x + 346);

//res1.Run("765");   // Match, value: [1111.0]
```

## SelectMany method
SelectMany method binds the two Parser delegate.  
SelectMany method is monadic Bind function.

```csharp
var res1 = GeneralizedMonadicParser.Regex("[0-9][0-9]").SelectMany(x => GeneralizedMonadicParser.Str(x));

//res1.Run("2727");   // Match
//res1.Run("2728");   // No Match
```

SelectMany method has an override which can use LINQ query syntax.

```csharp
var res1 = from a in GeneralizedMonadicParser.Real()
           from b in GeneralizedMonadicParser.Real()
           select a + b;

//res1.Run("765  346", " +");   // Match, value: [1111.0]
```

## Choice method
Choice method returns the result of first argument if it is matched,
otherwise returns the result of second argument.

```csharp
var res1 = GeneralizedMonadicParser.Str("765").Choice(GeneralizedMonadicParser.Str("346"));

//res1.Run("765");   // Match
//res1.Run("346");   // Match
//res1.Run("666");   // No Match
```

## Or method
Or method matches all selection of matched branch.  
This method make to be able to match ambiguously.

```csharp
var res1 = GeneralizedMonadicParser.Str("76").Or(GeneralizedMonadicParser.Str("765"));

//res1.Run("76");    // Match: value: [76]
//res1.Run("765");   // Match: value: [76, 765]
//res1.Run("666");   // No Match
```

## Option method
Option method returns the result of argument if it is matched,
otherwise return the second argument as a value.

```csharp
var res1 = GeneralizedMonadicParser.Str("765").Option("000");

//res1.Run("765");   // Match, value: [765]
//res1.Run("876");   // Match, value: [000]
```

## OptionOr method
Ambiguous version of Option method.

```csharp
var res1 = GeneralizedMonadicParser.Str("765").Option("000");

//res1.Run("765");   // Match, value: [765, 000]
//res1.Run("876");   // Match, value: [000]
```

## Delimit method
Delimit method aggregates the value of first argument by function given the third argument
and the second argument as a delimiter.  
This method aggregates left associative.

```csharp
var res1 = GeneralizedMonadicParser.Real().Delimit(
             GeneralizedMonadicParser.Str("+"), (x, op, y) => x + y);

//res1.Run("1+2+3");   // Match, value: [6]
```

## DelimitRight method
DelimitRight method is similar to Delimit method but it is right associative.

## OneOrMore method
OneOrMore method aggregates the value of first argument by function given the second argument.

```csharp
var res1 = GeneralizedMonadicParser.Real().OneOrMore((x, y) => x + y);

//res1.Run("1 2 3", " +");   // Match, value: [6]
```

## ZeroOrMore method
ZeroOrMore method aggregates the value of first argument by function given the second argument.  
If input string does not match first argument then returns the value of third argument.  
The third argument is optional, then the third value is default(T).

```csharp
var res1 = GeneralizedMonadicParser.Real().ZeroOrMore((x, y) => x + y, -1);

//res1.Run("1 2 3", " +");   // Match, value: [6]
//res1.Run("", " +");        // Match, value: [-1]
```

## DelimitOr, DelimitRightOr, OneOfMoreOr, ZeroOrMoreOr method
Ambiguous version of Delimit, DelimitRight, OneOfMore or ZeroOrMore methods, respectively.

```csharp
var res1 = GeneralizedMonadicParser.Real().OneOrMore((x, y) => x + y);

//res1.Run("1 2 3", " +");   // Match, value: [6, 3, 1]
```

## Lookahead method
Lookahead method matches if pattern of the argument is matched but position does not advance.

```csharp
var res1 = GeneralizedMonadicParser.Str("876").LookAhead()
            .SelectMany(x => GeneralizedMonadicParser.Real());

//res1.Run("876.5");   // Match
//res1.Run("666");     // No Match
```

## Not method
Not method matches pattern if pattern of the argument is not matched.

```csharp
var res1 = GeneralizedMonadicParser.Str("666").Not()
            .SelectMany(x => GeneralizedMonadicParser.Real());

//res1.Run("876.5");   // Match
//res1.Run("666");     // No Match
```

## Concat method
Concat method concatenates two Parser delegate.  
The result of value is the value of second argument.

```csharp
var res1 = GeneralizedMonadicParser.Str("76").Concat(GeneralizedMonadicParser.Str("5"));

//res1.Run("765");   // Match
```

## ConcatLeft method
Concat method concatenates two Parser delegate.  
The result of value is the value of first argument.

## Letrec method
Letrec method can recurse in the method.  
Arguments of Letrec method is a function whose arguments are Parser delegale and
returns a Parser delegate.

```csharp
var expr1 = Letrec<string, string>((x, y) => from a in Str("(")
                                             from b in y.Choice(Str(""))
                                             from c in Str(")")
                                             select a + b + c,
                                   (x, y) => from a in Str("[")
                                             from b in x
                                             from c in Str("]")
                                             select a + b + c);

//expr1("([([()])])");   // Match
//expr1("([([()])]");    // Not Match
```

