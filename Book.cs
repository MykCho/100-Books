using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _100_Books
{
    public class Book
    {
        public string Title { get; set; }
        public string LongestSentenceByNumberOfCharacters{ get; set; }
        private int MostCharactersInSentence { get; set; }
        public string ShortestSentenceByNumberOfWords { get; set; }
        public string LongestWord { get; set; }
        private int LeastWordsInSentence { get; set; }
        public Dictionary<char, int> LettersNumberOfUses = new Dictionary<char, int>();
        public Dictionary<string, int> WordsNumberOfUses = new Dictionary<string, int>();
        public bool ProcessedOK { get; set; }

        protected void SentenceProcess(string sentence, char sentenceEnding)
        {
            sentence = sentence.Trim() + sentenceEnding;
            //Console.WriteLine(sentence);

            //setting longest sentence
            if (sentence.Length > MostCharactersInSentence)
            {
                LongestSentenceByNumberOfCharacters = sentence;
                MostCharactersInSentence = sentence.Length;
            }

            //shortest sentence
            if (ShortestSentenceByNumberOfWords == null) ShortestSentenceByNumberOfWords = sentence;
            
            string[] words = sentence.Split(new char[] {' ', '.', ',', '"', '?', '!', '[', ']', '-'}, StringSplitOptions.RemoveEmptyEntries);
            
            if (LeastWordsInSentence == 0) LeastWordsInSentence = words.Length; //amount of words
            
            if (words.Length < LeastWordsInSentence && words.Length > 0)
            {
                LeastWordsInSentence = words.Length;
                ShortestSentenceByNumberOfWords = sentence;
            }

            foreach (var word in words)
            {
               if (LongestWord == null) LongestWord = word;
                if (LongestWord.Length < word.Length) LongestWord = word;
                if (WordsNumberOfUses.ContainsKey(word.ToLower())) WordsNumberOfUses[word.ToLower()]++;
                else WordsNumberOfUses[word.ToLower()] = 1;
                
                
                foreach (var letter in word)
                {
                    if (Char.IsLetter(letter)) {
                        if (LettersNumberOfUses.ContainsKey(letter)) LettersNumberOfUses[letter]++;
                        else LettersNumberOfUses[letter] = 1;
                    }
                }
            }

        }
        public Book(string fileName) {
            if (!File.Exists(fileName)) { throw new FileNotFoundException($"Init file {fileName} is not found."); }

            using (StreamReader sr = File.OpenText(fileName))
            {
                var buffer = "";
                var titleString = "Title: ";
                var endString = "*** END OF THE PROJECT GUTENBERG";
                var startString = "*** START OF THE PROJECT GUTENBERG";
                var titleIsFound = false;
                var startIsFound = false;
                var endIsFound = false;
                var sentence = "";
                var sentenceTemp = "";
                var skipRegex = "Table of Contents|\\[Illustration\\]|Contents|Chapter.+|CHAPTER.+|[A-Z]|\\s|-|\\.";
                
                var watch = new Stopwatch();
                watch.Start();

                while (sr.Peek() >= 0)
                {
                    buffer = sr.ReadLine();
                                    
                    if (buffer.Contains(endString)) endIsFound = true;

                    if (endIsFound) continue; //no need to do anything, just blankly steamroll to the end
                    
                    if (!titleIsFound && buffer.Contains(titleString)) //check is done just in case there is another such string in text
                    {
                        Title = buffer.Substring(buffer.IndexOf(titleString) + titleString.Length);
                        titleIsFound = true;
                    }

                    if (buffer.Contains(startString))
                    {
                        startIsFound = true;
                        continue; //without continue it will consider it as a sentence
                    }

//-- --- --- --- ---------------------here's where main text disemboweling should start
                    
                    if (startIsFound)
                    {
                        //this is to skip empty lines and ALLCAPS, like "CHAPTER I. MEET OUR LOVELY PROTAGONIST"
                        //will work if line consists of "Chapter IX. Whatever"
                        //making it to account for something like "III The Time Traveller Returns" will trigger false positives
                        if (Regex.Replace(buffer, skipRegex, "") == "") continue;

                        buffer = sentenceTemp + " " + buffer;
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            if (buffer[i] == '.' || buffer[i] == '?' || buffer[i] == '!')
                            {
                                var sentenceEnding = buffer[i];
                                sentence = buffer.Substring(0, i);
                                buffer = buffer.Remove(0, i+1);
                                this.SentenceProcess(sentence, sentenceEnding);
                                i = 0;
                            }

                        }
                        sentenceTemp = buffer;
                        
                    }

                    
                }

                watch.Stop();
                Console.WriteLine(watch.Elapsed.ToString());
                
            }
            
        }

    
    }
}
