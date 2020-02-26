using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Model
{
    interface FusionStrategy<T>
    {
        List<T> Fusion(List<T[]> rankBoards, params double[] weights);
    }


    class BordaCount<T> : FusionStrategy<T>
    {
        public List<T> Fusion(List<T[]> rankBoards, params double[] weights)
        {
            Dictionary<T, double> finalRankBoard = new Dictionary<T, double>();

            for(int i = 0; i < rankBoards.Count; i++)
            {
                AddBoard(finalRankBoard, rankBoards[i], weights[i]);
            }

            var orderedFinalRank = from r in finalRankBoard orderby r.Value descending select r.Key;
            return orderedFinalRank.ToList();
        }

        private void AddBoard(Dictionary<T, double> finalRankBoard, T[] rankBoard, double weight)
        {
            for(int i = 0; i < rankBoard.Length; i++)
            {
                T key = rankBoard[i];
                double currentScore = 0; // actual score for the same entry
                double score = weight * (rankBoard.Length - i); // score for this enty
                // if entry is present, sum the scores, otherwise add the new entry
                if(finalRankBoard.TryGetValue(key, out currentScore))
                {
                    finalRankBoard[rankBoard[i]] = currentScore + score;
                } else
                {
                    finalRankBoard.Add(rankBoard[i], score);
                }
            }
        }
    }
}
