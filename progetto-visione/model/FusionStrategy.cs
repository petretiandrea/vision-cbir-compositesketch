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
        Rank<T, double> Fusion<T>(params Rank<T, double>[] rankBoards);
    }

    public class WeightedSum : FusionStrategy
    {
        public double[] Weights { get; private set; }

        public WeightedSum(params double[] weights)
        {
            this.Weights = weights;
        }

        public Rank<T, double> Fusion<T>(params Rank<T, double>[] rankBoards)
        {
            Dictionary<T, double> finalRankBoard = new Dictionary<T, double>();

            for(int i = 0; i < rankBoards.Length; i++)
            {
                AddBoard(finalRankBoard, rankBoards[i], Weights[i]);
            }

            var orderedFinalRank = from r in finalRankBoard orderby r.Value descending select Tuple.Create(r.Key, r.Value);
            return orderedFinalRank.ToRank();
        }

        private void AddBoard<T>(Dictionary<T, double> finalRankBoard, Rank<T, double> rankBoard, double weight)
        {
            for(int i = 0; i < rankBoard.Count; i++)
            {
                T key = rankBoard[i].Item1;
                double currentScore = 0; // actual score for the same entry
                //double score = weight * (rankBoard.Count - i); // score for this enty
                double score = weight * rankBoard[i].Item2;
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
