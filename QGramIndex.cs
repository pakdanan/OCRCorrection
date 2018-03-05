using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Position = System.Collections.Generic.KeyValuePair<int, int>;

namespace WinOCRCorrection
{
    public class QGramIndex
    {
        private readonly int m_Q;
        private readonly IList<string> m_Data;
        private Position[] m_SA;
        private Dictionary<string, string> m_Dir;
        private List<int> posts;
        public QGramIndex(IList<string> strings, int q)
        {
            m_Q = q;
            m_Data = strings;
            MakeSuffixArray();
            MakeIndex();
        }

        public int this[string s] { get { return FindInIndex(s); } }

        private int FindInIndex(string s)
        {
            return 0;
            //int idx;
            //if (!m_Dir.TryGetValue(s, out idx))
            //    return -1;
            //return m_SA[idx].Key;
        }

        private void MakeSuffixArray()
        {
            int size = m_Data.Sum(str => str.Length < m_Q ? 0 : str.Length - m_Q + 1);
            m_SA = new Position[size];
            int pos = 0;
            for (int i = 0; i < m_Data.Count; ++i)
                for (int j = 0; j <= m_Data[i].Length - m_Q; ++j)
                    m_SA[pos++] = new Position(i, j);

            Array.Sort(
                m_SA,
                (x, y) => string.CompareOrdinal(
                    m_Data[x.Key].Substring(x.Value),
                    m_Data[y.Key].Substring(y.Value)
                )
            );
        }

        private void MakeIndex()
        {
            m_Dir = new Dictionary<string, string>(m_SA.Length);
            // Every q-gram is a prefix in the suffix table.
            for (int i = 0; i < m_SA.Length; ++i)
            {
                var pos = m_SA[i];
                //posts.Add(pos.Value);
                var key= m_Data[pos.Key].Substring(pos.Value, m_Q)+ pos.Value.ToString();
                if (m_Dir.ContainsKey(key) )
                    m_Dir[key] = m_Dir[key]+","+ m_Data[pos.Key];// i;
                else
                    m_Dir[key] =m_Data[pos.Key];// i;
            }

        }
    }
    class QGramIndex1
    {
        private readonly int m_Maxlen;
        private readonly string m_Data;
        private readonly int m_Q;
        private int[] m_SA;
        private Dictionary<string, int> m_Dir = new Dictionary<string, int>();

        private struct StrCmp : IComparer<int>
        {
            public readonly String Data;
            public StrCmp(string data) { Data = data; }
            public int Compare(int x, int y)
            {
                return string.CompareOrdinal(Data.Substring(x), Data.Substring(y));
            }
        }

        private readonly StrCmp cmp;

        public QGramIndex1(IList<string> strings, int maxlen, int q)
        {
            m_Maxlen = maxlen;
            m_Q = q;

            var sb = new StringBuilder(strings.Count * maxlen);
            foreach (string str in strings)
                sb.AppendFormat(str.PadRight(maxlen, '\u0000'));
            m_Data = sb.ToString();
            cmp = new StrCmp(m_Data);
            MakeSuffixArray();
            MakeIndex();
        }

        public int this[string s] { get { return FindInIndex(s); } }

        private void MakeSuffixArray()
        {
            // Approx. runtime: n^3 * log n!!!
            // But I claim the shortest ever implementation of a suffix array!
            m_SA = Enumerable.Range(0, m_Data.Length).ToArray();
            Array.Sort(m_SA, cmp);
        }

        private int FindInArray(int ith)
        {
            return Array.BinarySearch(m_SA, ith, cmp);
        }

        private int FindInIndex(string s)
        {
            int idx;
            if (!m_Dir.TryGetValue(s, out idx))
                return -1;
            return m_SA[idx] / m_Maxlen;
        }

        private string QGram(int i)
        {
            return i > m_Data.Length - m_Q ?
                m_Data.Substring(i) :
                m_Data.Substring(i, m_Q);
        }

        private void MakeIndex()
        {
            for (int i = 0; i < m_Data.Length; ++i)
            {
                int pos = FindInArray(i);
                if (pos < 0) continue;
                m_Dir[QGram(i)] = pos;
            }
        }
    }
}
