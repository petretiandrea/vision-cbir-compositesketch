using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Model;

namespace Vision.Model
{
    public interface FusionStrategy
    {
        Rank<T, double> Fusion<T, S>(params Rank<T, S>[] rankBoards);
    }

    public class BordaCount : FusionStrategy
    {
        private double[] weights;

        public BordaCount(params double[] weights)
        {
            this.weights = weights;
        }

        public Rank<T, double> Fusion<T, S>(params Rank<T, S>[] rankBoards)
        {
            Dictionary<T, double> finalRankBoard = new Dictionary<T, double>();

            for(int i = 0; i < rankBoards.Length; i++)
            {
                AddBoard(finalRankBoard, rankBoards[i], weights[i]);
            }

            var orderedFinalRank = from r in finalRankBoard orderby r.Value descending select Tuple.Create(r.Key, r.Value);
            return orderedFinalRank.ToRank();
        }

        private void AddBoard<T, S>(Dictionary<T, double> finalRankBoard, Rank<T, S> rankBoard, double weight)
        {
            for(int i = 0; i < rankBoard.Count; i++)
            {
                T key = rankBoard[i].Item1;
                double currentScore = 0; // actual score for the same entry
                double score = weight * (rankBoard.Count - i); // score for this enty
                // if entry is present, sum the scores, otherwise add the new entry
                if(finalRankBoard.TryGetValue(key, out currentScore))
                {
                    finalRankBoard[key] = currentScore + score;
                } else
                {
                    finalRankBoard.Add(key, score);
                }
            }
        }
    }
}
