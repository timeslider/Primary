
public class PolyominoChecker
{
    private Dictionary<string, int> stateNumbers = new Dictionary<string, int>();
    private List<int> transitions = new List<int>();
    private const int FAILSTATE = -1;

    // Disjoint Set operations remain the same
    private int Find(int[] sets, int s)
    {
        int parent = sets[s];
        if (parent > 0)
        {
            return s;
        }
        else if (parent < 0)
        {
            parent = Find(sets, ~parent);
            sets[s] = ~parent;
            return parent;
        }
        else
        {
            sets[s] = 1;
            return s;
        }
    }

    private bool Union(int[] sets, int x, int y)
    {
        x = Find(sets, x);
        y = Find(sets, y);
        if (x == y) return false;

        int szx = sets[x];
        int szy = sets[y];
        if (szx < szy)
        {
            sets[y] += szx;
            sets[x] = ~y;
        }
        else
        {
            sets[x] += szy;
            sets[y] = ~x;
        }
        return true;
    }

    public bool IsPolyomino(ulong bitboard)
    {
        int state = stateNumbers["00000000"]; // Start with empty state

        // Process each row of the bitboard
        for (int row = 0; row < 8; row++)
        {
            byte rowBits = (byte)((bitboard >> (row * 8)) & 0xFF);
            int nextState = transitions[state * 256 + row];

            state = nextState;
        }

        return CheckFinalState(state);
    }

    private bool CheckFinalState(int state)
    {
        if (state == FAILSTATE) return false;

        // Get the state code for this state number
        string finalStateCode = stateNumbers.FirstOrDefault(x => x.Value == state).Key;
        if (finalStateCode == null) return false;

        // Count unique non-zero digits (excluding '0')
        return finalStateCode.Where(c => c != '0').Select(c => c).Distinct().Count() == 1;
    }

    public PolyominoChecker()
    {
        // Initialize with empty state
        int startState = ExpandState("00000000", 255);
    }

    private int ExpandState(string stateCode, int nextrow)
    {
        // convert the next row into a disjoint set array
        int[] sets = new int[8];
        for (int i = 0; i < 8; ++i)
        {
            sets[i] = (~nextrow >> i) & 1;
        }
        for (int i = 0; i < 7; ++i)
        {
            if (((~nextrow >> i) & 3) == 3)
            {
                Union(sets, i, i + 1);
            }
        }

        // map from state code island to first attached set in sets
        int[] links = new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };
        for (int i = 0; i < 8; ++i)
        {
            char digit = stateCode[i];
            if (sets[i] > 0 && digit > '0')
            {
                int topisland = digit - '1';
                int bottomSet = links[topisland];
                if (bottomSet < 0)
                {
                    links[topisland] = i;
                }
                else
                {
                    Union(sets, bottomSet, i);
                }
            }
        }

        // fail if there are disconnected islands in the previous row
        for (int i = 0; i < 8; ++i)
        {
            char digit = stateCode[i];
            if (digit > '0' && links[digit - '1'] < 0)
            {
                return FAILSTATE;
            }
        }

        char nextSet = '1';
        char[] newChars = "00000000".ToCharArray();

        // Reset links for new state
        for (int i = 0; i < 8; ++i)
        {
            links[i] = -1;
        }

        // Create new state code
        for (int i = 0; i < 8; ++i)
        {
            if (sets[i] != 0)
            {
                int set = Find(sets, i);
                int link = links[set];
                if (link >= 0)
                {
                    newChars[i] = newChars[link];
                }
                else
                {
                    newChars[i] = nextSet++;
                    links[set] = i;
                }
            }
        }

        string newStateCode = new string(newChars);

        // Get or create state number
        if (stateNumbers.ContainsKey(newStateCode))
        {
            return stateNumbers[newStateCode];
        }

        int newState = stateNumbers.Count;
        stateNumbers.Add(newStateCode, newState);

        // Fill transition table
        while (transitions.Count <= (newState + 1) * 256)
        {
            transitions.Add(FAILSTATE);
        }

        for (int i = 0; i < 256; ++i)
        {
            transitions[newState * 256 + i] = ExpandState(newStateCode, i);
        }

        return newState;
    }
}