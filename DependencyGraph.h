#include <vector>
#include <string>

using namespace std;

class DependencyGraph
{
	public:
	DependencyGraph();
	~DependencyGraph();
	
	vector<string> GetDependents(string);

	private:
};