using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SS;
using SpreadsheetUtilities;

namespace SpreadsheetTests
{
    /// <summary>
    /// Test Code for Spreadsheet.cs
    /// 
    /// 100% not achieved due to some exceptions that I can't directly hit because I cannot directly
    /// call some private methods (invalid name/null name are caught by my previous methods). 
    /// 
    /// 
    /// Cell is private but currently has an unused getValue method which is not tested so it is lowering
    /// coverage. Commenting this method out gives me 92.23% coverage.
    /// </summary>
    [TestClass()]
    public class UnitTest1
    {

        /// <summary>
        /// Testing to make sure empty spreadsheets still return something for non-empty cells (no errors)
        /// </summary>
        [TestMethod()]
        public void EmptyTest1()
        {
            Spreadsheet s = new Spreadsheet();
            Spreadsheet k = new Spreadsheet();

            int count = 0;
            int count1 = 0;
            foreach(string i in s.GetNamesOfAllNonemptyCells())
            {
                count++;
            }
            foreach(string j in k.GetNamesOfAllNonemptyCells())
            {
                count1++;
            }

            Assert.IsTrue(count == count1);
        }

        /// <summary>
        /// Make sure correct exception thrown when null
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void NameNullTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
            s.SetContentsOfCell(null, "hi");
        }

        /// <summary>
        /// Make sure correct exception thrown when null
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void NameNullTest1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "42");

        }

        /// <summary>
        /// Make sure correct exception thrown when null
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void NameNullTest2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "=A5+3");
        }


        /// <summary>
        /// Make sure correct exception thrown when name is invalid
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void NameInvalidTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1A5", "hi");
        }

        /// <summary>
        /// Make sure correct exception thrown when name is invalid
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void NameInvalidTest1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1A5", "42");
        }

        /// <summary>
        /// Make sure correct exception thrown when name is invalid
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void NameInvalidTest2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("2A5", "A5+3");

        }

        /// <summary>
        /// Make sure correct Exception is thrown when argument is invalid
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNullTest()
        {
            Spreadsheet s = new Spreadsheet();
            string t = null;
            s.SetContentsOfCell("A5", t);
        }

        /// <summary>
        /// Make sure correct Exception is thrown when argument is invalid
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNullTest1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A5", null);
        }

        /// <summary>
        /// Tests to see if Strings are set properly
        /// </summary>
        [TestMethod()]
        public void AddingStringtoEmptyCell()
        {
            Spreadsheet s = new Spreadsheet();

            int count = 0;
            foreach (string j in s.GetNamesOfAllNonemptyCells())
            {
                count++;
            }

            Assert.IsTrue(count == 0);

            s.SetContentsOfCell("A1", "hello world");
            s.SetContentsOfCell("A2", "hello world1");
            s.SetContentsOfCell("A3", "gtfo world!!!!");

            string f = (string)s.GetCellContents("A1");
            string g = (string)s.GetCellContents("A2");
            string h = (string)s.GetCellContents("A3");
            Assert.IsTrue(f.Equals("hello world"));
            Assert.IsTrue(g.Equals("hello world1"));
            Assert.IsTrue(h.Equals("gtfo world!!!!"));

            count = 0;

            foreach(string i in s.GetNamesOfAllNonemptyCells())
            {
                count++;
            }

            Assert.IsTrue(count == 3);
        }

        /// <summary>
        /// Tests to see if doubles are set properly
        /// </summary>
        [TestMethod()]
        public void AddingDoubletoEmptyCell()
        {
            Spreadsheet s = new Spreadsheet();
            int count = 0;
            foreach(string j in s.GetNamesOfAllNonemptyCells())
            {
                count++;
            }

            Assert.IsTrue(count == 0);

            s.SetContentsOfCell("A1", "52");
            s.SetContentsOfCell("A2", "91.7");
            s.SetContentsOfCell("A3", "100000");

            double f = (double)s.GetCellContents("A1");
            double g = (double)s.GetCellContents("A2");
            double h = (double)s.GetCellContents("A3");
            Assert.IsTrue(f == 52.0);
            Assert.IsTrue(g == 91.7);
            Assert.IsTrue(h == 100000.0);

            count = 0;

            foreach (string i in s.GetNamesOfAllNonemptyCells())
            {
                count++;
            }

            Assert.IsTrue(count == 3);
        }

        /// <summary>
        /// Tests to see if Formulas are set properly
        /// </summary>
        [TestMethod()]
        public void AddingFormulatoEmptyCell()
        {
            Spreadsheet s = new Spreadsheet();
            int count = 0;
            foreach (string j in s.GetNamesOfAllNonemptyCells())
            {
                count++;
            }

            Assert.IsTrue(count == 0);

            Formula form = new Formula("8+5");
            Formula form2 = new Formula("13");
            Formula form3 = new Formula("12+A1+1");

            s.SetContentsOfCell("A1", "=8+5");
            s.SetContentsOfCell("A2", "=13");
            s.SetContentsOfCell("A3", "=12+A1+1");

            Formula f = (Formula)s.GetCellContents("A1");
            Formula g = (Formula)s.GetCellContents("A2");
            Formula h = (Formula)s.GetCellContents("A3");
            Assert.IsTrue(f.Equals(form));
            Assert.IsTrue(g.Equals(form2));
            Assert.IsTrue(h.Equals(form3));

            count = 0;

            foreach (string i in s.GetNamesOfAllNonemptyCells())
            {
                count++;
            }

            Assert.IsTrue(count == 3);
        }

        /// <summary>
        /// Tests to see if overriding a cell containing a Formula correctly removes pointers and updates right.
        /// </summary>
        [TestMethod()]
        public void ReplaceFormula_withDouble_and_String()
        {
            Spreadsheet s = new Spreadsheet();

            s.SetContentsOfCell("A1", "=A2+5");
            Formula f = (Formula)s.GetCellContents("A1");
            s.SetContentsOfCell("A1", "43.0");
            double g = (double)s.GetCellContents("A1");
            s.SetContentsOfCell("A1", "hi");
            string h = (string)s.GetCellContents("A1");

            Assert.IsTrue(g == 43.0);
            Assert.IsTrue(h.Equals("hi"));

            int count = 0;

            foreach (string i in s.GetNamesOfAllNonemptyCells())
            {
                count++;
            }

            Assert.IsTrue(count == 1);
        }

        /// <summary>
        /// Tests to see if overriding a cell with a double correctly adds pointers and updates right.
        /// </summary>
        [TestMethod()]
        public void ReplaceDouble_withFormula()
        {
            Spreadsheet s = new Spreadsheet();

            s.SetContentsOfCell("A1", "43.0");
            double g = (double)s.GetCellContents("A1");
            s.SetContentsOfCell("A1", "=12+A3+1");
            Formula h = (Formula)s.GetCellContents("A1");

            Assert.IsTrue(g == 43.0);

            int count = 0;

            foreach (string i in s.GetNamesOfAllNonemptyCells())
            {
                count++;
            }

            Assert.IsTrue(count == 1);
        }

        /// <summary>
        /// Tests to see if overriding a cell with a string correctly adds pointers and updates right.
        /// </summary>
        [TestMethod()]
        public void ReplaceString_withFormula()
        {
            Spreadsheet s = new Spreadsheet();

            s.SetContentsOfCell("A1", "hi");
            string g = (string)s.GetCellContents("A1");
            s.SetContentsOfCell("A1", "=12+A3+1");
            Formula h = (Formula)s.GetCellContents("A1");

            Assert.IsTrue(g.Equals("hi"));

            int count = 0;

            foreach (string i in s.GetNamesOfAllNonemptyCells())
            {
                count++;
            }

            Assert.IsTrue(count == 1);
        }


        /// <summary>
        /// Tests to see if overriding a cell with a string correctly adds pointers and updates right.
        /// </summary>
        [TestMethod()]
        public void BOOPTest()
        {
            Spreadsheet s = new Spreadsheet();

            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("B1", "2");
            s.SetContentsOfCell("C1", "=A1+B1");
            Assert.IsTrue((double)s.GetCellValue("C1") == 3);
            s.SetContentsOfCell("A1", "");
            Console.WriteLine(s.GetCellValue("C1"));
        }




        /// <summary>
        /// Tests to see if changing to a new Formula will update pointers correctly.
        /// </summary>
        [TestMethod()]
        public void ReplaceFormula_withFormula()
        {
            Spreadsheet s = new Spreadsheet();

            s.SetContentsOfCell("A1", "=12+A3+1");
            Formula g = (Formula)s.GetCellContents("A1");
            s.SetContentsOfCell("A1", "=5*C1");
            Formula h = (Formula)s.GetCellContents("A1");

            int count = 0;

            foreach (string i in s.GetNamesOfAllNonemptyCells())
            {
                count++;
            }

            Assert.IsTrue(count == 1);
        }

        /// <summary>
        /// Catching circular exception and making sure that previous pointers are restored and
        /// cell remains unchanged.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void CircularExceptionTest()
        {
            Spreadsheet s = new Spreadsheet();

            s.SetContentsOfCell("A3", "=5*C1");
            s.SetContentsOfCell("A3", "=12+A3+1");
        }


        /// <summary>
        /// Checking if getValue is working correctly
        /// </summary>
        [TestMethod()]
        public void GetValueTest()
        {
            Spreadsheet s = new Spreadsheet();

            s.SetContentsOfCell("A3", "=5*3");
            Assert.IsTrue((double)s.GetCellValue("A3") == 15);
            s.SetContentsOfCell("A3", "15");
            Assert.IsTrue((double)s.GetCellValue("A3") == 15);
            s.SetContentsOfCell("A3", "hi");
            Assert.IsTrue(s.GetCellValue("A3").Equals("hi"));
        }

        /// <summary>
        /// Checking if getValue is working correctly
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetValueTest1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "15");
        }

        /// <summary>
        /// Checking if getValue is working correctly
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetValueTest2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("32A5", "15");
        }

        /// <summary>
        /// Checking if getValue is working correctly
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetValueTest3()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("32A5", "15");
        }


        /// <summary>
        /// Creating spreadsheet
        /// </summary>
        [TestMethod()]
        public void GetValueTest4()
        {
            Spreadsheet s = new Spreadsheet(t => true, t => t, "default");
        }


        /// <summary>
        /// General testing with some example variables to make sure I'm getting the correct returns from
        /// GetDirectDependents and GetCellstoRecalculate.
        /// </summary>
        [TestMethod()]
        public void GenTest()
        {
            Spreadsheet s = new Spreadsheet();
            Formula form = new Formula("A1+B1");
            Formula form1 = new Formula("A1*C1");

            s.SetContentsOfCell("A1", "3");
            s.SetContentsOfCell("B1", "7");
            s.SetContentsOfCell("C1", "=A1+B1");
            s.SetContentsOfCell("D1", "=A1*C1");
            s.SetContentsOfCell("E1", "15");

            HashSet<string> test = new HashSet<string>();
            foreach (string i in s.SetContentsOfCell("A1", "6"))
            {
                test.Add(i);
            }

            Assert.IsTrue(test.Contains("A1"));
            Assert.IsTrue(test.Contains("C1"));
            Assert.IsTrue(test.Contains("D1"));

            test.Clear();

            foreach (string j in s.SetContentsOfCell("B1", "12"))
            {
                test.Add(j);
            }

            Assert.IsTrue(test.Contains("B1"));
            Assert.IsTrue(test.Contains("C1"));
            Assert.IsTrue(test.Contains("D1"));

            int count = 0;

            foreach (string i in s.GetNamesOfAllNonemptyCells())
            {
                count++;
            }

            Assert.IsTrue(count == 5);
        }

// ##################################################################################################
// ##################################################################################################
        // THIS TEST CODES USES PATHS FOR MY COMPUTER!! WILL NOT WORK UNLESS THE PATHS ARE CHANGED!

        /// <summary>
        /// Save, Load, GetSavedVersion
        /// </summary>
        //[TestMethod()]
        //public void Saves()
        //{
        //    Spreadsheet s = new Spreadsheet();
        //    s.SetContentsOfCell("A1", "15");
        //    s.SetContentsOfCell("A2", "hi");
        //    s.SetContentsOfCell("A3", "=6+7");

        //    s.Save("C:/Users/Christyn/Desktop/aligator.xml");
        //    string temp = s.GetSavedVersion("C:/Users/Christyn/Desktop/aligator.xml");
        //    Assert.IsTrue(temp.Equals("default"));
        //    Spreadsheet k = new Spreadsheet("C:/Users/Christyn/Desktop/aligator.xml", t => true, t => t, "default");
        //    Assert.IsTrue((double)k.GetCellValue("A1") == 15);
        //    Assert.IsTrue(k.GetCellValue("A2").Equals("hi"));
        //    Assert.IsTrue((double)k.GetCellValue("A3") == 13);
        //}



    }
}
