using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace WinOCRCorrection
{
    public class Correction
    {
        public const string MariaDBConn = "server=127.0.0.1;user=root;database=ocr_correction;port=3307;password=kompas";
        const string SeleniumFolder = @"C:\selenium";

        public Dictionary<int, string> GetTokensFromText(string text)
        {
            Dictionary<int, string> dicTokens = new Dictionary<int, string>();
            char[] seps = { '\t', '\n', '\r', ' ' }; //{ '\t', '\n', '\r', '+', '\\', '*', '/', '(', ')', ' ' };
            string[] token1s = text.Split(seps);
            int i = 1;
            foreach (string token1 in token1s)
            {
                //if (token1.Any(Char.IsLetter) && token1 != "" & token1.Length > 1)
                //{
                //    //Regex rgx = new Regex("[^a-zA-Z -]");
                //    //token1 = rgx.Replace(token1, "");
                //    dicTokens.Add(i, token1);
                //    i++;
                //}
                if (token1 != "")
                {
                    //Regex rgx = new Regex("[^a-zA-Z -]");
                    //token1 = rgx.Replace(token1, "");
                    dicTokens.Add(i, token1);
                    i++;
                }
            }
            return dicTokens;
        }
        public string DeHyphenate(string pathArticle)
        {
            StringBuilder sB = new StringBuilder();
            string[] tmpLines = File.ReadAllLines(pathArticle);
            for (int j = 0; j < tmpLines.Length; j++)
            {
                //Regex rgx = new Regex("[^a-zA-Z -]"); //clean non alphabet & -
                //tmpLines[j] = rgx.Replace(tmpLines[j], "");

                if (j > 0)
                {
                    string[] tokenLineBefore = tmpLines[j - 1].Split('\t', ' ');// tmpLines[j - 1].Split('\t', '+', '\\', '*', '/', '(', ')', ' ');
                    string firstPart = tokenLineBefore[tokenLineBefore.Length - 1];
                    string secondPart = tmpLines[j].Split('\t', ' ')[0]; // tmpLines[j].Split('\t', '+', '\\', '*', '/', '(', ')', ' ')[0];
                    // deHyphenate based on '-':
                    if (firstPart.EndsWith("-"))
                    {
                        sB.Remove(sB.Length - 1, 1).Append(tmpLines[j]);
                    }

                    else
                    {
                        sB.Append(" ");
                        sB.Append(tmpLines[j]);
                    }
                }
                else
                    sB.Append(tmpLines[j]);
            }
            return sB.ToString();
        }

        public bool CheckUnigram(string unigram, string sqlQuery, out int frequency)
        {
            frequency = 0;
            bool res = false;
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = MariaDBConn;
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = sqlQuery;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@unigram", unigram);
                cmd.Parameters["@unigram"].Direction = ParameterDirection.Input;
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    frequency = Convert.ToInt32(result);
                    res = true;
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                // MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message);
                // res = ex.Message;
                throw new Exception(ex.Message);
            }
            conn.Close();
            return res;
        }

        public string ChangeOldToNewSpell(string word)
        {
            word = new Regex("tj", RegexOptions.IgnoreCase).Replace(word, "c");
            word = new Regex("dj", RegexOptions.IgnoreCase).Replace(word, "|");
            word = new Regex("nj", RegexOptions.IgnoreCase).Replace(word, "ny");
            word = new Regex("sj", RegexOptions.IgnoreCase).Replace(word, "sy");
            word = new Regex("ch", RegexOptions.IgnoreCase).Replace(word, "kh");
            word = new Regex("j", RegexOptions.IgnoreCase).Replace(word, "y");

            word = word.Replace("|", "j");
            // special cases:
            word = new Regex("setya", RegexOptions.IgnoreCase).Replace(word, "setia");
            word = new Regex("konsekwen", RegexOptions.IgnoreCase).Replace(word, "konsekuen");
            word = new Regex("dgn", RegexOptions.IgnoreCase).Replace(word, "dengan");
            word = new Regex("utk", RegexOptions.IgnoreCase).Replace(word, "untuk");
            word = new Regex("putera", RegexOptions.IgnoreCase).Replace(word, "putra");
            word = new Regex("puteri", RegexOptions.IgnoreCase).Replace(word, "putri");
            word = new Regex("inggeris", RegexOptions.IgnoreCase).Replace(word, "inggris");

            return word;
        }

        public string ChangeNewToOldSpell(string word)
        {
            word = new Regex("j", RegexOptions.IgnoreCase).Replace(word, "dj");
            word = new Regex("c", RegexOptions.IgnoreCase).Replace(word, "tj");
            word = new Regex("ny", RegexOptions.IgnoreCase).Replace(word, "nj");
            word = new Regex("sy", RegexOptions.IgnoreCase).Replace(word, "sj");
            word = new Regex("kh", RegexOptions.IgnoreCase).Replace(word, "ch");
            word = new Regex("y", RegexOptions.IgnoreCase).Replace(word, "j");
            // special cases:
            word = new Regex("konsekuen", RegexOptions.IgnoreCase).Replace(word, "konsekwen");
            word = new Regex("putra", RegexOptions.IgnoreCase).Replace(word, "putera");
            word = new Regex("putri", RegexOptions.IgnoreCase).Replace(word, "puteri");
            word = new Regex("inggris", RegexOptions.IgnoreCase).Replace(word, "inggeris");

            return word;
        }

        public int SearchPhrase(string phrase)
        {
            int hits = 0;
            // Put the implementation of searching phrase (trigram/bigram words) in large Indonesian corpus here
            return hits;
        }

        public int GoogleSearch(string phrase, string error, out string suggestion)
        {
            suggestion = "";
            int hits = 0;
            string url = string.Format("https://www.google.co.id/search?q=\"{0}\"", phrase.Trim());
            Random r = new Random();
            int interval = r.Next(5000, 9000);
            Thread.Sleep(interval);
            using (ChromeDriver driver = new ChromeDriver(SeleniumFolder))
            {
                driver.Navigate().GoToUrl(url);
                try
                {
                    int.TryParse(Regex.Replace(driver.FindElement(By.XPath("//div[@id=\"resultStats\"]")).Text, "[^0-9]", ""), out hits);
                }
                catch { }
                try
                {
                    IWebElement noresultNode = driver.FindElement(By.XPath("//div[@id=\"topstuff\"]"));
                    if (noresultNode.Text.StartsWith("Hasil untuk") && noresultNode.Text.Contains("tidak ditemukan"))
                        hits = 0;
                }
                catch { }
                try
                {
                    IWebElement spellNode = driver.FindElement(By.XPath("//a[@class=\"spell\"]"));
                    var tmpA = spellNode.GetAttribute("innerHTML").Replace("&quot;", "");
                    string[] arrTmp = Regex.Split(tmpA, "<b><i>");
                    Dictionary<string, int> candidates = new Dictionary<string, int>();
                    foreach (string tmp in arrTmp)
                    {
                        if (tmp.Trim().Contains("</i></b>"))
                        {
                            int posAkhir = tmp.Trim().IndexOf("</i></b>");
                            string nline = tmp.Trim().Substring(0, posAkhir);
                            int levenshtein = EditDistance.LevenshteinDistance(nline, error, 4);
                            if (levenshtein != -1 && levenshtein <= 4)
                                candidates.Add(nline, levenshtein);
                        }
                    }
                    if (candidates.Count > 1)
                    {
                        var temp = candidates.OrderBy(p => p.Value).ToDictionary(p => p.Key, p => p.Value);
                        suggestion = temp.First().Key;
                    }
                    if (candidates.Count == 1)
                        suggestion = candidates.First().Key;

                }
                catch { }

            }

            return hits;
        }

        public List<CorrectionCandidate> GetCandidates(string spName, int key, string error, string root, string prefix, string suffix, int minSameBigramAmount, int minLengthVariant, int maxLevensthein, out string log)
        {
            // sample: call getCandidates('depat',2,0,1);
            log = "";
            List<CorrectionCandidate> lsCandidates = new List<CorrectionCandidate>();
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = MariaDBConn;
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Word", root);
                cmd.Parameters["@Word"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@MinSameBigramAmount", minSameBigramAmount);
                cmd.Parameters["@MinSameBigramAmount"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@MinLengthVariant", minLengthVariant);
                cmd.Parameters["@MinLengthVariant"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@MaxLevensthein", maxLevensthein);
                cmd.Parameters["@MaxLevensthein"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@IsLemma", 1);
                cmd.Parameters["@IsLemma"].Direction = ParameterDirection.Input;
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    string stem = dataReader["Unigram"].ToString();
                    Affixer affixer = new Affixer();
                    string sCandidate = affixer.Affixing(stem, prefix, suffix);
                    int levensthein = EditDistance.LevenshteinDistance(sCandidate, error, 2);
                    if (levensthein != -1 && levensthein <= 2)
                    {
                        CorrectionCandidate candidate = new CorrectionCandidate
                        {
                            Key = key,
                            Error = error,
                            Candidate = sCandidate,
                            SameBigramAmount = Convert.ToInt32(dataReader["SameBigramAmount"]),
                            Frequency = 0,
                            LengthDifference = Convert.ToInt32(dataReader["LengthDifference"]),
                            Levensthein = levensthein
                        };
                        lsCandidates.Add(candidate);

                    }

                }
                //close Data Reader
                dataReader.Close();

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
            conn.Close();

            // Find Frequency then Update to list:
            List<string> sCandidates = new List<string>(); ;
            foreach (CorrectionCandidate cc in lsCandidates)
            {
                sCandidates.Add(cc.Candidate);
            }
            Dictionary<string, int> dicCandidateAndFreq = GetFrequencies(sCandidates.ToArray());
            foreach (CorrectionCandidate candidate in lsCandidates)
            {
                if (dicCandidateAndFreq.ContainsKey(candidate.Candidate))
                    candidate.Frequency = dicCandidateAndFreq[candidate.Candidate];
                log += candidate.Candidate + "," + candidate.Levensthein + "," + candidate.Frequency.ToString() + ";";
            }

            log = string.Format("[{0},{1},{2}][{3}]", minSameBigramAmount, minLengthVariant, maxLevensthein, log);

            return lsCandidates;
        }


        public List<CorrectionCandidate> GetCandidates(string spName, int key, string error, int minSameBigramAmount, int minLengthVariant, int maxLevensthein, out string log)
        {
            // sample: call getCandidates('depat',2,0,1);
            log = "";
            List<CorrectionCandidate> lsCandidates = new List<CorrectionCandidate>();
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = MariaDBConn;
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Word", error);
                cmd.Parameters["@Word"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@MinSameBigramAmount", minSameBigramAmount);
                cmd.Parameters["@MinSameBigramAmount"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@MinLengthVariant", minLengthVariant);
                cmd.Parameters["@MinLengthVariant"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@MaxLevensthein", maxLevensthein);
                cmd.Parameters["@MaxLevensthein"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@IsLemma", -1);
                cmd.Parameters["@IsLemma"].Direction = ParameterDirection.Input;
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    CorrectionCandidate candidate = new CorrectionCandidate
                    {
                        Key = key,
                        Error = error,
                        Candidate = dataReader["Unigram"].ToString(),
                        SameBigramAmount = Convert.ToInt32(dataReader["SameBigramAmount"]),
                        Frequency = Convert.ToInt32(dataReader["Frequency"]),
                        LengthDifference = Convert.ToInt32(dataReader["LengthDifference"]),
                        Levensthein = Convert.ToInt32(dataReader["Levenshtein"])
                    };
                    lsCandidates.Add(candidate);
                    log += candidate.Candidate + "," + candidate.Levensthein + "," + candidate.Frequency.ToString() + ";";
                }
                //close Data Reader
                dataReader.Close();
                log = string.Format("[{0},{1},{2}][{3}]", minSameBigramAmount, minLengthVariant, maxLevensthein, log);

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
            conn.Close();
            return lsCandidates;
        }

        public int GetSearchPhraseHits(string phrase, bool isUsingGoogle)
        {
            string suggestion; string error = "";
            int hits = 0;
            if (isUsingGoogle)
                hits = GoogleSearch(phrase, error, out suggestion);
            else
                hits = SearchPhrase(phrase);
            return hits;
        }
        private int GetTrigramSearchHits(Dictionary<int, string> deHyphenateTokens, CorrectionCandidate candidate, bool isUsingGoogle, out string log)
        {
            int hits = 0;
            log = "";
            string phrase = "";
            Correction correct = new Correction();
            if (candidate.Key - 2 >= deHyphenateTokens.Keys.Min() && candidate.Key - 1 >= deHyphenateTokens.Keys.Min())
            {
                phrase = string.Format("{0} {1} {2}", correct.ChangeOldToNewSpell(deHyphenateTokens[candidate.Key - 2]), correct.ChangeOldToNewSpell(deHyphenateTokens[candidate.Key - 1]), candidate.Candidate);
                int hit = GetSearchPhraseHits(phrase, isUsingGoogle);
                log += string.Format("({0},{1})", phrase, hit);
                hits += hit;
            }
            if (candidate.Key - 1 >= deHyphenateTokens.Keys.Min() && deHyphenateTokens.Keys.Max() >= candidate.Key + 1)
            {
                phrase = string.Format("{0} {1} {2}", correct.ChangeOldToNewSpell(deHyphenateTokens[candidate.Key - 1]), candidate.Candidate, correct.ChangeOldToNewSpell(deHyphenateTokens[candidate.Key + 1]));
                int hit = GetSearchPhraseHits(phrase, isUsingGoogle);
                log += string.Format("({0},{1})", phrase, hit);
                hits += hit;
            }
            if (deHyphenateTokens.Keys.Max() >= candidate.Key + 1 && deHyphenateTokens.Keys.Max() >= candidate.Key + 2)
            {
                phrase = string.Format("{0} {1} {2}", candidate.Candidate, correct.ChangeOldToNewSpell(deHyphenateTokens[candidate.Key + 1]), correct.ChangeOldToNewSpell(deHyphenateTokens[candidate.Key + 2]));
                int hit = GetSearchPhraseHits(phrase, isUsingGoogle);
                log += string.Format("({0},{1})", phrase, hit);
                hits += hit;
            }
            candidate.Hits = hits;
            return hits;
        }
        private int GetBigramSearchHits(Dictionary<int, string> deHyphenateTokens, CorrectionCandidate candidate, bool isUsingGoogle, out string log)
        {
            Correction correct = new Correction();
            int hits = 0;
            log = "";
            string phrase = "";
            if (candidate.Key - 1 >= deHyphenateTokens.Keys.Min())
            {
                phrase = string.Format("{0} {1}", correct.ChangeOldToNewSpell(deHyphenateTokens[candidate.Key - 1]), candidate.Candidate);
                int hit = GetSearchPhraseHits(phrase, isUsingGoogle);
                log += string.Format("({0},{1})", phrase, hit);
                hits += hit;
            }
            if (deHyphenateTokens.Keys.Max() >= candidate.Key + 1)
            {
                phrase = string.Format("{0} {1}", candidate.Candidate, correct.ChangeOldToNewSpell(deHyphenateTokens[candidate.Key + 1]));
                int hit = GetSearchPhraseHits(phrase, isUsingGoogle);
                log += string.Format("({0},{1})", phrase, hit);
                hits += hit;
            }
            candidate.Hits = hits;
            return hits;
        }

        public int getMinSameBigramAmount(int wordLength)
        {
            int jumlahBigram = wordLength - 1;
            int minJumlahBigramBedaDiTengahKata = 3; // setara 2 karakter berbeda di tengah kata
            if (jumlahBigram == 1 || jumlahBigram == 2 || jumlahBigram == 3)
                return 1;
            else if (jumlahBigram == 4)
                return 2;
            else if (jumlahBigram == 5)
                return 3;
            else
                return jumlahBigram - minJumlahBigramBedaDiTengahKata;
        }

        public string GetSuggestion(string sPNameForGetCandidates, Dictionary<int, string> deHyphenateTokens, int key, string value,  out string log, bool withStemAndAffixCorr, bool withSearch )
        {
            log = "";
            string error = value;

            string suggestion ;
            int minSameBigramAmount = getMinSameBigramAmount(error.Length);
            int minLengthVariant = 1; // batasi error Addition/Deletion =1 karakter.
            int maxLevensthein = 2;
            int minCandidates = 10;
            int maxCandidates = 10;
            string candidatesLog = "";
            List<CorrectionCandidate> candidates = new List<CorrectionCandidate>();
            candidates = GetCandidates(sPNameForGetCandidates, key, error, minSameBigramAmount, minLengthVariant, maxLevensthein, out candidatesLog);
            if (candidates.Count < minCandidates)
            {
                // bila tidak ada juga kurangi minSameBigramAmount : 
                if (minSameBigramAmount - 1 > 0)
                {
                    candidates = GetCandidates(sPNameForGetCandidates, key, error, minSameBigramAmount - 1, minLengthVariant, maxLevensthein, out candidatesLog);
                    if (candidates.Count < minCandidates && withStemAndAffixCorr)
                    {
                        // bila tidak ada juga ambil dari stem :
                        Stemmer stemmer = new Stemmer();
                        string prefix; string suffix;
                        string errorRoot = stemmer.StemmingWithoutChecking(error, out prefix, out suffix);
                        if (error != errorRoot && errorRoot.Length>=3)
                        {
                            minSameBigramAmount = getMinSameBigramAmount(errorRoot.Length);
                            if (minSameBigramAmount > 0)
                            {
                                var temp1 = GetCandidates(sPNameForGetCandidates, key, error, errorRoot, prefix, suffix, minSameBigramAmount, minLengthVariant, maxLevensthein, out candidatesLog);
                                candidates.AddRange(temp1.Where(x => candidates.FirstOrDefault(y => y.Candidate == x.Candidate) == null).ToList());
                            }
                        }
                        // bila tidak ada juga coba koreksi afiksnya :
                        string candidatesFromAffixCorrLog;
                        var temp2 = GetCandidatesFromAffixCorrection(key, error, out candidatesFromAffixCorrLog);
                        candidates.AddRange(temp2.Where(x => candidates.FirstOrDefault(y => y.Candidate == x.Candidate) == null).ToList());
                        candidatesLog += candidatesFromAffixCorrLog;

                    }
                }
            }
            if (candidates.Count == 0)
            {
                log = "No candidates";
                return value;    // Asumsi kalau itu kata yang benar dan tidak tercover di kamus
            }
            else if (candidates.Count == 1)
            {
                log += candidatesLog;
                return candidates[0].Candidate;
            }
            else
            {
                var candidatesEqError = candidates.Where(o => o.Candidate.Equals(error,StringComparison.OrdinalIgnoreCase)).ToList();
                if (candidatesEqError.Count>0)
                {
                    log = "candidate=error";
                    return candidatesEqError[0].Candidate;    // bila ada kandidat yg sama persis dg error maka jadikan itu saran;
                }
                if (candidates.Count > maxCandidates)
                {
                    //batasi jumlah kandidat by smallest levenstein then by Frequency:
                    var temp = candidates.OrderBy(o => o.Levensthein).ThenByDescending(o => o.Frequency).ToList();
                    candidates = new List<CorrectionCandidate>();   //reset
                    for (int i = 0; i < maxCandidates; i++)
                    {
                        candidates.Add(temp[i]);
                    }
                }


                string logsearch;
                List<CorrectionCandidate> SortedCandidates = new List<CorrectionCandidate>();
                if (withSearch)
                {
                    // for each candidate search using Trigram:
                    foreach (CorrectionCandidate candidate in candidates)
                    {
                        candidate.NGram = 3;
                        candidate.Hits = GetTrigramSearchHits(deHyphenateTokens, candidate, false, out logsearch);
                        log += string.Format("[{0}:{1},{2}]", candidate.Candidate, logsearch, candidate.Hits);
                    }

                    // if no one has hits > 0 then search bigram for each candidate:
                    if (candidates.Where(p => p.Hits > 0).Count() == 0)
                    {
                        foreach (CorrectionCandidate candidate in candidates)
                        {
                            // Search using Bigram:
                            candidate.NGram = 2;
                            candidate.Hits = GetBigramSearchHits(deHyphenateTokens, candidate, false, out logsearch);
                            log += string.Format("[{0}:{1},{2}]", candidate.Candidate, logsearch, candidate.Hits);
                        }
                        SortedCandidates = candidates.OrderByDescending(o => o.Hits).ThenBy(o => o.Levensthein).ThenByDescending(o => o.Frequency).ToList();
                    }
                    else // if trigram search results ones that has hits > 0 then sorting by largest hits, then smallest levenstein:
                    {
                        SortedCandidates = candidates.OrderByDescending(o => o.Hits).ThenBy(o => o.Levensthein).ThenByDescending(o => o.Frequency).ToList();
                    }
                }
                else
                    SortedCandidates = candidates.OrderBy(o => o.Levensthein).ThenByDescending(o => o.Frequency).ToList();

                // FINAL SUGGESTION:
                suggestion = SortedCandidates[0].Candidate;

                log = candidatesLog + log;
                return suggestion;
            }
        }

        public string GetGoogleSuggestionFromTrigram(Dictionary<int, string> deHyphenateTokens, int key, out int totalHits, out string log)
        {
            Correction corr = new Correction();
            log = "";
            Dictionary<int, string> suggestions = new Dictionary<int, string>();
            string suggestion = deHyphenateTokens[key];
            string googleSuggestion = "";
            totalHits = 0;
            string phrase;
            if (key - 2 >= deHyphenateTokens.Keys.Min() && key - 1 >= deHyphenateTokens.Keys.Min())
            {
                phrase = corr.ChangeOldToNewSpell(string.Format("{0} {1} {2}", deHyphenateTokens[key - 2], deHyphenateTokens[key - 1], deHyphenateTokens[key]));
                int hits = GoogleSearch(phrase, deHyphenateTokens[key], out googleSuggestion);
                if (googleSuggestion != "")
                {
                    log += string.Format("({0},'{1}',{2})", googleSuggestion, phrase, hits);
                    return googleSuggestion;
                }
                log += string.Format("({0},'{1}',{2})", googleSuggestion, phrase, hits);
                totalHits += hits;
            }
            if (key - 1 >= deHyphenateTokens.Keys.Min() && deHyphenateTokens.Keys.Max() >= key + 1)
            {
                phrase = corr.ChangeOldToNewSpell(string.Format("{0} {1} {2}", deHyphenateTokens[key - 1], deHyphenateTokens[key], deHyphenateTokens[key + 1]));
                int hits = GoogleSearch(phrase, deHyphenateTokens[key], out googleSuggestion);
                if (googleSuggestion != "")
                {
                    log += string.Format("({0},'{1}',{2})", googleSuggestion, phrase, hits);
                    return googleSuggestion;
                }
                log += string.Format("({0},'{1}',{2})", googleSuggestion, phrase, hits);
                totalHits += hits;
            }
            if (deHyphenateTokens.Keys.Max() >= key + 1 && deHyphenateTokens.Keys.Max() >= key + 2)
            {
                phrase = corr.ChangeOldToNewSpell(string.Format("{0} {1} {2}", deHyphenateTokens[key], deHyphenateTokens[key + 1], deHyphenateTokens[key + 2]));
                int hits = GoogleSearch(phrase, deHyphenateTokens[key], out googleSuggestion);
                if (googleSuggestion != "")
                {
                    log += string.Format("({0},'{1}',{2})", googleSuggestion, phrase, hits);
                    return googleSuggestion;
                }
                log += string.Format("({0},'{1}',{2})", googleSuggestion, phrase, hits);
                totalHits += hits;
            }
            return suggestion;
        }

        public string GetGoogleSuggestionFromBigram(Dictionary<int, string> deHyphenateTokens, int key, out int totalHits, out string log)
        {
            Correction corr = new Correction();
            log = ""; totalHits = 0;
            Dictionary<int, string> suggestions = new Dictionary<int, string>();
            string suggestion = deHyphenateTokens[key];
            string googleSuggestion = "";
            string phrase;
            if (key - 1 >= deHyphenateTokens.Keys.Min())
            {
                phrase = corr.ChangeOldToNewSpell(string.Format("{0} {1}", deHyphenateTokens[key - 1], deHyphenateTokens[key]));
                int hits = GoogleSearch(phrase, deHyphenateTokens[key], out googleSuggestion);
                if (googleSuggestion != "")
                {
                    log += string.Format("({0},'{1}',{2})", googleSuggestion, phrase, hits);
                    return googleSuggestion;
                }
                log += string.Format("({0},'{1}',{2})", googleSuggestion, phrase, hits);
                totalHits += hits;
            }
            if (deHyphenateTokens.Keys.Max() >= key + 1)
            {
                phrase = corr.ChangeOldToNewSpell(string.Format("{0} {1}", deHyphenateTokens[key], deHyphenateTokens[key + 1]));
                int hits = GoogleSearch(phrase, deHyphenateTokens[key], out googleSuggestion);
                if (googleSuggestion != "")
                {
                    log += string.Format("({0},'{1}',{2})", googleSuggestion, phrase, hits);
                    return googleSuggestion;
                }
                log += string.Format("({0},'{1}',{2})", googleSuggestion, phrase, hits);
                totalHits += hits;
            }

            return suggestion;
        }

        public List<CorrectionCandidate> GetCandidatesFromAffixCorrection(int key, string error, out string log)
        {
            //[prefix-1] + [prefix-2] + root + [suffix] + [possessive] + [particle]
            //1.Particles: -lah, -kah, -pun, -tah.
            //2.Possessives: -ku, -mu, -nya.
            //3.Suffixes: -i, -an, -kan.
            //4.Prefixes: meN -, beN -, peN -, teN -, di -, ke -, se -.
            log = "";
            List<string> Prefixs = new List<string>() { "", "di", "ke", "se", "ber", "bel", "be", "te", "ter", "me", "mem", "men", "meng", " menge", "meny", "pe", "per", "pem", "pen", "peng", "penge", "peny", "pel", "memper", "mempel", "menter", "member", "diper", "diter", "dipel", " diber", "keber", "keter" };
            List<string> baseSuffixes = new List<string>() { "i", "an", "kan" };
            List<string> possessives = new List<string>() { "ku", "mu", "nya" };
            List<string> particles = new List<string>() { "lah", "kah", "pun", "tah" };
            //List<string> akhirans = new List<string>() { "", "i", "an", "kan", "ku", "mu", "nja", "lah", "kah", "pun", "tah", "iku", "imu", "inja", "anku", "anmu", "annja", "kanku", "kanmu", "kannja", "ilah", "ikah", "ipun", "itah", "anlah", "ankah", "anpun", "antah", "kanlah", "kankah", "kanpun", "kantah", "kulah", "kukah", "kupun", "kutah", " mulah", "mukah", "mupun", "mutah", " nyalah", "nyakah", "nyapun", "nyatah", "ikulah", "ikukah", "ikupun", "ikutah", "imulah", "imukah", "imupun", "imutah", "inyalah", "inyakah", "inyapun", "inyatah", "ankulah", "ankukah", "ankupun", "ankutah", "anmulah", "anmukah", "anmupun", "anmutah", "annyalah", "annyakah", "annyapun", "annyatah", "kankulah", "kankukah", "kankupun", "kankutah", "kanmulah", "kanmukah", "kanmupun", "kanmutah", "kannjalah", "kannjakah", "kannjapun", "kannjatah" };
            List<string> suffixes = new List<string>() { "" };
            suffixes.AddRange(baseSuffixes);
            suffixes.AddRange(possessives);
            suffixes.AddRange(particles);

            foreach (string s in baseSuffixes)
            {
                foreach (string po in possessives)
                {
                    suffixes.Add(s + po);  // contoh: diperbaikinya
                    foreach (string pa in particles)
                    {
                        suffixes.Add(s + pa); //contoh:dipelukanmulah
                    }
                }
            }

            foreach (string s in baseSuffixes)
            {
                foreach (string pa in particles)
                {
                    suffixes.Add(s + pa);   // contoh: pertahankanlah
                }
            }


            List<CorrectionCandidate> candidates = new List<CorrectionCandidate>();
            Dictionary<string, int> dicCandidates = new Dictionary<string, int>();
            Correction correct = new Correction();

            string rootWord = GetRootWord(correct.ChangeOldToNewSpell(error));
            if (rootWord == "" || rootWord.Length < 3)
                return candidates;

            foreach (string prefix in Prefixs)
            {
                foreach (string suffix in suffixes)
                {
                    Affixer affixer = new Affixer();
                    string candidate = correct.ChangeNewToOldSpell(affixer.Affixing(correct.ChangeNewToOldSpell(rootWord), prefix, suffix));
                    int levenshtein = EditDistance.LevenshteinDistance(candidate, error, 2);
                    if (levenshtein != -1 && levenshtein <= 2)
                    {
                        if (!dicCandidates.ContainsKey(correct.ChangeOldToNewSpell(candidate)))
                        {
                            dicCandidates.Add(correct.ChangeOldToNewSpell(candidate), levenshtein);
                        }
                    }
                }
            }
            if (dicCandidates.Count == 0)
                return candidates;

            Dictionary<string, int> dicCandidateAndFreq = GetFrequencies(dicCandidates.Keys.ToArray());

            foreach (KeyValuePair<string, int> can in dicCandidates)
            {
                int frequency = 0;
                if (dicCandidateAndFreq.ContainsKey(can.Key))
                    frequency = dicCandidateAndFreq[can.Key];
                CorrectionCandidate corrcandidate = new CorrectionCandidate
                {
                    Key = key,
                    Error = error,
                    Candidate = can.Key,
                    SameBigramAmount = -1,
                    Frequency = frequency,
                    LengthDifference = Math.Abs(can.Key.Length - correct.ChangeOldToNewSpell(error).Length),
                    Levensthein = can.Value
                };
                candidates.Add(corrcandidate);
                log += can.Key + "," + can.Value + "," + frequency.ToString() + ";";
            }

            if (log.Length > 0)
                log = "[" + log + "]";
            return candidates;
        }

        public int InsertOCRAndTruth(string articleFile, int key, string oCR, string truth)
        {
            int res = 0;
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = MariaDBConn;
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = @"
                    INSERT INTO analysis (ArticleFile, Kunci, OCR, Truth, NCorrection, Correction, WOStemCorrection, WOSearchCorrection, WOStemSearchCorrection, HunspellCorrection, GooglePureCorrection )  
                    VALUES(@ArticleFile, @Key, @OCR, @Truth,'-','-','-','-','-','-','-')
                    ON DUPLICATE KEY UPDATE Kunci=@Key ;";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ArticleFile", articleFile);
                cmd.Parameters.AddWithValue("@Key", key);
                cmd.Parameters.AddWithValue("@OCR", oCR);
                cmd.Parameters.AddWithValue("@Truth", truth);
                res = cmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
            conn.Close();
            return res;
        }


        public int UpdateFields(string articleFile, int key, Dictionary<string, string> fieldValues)
        {
            int res = 0;
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = MariaDBConn;
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                string qry;
                qry = "Update Analysis Set ";
                foreach (var fieldVal in fieldValues)
                {
                    qry += string.Format("{0} = @{1}, ", fieldVal.Key, fieldVal.Key);
                }
                qry = qry.Remove(qry.LastIndexOf(","), 1);
                qry += "Where ArticleFile=@ArticleFile And Kunci=@Key";
                cmd.CommandText = qry;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ArticleFile", articleFile);
                cmd.Parameters.AddWithValue("@Key", key);
                foreach (var fieldVal in fieldValues)
                {
                    cmd.Parameters.AddWithValue("@" + fieldVal.Key, fieldVal.Value);
                }
                res = cmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
            conn.Close();
            return res;
        }

        private string GetRootWord(string error)
        {
            string rootWord = "";
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = MariaDBConn;
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = @"
                select unigram from unigrams where isLemma=1 and @error like concat('%',unigram,'%') order by length(unigram) desc limit 1;";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@error", error);
                var obj = cmd.ExecuteScalar();
                rootWord = Convert.ToString(obj);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
            conn.Close();
            return rootWord;
        }

        private Dictionary<string, int> GetFrequencies(string[] arrayOfUnigram)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = MariaDBConn;
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = @"select unigram,Frequency from unigrams where unigram in ('" + string.Join("','", arrayOfUnigram) + "') ;";
                cmd.CommandType = CommandType.Text;
                //cmd.Parameters.AddWithValue("@listOfUnigrams", "'" + string.Join("','", arrayOfUnigram) + "'");
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    dic.Add(dataReader["Unigram"].ToString(), Convert.ToInt32(dataReader["Frequency"]));
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
            conn.Close();
            return dic;
        }


    }

    public class CorrectionCandidate
    {
        public int Key { get; set; }
        public string Error { get; set; }
        public string Candidate { get; set; }
        public int SameBigramAmount { get; set; }
        public int Frequency { get; set; }
        public int LengthDifference { get; set; }
        public int Levensthein { get; set; }
        public int Hits { get; set; }
        public int NGram { get; set; }

    }

}
