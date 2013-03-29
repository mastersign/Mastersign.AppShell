using System.Collections.Generic;

namespace de.mastersign.shell
{
    public class History
    {
        public int Size { get; set; }

        public IList<string> Items { get; private set; }

        public bool PreventDoubles { get; set; }

        public History()
            : this(20)
        {
        }

        public History(int historySize)
        {
            Size = historySize;
            Items = new List<string>(Size + 1);
            PreventDoubles = true;
        }

        public string this[int index]
        {
            get { return Items[index]; }
        }

        public void Add(string line)
        {
            if (PreventDoubles)
            {
                if (Items.Contains(line))
                {
                    Items.Remove(line);
                }
            }
            Items.Add(line);
            if (Items.Count > Size)
            {
                Items.RemoveAt(0);
            }
        }

        public int Count
        {
            get { return Items.Count; }
        }

        public Navigator GetNavigator()
        {
            return new Navigator(this);
        }

        public class Navigator
        {
            public History NavigatedHistory { get; private set; }

            public int Position { get; set; }

            public Navigator(History navigatedHistory)
            {
                NavigatedHistory = navigatedHistory;
                Reset();
            }

            public void Reset()
            {
                Position = NavigatedHistory.Count;
            }

            public void CompleteOperationWith(string historyItem)
            {
                NavigatedHistory.Add(historyItem);
                Reset();
            }

            public string Current
            {
                get
                {
                    return Position < NavigatedHistory.Count
                               ? NavigatedHistory[Position]
                               : null;
                }
            }

            public bool CanMoveBack
            {
                get { return Position > 0; }
            }

            public bool CanMoveForward
            {
                get { return Position < NavigatedHistory.Count - 1; }
            }

            public string MoveBack()
            {
                if (!CanMoveBack)
                {
                    return Current;
                }
                Position--;
                return Current;
            }

            public string MoveForward()
            {
                if (!CanMoveForward)
                {
                    return Current;
                }
                Position++;
                return Current;
            }
        }
    }
}