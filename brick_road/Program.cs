using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
                if(rnd.Next(0, 10) > 5)
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

        static int GenerateFileExtFromPrev(ref Dictionary<int,uint> fxc)
        {
            int[] probs = new int[file_extentions.Length];
            
            //adjust for documents
            if(fxc[3] >= 1 || fxc[4] >= 1 || fxc[5] >= 1 || fxc[17] >= 1)
            {
                probs[3] += 12;
                probs[4] += 12;
                probs[5] += 10;
                probs[17] += 12;
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
            }

            //adjust for pics
            if(fxc[20] >= 1 || fxc[21] >= 1 || fxc[22] >= 1)
            {
                probs[20] = Math.Max(5 * (int)fxc[20], 80);
                probs[21] = Math.Max(5 * (int)fxc[21], 80);
                probs[22] = Math.Max(6 * (int)fxc[22], 85);
            }

            //adjust for random things
            if (fxc[11] >= 1)
                probs[11] += Math.Max(4 * (int)fxc[11], 70);
            if (fxc[9] >= 1)
                probs[9] += Math.Max(4 * (int)fxc[11], 70);
            if (fxc[10] >= 1)
            {
                probs[10] += Math.Max(4 * (int)fxc[11], 70);
            }
            if (fxc[8] >= 1)
            {
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




            return -1;
        }

        static void GenerateFile(string path, ref Dictionary<int,uint> file_ex_count)
        {
            //generate random name
            string name = "";
            {
                StringBuilder sb_name = new StringBuilder();
                int max = rnd.Next(6, 20);
                for (int i = 0; i < max; ++i)
                {
                    sb_name.Append(random_char());
                }
                int fxidx = rnd.Next(0, file_extentions.Length);
                file_ex_count[fxidx]++;
                name = sb_name.ToString();
                name += file_extentions[fxidx];
            }
            string fpth = path + "\\" + name;
            if (fpth.Length > 230) return;
            StringBuilder sb_tx = new StringBuilder();
            int mx = rnd.Next(20, 2000);
            for (int i = 0; i < mx; ++i)
            {
                char c = (char)rnd.Next(0, 255);
                sb_tx.Append(c);
            }
            File.WriteAllText(fpth, sb_tx.ToString());
        }

        static void GenerateDirectory(string path)
        {
            if (rnd.Next(0, 100) < 2)
            {
                Console.Write("*");
                return;
            }
            //generate random name
            string name = "";
            {
                StringBuilder sb_name = new StringBuilder();
                int max = rnd.Next(6, 20);
                for(int i = 0; i < max; ++i)
                {
                    sb_name.Append(random_char());
                }
                name = sb_name.ToString();
            }
            string root_path = path + "\\" + name;
            if (root_path.Length > 230) return;
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
            if(args.Length == 0)
            {
                Console.WriteLine("Error: not enough arguments");
                return;
            }
            string root_dir_p = args[0];
            //GenerateFile(root_dir_p);
            //return;
            var root_dir = Directory.CreateDirectory(root_dir_p);
            int num_dirs = rnd.Next(1, 8);
            for(int i = 0; i < num_dirs; ++i)
            {
                GenerateDirectory(root_dir_p);
            }
        }
    }
}
