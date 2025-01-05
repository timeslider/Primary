using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primary_Puzzle_Solver
{
    [Flags]
    public enum TileType
    {
        None = 0,
        HeroRed = 1,
        HeroYellow = 2,
        HeroBlue = 4,
        Wall = 8
    }

    public enum MoveDirection
    {
        //None,
        U,
        D,
        L,
        R
    }
}
