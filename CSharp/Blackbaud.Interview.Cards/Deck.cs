namespace Blackbaud.Interview.Cards;

/// <summary>
/// A deck of cards
/// </summary>
public class Deck : IDeck
{
    private readonly Stack<Card> _stackOfCards;

    /// <summary>
    /// Private constructor for a new deck of <paramref name="cards"/>.
    /// Use Deck.NewDeck() static factory method.
    /// </summary>
    /// <param name="cards"></param>
    private Deck(IEnumerable<Card> cards)
    {
        _stackOfCards = new Stack<Card>(cards);
    }

    /// <summary>
    /// Creates and returns a new deck of cards.
    /// </summary>
    /// <returns></returns>
    public static Deck NewDeck()
    {
        return new Deck(AllCards());
    }

    /// <summary>
    /// Returns an enumerable of every card in the canonical deck order.
    /// </summary>
    private static IEnumerable<Card> AllCards()
    {
        return Enum.GetValues<Suit>().SelectMany(suit =>
            Enum.GetValues<Rank>().Select(rank =>
                new Card(rank, suit)));
    }

    /// <summary>
    /// The number of remaining cards in the deck
    /// </summary>
    public int RemainingCards => _stackOfCards.Count;

    /// <summary>
    /// Returns true if there are no remaining cards in the deck
    /// </summary>
    public bool Empty => RemainingCards == 0;

    /// <summary>
    /// Removes the next card from the deck.
    /// </summary>
    /// <returns>The next card from the deck.
    /// Returns null if no cards remain.</returns>
    public Card NextCard()
    {
        if (!Empty)
        {
            var nextCard = _stackOfCards.Pop();
            return nextCard;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Draws up to <paramref name="count"/> cards from the top of the deck.
    /// If fewer than <paramref name="count"/> cards remain, returns all remaining cards.
    /// Returns an empty list if <paramref name="count"/> is less than or equal to zero.
    /// </summary>
    /// <param name="count">Number of cards to draw.</param>
    /// <returns>List of drawn cards in draw order (first drawn first in list).</returns>
    public IReadOnlyList<Card> Draw(int count)
    {
        if (count <= 0)
            return Array.Empty<Card>();

        var drawn = new List<Card>(Math.Min(count, _stackOfCards.Count));
        for (int i = 0; i < count && _stackOfCards.Count > 0; i++)
        {
            drawn.Add(_stackOfCards.Pop());
        }

        return drawn;
    }

    /// <summary>
    /// Removes and returns all remaining cards from the deck in draw order.
    /// </summary>
    public IReadOnlyList<Card> TakeAll()
    {
        var list = new List<Card>(_stackOfCards.Count);
        while (_stackOfCards.Count > 0)
        {
            list.Add(_stackOfCards.Pop());
        }

        return list;
    }

    /// <summary>
    /// Resets the deck to a full, ordered deck (same order as produced by <see cref="NewDeck"/>).
    /// Any remaining cards are removed and the deck is refilled.
    /// </summary>
    public void Reset()
    {
        _stackOfCards.Clear();
        foreach (var card in AllCards())
            _stackOfCards.Push(card);
    }

    /// <summary>
    /// Sorts the remaining cards in the deck using the provided comparer.
    /// Default comparer sorts by Rank then Suit (ascending).
    /// After sorting the top of the stack will be the last element in the sorted order.
    /// </summary>
    public void Sort(IComparer<Card>? comparer = null)
    {
        var cards = _stackOfCards.ToList();
        comparer ??= Comparer<Card>.Create((a, b) =>
        {
            var rankComparison = a.Rank.CompareTo(b.Rank);
            if (rankComparison != 0)
                return rankComparison;

            return a.Suit.CompareTo(b.Suit);
        });

        cards.Sort(comparer);
        _stackOfCards.Clear();
        foreach (var card in cards)
            _stackOfCards.Push(card);
    }

    /// <summary>
    /// Convenience: sort by rank then suit.
    /// </summary>
    public void SortByRankThenSuit() => Sort();

    /// <summary>
    /// Prints all remaining cards to the console, removing them from the deck.
    /// Uses the same output format as the previous inline loops: "{short} - {full}".
    /// Note: presentation method retained for backward compatibility; Program should prefer TakeAll().
    /// </summary>
    public void PrintAllCards()
    {
        while (!Empty)
        {
            var card = NextCard();
            if (card is not null)
            {
                Console.WriteLine($"{card.ToShortString()} - {card}");
            }
        }
    }

    // C#
    public void Shuffle(int times)
    {
        var cards = _stackOfCards.ToList();
        var rng = new Random();
        for (int t = 0; t < times; t++)
        {
            Console.WriteLine($"Shuffling {times}...");
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
        }
        _stackOfCards.Clear();
        foreach (var card in cards)
            _stackOfCards.Push(card);
    }

}
