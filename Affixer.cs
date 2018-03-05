using System.Text.RegularExpressions;

namespace WinOCRCorrection
{
    public class Affixer
    {

        public string Affixing(string lemma, string prefix, string suffix)
        {
            lemma = lemma.ToLowerInvariant();
            prefix = prefix.ToLowerInvariant();
            suffix = suffix.ToLowerInvariant();
            #region menN-
            if (prefix == "menge") //mengebom
            {
                if (lemma.Length < 4)
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
            }
            if (prefix == "meng")
            {
                if (Regex.IsMatch(lemma, "^(([aiueoghq])|(k[^aiueo]))")) //mengatur,mengkritik
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
                if (Regex.IsMatch(lemma, "^k[aiueo]")) //mengatakan
                {
                    lemma = lemma.Remove(0, 1);
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
                }
            }
            if (prefix == "men")
            {
                if (Regex.IsMatch(lemma, "^[cdjszt]") || Regex.IsMatch(lemma, "^(t[^aiueo])")) //mencuci,mentraktir
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
                if (Regex.IsMatch(lemma, "^(t[aiueo])")) //menandai
                {
                    lemma = lemma.Remove(0, 1);
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
                }
            }
            if (prefix == "mem")
            {
                if (Regex.IsMatch(lemma, "^(p[aiueo])")) //memanggil
                {
                    lemma = lemma.Remove(0, 1);
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
                }
                if (Regex.IsMatch(lemma, "^(p[^aiueo])") || Regex.IsMatch(lemma, "^[bfv]")) //memproduksi,membawa
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
            }
            if (prefix == "meny")
            {
                if (Regex.IsMatch(lemma, "^(s[aiueo])")) //menyembunyikan
                {
                    lemma = lemma.Remove(0, 1);
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
                }
            }
            if (prefix == "me")
            {
                if (Regex.IsMatch(lemma, "^[lrwyv][aiueo]")) //melawan
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
            }
            #endregion

            #region peN-
            if (prefix == "per")
            {
                if (Regex.IsMatch(lemma, "^(r[aiueo])")) //perusakan
                {
                    lemma = lemma.Remove(0, 1);
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
                }
            }
            if (prefix == "pem")
            {
                if (Regex.IsMatch(lemma, "^(pr[aiueo])")) //pemrograman,pemrosesan
                {
                    lemma = lemma.Remove(0, 1);
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
                }
            }
            if (prefix == "pen")
            {
                if (Regex.IsMatch(lemma, "^(t[aiueo])")) //penulisan
                {
                    lemma = lemma.Remove(0, 1);
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
                }
            }
            if (prefix == "peng")
            {
                if (Regex.IsMatch(lemma, "^(k[aiueo])")) //pengupasan
                {
                    lemma = lemma.Remove(0, 1);
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
                }
            }
            if (prefix == "peny")
            {
                if (Regex.IsMatch(lemma, "^(s[aiueo])")) //penyuntikan
                {
                    lemma = lemma.Remove(0, 1);
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
                }
            }
            if (prefix == "pel")
            {
                if (Regex.IsMatch(lemma, "^(l[aiueo])")) //pelaporan
                {
                    lemma = lemma.Remove(0, 1);
                    return string.Format("{0}{1}{2}", prefix, lemma, suffix);
                }
            }
            #endregion
            //default route:
            return string.Format("{0}{1}{2}", prefix, lemma, suffix);

        }
    }
}
