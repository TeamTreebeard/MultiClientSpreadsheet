#include <string>
#include <vector>

using namespace std;

class Spreadsheet
{
	public:

	/*
	If the name is null or invalid, throws an InvalidNameException.
	Otherwise, returns the value (as opposed to the contents) of the
	named cell.
	*/
	string GetCellValue(string name);
	/*
	If the name is null or invalid, throws an InvalidNameException.
	Otherwise, returns the contents (as opposed to the value) of the
	named cell. 
	*/
	string GetCellContents(string name);
	/*
	Enumerates the names of all the non-empty cells in the spreadsheet.
	*/
	vector<string> GetNamesOfAllNonemptyCells();
	/*
	If content is null, throws an ArgumentNullException.
	
	Otherwise, if name is null or invalid, throws an InvalidNameException.
	
	Otherwise, if content parses as a double, the contents of the named
	cell becomes that double.
	
	Otherwise, if the content begins with the character '=', an attempt is
	made to parse the remainder of content into a Formula using the Formula
	constructor. There are then three possibilities:
	
		(1) If the remainder of content cannot be parsed into a Formula, a
			FormulaFormatException is thrown.
		(2) Otherwise, if changing the contents of the named cell to be f
		would cause a circular dependency, a CircularException is thrown.
		(3) Otherwise, the contents of the named cell becomes f.
		
	Otherwise, the contents of the name cell become content.
	
	If an exception is not thrown, the method returns a set consisting of
	name plus the names of all other cells whose value depends, directly
	or indirectly, on the named cell.
	
	For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
	set {A1, B1, C1} is returned.
	*/
	vector<string> SetContentsOfCell(string name, string content);
	/*
	If name is null, throws an ArgumentNull Exception
	
	Otherwise, if name isn't a valid cell name, throws an InvalidNameException
	
	Otherwise, returns an vector, without duplicates, of the names of all
	cells whose values depend directly on the values of the named cell. In
	other words, returns an enumeration, without duplicates, of the names
	of all cells that contain formulas containing name.
	
	For example, suppose that
	A1 contains 3
	B1 contains the formula A1 * A1
	C1 contains the formula B1 + A1
	D1 contains the formula B1 - C1
	The direct dependents of A1 are B1 and C1
	*/
	vector<string> GetDirectDependents(string name);
	/*
	Requires that names be non-null. Also requires that if names contains s,
	then s must be a valid non-null cell name.
	
	If any of the named cells are involved in a circular dependency,
	throws a CircularException.
	
	Otherwise, returns an enumeration of the names of all cells whose values must 
	be recalculated, assuming that the contents of each cell named in the names has changed.
	The names are enumerated in the order in which the calculation should be done.
	
	For example, suppose that
	A1 contains 5
	B1 contains 7
	C1 contains the formula A1 + B1
	D1 contains the formula A1 * C1
	E1 contains 15
	
	If A1 and B1 have changed, then A1, B1, C1, and D1 must be recalculated, 
	and they must be recalculated in either the order A1,B1,C1,D1 or B1,A1,C1,D1.
	The method will produce one of those enumerations.
	*/
	vector<string> GetCellsToRecalculate(string name);
	/*
	Creates a saved .txt file in a relative path that contains each non empty
	cell name and the corresponding content to that cell so that when the file
	is opened, the same Spreadsheet can be build with it when using Open.
	*/
	void Save(string filename);
	/*
	Looks for a .txt file in a relative path with the file name. Will then read
	the file's contents and builds a new Spreadsheet.
	*/
	void Open(string filename);

	private:
	
	/*
	Helper function for GetCellsToRecalculate that does most all of the actual
	computation required to return the correct set of cell names.
	*/
	vector<string> GetCellsToRecalculate(vector<string> names);
	/*
	Helper function for the GetCellsToRecalculat that keeps track of which cells
	have been visited already.
	*/
	void Visit(string start, string name, vector<string> visited, vector<string> changed);
};