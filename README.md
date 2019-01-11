# Voyager
Solver for discreete value optimization problem (Traveling Salesman Problem). Uses "Branch and Bound" (BnB) method.
___________________________________________________________________________________________________________________________________
Use: 
- Solver.BranchAndBound() demostrates algorithm on (pseudo?-)randomly generated matrix, 
- Solver.BranchAndBound(List<List<int>> transportMatrix) solves problem for particular transport matrix.
 ___________________________________________________________________________________________________________________________________
Description of algorithm:
    
  1. Find upper and lower bound in branch (or at the start or the tree of solution set); Discard this branch if it contains no 
     solution, or lower bound is worse than record solution cost;
    
  2. If there is no record - set new record solution;
  
  3. If no cities visited, try to add lowest "cost" edge for the transport graph (defined by transport matrix);
  
  4. Create two branches if no optimal solution has been found; One branch includes edge x, the second - excludes x (this divides solution subset in two parts, so the algorithm is kinda binary search). The new edge is chosen greedy way too. Else return the solution;
  
  5. Repeat in each created branch 1-4;
  
  At each iteration method uses a greedy algorithm to find solution to use it as Upper Bound of optimal solution: "go by the
  shortest way possible to the next city, until you solve problem (TSP)".
  
  Lower Bound is being found as MIN {
                                     SUM[rows](row -> minimal element in each row), 
                                     SUM[columns](column -> minimal subtraction of element and minimal element in row)
                                    }.
  
  
