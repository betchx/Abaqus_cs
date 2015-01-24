using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    internal class GenerateEnumerator : IEnumerator<uint>
    {
        uint start, last, step, current;
        enum State
        {
            Init,
            Loop,
            End,
        }
        State state;
        public GenerateEnumerator(uint start, uint last, uint step)
        {
            if (start > last) throw new ArgumentException("the 'start' must not be grator than the 'last'");
            this.start = start;
            this.last = last;
            this.step = step;
            Reset();
        }
        public uint Current {
            get {
                switch (state)
                {
                    case State.Init:
                        throw new InvalidOperationException("列挙は開始していません．");
                    case State.Loop:
                        return current;
                    case State.End:
                        throw new InvalidOperationException("列挙はすでに終了しています");
                    default:
                        throw new InvalidProgramException();
                }
            }
        }
        public void Dispose() { } // Do nothing
        public void Reset() {
            state = State.Init;
        }
        object System.Collections.IEnumerator.Current { get { return Current; } }

        public bool MoveNext()
        {
            switch (state)
            {
                case State.Init:
                    current = start;
                    state = State.Loop;
                    return true;
                case State.Loop:
                    current += step;
                    if (current <= last) return true;

                    // Loop is over.  change state to end.
                    state = State.End;
                    break;
                case State.End:
                    break;
                default:
                    throw new InvalidProgramException();
            }
            return false;
        }
    }
}
