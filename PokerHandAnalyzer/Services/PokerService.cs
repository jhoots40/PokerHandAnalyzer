using PokerEquityCalculator;

namespace PokerHandAnalyzer.Services
{
    public class PokerService
    {
        private readonly MonteCarloPokerSimulator _simulator;

        public PokerService()
        {
            _simulator = new MonteCarloPokerSimulator();
        }

        public List<Double> GetEquity(string hand1, string hand2, string board)
        {
            return _simulator.CalculateEquity(hand1, hand2, board);
        }
    }
}
