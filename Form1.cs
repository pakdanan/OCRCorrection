using NHunspell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using Nest;
using System.Xml;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace WinOCRCorrection
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }


        Dictionary<int, string> errors;
        Dictionary<int, string> deHyphenateTokens;
        string articleFileName; string articleFileGT; string articleFile;

        int indexTimerGoogle;

        private string getSPNameForGetCandidates()
        {
            return "getCandidates";
        }
        private string getSQLQueryToCheckUnigram()
        {
            return "SELECT Frequency FROM unigrams WHERE Confidence=10 AND unigram=@unigram";
        }
        private string getFieldNameFromOption()
        {
            string fieldName = "";
            if (cbMethod.SelectedItem.ToString() == "Check:Dictionary, Suggestion:Google")
                fieldName = "GooglePureCorrection";
            if (cbMethod.SelectedItem.ToString() == "Check:Dictionary, Candidates:SimilarBigram+Stem&AffixCorrection, Suggestion:PhraseSearch")
                fieldName = "NCorrection";
            if (cbMethod.SelectedItem.ToString() == "Check:Dictionary+Stemmer, Candidates:SimilarBigram+Stem&AffixCorrection, Suggestion:PhraseSearch")
                fieldName = "Correction";
            if (cbMethod.SelectedItem.ToString() == "Check:Dictionary, Candidates:SimilarBigram+Stem&AffixCorrection, Suggestion:Distance+Freq")
                fieldName = "WOSearchCorrection";
            if (cbMethod.SelectedItem.ToString() == "Check:Dictionary, Candidates:SimilarBigram, Suggestion:PhraseSearch")
                fieldName = "WOStemCorrection";
            if (cbMethod.SelectedItem.ToString() == "Check:Dictionary, Candidates:SimilarBigram, Suggestion:Distance+Freq")
                fieldName = "WOStemSearchCorrection";
            if (cbMethod.SelectedItem.ToString() == "Hunspell")
                fieldName = "HunspellCorrection";
            return fieldName;
        }
        private bool getWithSearchParamFromOption()
        {
            if (cbMethod.SelectedItem.ToString().Contains("Suggestion:Phrase Search"))
                return true;
            else
                return false;
        }
        private bool getWithStemAndAffixCorrParamFromOption()
        {
            if (cbMethod.SelectedItem.ToString().Contains("Stem&Affix Correction"))
                return true;
            else
                return false;
        }
        private void btCorrectIt_Click(object sender, EventArgs e)
        {
            if (cbMethod.SelectedItem.ToString() == "-- Choose method --")
            {
                MessageBox.Show("Choose method first", "", MessageBoxButtons.OK);
                return;
            }
            if (!File.Exists(txOCR.Text.Trim()))
            {
                MessageBox.Show("Browse file first", "", MessageBoxButtons.OK);
                return;
            }

            ResetControls(false);

            articleFile = txOCR.Text.Trim();
            Correction correction = new Correction();
            Stemmer stemmer = new Stemmer();

            // DeHyphenate and clean text:
            string dehyphenatedText = correction.DeHyphenate(articleFile);
            rtbOCR.Text = dehyphenatedText;

            // for analysis:
            string dehyphenatedTextGT = "";
            if (File.Exists(articleFile.Substring(0, articleFile.Length - 4) + "GT.txt"))
                articleFileGT = articleFile.Substring(0, articleFile.Length - 4) + "GT.txt";
            articleFileName = Path.GetFileName(articleFile);
            if (!string.IsNullOrEmpty(articleFileGT))
                dehyphenatedTextGT = correction.DeHyphenate(articleFileGT);

            // tokenize:
            deHyphenateTokens = correction.GetTokensFromText(dehyphenatedText);

            Regex rgx = new Regex("[^a-zA-Z]"); //omit all non alphabet word And clean word from non alphabet: 

            // for analysis:
            Dictionary<int, string> deHyphenateTokensGT = new Dictionary<int, string>();
            if (!string.IsNullOrEmpty(articleFileGT))
            {
                deHyphenateTokensGT = correction.GetTokensFromText(dehyphenatedTextGT);
                foreach (KeyValuePair<int, string> token in deHyphenateTokens)
                {
                    correction.InsertOCRAndTruth(articleFileName, token.Key, rgx.Replace(token.Value, ""), rgx.Replace(deHyphenateTokensGT[token.Key], ""));
                }
            }

            // Omit non character,single char, All Capitals word, and clean word from non alphabet:
            var tmp = deHyphenateTokens.Where(p => p.Value.Length > 1).ToDictionary(p => p.Key, p => p.Value);
            tmp = tmp.Where(p => p.Value.Any(Char.IsLetter)).ToDictionary(p => p.Key, p => rgx.Replace(p.Value, ""));
            Dictionary<int, string> cleanTokens = tmp.Where(p => !p.Value.All(Char.IsUpper)).ToDictionary(p => p.Key, p => p.Value);



            // Find Suggestion:
            if (cbMethod.SelectedItem.ToString().EndsWith("Hunspell"))
            {
                string hunspellLog = "";
                // find Suggestion using Hunspell:
                foreach (KeyValuePair<int, string> err in cleanTokens)
                {
                    string errInNewSpell = correction.ChangeOldToNewSpell(err.Value).ToLowerInvariant();
                    List<string> hunspellSuggestions = new List<string>();
                    using (SpellEngine engine = new SpellEngine())
                    {
                        LanguageConfig idConfig = new LanguageConfig();
                        idConfig.LanguageCode = "id";
                        idConfig.HunspellAffFile = "id_ID.aff";
                        idConfig.HunspellDictFile = "id_ID.dic";
                        idConfig.HunspellKey = "";
                        engine.AddLanguage(idConfig);
                        bool correct = engine["id"].Spell(errInNewSpell);
                        if (!correct)
                        {
                            hunspellSuggestions = engine["id"].Suggest(errInNewSpell);
                            if (hunspellSuggestions.Count > 0 && err.Value != correction.ChangeNewToOldSpell(hunspellSuggestions[0]))
                                deHyphenateTokens[err.Key] = "[" + correction.ChangeNewToOldSpell(hunspellSuggestions[0]) + "]";
                            // for analysis:
                            if (!string.IsNullOrEmpty(articleFileGT))
                                correction.UpdateFields(articleFileName, err.Key, new Dictionary<string, string> { { getFieldNameFromOption(), rgx.Replace(deHyphenateTokens[err.Key], "") }, { getFieldNameFromOption().Replace("Correction", "Log"), hunspellLog } });
                        }
                        else
                        {
                            // for analysis:
                            if (!string.IsNullOrEmpty(articleFileGT))
                                correction.UpdateFields(articleFileName, err.Key, new Dictionary<string, string> { { getFieldNameFromOption(), err.Value }, { getFieldNameFromOption().Replace("Correction", "Log"), err.Value + " is correct" } });
                        }
                    }

                }
                ResetControls(true);
                return;
            }


            //check only unique word (assumption:duplicate word is correct word) :
            Dictionary<int, string> checkTokens = cleanTokens;
            var duplicateValues = checkTokens.GroupBy(x => x.Value).Where(x => x.Count() > 1);

            List<int> duplicateKeys = new List<int>();
            foreach (var item in checkTokens)
            {
                foreach (var dup in duplicateValues)
                {
                    if (item.Value == dup.Key)
                        duplicateKeys.Add(item.Key);
                }
            }
            foreach (var dupkey in duplicateKeys)
            {
                // for analysis
                if (!string.IsNullOrEmpty(articleFileGT))
                    correction.UpdateFields(articleFileName, dupkey, new Dictionary<string, string> { { "NCorrection", checkTokens[dupkey] }, { "NLog", "Duplicate" }, { "Correction", checkTokens[dupkey] }, { "Log", "Duplicate" }, { "WOSearchCorrection", checkTokens[dupkey] }, { "WOSearchLog", "Duplicate" }, { "WOStemCorrection", checkTokens[dupkey] }, { "WOStemLog", "Duplicate" }, { "WOStemSearchCorrection", checkTokens[dupkey] }, { "WOStemSearchLog", "Duplicate" }, { "GooglePureCorrection", checkTokens[dupkey] }, { "GooglePureLog", "Duplicate" } });
                checkTokens.Remove(dupkey);
            }


            //Check Word using Dictionary(kbbi+kompas pilihan, entitas kota,negara, nama pahlawan dari wiki ):
            errors = new Dictionary<int, string>();
            foreach (KeyValuePair<int, string> token in checkTokens)
            {
                // change Soewandi to Modern Spelling:
                string wordToCheck = correction.ChangeOldToNewSpell(token.Value).ToLowerInvariant();

                // check word in Dictionary and Add to Error list if not there:
                int frequency;
                if (!correction.CheckUnigram(wordToCheck, getSQLQueryToCheckUnigram(), out frequency))
                {
                    if (cbMethod.SelectedItem.ToString().Contains("Stemmer"))
                    {
                        // check again its stem in dictionary :
                        string stem = stemmer.Stemming(wordToCheck);
                        if (wordToCheck != stem && stemmer.checkStem(stem))
                        {
                            // for analysis
                            if (!string.IsNullOrEmpty(articleFileGT))
                                correction.UpdateFields(articleFileName, token.Key, new Dictionary<string, string> { { getFieldNameFromOption(), token.Value }, { getFieldNameFromOption().Replace("Correction", "Log"), stem + " is word" } });
                        }
                        else // jika tidak ada di kamus:
                            errors.Add(token.Key, wordToCheck);
                    }
                    else
                        errors.Add(token.Key, wordToCheck);

                }
                else // jika ada di kamus: 
                {
                    // for analysis
                    if (!string.IsNullOrEmpty(articleFileGT))
                        correction.UpdateFields(articleFileName, token.Key, new Dictionary<string, string> { { getFieldNameFromOption(), token.Value }, { getFieldNameFromOption().Replace("Correction", "Log"), wordToCheck + " is correct" } });
                }
            }


            // Find Suggestion:
            if (cbMethod.SelectedItem.ToString().EndsWith("Google"))
            {
                timerGoogle.Enabled = true;
                indexTimerGoogle = 0;
                return;
            }
            else
            {

                foreach (KeyValuePair<int, string> err in errors)
                {
                    //get suggestion:
                    string log; string suggestion;
                    suggestion = correction.GetSuggestion(getSPNameForGetCandidates(), deHyphenateTokens, err.Key, err.Value, out log, getWithStemAndAffixCorrParamFromOption(), getWithSearchParamFromOption());

                    // Change suggestion back to Old Spell if any suggestions:
                    if (log != "No candidates" )
                        suggestion = correction.ChangeNewToOldSpell(suggestion);

                    // update token dic with suggestion:
                    if (!suggestion.Equals(deHyphenateTokens[err.Key],StringComparison.OrdinalIgnoreCase))
                        deHyphenateTokens[err.Key] = "[" + suggestion + "]";

                    // for analysis:
                    if (!string.IsNullOrEmpty(articleFileGT))
                        correction.UpdateFields(articleFileName, err.Key, new Dictionary<string, string> { { getFieldNameFromOption(), suggestion }, { getFieldNameFromOption().Replace("Correction", "Log"), log } });

                }
                ResetControls(true);
            }


        }

        private void ResetControls(bool isFinish)
        {
            if (isFinish)
            {
                if (cbMethod.SelectedItem.ToString() == "Google")
                    timerGoogle.Enabled = false;
                DisplayTextToEditor();
                btCorrectIt.Text = "Correct";
                Cursor = Cursors.Default;
            }
            else
            {
                rtbCorr.Clear();
                Cursor = Cursors.WaitCursor;
                btCorrectIt.Text = "Process..";
                toolStripStatusLabel2.Text = "";
            }
        }

        private void timerGoogle_Tick(object sender, EventArgs e)
        {
            timerGoogle.Enabled = false;

            Correction correction = new Correction();
            // find Suggestion using Google:

            int Key = errors.ElementAtOrDefault(indexTimerGoogle).Key;
            if (Key == 0)
            {
                ResetControls(true);
                return;
            }

            string logGoogle; int totalHits;
            string googleSuggestion = correction.GetGoogleSuggestionFromTrigram(deHyphenateTokens, Key, out totalHits, out logGoogle);
            // jika suggestion sama dengan err & totalhits kosong (artinya pencarian tidak ada hasil) coba search bigram:
            if (googleSuggestion == deHyphenateTokens[Key] && totalHits == 0)
            {
                string logGoogleBigram;
                googleSuggestion = correction.GetGoogleSuggestionFromBigram(deHyphenateTokens, Key, out totalHits, out logGoogleBigram);
                logGoogle += logGoogleBigram;
            }
            // jika suggestion berbeda dg err maka Change suggestion back to Old Spell:
            if (googleSuggestion != deHyphenateTokens[Key])
            {
                googleSuggestion = correction.ChangeNewToOldSpell(googleSuggestion);
                // update token dic with suggestion:
                deHyphenateTokens[Key] = "[" + googleSuggestion + "]";
            }
            else
            {
                googleSuggestion = deHyphenateTokens[Key];
                logGoogle = deHyphenateTokens[Key] + " is correct";
            }

            // for analysis:
            if (!string.IsNullOrEmpty(articleFileGT))
                correction.UpdateFields(articleFileName, Key, new Dictionary<string, string> { { getFieldNameFromOption(), googleSuggestion }, { getFieldNameFromOption().Replace("Correction", "Log"), logGoogle } });

            indexTimerGoogle++;
            Random r = new Random();
            int interval = r.Next(7000, 13000);
            timerGoogle.Interval = interval;
            timerGoogle.Enabled = true;

        }

        private void btOpenOCR_Click(object sender, EventArgs e)
        {

            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txOCR.Text = openFile.FileName;
                if (File.Exists(txOCR.Text))
                {
                    rtbOCR.Text = File.ReadAllText(txOCR.Text);
                    rtbCorr.Clear();
                }
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            cbMethod.Items.Add("-- Choose method --");
            cbMethod.Items.Add("Check:Dictionary, Suggestion:Google");
            cbMethod.Items.Add("Check:Dictionary, Candidates:SimilarBigram + Stem&Affix Correction, Suggestion:PhraseSearch");
            cbMethod.Items.Add("Check:Dictionary+Stemmer, Candidates:SimilarBigram+Stem&AffixCorrection, Suggestion:PhraseSearch");
            cbMethod.Items.Add("Check:Dictionary, Candidates:SimilarBigram+Stem&AffixCorrection, Suggestion:Distance+Freq");
            cbMethod.Items.Add("Check:Dictionary, Candidates:SimilarBigram, Suggestion:Phrase Search");
            cbMethod.Items.Add("Check:Dictionary, Candidates:SimilarBigram, Suggestion:Distance+Freq");
            cbMethod.Items.Add("Hunspell");

            cbMethod.SelectedIndex = 0;
            toolStripStatusLabel1.Text = "OCR Error Correction " + Convert.ToChar(0169) + " 2018 Danan Purwantoro   ";

        }

        private void DisplayTextToEditor()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var s in deHyphenateTokens.Values)
            {
                sb.Append(s); sb.Append(" ");
            }
            rtbCorr.Text = sb.ToString();

            int counter = 0;
            //highlight correction:
            foreach (string token in rtbCorr.Text.Split(' '))
            {
                if (token.StartsWith("[") && token.EndsWith("]"))
                {
                    int index = 0;
                    int lastIndex = rtbCorr.Text.LastIndexOf(token);
                    while (index <= lastIndex)
                    {
                        rtbCorr.Find(token, index, rtbCorr.TextLength, RichTextBoxFinds.None);
                        rtbCorr.SelectionBackColor = Color.Yellow;
                        index = rtbCorr.Text.IndexOf(token, index) + 1;
                        counter++;
                    }
                }
            }

            toolStripStatusLabel2.Text = "  " + counter.ToString() + " corrected";

        }

        private void btSave_Click(object sender, EventArgs e)
        {
            saveCorr.FileName = Path.GetFileNameWithoutExtension(txOCR.Text) + "Corr.txt";
            saveCorr.ShowDialog();
        }

        private void saveCorr_FileOk(object sender, CancelEventArgs e)
        {
            File.WriteAllText(saveCorr.FileName, rtbCorr.Text.Replace("[", "").Replace("]", ""));
        }

        private void btCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(rtbCorr.Text.Replace("[", "").Replace("]", ""));
        }


    }


}
