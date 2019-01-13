# OptiSolver
Solver for discrete value optimization problem (Traveling Salesman Problem). Uses "Branch and Bound" (BnB) method.
Probably, will add some more problems and algorithms in the future. 

## Use: 

- **Solver.BranchAndBound()** demostrates algorithm on (pseudo?-)randomly generated matrix, 
- **Solver.BranchAndBound(int[][] transportMatrix)** solves problem for particular transport matrix. Program will read from Cities.txt in the Input folder which is near .exe file of the program.

## Description of algorithm:
  
  1. *Find upper and lower bound in branch (or at the start or the tree of solution set); **If** this branch contains no solution, or Lower Bound is less than **record solution cost** - discard this branch;*
  
  2. **If** *there is no record solution at all  - set **new record solution**. **If** there is a new record - remember **new record solution**;*
  **If** *Lower Bound == Upper Bound => discard this branch, as we found best possible solution in it (locally optimal); **Return** the current record solution;
 **Else** continue;*
  
  3. ***If** no cities visited, try to add lowest "cost" edge for the transport graph (defined by transport matrix); 
  **Else** create two branches if no optimal solution has been found; One branch includes edge x, the second - excludes x (this divides solution subset in two parts, so the algorithm is kinda binary search). The new edge is chosen greedy way too;*
  
  5. *Repeat in each branch steps 1-4 by recursive call of the function first for the left branch, then for the right.*
  
  At each iteration method uses a greedy algorithm to find solution to use it as Upper Bound of optimal solution: "go by the
  shortest way possible to the next city, until you solve problem (TSP)".
  
  Lower Bound is being found as 
  MIN {
                                     SUM[rows](row -> minimal element in each row), 
                                     SUM[columns](column -> minimal subtraction of element and minimal element in row)
      }
  
  
