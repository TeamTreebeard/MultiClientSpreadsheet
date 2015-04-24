#ifndef DEFINE_SPREADSHEET_H
#define DEFINE_SPREADSHEET_H

#include <string>
#include <vector>
#include <map>
#include <stack>
#include "DependencyGraph.h"
#include "user.h"

using namespace std;

class Spreadsheet
{
  // Struct containing a cell name and contents
  struct cell{
    string name;
    string content;
  };

	public:

	/*
	Default constructor
	*/
	Spreadsheet();
	/*
	Normal constructor, uses filename to open an existing spreadsheet.
	If there is no existing spreadsheet with that filename, will create
	a new spreadsheet using that filename.
	*/
	Spreadsheet(string filename);
	/*
	Default destructor
	*/
	~Spreadsheet();
	
	//copy constructor
	Spreadsheet(const Spreadsheet & other);
	
	//
	const Spreadsheet& operator= (const Spreadsheet & rhs);
	
		/*
	If the name is null or invalid, throws an InvalidNameException.
	Otherwise, returns the value (as opposed to the contents) of the
	named cell.
	*/
	string GetCellValue(string name);

	/* Undoes the last change made to the spreadhsheet and sends it
           to the users
	*/
	string undo();

	/*
	  Adds a new user to the spreadsheet
	 */
	void addUser(user newUser);

	// Removes the specified user from the spreadsheet
	void removeUser(int socket);

	// 
	map<string, string> getSheet();

	// Returns the name of the spreadsheet
	string getName();
	
	//returns vector of users
	vector<int> getSocketList();

	// Returns true or false if the user is already in the spreadsheet
	bool containsUser(int ID);

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
	bool SetContentsOfCell(string name, string content, bool isUndo);

	/*
	Creates a saved .txt file in a relative path that contains each non empty
	cell name and the corresponding content to that cell so that when the file
	is opened, the same Spreadsheet can be build with it when using Open.
	*/
	void Save();
	/*
	Looks for a .txt file in a relative path with the file name. Will then read
	the file's contents and builds a new Spreadsheet.
	*/
	map<string,string>& Open(string filename);

	private:
	/*
	Keeps track of the cells another cell is dependent on. Necessary when updating
	a Formula to see which of the other cells are also changing.
	*/
	DependencyGraph graph;
	/*
	Keeps track of non-empty cells, key is name, value is contents
	*/
	map<string, string> sheet;
	
	// The name of the spreadsheet
	string ss_name;
	
	// Holds all the user changes in the spreadsheet
	std::stack<cell> undoList; 

	// Holds all the users in the spreadsheet
	std::vector<user> userList;

	// Normalizes the string provided 
	string normalize(string content);

	// Gets all the varibles in the given formula
	vector<string> getVariables(string content);
	
	private:
	
	void CircularCheck(string name);
	void GetAllDependents(string name, vector<string>& cell_list);
};



#endif
