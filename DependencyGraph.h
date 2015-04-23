#ifndef DEPENDENCYGRAPH_H
#define DEPENDENCYGRAPH_H

#include <vector>
#include <string>
#include <map>

using namespace std;

class Node
{
	public:
	Node();
	~Node();
  
	//copy constructor
	Node(const Node & other);
	//operator= required to be implemented by copy constructor
	const Node& operator= (const Node & rhs);
	
	void AddDependents(string s);
	void AddDependees(string s);
	vector<string> GetDependents();
	vector<string> GetDependees();
	void RemoveDependents(string s);
	void RemoveDependees(string s);
	bool HasDependents();
	bool HasDependees();
	
	private:
	vector<string> dependents;
	vector<string> dependees;
};

class DependencyGraph
{
	public:
	DependencyGraph();
	~DependencyGraph();
	//copy constructor
	DependencyGraph(const DependencyGraph & other);
	//operator= required to be implemented by copy constructor
	const DependencyGraph& operator= (const DependencyGraph & rhs);
	int get_Size();
	bool HasDependents(string s);
	bool HasDependees(string s);
	vector<string> GetDependents(string s);
	vector<string> GetDependees(string s);
	void AddDependency(string s, string t);
	void RemoveDependency(string s, string t);
	void ReplaceDependents(string s, vector<string> newDependents);
	void ReplaceDependees(string s, vector<string> newDependees);
	
	private:
	
	int Size;
	Node* node;
	map<string,Node> lookup;
	vector<string> pivot;
	bool ContainsKey(string s);
};

#endif