using System.Diagnostics;
using System.Text;

namespace FiveLetters
{
    internal class Program
    {
        static List<string> LoadWords(string filename)
        {
            HashSet<string> words = [];
            bool noDuplicates = true;
            using (StreamReader reader = new(filename, Encoding.UTF8))
            {
                string? word;
                while ((word = reader.ReadLine()) != null)
                {
                    noDuplicates &= words.Add(word);
                }
            }

            if (!noDuplicates) {
                Console.WriteLine("The dictionary contains duplicates.");
            }

            return [.. words.Order()];
        }

        static int GetMatchWordCount(List<Word> words, Word word, Word guess, 
            int currentApplicableWordCount, int observedMinApplicableWordCount) {
            int applicableWordsCount = currentApplicableWordCount;
            State state = new(word, guess);
            foreach (Word wordToCheck in words)
            {
                if (state.MatchWord(wordToCheck))
                {
                    ++applicableWordsCount;
                    if (applicableWordsCount > observedMinApplicableWordCount)
                    {
                        return applicableWordsCount;
                    }
                }
            }
            return applicableWordsCount;
        }

        static List<Word> GetCandidates(List<Word> words, List<Word> globalWords)
        {
            if (words.Count == 1) {
                return words;
            }

            int minApplicableWordsCount = words.Count * words.Count;
            List<Word> candidatesMin = [];
            foreach (Word guess in globalWords)
            {
                int applicableWordsCount = 0;
                foreach (Word word in words)
                {
                    applicableWordsCount = GetMatchWordCount(words, word, guess, 
                        applicableWordsCount, minApplicableWordsCount);
                    if (applicableWordsCount > minApplicableWordsCount)
                    {
                        break;
                    }
                }
                if (applicableWordsCount < minApplicableWordsCount)
                {
                    candidatesMin.Clear();
                    minApplicableWordsCount = applicableWordsCount;
                }

                if (minApplicableWordsCount == applicableWordsCount)
                {
                    candidatesMin.Add(guess);
                }
            }

            return candidatesMin;
        }

        static void GetMetric(List<Word> words, Word guess)
        {
            int applicableWordsCount = 0;
            foreach (Word word in words)
            {
                applicableWordsCount = GetMatchWordCount(words, word, guess,
                    applicableWordsCount, words.Count * words.Count);
            }
            Console.WriteLine("Word: {0}, Metric {1}. Mean number of words: {2}.", guess,
                applicableWordsCount, Math.Round(applicableWordsCount / (double)words.Count));
        }

        static List<Word> FilterWords(List<Word> words, State state)
        {
            List<Word> result = [];
            foreach (Word word in words)
            {
                if (state.MatchWord(word))
                {
                    result.Add(word);
                }
            }
            return result;
        }

        static int HiddenWordGame(int index, List<Word> globalWords, Word firstGuess)
        {
            List<Word> words = globalWords;
            Word hiddenWord = words[index];
            Word guess = firstGuess;
            int attemptCount = 1;
            while (guess != hiddenWord)
            {
                State currentState = new(hiddenWord, guess);
                words = FilterWords(words, currentState);
                List<Word> candidates = GetCandidates(words, globalWords);
                guess = candidates[0];
                ++attemptCount;
            }
            return attemptCount;
        }

        static void GetFirstCandidate(List<Word> words)
        {
            Console.WriteLine("Getting first candidate...");
            Stopwatch stopwatch = Stopwatch.StartNew();
            List<Word> candidates = GetCandidates(words, words);
            stopwatch.Stop();
            Console.WriteLine("Candidates: {0}.", string.Join(", ", candidates));
            Console.WriteLine("Time Elapsed: {0}.", stopwatch.Elapsed);
        }

        static void CollectStats(List<Word> words, Word firstGuess)
        {
            Console.WriteLine("Collecting stats...");
            Stopwatch stopwatch = Stopwatch.StartNew();
            int maxAttempts = 0;
            List<Word> fails = [];
            for (int i = 0; i < words.Count; ++i)
            {
                int attempt_count = HiddenWordGame(i, words, firstGuess);
                if (attempt_count > 6)
                {
                    fails.Add(words[i]);
                }
                if (attempt_count > maxAttempts)
                {
                    maxAttempts = attempt_count;
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Fail count: {0}, Max attempts: {1}.", fails.Count, maxAttempts);
            if (fails.Count > 0) {
                Console.WriteLine("Fail words: {0}.", string.Join(", ", fails));
            }
            Console.WriteLine("Time Elapsed: {0}.", stopwatch.Elapsed);
        }

        static State? GetMask(Word guess)
        {
            do
            {
                Console.Write("Enter state (e.g gwwwh, g - not present, w - wrong place, " + 
                    "y - correct place; yyyyy - to exit): ");
                string enteredValue = Console.ReadLine() ?? "";
                Console.WriteLine();
                if (enteredValue == "yyyyy")
                {
                    return null;
                }
                try
                {
                    return new(enteredValue, guess);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Input `{0}` is incorrect, please repeat.", enteredValue);
                }
            } while (true);
        }

        static void PlayInteractiveGame(List<Word> globalWords, Word firstGuess) {
            Word guess = firstGuess;
            int attempt = 0;
            List<Word> words = globalWords;
            do
            {
                ++attempt;
                Console.WriteLine("There are {0} words left.", words.Count);
                if (words.Count < 10) {
                    Console.WriteLine("Words left: {0}.", string.Join(", ", words));
                }
                Console.Write("Attempt: {0}, Guess: ", attempt);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(guess);
                Console.ResetColor();
                Console.WriteLine(".");
                State? state = GetMask(guess);
                if (state == null)
                {
                    break;
                }
                words = FilterWords(words, state.Value);
                if (words.Count <= 0) {
                    Console.WriteLine("No words left. It means that one of the previous " + 
                        "mask was entered incorrectly.");
                    break;
                }
                List<Word> candidates = GetCandidates(words, globalWords);
                if (candidates.Count <= 0)
                {
                    Console.WriteLine("No candidates left. It means that one of the previous " + 
                        "mask was entered incorrectly.");
                    break;
                }
                guess = candidates[0];
            } while (attempt < 6);
        }

        static string Contains(List<string> words, string word) {
            if (words.BinarySearch(word) < 0) {
                Console.WriteLine("The dictionary doesn't contain '{0}'", word);
                ShowHelpAndTerminate();
            }
            return word;
        }

        static void ShowHelpAndTerminate() {
            string exeName = AppDomain.CurrentDomain.FriendlyName;                 
            Console.WriteLine("Three way of usage: ");
            Console.WriteLine();
            Console.WriteLine("\t$ {0} first /path/to/dictionary", exeName);
            Console.WriteLine("\t\tComputes the best initial suggestion(s).");
            Console.WriteLine();
            Console.WriteLine("\t$ {0} stats /path/to/dictionary suggest", exeName);
            Console.WriteLine("\t\tCollects and shows stats.");
            Console.WriteLine();
            Console.WriteLine("\t$ {0} interactive /path/to/dictionary suggest", exeName);
            Console.WriteLine("\t\tStarts interactive mode to play the game.");
            Console.WriteLine();
            Console.WriteLine("\t$ {0} metric /path/to/dictionary suggest", exeName);
            Console.WriteLine("\t\tComputes the metric.");
            Console.WriteLine();
            Console.WriteLine("The '/path/to/dictionary' is path to a file that contains"); 
            Console.WriteLine("russian words.");
            Console.WriteLine("Each line of the file represents a single 5 letter russian word.");
            Console.WriteLine("All the letters must be in lowercase.");
            Console.WriteLine("The letter 'ё' must be replaced with 'е'.");
            Console.WriteLine("Duplicates are allowed but will be ignored.");
            Console.WriteLine("The dictionary must not be empty.");
            Console.WriteLine("The codepage must be UTF-8.");
            Console.WriteLine();
            Console.WriteLine("The 'suggest' is a 5 letter russian word that starts the game.");
            Console.WriteLine("The dictionary must contain the suggest.");
            Environment.Exit(1);
        }

        static void Main(string[] args)
        {       
            Console.OutputEncoding = Encoding.UTF8;
            if (args.Length < 2 || args[0] != "first" && args.Length < 3) {
                ShowHelpAndTerminate();
            }

            List<string> allWords = LoadWords(args[1]);
            List<Word> fiveLetterWords = allWords.Select(word => new Word(word)).ToList();
            Console.WriteLine(string.Format("Loaded {0} words.", fiveLetterWords.Count));
            if (fiveLetterWords.Count <= 0) {
                Console.WriteLine("The dictionary doesn't contain words.");
                Environment.Exit(1);
            }

            switch (args[0]) {
                case "first":
                    GetFirstCandidate(fiveLetterWords);
                    break;
                case "stats":
                    CollectStats(fiveLetterWords, new Word(Contains(allWords, args[2])));
                    break;
                case "interactive":
                    PlayInteractiveGame(fiveLetterWords, new Word(Contains(allWords, args[2])));
                    break;
                case "metric":
                    GetMetric(fiveLetterWords, new Word(Contains(allWords, args[2])));
                    break;
                default:
                    ShowHelpAndTerminate();
                    break;
            }
        }
    }
}