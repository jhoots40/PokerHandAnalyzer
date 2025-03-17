using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;

namespace PokerEquityCalculator
{
    public class MonteCarloPokerSimulator
    {
        private static readonly Random random = new Random();
        private static readonly string Ranks = "23456789TJQKA";

        public List<Double> CalculateEquity(string player1Hand, string player2Hand, string boardCards = "", int simulations = 10000)
        {
            List<Double> equities = new List<Double>();
            Hand player1 = new Hand(player1Hand.Substring(0, 2), player1Hand.Substring(2, 2));
            Hand player2 = new Hand(player2Hand.Substring(0, 2), player2Hand.Substring(2, 2));

            List<Card> board;

            if(boardCards != null) board = Enumerable.Range(0, boardCards.Length / 2).Select(i => new Card(boardCards.Substring(i * 2, 2))).ToList();
            else board = new List<Card>();
            List<Card> deck = GenerateDeck();

            // Remove known cards from the deck
            deck.RemoveAll(c => player1.Cards.Contains(c) || player2.Cards.Contains(c) || board.Contains(c));

            double p1Wins = 0, p2Wins = 0, ties = 0;

            for (int i = 0; i < simulations; i++)
            {
                List<Card> finalBoard = new List<Card>(board);
                List<Card> startingDeck = new List<Card>(deck);
                while (finalBoard.Count < 5) // Fill remaining community cards
                    finalBoard.Add(DrawRandomCard(startingDeck));

                int result = CompareHands(player1.Cards.Concat(finalBoard).ToList(), player2.Cards.Concat(finalBoard).ToList());
                if (result == 1) p1Wins++;
                else if (result == -1) p2Wins++;
                else
                {
                    ties++;
                }
            }

            equities.Add((double)p1Wins / simulations);
            equities.Add((double)p2Wins / simulations);
            equities.Add((double)ties / simulations);

            return equities;
        }

        private List<Card> GenerateDeck()
        {
            char[] suits = { 's', 'h', 'd', 'c' };
            string ranks = "23456789TJQKA";
            return (from rank in ranks from suit in suits select new Card($"{rank}{suit}")).ToList();
        }

        private Card DrawRandomCard(List<Card> deck)
        {
            int index = random.Next(deck.Count);
            Card card = deck[index];
            deck.RemoveAt(index);
            return card;
        }

        public static int CompareHands(List<Card> hand1, List<Card> hand2)
        {
            var rank1 = EvaluateHand(hand1);
            var rank2 = EvaluateHand(hand2);

            if (rank1 > rank2) return 1;
            if (rank1 < rank2) return -1;

            return CompareSameRankHands(hand1, hand2, rank1);
        }

        private static int EvaluateHand(List<Card> hand)
        {
            var rankCounts = hand.GroupBy(card => card.Rank).ToDictionary(g => g.Key, g => g.Count());
            var suitCounts = hand.GroupBy(card => card.Suit).ToDictionary(g => g.Key, g => g.Count());
            var orderedRanks = hand.Select(card => Ranks.IndexOf(card.Rank)).OrderByDescending(r => r).ToList();

            bool isFlush = suitCounts.Values.Any(count => count >= 5);
            bool isStraight = IsStraight(orderedRanks);

            if (isFlush && isStraight && orderedRanks[0] == 0) return 10; // Royal Flush
            if (isFlush && isStraight) return 9; // Straight Flush
            if (rankCounts.Values.Contains(4)) return 8; // Four of a Kind
            if (rankCounts.Values.Contains(3) && rankCounts.Values.Contains(2)) return 7; // Full House
            if (isFlush) return 6; // Flush
            if (isStraight) return 5; // Straight
            if (rankCounts.Values.Contains(3)) return 4; // Three of a Kind
            if (rankCounts.Values.Count(v => v == 2) == 2) return 3; // Two Pair
            if (rankCounts.Values.Contains(2)) return 2; // One Pair
            return 1; // High Card
        }

        private static bool IsStraight(List<int> orderedRanks)
        {
            orderedRanks = orderedRanks.Distinct().ToList();
            for (int i = 0; i <= orderedRanks.Count - 5; i++)
            {
                if (orderedRanks[i] - orderedRanks[i + 4] == 4) return true;
            }
            return orderedRanks.Contains(0) && orderedRanks.Contains(9) && orderedRanks.Contains(10) && orderedRanks.Contains(11) && orderedRanks.Contains(12);
        }

        private static int CompareSameRankHands(List<Card> hand1, List<Card> hand2, int rank)
        {
            //var sorted1 = hand1.Select(card => Ranks.IndexOf(card.Rank)).OrderByDescending(r => r).ToList();
            //var sorted2 = hand2.Select(card => Ranks.IndexOf(card.Rank)).OrderByDescending(r => r).ToList();

            //for (int i = 0; i < sorted1.Count; i++)
            //{
            //    if (sorted1[i] > sorted2[i]) return 1;
            //    if (sorted1[i] < sorted2[i]) return -1;
            //}
            //return 0; // Tie


            var grouped1 = hand1.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ThenByDescending(g => Ranks.IndexOf(g.Key)).ToList();
            var grouped2 = hand2.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ThenByDescending(g => Ranks.IndexOf(g.Key)).ToList();

            switch (rank)
            {
                case 8: // Four of a Kind
                    return CompareGroupedHands(grouped1, grouped2, 4);
                case 7: // Full House
                    return CompareGroupedHands(grouped1, grouped2, 3, 2);
                case 4: // Three of a Kind
                    return CompareGroupedHands(grouped1, grouped2, 3);
                case 3: // Two Pair
                    return CompareGroupedHands(grouped1, grouped2, 2, 2);
                case 2: // One Pair
                    return CompareGroupedHands(grouped1, grouped2, 2);
                default: // High Card, Flush, Straight, or Straight Flush
                    return CompareHighCards(hand1, hand2, 5);
            }
        }

        /// Compares grouped hands based on the primary sets (e.g., Four of a Kind, Three of a Kind, Pairs)
        private static int CompareGroupedHands(List<IGrouping<char, Card>> grouped1, List<IGrouping<char, Card>> grouped2, params int[] mainGroups)
        {
            for (int i = 0; i < mainGroups.Length; i++)
            {
                int rank1 = Ranks.IndexOf(grouped1[i].Key);
                int rank2 = Ranks.IndexOf(grouped2[i].Key);
                if (rank1 > rank2) return 1;
                if (rank1 < rank2) return -1;
            }

            var kickers1 = grouped1.Where(g => g.Count() == 1).SelectMany(g => g).ToList();
            var kickers2 = grouped2.Where(g => g.Count() == 1).SelectMany(g => g).ToList();

            int cardsUsed = 0;
            foreach(int n in mainGroups) cardsUsed += n;

            return CompareHighCards(kickers1, kickers2, 5 - cardsUsed);
        }

        /// Compares high cards in descending order
        private static int CompareHighCards(List<Card> hand1, List<Card> hand2, int numKickers)
        {
            var sorted1 = hand1.Select(card => Ranks.IndexOf(card.Rank)).OrderByDescending(r => r).ToList();
            var sorted2 = hand2.Select(card => Ranks.IndexOf(card.Rank)).OrderByDescending(r => r).ToList();

            for (int i = 0; i < numKickers; i++)
            {
                if (sorted1[i] > sorted2[i]) return 1;
                if (sorted1[i] < sorted2[i]) return -1;
            }
            return 0;
        }
    }
}
