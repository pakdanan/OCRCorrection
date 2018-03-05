using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WinOCRCorrection
{
    public class Stemmer
    {

        private bool cekKataDasar(string kata)
        {
            bool isExist = false;
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = Correction.MariaDBConn;
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = @"select unigram from unigrams where IsLemma=1 and unigram=@unigram ;";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@unigram", kata);
                var obj = cmd.ExecuteScalar();
                if (obj!=null)
                    isExist = true;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
            conn.Close();
            return isExist;

        }
        private string HapusAkhiran(string kata)
        {
            Match matchkahlah = Regex.Match(kata, @"([A-Za-z]+)([klt]ah|pun|ku|mu|nya)$", RegexOptions.IgnoreCase);
            if (matchkahlah.Success)
            {
                string key = matchkahlah.Groups[1].Value;
                return HapusAkhiranKepunyaan(key);
            }
            return HapusAkhiranKepunyaan(kata);
        }


        private string HapusAkhiran(string kata, out string suffix)
        {
            string tanpaAkhiran = "";
            Match matchkahlah = Regex.Match(kata, @"([A-Za-z]+)([klt]ah|pun|ku|mu|nya)$", RegexOptions.IgnoreCase);
            if (matchkahlah.Success)
            {
                string key = matchkahlah.Groups[1].Value;
                suffix = matchkahlah.Groups[2].Value;
                string suffixAwal;
                tanpaAkhiran = HapusAkhiranKepunyaan(key, out suffixAwal);
                suffix = suffixAwal + suffix;
                return tanpaAkhiran;
            }
            tanpaAkhiran = HapusAkhiranKepunyaan(kata, out suffix);
            return tanpaAkhiran;
        }
        private string HapusAkhiranKepunyaan(string kata)
        {
            Match matchkahlah = Regex.Match(kata, @"([A-Za-z]+)(nya|[km]u)$", RegexOptions.IgnoreCase);
            if (matchkahlah.Success)
            {
                string key = matchkahlah.Groups[1].Value;
                return key;
            }
            return kata;
        }
        private string HapusAkhiranKepunyaan(string kata, out string suffix)
        {
            suffix = "";
            Match matchkahlah = Regex.Match(kata, @"([A-Za-z]+)(nya|[km]u)$", RegexOptions.IgnoreCase);
            if (matchkahlah.Success)
            {
                string key = matchkahlah.Groups[1].Value;
                suffix = matchkahlah.Groups[2].Value;
                return key;
            }
            return kata;
        }
        private string HapusAkhiranIAnKan(string kata)
        {
            string kataasal = kata;
            if (Regex.IsMatch(kata, "(kan)$"))
            {
                string kata1 = Regex.Replace(kata, "(kan)$", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
            }

            if (Regex.IsMatch(kata, "(i|an)$"))
            {
                string kata2 = Regex.Replace(kata, "(i|an)$", "");
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
            }
            return kataasal;
        }
        private string HapusAkhiranIAnKan(string kata, out string suffix)
        {
            string kataasal = kata;
            suffix = "";
            if (Regex.IsMatch(kata, "(kan)$"))
            {
                string kata1 = Regex.Replace(kata, "(kan)$", "");
                suffix = "kan";
                return kata1;
            }
            if (Regex.IsMatch(kata, "(i)$"))
            {
                string kata2 = Regex.Replace(kata, "(i)$", "");
                suffix = "i";
                return kata2;
            }
            if (Regex.IsMatch(kata, "(an)$"))
            {
                string kata2 = Regex.Replace(kata, "(an)$", "");
                suffix = "an";
                return kata2;
            }
            return kataasal;
        }


        private string hapus_derivation_prefix(string kata)
        {
            string kataasal = kata;

            if (Regex.IsMatch(kata, "^(di|[ks]e)"))
            {
                string kata1 = Regex.Replace(kata, "^(di|[ks]e)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }

                string kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
            }


            #region cek te- me- be- pe-
            if (Regex.IsMatch(kata, "^([tmbp]e)")) //cek te- me- be- pe-
            {
                #region awalan be-

                if (Regex.IsMatch(kataasal, "^(be)"))
                {
                    if (Regex.IsMatch(kataasal, "^(ber)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(ber)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                        kata1 = Regex.Replace(kata, "^(ber)", "r");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }

                    if (Regex.IsMatch(kata, "^(ber)[^aiueor][A-Za-z](?!er)"))
                    {
                        string kata1 = Regex.Replace(kata, "^(ber)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }

                    if (Regex.IsMatch(kata, "^(ber)[^aiueor][A-Za-z]er[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(ber)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }

                    if (Regex.IsMatch(kata, "^(belajar)"))
                    {
                        string kata1 = Regex.Replace(kata, "^(bel)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(ber)[^aiueor]er[^aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(be)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }


                }

                #endregion

                #region awalan te-
                if (Regex.IsMatch(kata, "^(te)"))
                {
                    if (Regex.IsMatch(kata, "^(terr)"))
                    {
                        return kata;
                    }
                    if (Regex.IsMatch(kata, "^(ter)[aioue]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(ter)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                        kata1 = Regex.Replace(kata, "^(ter)", "r");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    //teko kene
                }
                if (Regex.IsMatch(kata, "^(ter)[^aiueor]er[aiueo]"))
                {
                    string kata1 = Regex.Replace(kata, "^(ter)", "");
                    if (cekKataDasar(kata1))
                    {
                        return kata1;
                    }
                    string kata2 = HapusAkhiranIAnKan(kata1);
                    if (cekKataDasar(kata2))
                    {
                        return kata2;
                    }
                }
                if (Regex.IsMatch(kata, "^(ter)[^aiueor](?!er)"))
                {
                    string kata1 = Regex.Replace(kata, "^(ter)", "");
                    if (cekKataDasar(kata1))
                    {
                        return kata1;
                    }
                    string kata2 = HapusAkhiranIAnKan(kata1);
                    if (cekKataDasar(kata2))
                    {
                        return kata2;
                    }
                }
                if (Regex.IsMatch(kata, "^(te)[^aiueor]er[aiueo]"))
                {
                    string kata1 = Regex.Replace(kata, "^(te)", "");
                    if (cekKataDasar(kata1))
                    {
                        return kata1;
                    }
                    string kata2 = HapusAkhiranIAnKan(kata1);
                    if (cekKataDasar(kata2))
                    {
                        return kata2;
                    }
                }
                if (Regex.IsMatch(kata, "^(ter)[^aiueor]er[^aiueo]"))
                {
                    string kata1 = Regex.Replace(kata, "^(ter)", "");
                    if (cekKataDasar(kata1))
                    {
                        return kata1;
                    }
                    string kata2 = HapusAkhiranIAnKan(kata1);
                    if (cekKataDasar(kata2))
                    {
                        return kata2;
                    }
                }

                #endregion

                #region awalan me-
                if (Regex.IsMatch(kata, "^(me)"))
                {
                    if (Regex.IsMatch(kata, "^(me)[lrwyv][aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(me)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(mem)[bfvp]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(mem)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(mem)((r[aiueo])|[aiueo])"))
                    {
                        string kata1 = Regex.Replace(kata, "^(mem)", "m");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                        kata1 = Regex.Replace(kata, "^(mem)", "p");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }

                    }
                    if (Regex.IsMatch(kata, "^(men)[cdjszt]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(men)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(men)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(men)", "n");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                        kata1 = Regex.Replace(kata, "^(men)", "t");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(meng)[ghqk]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(meng)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(meng)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(meng)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                        kata1 = Regex.Replace(kata, "^(meng)", "k");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                        kata1 = Regex.Replace(kata, "^(menge)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(meny)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(meny)", "s");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                        kata1 = Regex.Replace(kata, "^(me)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                }
                #endregion

                #region awalan pe-
                if (Regex.IsMatch(kata, "^(pe)"))
                {
                    if (Regex.IsMatch(kata, "^(pe)[wy]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pe)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(per)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(per)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                        kata1 = Regex.Replace(kata, "^(per)", "r");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(per)[^aiueor][A-Za-z](?!er)"))
                    {
                        string kata1 = Regex.Replace(kata, "^(per)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(per)[^aiueor][A-Za-z](er)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(per)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(pembelajaran)"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pembelajaran)", "ajar");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(pem)[bfv]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pem)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(pem)(r[aiueo]|[aiueo])"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pem)", "m");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                        kata1 = Regex.Replace(kata, "^(pem)", "p");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(pen)[cdjzt]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pen)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(pen)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pen)", "n");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                        kata1 = Regex.Replace(kata, "^(pen)", "t");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(peng)[^aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(peng)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(peng)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(peng)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                        kata1 = Regex.Replace(kata, "^(peng)", "k");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                        kata1 = Regex.Replace(kata, "^(penge)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(peny)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(peny)", "s");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                        kata1 = Regex.Replace(kata, "^(pe)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(pel)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pel)", "l");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(pelajar)"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pel)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(pe)[^rwylmn]er[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pe)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(pe)[^rwylmn](?!er)"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pe)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                    if (Regex.IsMatch(kata, "^(pe)[^aiueo]er[^aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pe)", "");
                        if (cekKataDasar(kata1))
                        {
                            return kata1;
                        }
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        if (cekKataDasar(kata2))
                        {
                            return kata2;
                        }
                    }
                }
            }

            #endregion
            #endregion

            #region memper- dkk

            if (Regex.IsMatch(kata, "^(memper)"))
            {
                string kata1 = Regex.Replace(kata, "^(memper)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                string kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
                kata1 = Regex.Replace(kata, "^(memper)", "r");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
            }

            if (Regex.IsMatch(kata, "^(mempel)"))
            {
                string kata1 = Regex.Replace(kata, "^(mempel)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                string kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
                kata1 = Regex.Replace(kata, "^(mempel)", "l");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }

            }
            if (Regex.IsMatch(kata, "^(menter)"))
            {
                string kata1 = Regex.Replace(kata, "^(menter)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                string kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
                kata1 = Regex.Replace(kata, "^(menter)", "r");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }

            }
            if (Regex.IsMatch(kata, "^(member)"))
            {
                string kata1 = Regex.Replace(kata, "^(member)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                string kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
                kata1 = Regex.Replace(kata, "^(member)", "r");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }

            }

            #endregion

            #region diper-
            if (Regex.IsMatch(kata, "^(diper)"))
            {
                string kata1 = Regex.Replace(kata, "^(diper)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                string kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
                kata1 = Regex.Replace(kata, "^(diper)", "r");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
            }
            #endregion

            #region diter-
            if (Regex.IsMatch(kata, "^(diter)"))
            {
                string kata1 = Regex.Replace(kata, "^(diter)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                string kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
                kata1 = Regex.Replace(kata, "^(diter)", "r");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
            }
            #endregion

            #region dipel-
            if (Regex.IsMatch(kata, "^(dipel)"))
            {
                string kata1 = Regex.Replace(kata, "^(dipel)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                string kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
                kata1 = Regex.Replace(kata, "^(dipel)", "l");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
            }
            #endregion

            #region diber-
            if (Regex.IsMatch(kata, "^(diber)"))
            {
                string kata1 = Regex.Replace(kata, "^(diber)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                string kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
                kata1 = Regex.Replace(kata, "^(diber)", "r");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
            }
            #endregion

            #region keber-
            if (Regex.IsMatch(kata, "^(keber)"))
            {
                string kata1 = Regex.Replace(kata, "^(keber)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                string kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
                kata1 = Regex.Replace(kata, "^(keber)", "r");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
            }
            #endregion

            #region keter-
            if (Regex.IsMatch(kata, "^(keter)"))
            {
                string kata1 = Regex.Replace(kata, "^(keter)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                string kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
                kata1 = Regex.Replace(kata, "^(keter)", "r");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }
            }
            #endregion

            #region berke-
            if (Regex.IsMatch(kata, "^(berke)"))
            {
                string kata1 = Regex.Replace(kata, "^(berke)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
                string kata2 = HapusAkhiranIAnKan(kata1);
                if (cekKataDasar(kata2))
                {
                    return kata2;
                }

            }
            #endregion

            #region tak-
            if (Regex.IsMatch(kata, "^(takdi)"))
            {
                string kata1 = Regex.Replace(kata, "^(takdi)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
            }
            if (Regex.IsMatch(kata, "^(tak)"))
            {
                string kata1 = Regex.Replace(kata, "^(tak)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
            }
            #endregion

            /// sumber: https://id.wikibooks.org/wiki/Bahasa_Indonesia/Prefiks &  https://id.wikipedia.org/wiki/Wikipedia:Daftar_kosakata_bahasa_Indonesia_yang_sering_salah_dieja
            #region antar-
            if (Regex.IsMatch(kata, "^(antar)"))
            {
                string kata1 = Regex.Replace(kata, "^(antar)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
            }
            #endregion
            #region anti-
            if (Regex.IsMatch(kata, "^(anti)"))
            {
                string kata1 = Regex.Replace(kata, "^(anti)", "");
                if (cekKataDasar(kata1))
                {
                    return kata1;
                }
            }
            #endregion


            //cek awalan di ke se te be me
            if (Regex.IsMatch(kata, "^(di|[kstbmp]e)") == false)
            {
                return kataasal;
            }
            return kataasal;

        }

        private string hapus_derivation_prefix(string kata, out string prefix)
        {
            string kataasal = kata;
            if (Regex.IsMatch(kata, "^(di|[ks]e)"))
            {
                string kata1 = Regex.Replace(kata, "^(di|[ks]e)", "");
                string kata2 = HapusAkhiranIAnKan(kata1);
                prefix = kata.Substring(0, 2);
                return kata2;
            }


            #region cek te- me- be- pe-
            if (Regex.IsMatch(kata, "^([tmbp]e)")) //cek te- me- be- pe-
            {
                #region awalan be-

                if (Regex.IsMatch(kataasal, "^(be)"))
                {
                    if (Regex.IsMatch(kataasal, "^(ber)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(ber)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "ber";
                        return kata2;
                        //kata1 = Regex.Replace(kata, "^(ber)", "r");
                        //kata2 = HapusAkhiranIAnKan(kata1);
                        //return kata2;
                    }

                    if (Regex.IsMatch(kata, "^(ber)[^aiueor][A-Za-z](?!er)"))
                    {
                        string kata1 = Regex.Replace(kata, "^(ber)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "ber";
                        return kata2;
                    }

                    if (Regex.IsMatch(kata, "^(ber)[^aiueor][A-Za-z]er[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(ber)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "ber";
                        return kata2;
                    }

                    if (Regex.IsMatch(kata, "^(belajar)"))
                    {
                        string kata1 = Regex.Replace(kata, "^(bel)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "bel";
                        return kata2;
                    }
                    if (Regex.IsMatch(kata, "^(ber)[^aiueor]er[^aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(be)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "ber";
                        return kata2;
                    }


                }

                #endregion

                #region awalan te-
                if (Regex.IsMatch(kata, "^(te)"))
                {
                    if (Regex.IsMatch(kata, "^(terr)"))
                    {
                        prefix = "";
                        return kata;
                    }
                    if (Regex.IsMatch(kata, "^(ter)[aioue]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(ter)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "ter";
                        return kata2;
                        //kata1 = Regex.Replace(kata, "^(ter)", "r");
                        //kata2 = HapusAkhiranIAnKan(kata1);
                        //return kata2;
                    }
                    //teko kene
                }
                if (Regex.IsMatch(kata, "^(ter)[^aiueor]er[aiueo]"))
                {
                    string kata1 = Regex.Replace(kata, "^(ter)", "");
                    string kata2 = HapusAkhiranIAnKan(kata1);
                    prefix = "ter";
                    return kata2;
                }
                if (Regex.IsMatch(kata, "^(ter)[^aiueor](?!er)"))
                {
                    string kata1 = Regex.Replace(kata, "^(ter)", "");
                    string kata2 = HapusAkhiranIAnKan(kata1);
                    prefix = "ter";
                    return kata2;
                }
                if (Regex.IsMatch(kata, "^(te)[^aiueor]er[aiueo]"))
                {
                    string kata1 = Regex.Replace(kata, "^(te)", "");
                    string kata2 = HapusAkhiranIAnKan(kata1);
                    prefix = "te";
                    return kata2;
                }
                if (Regex.IsMatch(kata, "^(ter)[^aiueor]er[^aiueo]"))
                {
                    string kata1 = Regex.Replace(kata, "^(ter)", "");
                    string kata2 = HapusAkhiranIAnKan(kata1);
                    prefix = "ter";
                    return kata2;
                }

                #endregion

                #region awalan me-
                if (Regex.IsMatch(kata, "^(me)"))
                {
                    if (Regex.IsMatch(kata, "^(me)[lrwyv][aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(me)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "me";
                        return kata2;
                    }
                    if (Regex.IsMatch(kata, "^(mem)[bfvp]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(mem)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "mem";
                        return kata2;
                    }
                    if (Regex.IsMatch(kata, "^(mem)((r[aiueo])|[aiueo])"))
                    {
                        //string kata1 = Regex.Replace(kata, "^(mem)", "m");
                        //string kata2 = HapusAkhiranIAnKan(kata1);
                        //return kata2;
                        string kata1 = Regex.Replace(kata, "^(mem)", "p");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "mem";
                        return kata2;

                    }
                    if (Regex.IsMatch(kata, "^(men)[cdjszt]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(men)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "men";
                        return kata2;

                    }
                    if (Regex.IsMatch(kata, "^(men)[aiueo]"))
                    {
                        //string kata1 = Regex.Replace(kata, "^(men)", "n"); 
                        //string kata2 = HapusAkhiranIAnKan(kata1);
                        //return kata2;
                        string kata1 = Regex.Replace(kata, "^(men)", "t"); //contoh: menandai
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "men";
                        return kata2;
                    }
                    if (Regex.IsMatch(kata, "^(meng)[ghqk]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(meng)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "meng";
                        return kata2;
                    }
                    if (Regex.IsMatch(kata, "^(meng)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(meng)", ""); //contoh: mengatur,mengaduk
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "meng";
                        return kata2;
                        //string kata1 = Regex.Replace(kata, "^(meng)", "k"); //contoh: mengupas
                        //string kata2 = HapusAkhiranIAnKan(kata1);
                        //return kata2;
                        //kata1 = Regex.Replace(kata, "^(menge)", "");
                        //if (cekKataDasar(kata1))
                        //{
                        //    return kata1;
                        //}
                        //kata2 = HapusAkhiranIAnKan(kata1);
                        //if (cekKataDasar(kata2))
                        //{
                        //    return kata2;
                        //}
                    }
                    if (Regex.IsMatch(kata, "^(meny)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(meny)", "s");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "meny";
                        return kata2;
                        //kata1 = Regex.Replace(kata, "^(me)", "");
                        //if (cekKataDasar(kata1))
                        //{
                        //    return kata1;
                        //}
                        //kata2 = HapusAkhiranIAnKan(kata1);
                        //if (cekKataDasar(kata2))
                        //{
                        //    return kata2;
                        //}
                    }
                }
                #endregion

                #region awalan pe-
                if (Regex.IsMatch(kata, "^(pe)"))
                {
                    if (Regex.IsMatch(kata, "^(pe)[wy]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pe)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "pe";
                        return kata2;
                    }
                    if (Regex.IsMatch(kata, "^(per)[aiueo]"))
                    {
                        //string kata1 = Regex.Replace(kata, "^(per)", "");
                        //string kata2 = HapusAkhiranIAnKan(kata1);
                        //return kata2;
                        string kata1 = Regex.Replace(kata, "^(per)", "r");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "per";
                        return kata2;
                    }
                    if (Regex.IsMatch(kata, "^(per)[^aiueor][A-Za-z](?!er)"))
                    {
                        string kata1 = Regex.Replace(kata, "^(per)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "per";
                        return kata2;
                    }
                    if (Regex.IsMatch(kata, "^(per)[^aiueor][A-Za-z](er)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(per)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "per";
                        return kata2;

                    }
                    if (Regex.IsMatch(kata, "^(pembelajaran)"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pembelajaran)", "ajar");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "pembelajaran";
                        return kata2;

                    }
                    if (Regex.IsMatch(kata, "^(pem)[bfv]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pem)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "pem";
                        return kata2;

                    }
                    if (Regex.IsMatch(kata, "^(pem)(r[aiueo]|[aiueo])"))
                    {
                        //string kata1 = Regex.Replace(kata, "^(pem)", "m");
                        //string kata2 = HapusAkhiranIAnKan(kata1);
                        //return kata2;
                        string kata1 = Regex.Replace(kata, "^(pem)", "p");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "pem";
                        return kata2;
                    }
                    if (Regex.IsMatch(kata, "^(pen)[cdjzt]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pen)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "pen";
                        return kata2;

                    }
                    if (Regex.IsMatch(kata, "^(pen)[aiueo]"))
                    {
                        //string kata1 = Regex.Replace(kata, "^(pen)", "n");
                        //string kata2 = HapusAkhiranIAnKan(kata1);
                        //return kata2;
                        string kata1 = Regex.Replace(kata, "^(pen)", "t");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "pen";
                        return kata2;
                    }
                    if (Regex.IsMatch(kata, "^(peng)[^aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(peng)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "peng";
                        return kata2;
                    }
                    if (Regex.IsMatch(kata, "^(peng)[aiueo]"))
                    {
                        //string kata1 = Regex.Replace(kata, "^(peng)", "");
                        //string kata2 = HapusAkhiranIAnKan(kata1);
                        //return kata2;
                        string kata1 = Regex.Replace(kata, "^(peng)", "k");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "peng";
                        return kata2;
                        //string kata1 = Regex.Replace(kata, "^(penge)", "");
                        //string kata2 = HapusAkhiranIAnKan(kata1);
                        //return kata2;

                    }
                    if (Regex.IsMatch(kata, "^(peny)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(peny)", "s");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "peny";
                        return kata2;
                        //kata1 = Regex.Replace(kata, "^(pe)", "");
                        //if (cekKataDasar(kata1))
                        //{
                        //    return kata1;
                        //}
                        //kata2 = HapusAkhiranIAnKan(kata1);
                        //if (cekKataDasar(kata2))
                        //{
                        //    return kata2;
                        //}
                    }
                    if (Regex.IsMatch(kata, "^(pel)[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pel)", "l");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "pel";
                        return kata2;

                    }
                    if (Regex.IsMatch(kata, "^(pelajar)"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pel)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "pel";
                        return kata2;

                    }
                    if (Regex.IsMatch(kata, "^(pe)[^rwylmn]er[aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pe)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "pe";
                        return kata2;

                    }
                    if (Regex.IsMatch(kata, "^(pe)[^rwylmn](?!er)"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pe)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "pe";
                        return kata2;

                    }
                    if (Regex.IsMatch(kata, "^(pe)[^aiueo]er[^aiueo]"))
                    {
                        string kata1 = Regex.Replace(kata, "^(pe)", "");
                        string kata2 = HapusAkhiranIAnKan(kata1);
                        prefix = "pe";
                        return kata2;

                    }
                }
            }

            #endregion
            #endregion

            #region memper- dkk

            if (Regex.IsMatch(kata, "^(memper)"))
            {
                string kata1 = Regex.Replace(kata, "^(memper)", "");
                string kata2 = HapusAkhiranIAnKan(kata1);
                prefix = "memper";
                return kata2;

                //kata1 = Regex.Replace(kata, "^(memper)", "r");
                //if (cekKataDasar(kata1))
                //{
                //    return kata1;
                //}
                //kata2 = HapusAkhiranIAnKan(kata1);
                //if (cekKataDasar(kata2))
                //{
                //    return kata2;
                //}
            }

            if (Regex.IsMatch(kata, "^(mempel)"))
            {
                string kata1 = Regex.Replace(kata, "^(mempel)", "");
                string kata2 = HapusAkhiranIAnKan(kata1);
                prefix = "mempel";
                return kata2;

                //kata1 = Regex.Replace(kata, "^(mempel)", "l");
                //if (cekKataDasar(kata1))
                //{
                //    return kata1;
                //}
                //kata2 = HapusAkhiranIAnKan(kata1);
                //if (cekKataDasar(kata2))
                //{
                //    return kata2;
                //}

            }
            if (Regex.IsMatch(kata, "^(menter)"))
            {
                string kata1 = Regex.Replace(kata, "^(menter)", "");
                string kata2 = HapusAkhiranIAnKan(kata1);
                prefix = "menter";
                return kata2;

                //kata1 = Regex.Replace(kata, "^(menter)", "r");
                //if (cekKataDasar(kata1))
                //{
                //    return kata1;
                //}
                //kata2 = HapusAkhiranIAnKan(kata1);
                //if (cekKataDasar(kata2))
                //{
                //    return kata2;
                //}

            }
            if (Regex.IsMatch(kata, "^(member)"))
            {
                string kata1 = Regex.Replace(kata, "^(member)", "");
                string kata2 = HapusAkhiranIAnKan(kata1);
                prefix = "member";
                return kata2;

                //kata1 = Regex.Replace(kata, "^(member)", "r");
                //if (cekKataDasar(kata1))
                //{
                //    return kata1;
                //}
                //kata2 = HapusAkhiranIAnKan(kata1);
                //if (cekKataDasar(kata2))
                //{
                //    return kata2;
                //}

            }

            #endregion

            #region diper-
            if (Regex.IsMatch(kata, "^(diper)"))
            {
                string kata1 = Regex.Replace(kata, "^(diper)", "");
                string kata2 = HapusAkhiranIAnKan(kata1);
                prefix = "diper";
                return kata2;

                //kata1 = Regex.Replace(kata, "^(diper)", "r");
                //if (cekKataDasar(kata1))
                //{
                //    return kata1;
                //}
                //kata2 = HapusAkhiranIAnKan(kata1);
                //if (cekKataDasar(kata2))
                //{
                //    return kata2;
                //}
            }
            #endregion

            #region diter-
            if (Regex.IsMatch(kata, "^(diter)"))
            {
                string kata1 = Regex.Replace(kata, "^(diter)", "");
                string kata2 = HapusAkhiranIAnKan(kata1);
                prefix = "diter";
                return kata2;

                //kata1 = Regex.Replace(kata, "^(diter)", "r");
                //if (cekKataDasar(kata1))
                //{
                //    return kata1;
                //}
                //kata2 = HapusAkhiranIAnKan(kata1);
                //if (cekKataDasar(kata2))
                //{
                //    return kata2;
                //}
            }
            #endregion

            #region dipel-
            if (Regex.IsMatch(kata, "^(dipel)"))
            {
                string kata1 = Regex.Replace(kata, "^(dipel)", "");
                string kata2 = HapusAkhiranIAnKan(kata1);
                prefix = "dipel";
                return kata2;

                //kata1 = Regex.Replace(kata, "^(dipel)", "l");
                //if (cekKataDasar(kata1))
                //{
                //    return kata1;
                //}
                //kata2 = HapusAkhiranIAnKan(kata1);
                //if (cekKataDasar(kata2))
                //{
                //    return kata2;
                //}
            }
            #endregion

            #region diber-
            if (Regex.IsMatch(kata, "^(diber)"))
            {
                string kata1 = Regex.Replace(kata, "^(diber)", "");
                string kata2 = HapusAkhiranIAnKan(kata1);
                prefix = "diber";
                return kata2;

                //kata1 = Regex.Replace(kata, "^(diber)", "r");
                //if (cekKataDasar(kata1))
                //{
                //    return kata1;
                //}
                //kata2 = HapusAkhiranIAnKan(kata1);
                //if (cekKataDasar(kata2))
                //{
                //    return kata2;
                //}
            }
            #endregion

            #region keber-
            if (Regex.IsMatch(kata, "^(keber)"))
            {
                string kata1 = Regex.Replace(kata, "^(keber)", "");
                string kata2 = HapusAkhiranIAnKan(kata1);
                prefix = "keber";
                return kata2;

                //kata1 = Regex.Replace(kata, "^(keber)", "r");
                //if (cekKataDasar(kata1))
                //{
                //    return kata1;
                //}
                //kata2 = HapusAkhiranIAnKan(kata1);
                //if (cekKataDasar(kata2))
                //{
                //    return kata2;
                //}
            }
            #endregion

            #region keter-
            if (Regex.IsMatch(kata, "^(keter)"))
            {
                string kata1 = Regex.Replace(kata, "^(keter)", "");
                string kata2 = HapusAkhiranIAnKan(kata1);
                prefix = "keter";
                return kata2;

                //kata1 = Regex.Replace(kata, "^(keter)", "r");
                //if (cekKataDasar(kata1))
                //{
                //    return kata1;
                //}
                //kata2 = HapusAkhiranIAnKan(kata1);
                //if (cekKataDasar(kata2))
                //{
                //    return kata2;
                //}
            }
            #endregion

            #region berke-
            if (Regex.IsMatch(kata, "^(berke)"))
            {
                string kata1 = Regex.Replace(kata, "^(berke)", "");
                string kata2 = HapusAkhiranIAnKan(kata1);
                prefix = "berke";
                return kata2;

            }
            #endregion

            /// sumber: https://id.wikibooks.org/wiki/Bahasa_Indonesia/Prefiks &  https://id.wikipedia.org/wiki/Wikipedia:Daftar_kosakata_bahasa_Indonesia_yang_sering_salah_dieja 
            #region antar- 
            if (Regex.IsMatch(kata, "^(antar)"))
            {
                string kata1 = Regex.Replace(kata, "^(antar)", "");
                prefix = "antar";
                return kata1;
            }
            #endregion
            #region anti- 
            if (Regex.IsMatch(kata, "^(anti)"))
            {
                string kata1 = Regex.Replace(kata, "^(anti)", "");
                prefix = "anti";
                return kata1;
            }
            #endregion

            //cek awalan di ke se te be me
            if (Regex.IsMatch(kata, "^(di|[kstbmp]e)") == false)
            {
                prefix = "";
                return kataasal;
            }
            prefix = "";
            return kataasal;

        }

        public bool checkStem(string stem)
        {
            return cekKataDasar(stem);
        }
        public string Stemming(string kata)
        {
            if (cekKataDasar(kata))
            {
                return kata;
            }

            kata = HapusAkhiran(kata);

            kata = HapusAkhiranIAnKan(kata);

            kata = hapus_derivation_prefix(kata);

            return kata;
        }
        public string StemmingWithoutChecking(string kata, out string prefix, out string suffix)
        {
            prefix = "";
            string suffix1;
            string suffix2;
            kata = HapusAkhiran(kata,out suffix2);
            kata = HapusAkhiranIAnKan(kata, out suffix1);
            suffix = suffix1 + suffix2;
            kata = hapus_derivation_prefix(kata, out prefix);

            return kata;
        }

    }
}
