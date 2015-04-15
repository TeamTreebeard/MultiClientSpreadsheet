using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace SS
{
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a valid cell name if and only if:
    ///   (1) its first character is an underscore or a letter
    ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
    /// Note that this is the same as the definition of valid variable from the PS3 Formula class.
    /// 
    /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    /// "25", "2x", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
    /// different cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
    /// a name, each cell has a contents and a value.  The distinction is important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private DependencyGraph graph;                      // keeps track of Cell dependencies in the spreadsheet
        private Dictionary<String, Cell> spread;            // represents spreadsheet by keeping track of names of Cells and the corresponding cell.
        bool change;                                // keeps track of if spreadsheet is changed after being opened

        /// <summary>
        /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
        /// spreadsheet consists of an infinite number of named cells.
        /// 
        /// A string is a valid cell name if and only if:
        ///   (1) its first character is an underscore or a letter
        ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
        /// Note that this is the same as the definition of valid variable from the PS3 Formula class.
        /// 
        /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
        /// "25", "2x", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
        /// different cell names.
        /// 
        /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
        /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
        /// a name, each cell has a contents and a value.  The distinction is important.
        /// 
        /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
        /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
        /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
        /// 
        /// In a new spreadsheet, the contents of every cell is the empty string.
        ///  
        /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
        /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
        /// in the grid.)
        /// 
        /// If a cell's contents is a string, its value is that string.
        /// 
        /// If a cell's contents is a double, its value is that double.
        /// 
        /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
        /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
        /// of course, can depend on the values of variables.  The value of a variable is the 
        /// value of the spreadsheet cell it names (if that cell's value is a double) or 
        /// is undefined (otherwise).
        /// 
        /// Spreadsheets are never allowed to contain a combination of Formulas that establish
        /// a circular dependency.  A circular dependency exists when a cell depends on itself.
        /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
        /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
        /// dependency.
        /// </summary>
        public Spreadsheet()
            : this(s => true, s => s, "default")
        {

        }

        /// <summary>
        /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
        /// spreadsheet consists of an infinite number of named cells.
        /// 
        /// A string is a cell name if and only if it consists of one or more letters,
        /// followed by one or more digits AND it satisfies the predicate IsValid.
        /// For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
        /// satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
        /// regardless of IsValid.
        /// 
        /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
        /// must be normalized with the Normalize method before it is used by or saved in 
        /// this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
        /// the Formula "x3+a5" should be converted to "X3+A5" before use.
        /// 
        /// A spreadsheet contains a cell corresponding to every possible cell name.  
        /// In addition to a name, each cell has a contents and a value.  The distinction is
        /// important.
        /// 
        /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
        /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
        /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
        /// 
        /// In a new spreadsheet, the contents of every cell is the empty string.
        ///  
        /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
        /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
        /// in the grid.)
        /// 
        /// If a cell's contents is a string, its value is that string.
        /// 
        /// If a cell's contents is a double, its value is that double.
        /// 
        /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
        /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
        /// of course, can depend on the values of variables.  The value of a variable is the 
        /// value of the spreadsheet cell it names (if that cell's value is a double) or 
        /// is undefined (otherwise).
        /// 
        /// Spreadsheets are never allowed to contain a combination of Formulas that establish
        /// a circular dependency.  A circular dependency exists when a cell depends on itself.
        /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
        /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
        /// dependency.
        /// </summary>
        public Spreadsheet(Func<string, bool> isVal, Func<string, string> norm, string vers)
            : base(isVal, norm, vers)
        {
            graph = new DependencyGraph();
            spread = new Dictionary<String, Cell>();

            change = false;
        }

        /// <summary>
        /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
        /// spreadsheet consists of an infinite number of named cells.
        /// 
        /// A string is a cell name if and only if it consists of one or more letters,
        /// followed by one or more digits AND it satisfies the predicate IsValid.
        /// For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
        /// satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
        /// regardless of IsValid.
        /// 
        /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
        /// must be normalized with the Normalize method before it is used by or saved in 
        /// this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
        /// the Formula "x3+a5" should be converted to "X3+A5" before use.
        /// 
        /// A spreadsheet contains a cell corresponding to every possible cell name.  
        /// In addition to a name, each cell has a contents and a value.  The distinction is
        /// important.
        /// 
        /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
        /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
        /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
        /// 
        /// In a new spreadsheet, the contents of every cell is the empty string.
        ///  
        /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
        /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
        /// in the grid.)
        /// 
        /// If a cell's contents is a string, its value is that string.
        /// 
        /// If a cell's contents is a double, its value is that double.
        /// 
        /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
        /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
        /// of course, can depend on the values of variables.  The value of a variable is the 
        /// value of the spreadsheet cell it names (if that cell's value is a double) or 
        /// is undefined (otherwise).
        /// 
        /// Spreadsheets are never allowed to contain a combination of Formulas that establish
        /// a circular dependency.  A circular dependency exists when a cell depends on itself.
        /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
        /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
        /// dependency.
        /// </summary>
        public Spreadsheet(string path, Func<string, bool> isVal, Func<string, string> norm, string vers)
            : base(isVal, norm, vers)
        {
            graph = new DependencyGraph();
            spread = new Dictionary<String, Cell>();

            if (path == null)
            {
                throw new SpreadsheetReadWriteException("the supplied file path is null.");
            }
            if (GetSavedVersion(path) != vers)
            {
                throw new SpreadsheetReadWriteException("Spreadsheet versions did not match");
            }
            string cellName = null;
            string cellContent = null;
            try
            {
                using (XmlReader reader = XmlReader.Create(path))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement()) //go through start elements
                        {
                            switch(reader.Name)
                            {
                                case "spreadsheet":
                                    break;
                                case "cell":
                                    break;
                                case "name":
                                    reader.Read();
                                    cellName = reader.Value;            // grab cell name
                                    if(!(isNameValid(cellName)))
                                    {
                                        throw new SpreadsheetReadWriteException("Cell name " + cellName + " is invalid.");
                                    }
                                    break;
                                case "contents":
                                    reader.Read();
                                    cellContent = reader.Value;          //grab cell contents
                                    break; 
                            }
                        }
                            else
                            {
                                if(reader.Name == "cell")
                                {
                                    if (cellName == null)
                                    {
                                        throw new SpreadsheetReadWriteException("XML doc missing cell name information");
                                    }
                                    if (cellContent == null)
                                    {
                                        throw new SpreadsheetReadWriteException("XML doc missing cell content information");
                                    }

                                    SetContentsOfCell(cellName, cellContent);

                                    cellName = null;
                                    cellContent = null;
                                }
                            }

                        }
                    }
                
                }
            catch(FormulaFormatException)
            {
                throw new SpreadsheetReadWriteException("Error encountered creating Formula");
            }
            catch(CircularException)
            {
                throw new SpreadsheetReadWriteException("Circular dependency encountered while writing spreadsheet");
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException("Spread sheet failed because " + e.GetBaseException());
            }

            change = false;
        }

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get
            {
                return change;
            }
            protected set
            {
                change = value;
            }
        }

        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement()) //go through start elements
                        {
                            if (reader.Name == "spreadsheet")
                            {
                                    return reader.GetAttribute("version");
                            }
                            else
                            {
                                throw new SpreadsheetReadWriteException("Spreadsheet is not first element");
                            }
                        }
                        else
                        {
                            throw new SpreadsheetReadWriteException("Spreadsheet element does not exist in XML document");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException("Spread sheet failed because " + e.GetBaseException());
            }
            throw new SpreadsheetReadWriteException("Spreadsheet version was not retrieved");
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            if(filename == null)
            {
                throw new SpreadsheetReadWriteException("filename is null");
            }

            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    Cell c;
                    object cellContents;
                    foreach (String i in GetNamesOfAllNonemptyCells())
                    {
                        spread.TryGetValue(i, out c);
                        cellContents = c.getContents();

                        if(cellContents is string)
                        {
                            string temp = (string)cellContents;
                            writer.WriteStartElement("cell");
                            writer.WriteElementString("name", i);
                            writer.WriteElementString("contents", temp);
                            writer.WriteEndElement();
                        }
                        else if(cellContents is double)
                        {
                            double temp = (double)cellContents;
                            writer.WriteStartElement("cell");
                            writer.WriteElementString("name", i);
                            writer.WriteElementString("contents", temp.ToString());
                            writer.WriteEndElement();
                        }
                        else if(cellContents is Formula)
                        {
                            Formula temp = (Formula)cellContents;
                            writer.WriteStartElement("cell");
                            writer.WriteElementString("name", i);
                            writer.WriteElementString("contents", "="+temp.ToString());
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch(System.IO.DirectoryNotFoundException)
            {
                throw new SpreadsheetReadWriteException("Cannot find specified directory");
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException("Spread sheet failed because " + e.GetBaseException());
            }
            change = false;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
          if (name == null)
            {
                throw new InvalidNameException();
            }
          name = Normalize(name);
          if ((Regex.IsMatch(name, "^[A-za-z]+[0-9]+$") && IsValid(name)))
          {
              Cell c;
              spread.TryGetValue(name, out c);
              try
              {
                  return c.getValue();
              }
              catch(NullReferenceException)
              {
                  return "";
              }
          }
          throw new InvalidNameException();
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException();
            }
             else if (name == null)
            {
                throw new InvalidNameException();
            }
            HashSet<string> @return;
            name = Normalize(name);
            if ((Regex.IsMatch(name, "^[A-za-z]+[0-9]+$") && IsValid(name)))
            {
                double temp;
                if (Double.TryParse(content, out temp))
                {
                    @return = (HashSet<string>)SetCellContents(name, temp);
                    change = true;
                    return @return;
                }
                if (content.StartsWith("="))
                {
                    content = content.Substring(1);
                    try
                    {
                        @return = (HashSet<string>)SetCellContents(name, new Formula(content));
                    }
                    catch(FormulaFormatException)
                    {
                        throw new FormulaFormatException("Formula is formatted improperly");
                    }
                    catch(CircularException)
                    {
                        throw new CircularException();
                    }
                    change = true;
                    return @return;
                }
                else
                {
                    @return = (HashSet<string>)SetCellContents(name, content);
                    change = true;
                    return @return;
                }
            }
            throw new InvalidNameException();
        }




        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            try
            {
                return spread.Keys;
            }
            catch(KeyNotFoundException)
            {
                return new HashSet<string>();
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (name == null)
            {
                throw new InvalidNameException();
            }
            name = Normalize(name);
            if (Regex.IsMatch(name, "^[A-za-z]+[0-9]+$") && IsValid(name))
            {
                Cell contents;
                if (spread.TryGetValue(name, out contents))
                {
                    return contents.getContents();
                }
                else
                {
                    return "";
                }
            }
            throw new InvalidNameException();
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            
            // name null removed as it is first checked by SetContentsofCell

            if ((Regex.IsMatch(name, "^[A-za-z]+[0-9]+$") && IsValid(name)))
            {
                if (spread.ContainsKey(name))
                {
                    spread.Remove(name);
                }
                spread.Add(name, new Cell(number));
                HashSet<string> final = new HashSet<string>();
                if(graph.HasDependents(name))
                {
                    HashSet<string> temp = new HashSet<string>();       //removes dependents if prev Cell was formula
                    graph.ReplaceDependents(name, temp);
                }
                if (graph.HasDependees(name))
                {
                    Cell c;
                    foreach (string i in GetCellsToRecalculate(name))
                    {
                        final.Add(i);
                        spread.TryGetValue(i, out c);
                        c.recalculate(GetValForLookup);
                    }
                }
                else
                {
                    final.Add(name);
                }
                
                return final;
            }
            throw new InvalidNameException();
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            // name null removed as it is first checked by SetContentsofCell

            if (text == null)
            {
                throw new ArgumentNullException();
            }
            // removed unnecessary code?
            else if ((Regex.IsMatch(name, "^[A-za-z]+[0-9]+$") && IsValid(name)))
            {
                if (spread.ContainsKey(name))
                {
                    spread.Remove(name);
                }
                spread.Add(name, new Cell(text));
                HashSet<string> final = new HashSet<string>();
                if (graph.HasDependents(name))
                {
                    HashSet<string> temp = new HashSet<string>();       //removes dependents if prev Cell was formula
                    graph.ReplaceDependents(name, temp);
                }
                if (graph.HasDependees(name))
                {
                    Cell c;
                    foreach (string i in GetCellsToRecalculate(name))
                    {
                        final.Add(i);
                        spread.TryGetValue(i, out c);
                        c.recalculate(GetValForLookup);
                    }
                }
                else
                {
                    final.Add(name);
                }

                return final;
            }
            throw new InvalidNameException();
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            // name null removed as it is first checked by SetContentsofCell

            if (formula == null)
            {
                throw new ArgumentNullException();
            }
            if ((Regex.IsMatch(name, "^[_A-za-z][_A-za-z0-9]*$") && IsValid(name)))
            {
                string stemp = null;
                double dtemp = double.PositiveInfinity;
                Formula ftemp = null;
                HashSet<string> oldVars = new HashSet<string>();
                if (spread.ContainsKey(name))
                {
                    Cell oldVal;
                    spread.TryGetValue(name, out oldVal);
                    if (oldVal.getContents() is string)
                    {
                        stemp = (string)oldVal.getContents();
                    }
                    else if (oldVal.getContents() is double)
                    {
                        dtemp = (double)oldVal.getContents();
                    }
                    else if (oldVal.getContents() is Formula)
                    {
                        ftemp = (Formula)oldVal.getContents();
                        oldVars = (HashSet<string>)ftemp.GetVariables();
                    }
                    spread.Remove(name);
                }
                    HashSet<string> final = new HashSet<string>();

                      if (graph.HasDependents(name))
                    {
                        HashSet<string> temp = (HashSet<string>)formula.GetVariables();
                        foreach (string l in temp)
                        {
                            if(!isNameValid(l))
                            {
                                throw new FormulaFormatException("You're using invalid cell names in your formula");
                            }
                        }
                        graph.ReplaceDependents(name, temp);
                    }
                    else
                    {
                        foreach (string k in formula.GetVariables())
                        {
                            if(isNameValid(k))
                            {
                            graph.AddDependency(name, k);
                            }
                            else
                            {
                                throw new FormulaFormatException("You're using invalid cell names in your formula");
                            }
                        }
                    }
                    try
                    {
                        foreach (string i in GetCellsToRecalculate(name))
                        {
                            final.Add(i);
                        }
                        spread.Add(name, new Cell(formula, GetValForLookup));
                        Cell p;
                        foreach(string j in final)
                        {
                            spread.TryGetValue(j, out p);
                            p.recalculate(GetValForLookup);
                        }
                        return final;
                    }
                    catch (CircularException)
                    {
                        try
                        {
                            graph.ReplaceDependents(name, oldVars);
                            if (stemp != null)
                            {
                                SetCellContents(name, stemp);
                            }
                            else if (!(double.IsPositiveInfinity(dtemp)))
                            {
                                SetCellContents(name, dtemp);
                            }
                            else if (ftemp != null)
                            {
                                SetCellContents(name, ftemp);
                            }
                        }
                        catch(NullReferenceException)
                        {

                        }
                        throw new CircularException(); 
                    }
            }
            throw new InvalidNameException();
        }

        /// <summary>
        /// Helper method to return value of cell if it is a double. Will return a FormulaError otherwise.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private object GetValForLookup(string name)
        {
            double temp;

            if (name == null)
                return new FormulaError();

            String s = Normalize(name);

            if (!(isNameValid(s)))
                return new FormulaError();

            try
            {
                Cell c;
                if (spread.TryGetValue(s, out c))
                    temp = (double)c.getValue();
                else
                    return new FormulaError();
            }
            catch (InvalidCastException)
            {
                return new FormulaError();
            }

            return temp;
        }

        private bool isNameValid(String s)
        {
            return ((Regex.IsMatch(s, "^[A-za-z]+[0-9]+$") && IsValid(s)));
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name != null)
            {
                name = Normalize(name);
                if (Regex.IsMatch(name, "^[A-za-z]+[0-9]+$") && IsValid(name))
                {
                    if(graph.HasDependees(name))
                    {
                        return graph.GetDependees(name);
                    }
                    return new HashSet<string>();
                }
                throw new InvalidNameException();
            }
            throw new ArgumentNullException();
        }


        /// <summary>
        /// Representation of a Cell in a spreadsheet.
        /// Cell can have a value represented by a (1) string (2) double (3) FormulaError.
        /// Cell can contain a (1)string (2)double (3)formula.
        /// </summary>
        private class Cell
        {
           private String _val, contentsS;
           private double _value = double.PositiveInfinity, _valueF = double.PositiveInfinity, contentsD = double.PositiveInfinity;
           private Formula contentsF;
           private FormulaError _error;

            /// <summary>
            /// Creates Cell which contains a string.
            /// </summary>
            /// <param name="s"></param>
            public Cell(String s)
            {
                contentsS = s;
                _val = s;
            }

            /// <summary>
            /// Creates cell which contains a double.
            /// </summary>
            /// <param name="d"></param>
            public Cell(double d)
            {
                contentsD = d;
                _value = d;
            }

            /// <summary>
            /// Creates Cell which contains a Formula.
            /// </summary>
            /// <param name="f"></param>
            /// <param name="lookup">lookup Func for method to return variable values</param>
            public Cell(Formula f, Func<string, object> lookup)
            {
                contentsF = f;

                if (f.Evaluate(lookup) is FormulaError)
                {
                    _error = (FormulaError)f.Evaluate(lookup);
                }
                else
                {
                    _valueF = (double)f.Evaluate(lookup);
                }

            }

            /// <summary>
            /// Returns the value of the Cell.
            /// </summary>
            /// <returns></returns>
            public object getValue()
            {
                if (_val != null)
                {
                    return _val;
                }
                if (!(double.IsPositiveInfinity(_value)))
                {
                    return _value;
                }
                else
                {
                    if (!(double.IsPositiveInfinity(_valueF)))
                    {
                        return _valueF;
                    }
                    return _error;
                }
            }

            /// <summary>
            /// Used to recalculate the values of Formula cells after a cell on which it depends
            /// has been changed
            /// </summary>
            public void recalculate(Func<string, object> lookup)
            {
                if (this.getContents() is Formula)
                {
                    Formula f = (Formula)this.getContents();
                    if (f.Evaluate(lookup) is FormulaError)
                    {
                        _error = (FormulaError)f.Evaluate(lookup);

                        if (_val != null)
                        {
                            _val = null;
                        }
                        if (!(double.IsPositiveInfinity(_value)))
                        {
                            _value = double.PositiveInfinity;
                        }
                        if (!(double.IsPositiveInfinity(_valueF)))
                        {
                            _valueF = double.PositiveInfinity;
                        }
                    }
                    else
                    {
                        _valueF = (double)f.Evaluate(lookup);
                    }
                }

            }
            /// <summary>
            /// Returns the contents of the Cell.
            /// </summary>
            /// <returns></returns>
            public object getContents()
            {
                if (contentsS != null)
                {
                    return contentsS;
                }
                if (!(double.IsPositiveInfinity(contentsD)))
                {
                    return contentsD;
                }
                else
                {
                    return contentsF;
                }
            }
        }
    }
}
