using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace Blackbaud.Interview.Cards.Tests;
public class DeckTests
{
    // Verifies that a new deck contains the canonical 52 cards.
    [Fact]
    public void CanCreateANewDeck()
    {
        var deck = Deck.NewDeck();
        Assert.Equal(52, deck.RemainingCards);
    }

    // Arrange: create and empty a deck.
    // Act: call NextCard on an empty deck.
    // Assert: NextCard returns null and the deck reports Empty.
    [Fact]
    public void NextCard_ReturnsNullWhenEmpty()
    {
        var deck = Deck.NewDeck();
        var all = deck.TakeAll();
        Assert.True(deck.Empty);
        var next = deck.NextCard();
        Assert.Null(next);
    }

    // Arrange: create a new deck.
    // Act: draw 5 cards.
    // Assert: exactly 5 cards are returned and remaining count is reduced by 5.
    [Fact]
    public void Draw_ReturnsUpToCountAndReducesRemaining()
    {
        var deck = Deck.NewDeck();
        var drawn = deck.Draw(5);
        Assert.Equal(5, drawn.Count);
        Assert.Equal(47, deck.RemainingCards);
    }

    // Arrange: create new deck.
    // Act: request non-positive counts.
    // Assert: Draw returns empty collections and the deck is unchanged.
    [Fact]
    public void Draw_NonPositiveCount_ReturnsEmpty()
    {
        var deck = Deck.NewDeck();
        var drawnZero = deck.Draw(0);
        var drawnNegative = deck.Draw(-3);
        Assert.Empty(drawnZero);
        Assert.Empty(drawnNegative);
        Assert.Equal(52, deck.RemainingCards);
    }

    // Arrange: create a new deck.
    // Act: TakeAll to remove all cards.
    // Assert: 52 cards returned and the deck is empty afterwards.
    [Fact]
    public void TakeAll_RemovesAllCards()
    {
        var deck = Deck.NewDeck();
        var taken = deck.TakeAll();
        Assert.Equal(52, taken.Count);
        Assert.True(deck.Empty);
    }

    // Arrange: draw a few cards from a fresh deck.
    // Act: call Reset.
    // Assert: deck is restored to full 52 cards.
    [Fact]
    public void Reset_RestoresFullOrderedDeck()
    {
        var deck = Deck.NewDeck();
        deck.Draw(3);
        Assert.True(deck.RemainingCards < 52);
        deck.Reset();
        Assert.Equal(52, deck.RemainingCards);
    }

    // Arrange: disturb deck order with Shuffle so Sort has effect.
    // Act: call Sort (default comparer: Rank then Suit).
    // Assert: remaining cards are ordered ascending by Rank then Suit.
    [Fact]
    public void Sort_SortsCardsByRankThenSuit()
    {
        var deck = Deck.NewDeck();
        // disturb the order first so Sort actually has an effect
        deck.Shuffle(3);

        deck.Sort(); // default sorts by Rank then Suit (ascending)

        var drawn = deck.TakeAll(); // TakeAll returns draw order (top first) which is reverse of push order
        var sequence = drawn.Reverse().ToList(); // reverse to get ascending order

        Assert.True(IsOrderedByRankThenSuitAscending(sequence));
    }

    // Arrange: create two decks, shuffle one slightly and use convenience method.
    // Act: SortByRankThenSuit and Sort on the other deck.
    // Assert: both produce the same ascending sequence.
    [Fact]
    public void SortByRankThenSuit_IsConvenienceForSort()
    {
        var deckA = Deck.NewDeck();
        deckA.Shuffle(1);
        deckA.SortByRankThenSuit();

        var deckB = Deck.NewDeck();
        deckB.Sort();

        var a = deckA.TakeAll().Reverse().ToList();
        var b = deckB.TakeAll().Reverse().ToList();

        // After sorting both should produce the same ascending sequence
        Assert.Equal(b.Select(c => (c.Rank, c.Suit)), a.Select(c => (c.Rank, c.Suit)));
    }

    // Arrange: capture console output.
    // Act: call PrintAllCards which writes lines and empties the deck.
    // Assert: 52 lines were written and the deck is empty afterwards.
    [Fact]
    public void PrintAllCards_PrintsAllLinesAndEmptiesDeck()
    {
        var deck = Deck.NewDeck();
        var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);
        try
        {
            deck.PrintAllCards();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        var output = sw.ToString();
        var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(52, lines.Length);
        Assert.True(deck.Empty);
    }

    // Arrange: create a new deck.
    // Act: Shuffle with zero times (no-op).
    // Assert: deck still has 52 cards.
    [Fact]
    public void Shuffle_WithZeroTimes_PreservesOrderCount()
    {
        var deck = Deck.NewDeck();
        deck.Shuffle(0); // should be a no-op
        Assert.Equal(52, deck.RemainingCards);
    }

    // Arrange: capture the canonical ordered sequence.
    // Act: Shuffle once and read resulting order.
    // Assert: order usually changes (very small chance of equality) and the multiset of cards is preserved.
    [Fact]
    public void Shuffle_OneTime_UsuallyChangesOrderAndPreservesCards()
    {
        var originalOrdered = Deck.NewDeck().TakeAll().Reverse()
            .Select(c => (c.Rank, c.Suit)).ToList();

        var deck = Deck.NewDeck();
        deck.Shuffle(1);

        var shuffled = deck.TakeAll().Reverse()
            .Select(c => (c.Rank, c.Suit)).ToList();

        // Very small probability of equality; this assertion is acceptable for tests
        Assert.False(originalOrdered.SequenceEqual(shuffled));

        // Verify card set is preserved (no duplicates/loss)
        var originalSet = originalOrdered.OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToList();
        var shuffledSet = shuffled.OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToList();
        Assert.Equal(originalSet, shuffledSet);
    }

    // Arrange: create a deck and shuffle multiple times.
    // Act: Shuffle repeatedly.
    // Assert: all 52 unique cards are still present and count preserved.
    [Fact]
    public void Shuffle_MultipleTimes_PreservesAllCards()
    {
        var originalSet = Deck.NewDeck().TakeAll()
            .Select(c => (c.Rank, c.Suit))
            .OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToList();

        var deck = Deck.NewDeck();
        deck.Shuffle(10);
        var shuffledSet = deck.TakeAll()
            .Select(c => (c.Rank, c.Suit))
            .OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToList();

        Assert.Equal(originalSet, shuffledSet);
        Assert.Equal(52, shuffledSet.Count);
    }

    // Arrange: call Shuffle with a negative times value.
    // Act & Assert: current implementation treats negative as a no-op and preserves count.
    [Fact]
    public void Shuffle_NegativeTimes_IsNoOp_PreservesCount()
    {
        var deck = Deck.NewDeck();
        deck.Shuffle(-1); // current implementation treats negative as no-op
        Assert.Equal(52, deck.RemainingCards);
    }

    // Helper to verify ascending order by Rank then Suit
    // Returns true if the supplied sequence is non-decreasing by Rank then Suit.
    private static bool IsOrderedByRankThenSuitAscending(IReadOnlyList<Card> cards)
    {
        for (int i = 1; i < cards.Count; i++)
        {
            var prev = cards[i - 1];
            var curr = cards[i];

            var rankComparison = prev.Rank.CompareTo(curr.Rank);
            if (rankComparison > 0)
                return false;

            if (rankComparison == 0 && prev.Suit.CompareTo(curr.Suit) > 0)
                return false;
        }

        return true;
    }
}
