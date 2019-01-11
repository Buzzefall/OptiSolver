# Voyager
Solver for discreete value optimization problem (Traveling Salesman Problem). Uses "Branch and Bound" (BnB) method.
___________________________________________________________________________________________________________________________________
Use: 
    - Solver.BranchAndBound() demostrates algorithm on (pseudo?-)randomly generated matrix, 
    - Solver.BranchAndBound(List<List<int>> transportMatrix) solves problem for particular transport matrix.
 ___________________________________________________________________________________________________________________________________
Description:
  
  Find upper and lower bound in branch (or at the start or the tree of solution set);
  
  If no cities visited, try to add lower "cost" edge for the transport graph (defined by transport matrix);
  
  Branch if no optimal solution has been found;
  
  Repeat.
  
  At each iteration method uses a greedy algorithm to find solution to use it as Upper Bound of optimal solution: "go by the
  shortest way possible to the next city, until you solve problem (TSP)".
  
  Lower Bound is being found as MIN {
                                     SUM[rows](row -> minimal element in each row), 
                                     SUM[columns](column -> minimal subtraction of element and minimal element in row)
                                    }.
  
  
