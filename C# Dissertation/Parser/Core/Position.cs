namespace AlexNoddings.Protocols.Parser.Core
{
    internal struct Position
    {
        internal int Offset { get; }
        internal int Line { get; }
        internal int Column { get; }

        public Position(int offset, int line, int column)
        {
            Offset = offset;
            Line = line;
            Column = column;
        }
    }
}
