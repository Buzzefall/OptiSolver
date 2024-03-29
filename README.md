# OptiSolver
Solver for discrete value optimization problem (Traveling Salesman Problem). Uses "Branch and Bound" (BnB) method.
Probably, will add some more problems and algorithms in the future. 

## Use 

- **Solver.BranchAndBound()** demostrates algorithm on (pseudo?-)randomly generated matrix, 
- **Solver.BranchAndBound(int[][] transportMatrix)** solves the problem for given transport matrix. Program will read from Cities.txt the Input folder (~/Input/Cities.txt, where ~ is the folder with program executable). If file is empty, that will trigger random matrix generation mode (for demonstration or test).

## Algorithm description

Greedy algorithm with branch-and-bound heuristic. At worst is it just exhaustive search, but frequently in practice is quite effective.
  
  1. Find **Upper Bound (UB)** and **lower Bound (LB)** in branch (or at the start or the tree of solution set); 
  **If** this branch contains no solution, or **LB** is more than record solution cost (lowest **UB** found) - discard this branch and return same record solution.
  
  2. **If** there is no **record solution** at all - set new **record solution**; 
  **If** **UB** < **record cost** this branch gives the **new record cost := UB**. Remember **new record solution**, which gives this **UB**.
  
  3. **If** **LB** == **UB** => Discard this branch, as we found best possible solution in branch (locally optimal); **Return** the current record solution;
 **Else** continue branching.
  
  4. **If** no cities visited, try to add lowest "cost" edge for the transport graph (defined by transport matrix); 
  **Else** create two branches if no optimal solution has been found; One branch includes edge **X**, the second - excludes **X** (this divides solution set in two subsets, so the algorithm is kinda binary search). The new edge is chosen greedy way too.
  
  5. Repeat in each branch steps 1-4 (depth-first left-to-right traversal).
  
  At each iteration method uses a greedy algorithm to find solution to use it as Upper Bound of optimal solution: "go by the
  shortest way possible to the next city, until you solve problem (TSP)" (since lowest cost solution is optimal, any other will do as Upper Bound).
  
  
  Lower Bound is being found as 
  MIN {
                                     SUM[rows](row -> minimal element in each row), 
                                     SUM[columns](column -> minimal subtraction of element and minimal element in row)
      }
  
  
