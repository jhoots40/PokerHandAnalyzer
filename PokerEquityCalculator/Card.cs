using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerEquityCalculator
{
    public class Card
    {
        public char Rank { get; }
        public char Suit { get; }

        public Card(string card)
        {
            if (card.Length != 2)
                throw new ArgumentException("Invalid card format. Example: 'As', 'Kd'");

            Rank = card[0]; // 'A', 'K', '2', ...
            Suit = card[1]; // 's', 'd', 'h', 'c'
        }

        public override string ToString() => $"{Rank}{Suit}";

        public override bool Equals(object? obj)
        {
            if (obj is Card other)
            {
                return Suit == other.Suit && Rank == other.Rank;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Suit, Rank);
        }
    }
}
