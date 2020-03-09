using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Model
{
    public class Rank<T, S> : List<Tuple<T, S>>
    {
        public Rank(IEnumerable<Tuple<T, S>> collection) : base(collection) { }

        public void NormalizeScore(Func<S[], Func<S, S>> normalizeFunction)
        {
            var scoreNormFunction = normalizeFunction(this.Select(r => r.Item2).ToArray());
            for(int i = 0; i < this.Count; i++)
            {
                this[i] = Tuple.Create(this[i].Item1, scoreNormFunction(this[i].Item2));
            }
        }
    }

    public static class Rank
    {
        public static Rank<T, S> Create<T, S>(IEnumerable<Tuple<T, S>> list) { return new Rank<T, S>(list); }
        
        public static Rank<T, double> FromMetric<T>(IEnumerable<Tuple<T, float[]>> db, float[] toCompareFeatures, FeatureCompareMetric compareMetric)
        {
            return Rank.Create(db.Select(item => Tuple.Create(item.Item1, compareMetric(toCompareFeatures, item.Item2)))
                .OrderByDescending(item => item.Item2)
                .ToList());
        }

        public static Rank<int, double> FromMetric(IEnumerable<float[]> db, float[] toCompareFeatures, FeatureCompareMetric compareMetric)
        {
            return Rank.Create(db.Select((item, index) => Tuple.Create(index, compareMetric(toCompareFeatures, item)))
                .OrderByDescending(item => item.Item2)
                .ToList());
        }
    }
}
