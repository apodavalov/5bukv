namespace FiveLetters
{
    readonly struct Letter : IEquatable<Letter>
    {
        internal Letter(char value)
        {
            if (value < 'а' || value > 'я')
            {
                throw new ArgumentException(string.Format(
                    "The character `{0}` is not a Russian lower case letter.", value));
            }
            Value = value - 'а';
        }

        internal readonly char ToChar() => (char)(Value + 'а');

        public readonly bool Equals(Letter other) => Value == other.Value;        

        public static bool operator ==(Letter left, Letter right) => left.Equals(right);

        public static bool operator !=(Letter left, Letter right) => !left.Equals(right);

        public static implicit operator int(Letter letter) => letter.Value;

        internal int Value { get; init; }

        public override readonly bool Equals(object? obj) => obj is Letter letter && Equals(letter);

        public override readonly int GetHashCode() => Value.GetHashCode();
    }
}