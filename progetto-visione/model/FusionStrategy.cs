using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Utils;

namespace Vision.Model
{
    public interface FusionStrategy<T>
    {
        Rank<T, double> Fusion<S>(params Rank<T, S>[] rankBoards);
    }

    public class Rank<T, S> : List<Tuple<T, S>>
    {
        public Rank(IEnumerable<Tuple<T, S>> collection) : base(collection) { }
    }

    public static class Rank {
        public static Rank<T, S> Create<T, S>(IEnumerable<Tuple<T, S>> list) { return new Rank<T, S>(list); }
        public static Rank<T, float> FromMetric<T>(IEnumerable<Tuple<T, float[]>> db, float[] toCompareFeatures, FeatureCompareMetric compareMetric)
        {
            return Rank.Create(db.Select(item => Tuple.Create(item.Item1, compareMetric(toCompareFeatures, item.Item2)))
                .OrderByDescending(item => item.Item2)
                .ToList());
        }
    }

    public class BordaCount<T> : FusionStrategy<T>
    {
        private double[] weights;

        public BordaCount(params double[] weights)
        {
            this.weights = weights;
        }

        public Rank<T, double> Fusion<S>(params Rank<T, S>[] rankBoards)
        {
            Dictionary<T, double> finalRankBoard = new Dictionary<T, double>();

            for(int i = 0; i < rankBoards.Length; i++)
            {
                AddBoard(finalRankBoard, rankBoards[i], weights[i]);
            }

            var orderedFinalRank = from r in finalRankBoard orderby r.Value descending select Tuple.Create(r.Key, r.Value);
            return orderedFinalRank.ToRank();
        }

        private void AddBoard<S>(Dictionary<T, double> finalRankBoard, Rank<T, S> rankBoard, double weight)
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
