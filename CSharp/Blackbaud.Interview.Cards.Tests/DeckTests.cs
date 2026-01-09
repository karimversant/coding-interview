using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace Blackbaud.Interview.Cards.Tests;
public class DeckTests
{
    [Fact]
    public void CanCreateANewDeck()
    {
        var deck = Deck.NewDeck();
        Assert.Equal(52, deck.RemainingCards);
    }

    [Fact]
    public void NextCard_ReturnsNullWhenEmpty()
    {
        var deck = Deck.NewDeck();
        var all = deck.TakeAll();
        Assert.True(deck.Empty);
        var next = deck.NextCard();
        Assert.Null(next);
    }

    [Fact]
    public void Draw_ReturnsUpToCountAndReducesRemaining()
    {
        var deck = Deck.NewDeck();
        var drawn = deck.Draw(5);
        Assert.Equal(5, drawn.Count);
        Assert.Equal(47, deck.RemainingCards);
    }

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

    [Fact]
    public void TakeAll_RemovesAllCards()
    {
        var deck = Deck.NewDeck();
        var taken = deck.TakeAll();
        Assert.Equal(52, taken.Count);
        Assert.True(deck.Empty);
    }

    [Fact]
    public void Reset_RestoresFullOrderedDeck()
    {
        var deck = Deck.NewDeck();
        deck.Draw(3);
        Assert.True(deck.RemainingCards < 52);
        deck.Reset();
        Assert.Equal(52, deck.RemainingCards);
    }

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

    [Fact]
    public void Shuffle_WithZeroTimes_PreservesOrderCount()
    {
        var deck = Deck.NewDeck();
        deck.Shuffle(0); // should be a no-op
        Assert.Equal(52, deck.RemainingCards);
    }

    // Helper to verify ascending order by Rank then Suit
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
