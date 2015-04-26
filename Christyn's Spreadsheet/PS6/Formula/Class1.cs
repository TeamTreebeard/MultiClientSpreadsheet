﻿// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax; variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private readonly string built;           // will contain proper Formula created from the passed "formula" string
        private readonly HashSet<String> num;    // will contain enumeration of variables


        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => Regex.IsMatch(s, "[A-Za-z][1-9][0-9]?"))
        {
            // calls following formula constructor with N = s=>s and V = S => true
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            num = new HashSet<string>();
            double dub = 0;
            built = "";
            int leftP = 0;
            int rightP = 0;
            string lastToken = "";      // "" is used to assure that the expression does not start with an unaccepted token.
            foreach (String i in GetTokens(formula))
            {
                if (i == "(")
                {
                    if ((lastToken == ")") || (lastToken == "0") || lastToken == "v")   // "0" and "v" are used to show that the last seen token was a double or accepted variable
                    {
                        throw new FormulaFormatException("Your expression contains an incorrect use of a closing parentheses");
                    }
                    leftP++;
                    built = built + i;
                    lastToken = "(";
                }
                else if (i == ")")
                {
                    if ((lastToken == "+") || (lastToken == "(") || lastToken == "")
                    {
                        throw new FormulaFormatException("Your expression contains an incorrect use of a closing parentheses");
                    }
                    rightP++;
                    if (rightP > leftP)
                    {
                        throw new FormulaFormatException("Your expression contains too many closing parantheses");
                    }
                    built = built + i;
                    lastToken = ")";
                }
                else if (i == "+" || i == "-" || i == "*" || i == "/")
                {
                    if ((lastToken == "+") || (lastToken == "(") || lastToken == "")
                    {
                        throw new FormulaFormatException("You cannot follow an operator or opening parentheses with an operator.");
                    }
                    else
                    {
                        built = built + i;
                        lastToken = "+";            // "+" is used to tell if the last token seen was ANY operator
                    }
                }
                else if (Double.TryParse(i, out dub))
                {
                    if (lastToken == "0" || lastToken == ")" || lastToken == "v")
                    {
                        throw new FormulaFormatException("You cannot follow a double, closing parentheses or variable with a double");
                    }
                    built = built + dub.ToString();
                    lastToken = "0";
                }
                else
                {
                    if (isValid(normalize(i)))
                    {
                        if (lastToken == "0" || lastToken == ")" || lastToken == "v")
                        {
                            throw new FormulaFormatException("You cannot follow a double, closing parentheses or variable with a variable");
                        }
                        built = built + normalize(i);
                        lastToken = "v";
                        num.Add(normalize(i));
                    }
                    else
                    {
                        throw new FormulaFormatException("You are using an invalid variable");
                    }
                }
            }
            if (built != null && built.Length == 0)
            {
                throw new FormulaFormatException("Your expression is empty or contains no valid tokens");
            }
            if (leftP != rightP)
            {
                throw new FormulaFormatException("Your expression contains an unequal number of parentheses");
            }
            if (lastToken == "(" || lastToken == "+")
            {
                throw new FormulaFormatException("Your expression cannot end with an opening parentheses or an operator");
            }
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, object> lookup)
        {
            Stack<double> val = new Stack<double>();
            Stack<String> oper = new Stack<String>();

            string store = this.ToString();
            string[] substrings = Regex.Split(store, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            double fin;
            double temp;
            double temp2;

            foreach (String i in substrings)
            {
                if (i == "")
                {
                    //do nothing
                }
                else if (i == "(")
                {
                    oper.Push(i);
                }

                else if (i == ")")
                {
                    if (oper.Peek() == "+")
                    {
                        temp = val.Pop();
                        temp2 = val.Pop();
                        temp = temp + temp2;
                        val.Push(temp);
                        oper.Pop();
                    }
                    else if (oper.Peek() == "-")
                    {
                        temp = val.Pop();
                        temp2 = val.Pop();
                        temp = temp2 - temp;
                        val.Push(temp);
                        oper.Pop();
                    }
                    oper.Pop();
                    if ((oper.Count != 0) && (oper.Peek() == "*"))
                    {
                        temp = val.Pop();
                        temp2 = val.Pop();
                        temp = temp * temp2;
                        oper.Pop();
                        val.Push(temp);
                    }
                    else if ((oper.Count != 0) && (oper.Peek() == "/"))
                    {

                        temp = val.Pop();
                        temp2 = val.Pop();
                        temp = temp2 / temp;
                        oper.Pop();

                        if (Double.IsPositiveInfinity(temp))
                        {
                            return new FormulaError("Formula Error");
                        }
                        val.Push(temp);
                    }
                }
                else if (i == "*" | i == "/")
                {
                    oper.Push(i);
                }
                else if (i == "+" | i == "-")
                {
                    if (oper.Count == 0)
                    {
                        oper.Push(i);
                    }
                    else if (oper.Peek() == "+")
                    {
                        temp = val.Pop();
                        temp2 = val.Pop();
                        temp = temp + temp2;
                        val.Push(temp);
                        oper.Pop();
                        oper.Push(i);
                    }
                    else if (oper.Peek() == "-")
                    {
                        temp = val.Pop();
                        temp2 = val.Pop();
                        temp = temp2 - temp;
                        val.Push(temp);
                        oper.Pop();
                        oper.Push(i);
                    }
                    else
                    {
                        oper.Push(i);
                    }
                }
                else if (Double.TryParse(i, out fin))
                {
                    if (oper.Count == 0)
                    {
                        val.Push(fin);
                    }
                    else if (oper.Peek() == "*")
                    {
                        temp = val.Pop() * fin;
                        oper.Pop();
                        val.Push(temp);
                    }

                    else if (oper.Peek() == "/")
                    {
                        temp = val.Pop() / fin;
                        oper.Pop();

                        if (Double.IsPositiveInfinity(temp))
                        {
                            return new FormulaError("Formula Error");
                        }
                        val.Push(temp);
                    }
                    else
                    {
                        val.Push(fin);
                    }
                }
                else
                {
                    if (oper.Count == 0)
                    {
                        try
                        {
                            val.Push((double)lookup(i));
                        }
                        catch (Exception)            //necessary?????
                        {
                            return new FormulaError("Formula Error");
                        }
                    }
                    else if (oper.Peek() == "+" || oper.Peek() == "-")
                    {
                        try
                        {
                            val.Push((double)lookup(i));
                        }
                        catch (Exception)            //necessary?????
                        {
                            return new FormulaError("Formula Error");
                        }
                    }
                    else if (oper.Peek() == "*")
                    {
                        try
                        {
                            temp = val.Pop() * (double)lookup(i);
                        }
                        catch (Exception)            //necessary?????
                        {
                            return new FormulaError("Formula Error");
                        }

                        oper.Pop();
                        val.Push(temp);
                    }
                    else if (oper.Peek() == "/")
                    {
                        try
                        {
                            temp2 = (double)lookup(i);
                        }
                        catch (Exception)            //necessary?????
                        {
                            return new FormulaError("Formula Error");
                        }
                        temp = val.Pop() / temp2;
                        oper.Pop();

                        if (Double.IsPositiveInfinity(temp))
                        {
                            return new FormulaError("Formula Error");
                        }

                        val.Push(temp);
                    }
                    else
                    {
                        try
                        {
                            val.Push((double)lookup(i));
                        }
                        catch (Exception)            //necessary?????
                        {
                            return new FormulaError("Formula Error");
                        }
                    }
                }
            }

            if (oper.Count == 1 && val.Count == 2)
            {
                if (oper.Peek() == "+")
                {
                    temp = val.Pop();
                    temp2 = val.Pop();
                    temp = temp2 + temp;
                    oper.Pop();
                    return temp;
                }
                else
                {
                    temp = val.Pop();
                    temp2 = val.Pop();
                    temp = temp2 - temp;
                    oper.Pop();
                    return temp;
                }
            }

            else
            {
                return val.Pop();
            }
        }
        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return num;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            string temp = "";
            temp = Regex.Replace(built, @"\s", "");
            return temp;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens, which are compared as doubles, and variable tokens,
        /// whose normalized forms are compared as strings.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Formula))
            {
                return false;
            }
            else
            {
                if (!(obj.ToString().Equals(this.ToString())))
                {
                    return false;
                }
                return true;
            }

        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (System.Object.ReferenceEquals(f1, f2))              //RefEquals to avoid null reference exceptions..
            {
                return true;
            }
            if (((object)f1 == null) || ((object)f2 == null))       // casting to object to use "normal" == method and avoid overflow issues..
            {
                return false;
            }
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method...
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}

