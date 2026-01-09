using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackbaud.Interview.Cards;

public interface IDeck
{
    int RemainingCards { get; }
    bool Empty { get; }
    Card NextCard();
    IReadOnlyList<Card> Draw(int count);
    IReadOnlyList<Card> TakeAll();
    void Reset();
    void Sort(IComparer<Card>? comparer = null);
    void SortByRankThenSuit();
    void Shuffle(int times);
}
