#include "DependencyGraph.h"

using namespace std;

Node::Node()
{
	
}
Node::~Node()
{
	
}
void Node::AddDependents(string s)
{
	dependents.push_back(s);
}
void Node::AddDependees(string s)
{
	dependees.push_back(s);
}
vector<string> Node::GetDependents()
{
	return dependents;
}
vector<string> Node::GetDependees()
{
	return dependees;
}
void Node::RemoveDependents(string s)
{
	for(int i = 0; i < dependents.size(); i++)
	{
		if(dependents[i] == s)
		{
			dependents.erase(dependents.begin()+i);
			break;
		}
	}
}
void Node::RemoveDependees(string s)
{
	for(int i = 0; i < dependees.size(); i++)
	{
		if(dependees[i] == s)
		{
			dependees.erase(dependees.begin()+i);
			break;
		}
	}
}
bool Node::HasDependents()
{
	if(dependents.size() > 0)
	{
		return true;
	}
	else
	{
		return false;
	}
}
bool Node::HasDependees()
{
	if(dependees.size() > 0)
	{
		return true;
	}
	else
	{
		return false;
	}
}

DependencyGraph::DependencyGraph()
{
	Size = 0;
}
DependencyGraph::~DependencyGraph()
{
	
}
int DependencyGraph::get_Size()
{
	return Size;
}
bool DependencyGraph::HasDependents(string s)
{
	bool contains_key = false;
	for(map<string,Node>::iterator it = lookup.begin(); it != lookup.end(); it++)
	{
		if((it->first) == s)
		{
			contains_key = true;
		}
	}
	
	if(contains_key)
	{
		node = lookup[s];
		return node.HasDependents();
	}
	else
	{
		return false;
	}
}
bool DependencyGraph::HasDependees(string s)
{
	bool contains_key = false;
	for(map<string,Node>::iterator it = lookup.begin(); it != lookup.end(); it++)
	{
		if((it->first) == s)
		{
			contains_key = true;
		}
	}
	
	if(contains_key)
	{
		node = lookup[s];
		return node.HasDependees();
	}
	else
	{
		return false;
	}
}
vector<string> DependencyGraph::GetDependents(string s)
{
	
}
vector<string> DependencyGraph::GetDependees(string s)
{
	
}
void DependencyGraph::AddDependency(string s, string t)
{
	
}
void DependencyGraph::RemoveDependency(string s, string t)
{
	
}
void DependencyGraph::ReplaceDependents(string s, vector<string> newDependents)
{
	
}
void DependencyGraph::ReplaceDependees(string s, vector<string> newDependees)
{
	
}
