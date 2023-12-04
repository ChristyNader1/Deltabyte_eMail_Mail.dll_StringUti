using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deltabyte.Util.Text
{
    public class Position
    {
        #region Properties
        public int Start { get; set; }
        public int End { get; set; }
        public String Old { get; set; }
        public String New { get; set; }
        #endregion

        #region Constructor
        public Position()
        {
            Start = 0;
            End = 0;
            Old = String.Empty;
            New = String.Empty;
        }
        public Position(String o, String n)
        {
            Start = 0;
            End = 0;
            Old = o;
            New = n;
        }
        public Position(int s, int e, string o, String n)
        {
            Start = s;
            End = e;
            Old = o;
            New = n;
        }
        public Position(Position p)
        {
            Start = p.Start;
            End = p.End;
            Old = p.Old;
            New = p.New;
        }
        #endregion

        #region Methods
        #endregion
    }
    public class StringInfo
    {
        #region Properties
        private List<Position> Values { get; set; }
        public List<Tuple<String,String>> Substitutes { get; set; }
        public Tuple<String, String> Substitute { get; set; }
        public String Original { get; set; }
        public String Changed { get; private set; }
        #endregion

        #region Constructor
        public StringInfo()
        {
            Values = new List<Position>();
            Substitutes = new List<Tuple<string, string>>();
            Substitute = null;
            Original = String.Empty;
            Changed = String.Empty;
        }
        public StringInfo(Tuple<String,String> s)
        {
            Values = new List<Position>();
            Substitutes = new List<Tuple<string, string>>();
            Substitute = new Tuple<string, string>(s.Item1, s.Item2);
            Original = String.Empty;
            Changed = String.Empty;
        }
        public StringInfo(List<Tuple<String, String>> sl)
        {
            Values = new List<Position>();
            Substitutes = new List<Tuple<string, string>>(sl);
            Substitute = null;
            Original = String.Empty;
            Changed = String.Empty;
        }
        public StringInfo(Tuple<String,String> s, String o)
        {
            Values = new List<Position>();
            Substitutes = new List<Tuple<string, string>>();
            Substitute = new Tuple<string, string>(s.Item1, s.Item2);
            Original = o;
            Changed = String.Empty;
        }
        public StringInfo(List<Tuple<String, String>> sl, String o)
        {
            Values = new List<Position>();
            Substitutes = new List<Tuple<string, string>>();
            Substitutes = new List<Tuple<string, string>>(sl);
            Original = o;
            Changed = String.Empty;
        }
        public StringInfo(StringInfo s)
        {
            Values = new List<Position>(s.Values);
            Substitutes = new List<Tuple<string, string>>(s.Substitutes);
            Substitute = new Tuple<string, string>(s.Substitute.Item1, s.Substitute.Item2);
            Original = String.Copy(s.Original);
            Changed = String.Copy(s.Changed);
        }
        #endregion

        #region Methods
        public Boolean Compare()
        {
            if (!Validate(Original)) return false;
            if (!Validate(Substitute)) return false;
            int first = Original.IndexOf(Substitute.Item1);
            while (first != -1)
            {
                Values.Add(new Position(first, first + Substitute.Item1.Length - 1, Substitute.Item1, Substitute.Item2));
                first = Original.IndexOf(Substitute.Item1, first + 1);
            }
            return true;
        }

        public Boolean Compare(Boolean b)
        {
            if (!Validate(Original)) return false;
            if (!Validate(Substitute)) return false;
            StringComparison cmp = (b) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            int first = Original.IndexOf(Substitute.Item1, cmp);
            while (first != -1)
            {
                Values.Add(new Position(first, first + Substitute.Item1.Length - 1, Substitute.Item1, Substitute.Item2));
                first = Original.IndexOf(Substitute.Item1, first + 1, cmp);
            }
            return true;
        }

        public Boolean MultiCompare()
        {
            if (!Validate(Original)) return false;
            if (!Validate(Substitutes)) return false;
            foreach (var item in Substitutes)
            {
                int first = Original.IndexOf(item.Item1);
                while (first != -1)
                {
                    Values.Add(new Position(first, first + item.Item1.Length - 1, item.Item1, item.Item2));
                    first = Original.IndexOf(item.Item1, first + 1);
                }
            }
            Values.Sort((x, y) => x.Start.CompareTo(y.Start));
            return true;
        }

        public Boolean MultiCompare(Boolean b)
        {
            if (!Validate(Original)) return false;
            if (!Validate(Substitutes)) return false;
            StringComparison cmp = (b) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            foreach (var item in Substitutes)
            {
                int first = Original.IndexOf(item.Item1, cmp);
                while (first != -1)
                {
                    Values.Add(new Position(first, first + item.Item1.Length - 1, item.Item1, item.Item2));
                    first = Original.IndexOf(item.Item1, first + 1, cmp);
                }
            }
            Values.Sort((x, y) => x.Start.CompareTo(y.Start));
            return true;
        }

        public Boolean Replace()
        {
            if (Values.Count() == 0) return false;
            Changed = String.Empty;
            int start = 0;
            foreach (var item in Values)
            {
                Changed += Original.Substring(start, item.Start - start) + item.New;
                start = item.End + 1;
            }
            Changed += Original.Substring(start);
            return true;
        }

        public int Find()
        {
            if (!Validate(Original)) return -1;
            if (!Validate(Substitute)) return -1;
            int i = 0;
            int first = Original.IndexOf(Substitute.Item1);
            while (first != -1)
            {
                i++;
                first = Original.IndexOf(Substitute.Item1, first + 1);
            }
            return i;
        }

        public int Find(String s)
        {
            if (!Validate(Original)) return -1;
            if (!Validate(s)) return -1;
            int i = 0;
            int first = Original.IndexOf(s);
            while (first != -1)
            {
                i++;
                first = Original.IndexOf(s, first + 1);
            }
            return i;
        }

        public int Multifind()
        {
            if (!Validate(Original)) return -1;
            if (!Validate(Substitutes)) return -1;
            int i = 0;
            foreach (var item in Substitutes)
            {
                int first = Original.IndexOf(item.Item1);
                while (first != -1)
                {
                    i++;
                    first = Original.IndexOf(item.Item1, first + 1);
                }
            }
            return i;
        }

        public int Multifind(List<String> sl)
        {
            if (!Validate(Original)) return -1;
            if (!Validate(sl)) return -1;
            int i = 0;
            foreach (var item in sl)
            {
                int first = Original.IndexOf(item);
                while (first != -1)
                {
                    i++;
                    first = Original.IndexOf(item, first + 1);
                }
            }
            return i;
        }
        public int Find(Boolean b)
        {
            if (!Validate(Original)) return -1;
            if (!Validate(Substitute)) return -1;
            StringComparison cmp = (b) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            int i = 0;
            int first = Original.IndexOf(Substitute.Item1, cmp);
            while (first != -1)
            {
                i++;
                first = Original.IndexOf(Substitute.Item1, first + 1, cmp);
            }
            return i;
        }

        public int Find(String s, Boolean b)
        {
            if (!Validate(Original)) return -1;
            if (!Validate(s)) return -1;
            StringComparison cmp = (b) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            int i = 0;
            int first = Original.IndexOf(s, cmp);
            while (first != -1)
            {
                i++;
                first = Original.IndexOf(s, first + 1, cmp);
            }
            return i;
        }

        public int Multifind(Boolean b)
        {
            if (!Validate(Original)) return -1;
            if (!Validate(Substitutes)) return -1;
            StringComparison cmp = (b) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            int i = 0;
            foreach (var item in Substitutes)
            {
                int first = Original.IndexOf(item.Item1, cmp);
                while (first != -1)
                {
                    i++;
                    first = Original.IndexOf(item.Item1, first + 1, cmp);
                }
            }
            return i;
        }

        public int Multifind(List<String> sl, Boolean b)
        {
            if (!Validate(Original)) return -1;
            if (!Validate(sl)) return -1;
            StringComparison cmp = (b) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            int i = 0;
            foreach (var item in sl)
            {
                int first = Original.IndexOf(item, cmp);
                while (first != -1)
                {
                    i++;
                    first = Original.IndexOf(item, first + 1, cmp);
                }
            }
            return i;
        }
        #endregion

        #region Helper
        private Boolean Validate(String s)
        {
            return String.IsNullOrWhiteSpace(s) ? false : true;
        }
        private Boolean Validate(Tuple<String,String> v)
        {
            if (String.IsNullOrWhiteSpace(v.Item1)) return false;
            //if (String.IsNullOrWhiteSpace(v.Item2)) return false;
            return true;
        }
        private Boolean Validate(List<String> sl)
        {
            foreach (var item in sl)
            {
                if (Validate(item) == false) return false;
            }
            return true;
        }
        private Boolean Validate(List<Tuple<String, String>> v)
        {
            foreach (var item in v)
            {
                if (Validate(item) == false) return false;
            }
            return true;
        }
        #endregion
    }
    public static class Strings
    {
        /// <summary>
        /// replace o with n in s
        /// </summary>
        /// <param name="s">text</param>
        /// <param name="o">old value</param>
        /// <param name="n">new value</param>
        /// <returns>changed text</returns>
        public static String Replace(String s, String o, String n)
        {
            StringInfo i = new StringInfo();
            i.Original = s;
            i.Substitute = new Tuple<string, string>(o, n);
            if (i.Compare()) if (i.Replace() == false) return s;
            return i.Changed;
        }

        /// <summary>
        /// replaces item1 with item2 in s
        /// </summary>
        /// <param name="s">text</param>
        /// <param name="r">old/new</param>
        /// <returns>changed text</returns>
        public static String Replace(String s, Tuple<String, String> r)
        {
            StringInfo i = new StringInfo();
            i.Original = s;
            i.Substitute = new Tuple<string, string>(r.Item1, r.Item2);
            if (i.Compare()) if (i.Replace() == false) return s;
            return i.Changed;
        }

        /// <summary>
        /// replace multiple item1 with item2 in s
        /// </summary>
        /// <param name="s">text</param>
        /// <param name="sl">list old/new values</param>
        /// <returns>changed text</returns>
        public static String Replace(String s, List<Tuple<String, String>> sl)
        {
            StringInfo i = new StringInfo();
            i.Original = s;
            i.Substitutes = new List<Tuple<string, string>>(sl);
            if (i.MultiCompare()) if (i.Replace() == false) return s;
            return i.Changed;
        }

        /// <summary>
        /// replaces item1 with item2 in s
        /// </summary>
        /// <param name="s">text</param>
        /// <param name="r">old/new values/param>
        /// <param name="b">true=ignore case</param>
        /// <returns>changed text</returns>
        public static String Replace(String s, Tuple<String, String> r, Boolean b)
        {
            StringInfo i = new StringInfo();
            i.Original = s;
            i.Substitute = new Tuple<string, string>(r.Item1, r.Item2);
            if (i.Compare(b)) if (i.Replace() == false) return s;
            return i.Changed;
        }

        /// <summary>
        /// replace multiple item1 with item2 in s
        /// </summary>
        /// <param name="s">text</param>
        /// <param name="sl">list old/new values</param>
        /// <param name="b">true=ignore case</param>
        /// <returns>changed text</returns>
        public static String Replace(String s, List<Tuple<String, String>> sl, Boolean b)
        {
            StringInfo i = new StringInfo();
            i.Original = s;
            i.Substitutes = new List<Tuple<string, string>>(sl);
            if (i.MultiCompare(b)) if (i.Replace() == false) return s;
            return i.Changed;
        }

        /// <summary>
        /// swap nibbles in byte
        /// </summary>
        /// <param name="s">text</param>
        /// <returns>decoded text</returns>
        public static String Decode(String s)
        {
            char[] a = s.ToArray();
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = ((char)(((a[i] & 0x0f) << 4) + ((a[i] & 0xf0) >> 4)));
            }
            return new String(a);
        }

    }
}
