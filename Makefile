all: run

run: server.cpp Spreadsheet.cpp user.cpp DependancyGraph.cpp
	g++ -o server server.cpp Spreadsheet.cpp user.cpp DependencyGraph.cpp -pthread
