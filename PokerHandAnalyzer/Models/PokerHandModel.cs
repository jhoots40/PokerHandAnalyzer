﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PokerHandAnalyzer.Models
{
    public class PokerHandModel
    {
        [Required(ErrorMessage = "Hero hand is required.")]
        [RegularExpression(@"^(A|K|Q|J|T|[2-9])(h|d|c|s)(A|K|Q|J|T|[2-9])(h|d|c|s)$", ErrorMessage = "Hero hand format is invalid.")]
        public string? HeroHand { get; set; }

        [Required(ErrorMessage = "Villain hand is required.")]
        [RegularExpression(@"^(A|K|Q|J|T|[2-9])(h|d|c|s)(A|K|Q|J|T|[2-9])(h|d|c|s)$", ErrorMessage = "Villain hand format is invalid.")]
        public string? VillainHand { get; set; }

        [RegularExpression(@"^$|^(A|K|Q|J|T|[2-9])(h|d|c|s)(A|K|Q|J|T|[2-9])(h|d|c|s)(A|K|Q|J|T|[2-9])(h|d|c|s)$", ErrorMessage = "Community cards must be 3 valid cards")]
        public string? CommunityCards { get; set; }

        public bool HasDuplicateCards()
        {
            var allCards = new HashSet<string>();
            var combinedCards = HeroHand + VillainHand + CommunityCards;

            for (int i = 0; i < combinedCards.Length; i += 2)
            {
                string card = combinedCards.Substring(i, 2);
                if (allCards.Contains(card)) return true;
                allCards.Add(card);
            }
            return false;
        }
    }
}

