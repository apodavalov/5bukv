namespace FiveLetters
{
    enum LetterState
    {
        Unknown,
        Absense,
        Presense
    }

    readonly record struct PositionState {
        internal required Letter Letter { get; init; }
        internal required bool Interpretation { get; init; }
        internal bool MatchLetter(Letter letter) => letter == Letter == Interpretation;
    }

    struct State
    {
        private LetterState[] _LetterStates;

        private PositionState?[] _PositionStates;

        internal State(Word word, Word guess)
        {
            _LetterStates = new LetterState[Word.AlphabetLetterCount];
            _PositionStates = new PositionState?[Word.WordLetterCount];

            foreach (Letter letter in guess)
            {
                _LetterStates[letter] = LetterState.Absense;
            }

            foreach (Letter letter in word)
            {
                if (_LetterStates[letter] == LetterState.Absense)
                {
                    _LetterStates[letter] = LetterState.Presense;
                }
            }

            for (int i = 0; i < Word.WordLetterCount; ++i)
            {
                if (word[i] == guess[i])
                {
                    _PositionStates[i] = new PositionState { Letter = guess[i], Interpretation = true };
                }
                else if (_LetterStates[guess[i]] == LetterState.Presense)
                {
                    _PositionStates[i] = new PositionState { Letter = guess[i], Interpretation = false };
                }
            }
        }

        internal readonly bool MatchWord(Word word)
        {
            bool[] metChars = new bool[Word.AlphabetLetterCount];
            int countPresence = 0;
            for (int i = 0; i < Word.WordLetterCount; ++i)
            {
                Letter letter = word[i];
                if (_LetterStates[letter] == LetterState.Absense)
                {
                    return false;
                }
                if (!(_PositionStates[i]?.MatchLetter(letter) ?? true)) {
                    return false;
                }
                if (!metChars[letter] && _LetterStates[letter] == LetterState.Presense)
                {
                    metChars[letter] = true;
                    ++countPresence;
                }
            }

            int stateCountPresence = 0;
            for (int i = 0; i < Word.AlphabetLetterCount; ++i)
            {
                if (_LetterStates[i] == LetterState.Presense)
                {
                    ++stateCountPresence;
                }
            }
            return stateCountPresence <= countPresence;
        }

        internal State(string value, Word guess)
        {
            if (value.Length != Word.WordLetterCount)
            {
                throw new ArgumentException(string.Format(
                    "The mask must contain exactly {0} characters.", Word.WordLetterCount));
            }

            _LetterStates = new LetterState[Word.AlphabetLetterCount];
            _PositionStates = new PositionState?[Word.WordLetterCount];

            for (int i = 0; i < Word.WordLetterCount; ++i)
            {
                Letter letter = guess[i];
                switch (value[i])
                {
                    case 'g':
                        // This is the case when game actually reveals more info than
                        // this model. We pretend that we received 'w' here and implement
                        // the same behavior. It makes the model less restrictive.
                        //
                        // Example: the hidden word is 'канон'.
                        //   1. 'катер' ('yyggg')
                        //   2. 'калан' ('yyggy', but the model yields 'yygwy')
                        //   3. 'камин' (yyggy)
                        //   4. 'канон' (yyyyy)
                        if (_LetterStates[letter] != LetterState.Presense)
                        {
                            _LetterStates[letter] = LetterState.Absense;
                        }
                        _PositionStates[i] =  new PositionState { Letter = letter, Interpretation = false};
                        break;
                    case 'w':
                        _LetterStates[letter] = LetterState.Presense;
                        _PositionStates[i] =  new PositionState { Letter = letter, Interpretation = false};
                        break;
                    case 'y':
                        _LetterStates[letter] = LetterState.Presense;
                        _PositionStates[i] =  new PositionState { Letter = letter, Interpretation = true };
                        break;
                    default:
                        throw new ArgumentException(string.Format(
                            "Value `{0}` contains at least one inacceptable " + 
                            "character. Expecting only the following characters: " + 
                            "`g`, `w`, `y`.", value));
                }
            }
        }
    }
}