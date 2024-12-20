*******************************************************************************************
# Game Of Life Program.
*******************************************************************************************

### GAME OF LIFE RULES:   
The universe of the Game of Life is an infinite, two-dimensional orthogonal grid of square cells, each of which is in one of two possible states, live or dead (or populated and unpopulated, respectively).
Every cell interacts with its eight neighbors, which are the cells that horizontally, vertically, or diagonally adjacent. At each step in time, the following transition occur:
1) Any live cell with fewer than two live neighbors dies, as if by underpopulation. 
2) Any live cell with two or three live neighbors lives on to the next generation. 
3) Any live cell with more than three live neighbors dies, as if by overpopulation. 
4) Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction. 

<br>

<div style="display:flex; justify-content:space-between;">
    <div>
        <img src='./assets/images/GameOfLife_3.gif' alt='GIF 1'/>
    </div>
    <div>
        <img src='./assets/images/GameOfLife_1.gif' alt='GIF 1'/>
    </div>
    <!-- <div>
        <img src='./assets/images/GameOfLife_2.gif' alt='GIF 2'/>
    </div> -->
</div>
        
<br>

### Implemented Command:
- `Left mouse click` &rarr; Select / Make Cell Alive 
- `Right mouse click` &rarr; Erase / Kill Cell
- `Ctrl + Right mouse click + Drag` &rarr; Select Multiple Cell
- `Ctrl + C or X (After Multiple Cell Selection)` &rarr; Copy / Cut Selected Cells
- `Ctrl + V` &rarr; Paste Selected Cells
- `Upload` from file
- `Export` to file


For more Patterns to upload:
- http://www.radicaleye.com/lifepage/picgloss/picgloss.html
- https://conwaylife.appspot.com/pattern/almosymmetric

