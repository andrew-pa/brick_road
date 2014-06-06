using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace brick_road
{


    class Program
    {
        static Random rnd;

        static string[] file_extentions = 
        {
            ".exe", //0
            ".bat", //1
            ".txt", //2
            ".docx", //3
            ".pptx", //4
            ".xlsx", //5
            ".sys", //6
            ".com", //7
            ".sav", //8
            ".bin", //9
            ".rom", //10
            ".zip", //11
            ".wav", //12
            ".midi", //13
            ".mp3", //14
            ".mp4", //15
            ".wma", //16
            ".pdf", //17
            ".dll", //18
            ".chm", //19
            ".gif", //20
            ".png", //21
            ".jpeg" //22
        };

        public static string Shuffle(String str)
        {
            var list = new SortedList<int, char>();
            foreach (var c in str)
                list.Add(rnd.Next(), c);
            return new string(list.Values.ToArray());
        }

        static char random_char()
        {
            if (rnd.Next(0, 10) < 1) return '_';
            if(rnd.Next(0, 10) < 3)
            {
                //number
                return (char)rnd.Next(48, 57);
            }
            else
            {
                if(rnd.Next(0, 10) > 4)
                {
                    //lowercase
                    return (char)rnd.Next(97, 122);
                }
                else
                {
                    //uppercase
                    return (char)rnd.Next(65, 90);
                }
            }
        }

        static DateTime RandomFileTime(DateTime start)
        {
            DateTime d = start;
            d = d.AddMilliseconds(rnd.Next(-998, 998) + rnd.NextDouble());
            d = d.AddSeconds(rnd.Next(-58, 58) + rnd.NextDouble());
            d = d.AddMinutes(rnd.Next(-58, 58) + rnd.NextDouble());
            d = d.AddHours(rnd.Next(-58, 58) + rnd.NextDouble());
            d = d.AddDays(rnd.Next(-30, 30) + rnd.NextDouble());
            d = d.AddYears(rnd.Next(-4, 0));
            return d;
        }

        static FileAttributes RandomFileAttribs()
        {
            if (rnd.Next(0, 20) == 7)
                return FileAttributes.Hidden;
            return FileAttributes.Normal;
        }

        enum NameGenerationMode
        {
            RandomChars,
            RandomWords,
            RandomCNNWords,
            RandomNames,
        }

        static NameGenerationMode namegen_mode;


        static char[] spacers =
        {
            ' ', '_', '-', '.',
        };

        #region name words
        static string[] name_words =
        {
            "noodle",
            "bob",
            "dole",
            "banana",
            "school",
            "game",
            "sonic",
            "the",
            "a",
            "in",
            "is",
            "lame",
            "bad",
            "stupid",
            "hedgehog",
            "halo",
            "cod",
            "ness",
            "factor",
            "math",
            "essay",
            "worm",
            "worksheet",
            "review",
            "history",
            "english",
            "swag",
            "paula",
            "ant",
            "shroom",
            "jeff",
            "llama",
            "spanish",
            "hate",
            "pics",
            "docs",
            "worms",
            "gams",
            "food",
            "stuff",
            "utils",
            "stufff",
            "junl",
            "art",
            "bob dole",
            "folder",
           "jo",
           "dungeon man",
           "starman",
           "pwn",
           "dwg",
           "doge",
           "coins",
        };
        #endregion

        static List<string> cnn_words = null;
        static string cnnurl = "http://cnn.com";
        static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), ""));
        }

        static string RandomName(bool really_bad_hack_please_remove = false)
        {
            if (cnn_words == null)
            {
                WebClient wc = new WebClient();
                string cnn_text = wc.DownloadString("http://cnn.com");
                string[] cnn_words_raw = cnn_text.Split(' ');
                cnn_words = new List<string>();
                foreach(string w in cnn_words_raw)
                {
                    if (w.Contains('<') || w.Contains('>') || w.Contains('/') || w.Contains('?') ||
                        w.Contains('\\') || w.Contains(':') || w.Contains('*') || w.Contains('|') || w.Contains('"')
                        || w.Contains('=') || w.Contains('(') || w.Contains(')') || w.Contains(',') || w.Contains('\'') 
                        || w.Contains("cnn") || w.Contains(';'))
                        continue;
                    string s = CleanFileName(w);
                    if (string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s))
                        continue;
                    cnn_words.Add(s);
                }
		if(really_bad_hack_please_remove) return "";
            }
            if (namegen_mode == NameGenerationMode.RandomChars)
            {
                StringBuilder sb_name = new StringBuilder();
                int max = rnd.Next(6, 20);
                for (int i = 0; i < max; ++i)
                {
                    sb_name.Append(random_char());
                }
                return sb_name.ToString();
            }
            else if(namegen_mode == NameGenerationMode.RandomWords)
            {
                char spacer = spacers[rnd.Next(spacers.Length)];
                int ln = rnd.Next(1, 5);
                StringBuilder sb = new StringBuilder();
                for(int i = 0; i < ln; ++i)
                {
                    if (rnd.Next(0, 100) < 45)
                    {
                        sb.Append(cnn_words[rnd.Next(cnn_words.Count)]);
                        if (i != ln - 1)
                            sb.Append(spacer);
                        continue;
                    }
                    string w = name_words[rnd.Next(name_words.Length)];
                    if(rnd.Next(0, 10) > 5)
                    {
                        string nw = char.ToUpper(w[rnd.Next(w.Length)]) + w.Substring(1);
                        sb.Append(w);
                    }
                    else sb.Append(w);
                    if (i != ln - 1)
                        sb.Append(spacer);
                }
                return sb.ToString();
            }
            else if(namegen_mode == NameGenerationMode.RandomCNNWords)
            {
                return cnn_words[rnd.Next(cnn_words.Count)];
            }

            throw new Exception();
        }

        static int GenerateFileExtFromPrev(ref Dictionary<int,uint> fxc)
        {
            int[] probs = new int[file_extentions.Length];

            for (int i = 0; i < file_extentions.Length; ++i)
            {
                probs[i] = rnd.Next(1, 5);
            }

            //adjust for documents
            if (fxc[3] >= 1 || fxc[4] >= 1 || fxc[5] >= 1 || fxc[17] >= 1)
            {
                probs[3] += 12 + (12 * (int)fxc[3]) / 2;
                probs[4] += 12 + (12 * (int)fxc[4]) / 2;
                probs[5] += 10 + (12 * (int)fxc[5]) / 2;
                probs[17] += 12 + (12 * (int)fxc[17]) / 2;

                probs[0] -= 16;
                probs[1] -= 2;
                probs[9] -= 2;
                probs[10] -= 5;
                probs[11] -= 3;
                probs[8] -= 4;
                probs[18] -= 7;
                probs[6] -= 8;
            }

            //adjust for media
            if(fxc[10] >= 1 || fxc[12] >= 1 || fxc[13] >= 1 || fxc[14] >= 1 || fxc[15] >=  1 || fxc[16] >= 1)
            {
                probs[10] += 10;
                probs[12] += 10;
                probs[13] += 3;
                probs[14] += 15;
                probs[15] += 11;
                probs[16] += 6;

                probs[0] -= 10;
                probs[1] -= 3;
                probs[9] -= 1;
                probs[10] -= 5;
                probs[11] -= 3;
                probs[8] -= 4;
                probs[18] -= 7;
                probs[6] -= 8;
            }

            //adjust for pics
            if(fxc[20] >= 1 || fxc[21] >= 1 || fxc[22] >= 1)
            {
                probs[20] = Math.Max(5 * (int)fxc[20], 80);
                probs[21] = Math.Max(5 * (int)fxc[21], 80);
                probs[22] = Math.Max(6 * (int)fxc[22], 85);

                probs[0] -= 9;
                probs[1] -= 2;
                probs[9] -= 2;
                probs[10] -= 5;
                probs[11] -= 3;
                probs[8] -= 4;
                probs[18] -= 7;
                probs[6] -= 8;
            }

            //adjust for random things
            if (fxc[11] >= 1)
                probs[11] += Math.Max(4 * (int)fxc[11], 70);
            if (fxc[9] >= 1)
                probs[9] += Math.Max(4 * (int)fxc[11], 70);
            if (fxc[10] >= 1)
            {
                probs[0] += 5;
                probs[9] += 2;
                probs[10] += Math.Max(4 * (int)fxc[11], 70);
            }
            if (fxc[8] >= 1)
            {
                probs[10] += 5;
                probs[0] += 5;
                probs[9] += 1;
                probs[8] += Math.Max(4 * (int)fxc[11], 70);
            }

            //adjust probs for occurances of .exe ext
            if (fxc[0] >= 1)
            {
                probs[0] += (int)Math.Ceiling(14.0*((double)fxc[0]/2.0));
                probs[1] += 10;
                probs[2] += 3;
                probs[6] += 2;
                probs[7] += 1;
                probs[8] += 4;
                probs[9] += 12;
                probs[10] += 4;
                probs[18] += 24;
                probs[19] += 18;

                probs[3] -= 4;
                probs[4] -= 4;
                probs[5] -= 4;
                probs[17] -= 3;
                probs[20] -= 2;
                probs[21] -= 1;
                probs[22] -= 2;
            }
            
            //probs for oc of .bat
            if(fxc[1] >= 1)
            {
                probs[0] += 30;
                probs[7] += 10;
                probs[18] += 2;
            }

            if(fxc[2] >= 1)
            {
                probs[0] += 5;
                probs[7] += 2;
                probs[10] += 2;
                probs[11] += 1;
            }

            if (fxc[0] >= 5) probs[0] -= 20;


            for (int i = 0; i < file_extentions.Length; ++i)
            {
                if (probs[i] < 0) continue;
                int p = rnd.Next(0, 100);
                if(p < probs[i])
                    return i;
            }

            return rnd.Next(file_extentions.Length);
        }
	
	static List<string> file_datas;
        static void GenerateFile(string path, ref Dictionary<int,uint> file_ex_count)
        {
            //generate random name
            string name = RandomName();
            {
                int fxidx = GenerateFileExtFromPrev(ref file_ex_count); //rnd.Next(0, file_extentions.Length);
                file_ex_count[fxidx]++;
                name += file_extentions[fxidx];
            }
            string fpth = path + "\\" + name;
            if (fpth.Length >= 200) return;
            
            try
            {
                File.WriteAllText(fpth, file_datas[rnd.Next(file_datas.Count)]);
                FileInfo fi = new FileInfo(fpth);
                fi.CreationTime = RandomFileTime(DateTime.Now);
                fi.LastAccessTime = RandomFileTime(fi.LastAccessTime);
                fi.LastWriteTime = RandomFileTime(fi.LastWriteTime);
                fi.Attributes = RandomFileAttribs();
            }
            catch { Console.WriteLine("!");  }
        }

        static void GenerateDirectory(string path)
        {
            if (rnd.Next(0, 100) < 2)
            {
                Console.Write("*");
                return;
            }
            //generate random name
            string name = RandomName();
            string root_path = path + "\\" + name;
            if (root_path.Length+2 > 180) return;
            Console.WriteLine("gendir {0}", root_path);
            Directory.CreateDirectory(root_path + "\\");

            //generate files w/ 75% prob
            bool generated_files = false;
            if(rnd.Next(100) < 75)
            {
                Dictionary<int, uint> fxc = new Dictionary<int,uint>();
                for (int i = 0; i < file_extentions.Length; ++i)
                {
                    fxc.Add(i, 0);
                }
                generated_files = true;
                int max = rnd.Next(8, 128);
                for(int i = 0; i < max; ++i)
                {
                    GenerateFile(root_path, ref fxc);
                }
            }

            //generate directories w/ 35% prob
            if(!generated_files || rnd.Next(100) < 35)
            {
                int max = rnd.Next(1, 16);
                for(int i = 0; i < max; ++i)
                {
                    GenerateDirectory(root_path);
                }
            }
        }

        static void Main(string[] args)
        {
            rnd = new Random(DateTime.Now.Millisecond);

            #region version option
            if (args[0] == "--version")
            {
                string message = "brick_road version [" + System.Reflection.Assembly.GetCallingAssembly().GetName().Version  + "] (C) Andrew Palmer 2014";
                Console.BufferHeight = 50;
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.Green;
               int ccy = Console.BufferHeight / 2;
                int ccx = Console.BufferWidth / 2;

                int smx = rnd.Next(0, Console.BufferWidth-1);
                int smy = rnd.Next(0, Console.BufferHeight-1);

                ConsoleColor snake_color = ConsoleColor.Green;

                Console.CursorVisible = false;

                while (true)
                {
                    //Console.Clear();

                    int d = rnd.Next(0, 4);
                    int v = rnd.Next(0, 2);

                    //0 1 2 3
                    //U D L R

                    if (d == 0) ccy += v;
                    if (d == 1) ccy -= v;
                    if (d == 2) ccx += v;
                    if (d == 3) ccx -= v; 

                    if (ccy < 0) ccy = Console.BufferHeight - 5;
                    if (ccx < 0) ccx = Console.BufferWidth - 5;
                    if (ccy >= Console.BufferHeight-1) ccy = 0;
                    if (ccx >= Console.BufferWidth-1) ccx = 0;

                    if(ccx == smx && ccy == smy)
                    {
                        //Console.Clear();
                        smx = rnd.Next(0, Console.BufferWidth - 1);
                        smy = rnd.Next(0, Console.BufferHeight - 1);
                        snake_color = (ConsoleColor)rnd.Next(0, 15);
                    }

                    Console.CursorTop = ccy; Console.CursorLeft = ccx;
                    Console.ForegroundColor = snake_color;
                    Console.Write((char)rnd.Next(0, 255));
                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.CursorTop = Console.BufferHeight / 2;
                    Console.CursorLeft = (Console.BufferWidth / 2) - message.Length / 2;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(message);
                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.CursorTop = smy;
                    Console.CursorLeft = smx;
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write((char)2);
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.ForegroundColor = ConsoleColor.Green;
                    
                }
            }
            #endregion
            if (args[0] == "--help")
            {
                return;
            }
            namegen_mode = NameGenerationMode.RandomWords;
            if(args.Length == 0)
            {
                Console.WriteLine("Error: not enough arguments");
                return;
            }
            if (args.Length >= 2)
                cnnurl = args[1];
            string root_dir_p = args[0];
            RandomName(true); //should be LoadTextFromWeb();
	    //create file data for files
	    file_datas = new List<string>();
	    for(int i = 0; i < 16; ++i)
	    {
		StringBuilder sb_tx = new StringBuilder();
		int mx = rnd.Next(20, 100000);
		for(int j = 0; j < mx; ++j)
		{
			if(rnd.Next(0, 3) == 0)	{
			sb_tx.Append(cnn_words[rnd.Next(cnn_words.Count)]);
			}
			else {
			char c = (char)rnd.Next(0, 255);
			sb_tx.Append(c);     
			}
		}	
		file_datas.Add(sb_tx.ToString());
	    }

            var root_dir = Directory.CreateDirectory(root_dir_p);
            int num_dirs = rnd.Next(4, 16);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for(int i = 0; i < num_dirs; ++i)
            {
                GenerateDirectory(root_dir_p);
            }
            sw.Stop();
            Console.WriteLine("Time: {0} ms", sw.ElapsedMilliseconds);
            Console.ReadKey();
        }
    }
}
