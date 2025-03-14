using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerEquityCalculator
{
    public class Hand
    {
        public List<Card> Cards { get; }

        public Hand(string card1, string card2)
        {
            Cards = new List<Card> { new Card(card1), new Card(card2) };
        }
    }
}
