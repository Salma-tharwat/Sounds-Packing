using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Threading;

namespace Sounds_Packing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public string infoPath; 

        static public List<int> secList = new List<int>();
        List<string> l = new List<string>();
        static string sourcePath;
        static string targetPath;
        static int maxSec;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if(openFileDialog.ShowDialog()==true)
            {
                infoPath = openFileDialog.FileName;

            }
        }
        public void ReadAudioFileInfo()
        {
            FileStream FS = new FileStream(infoPath, FileMode.Open);
            StreamReader file = new StreamReader(FS);
            
            while (file.Peek() != -1)
            {
                string readline = file.ReadLine();
                l.Add(readline);
            }

            
            for (int i = 1; i < l.Count; i++)
            {
                for (int j = 0; j < l[i].Length; j++)
                {
                    if (l[i][j] == ' ' && j >3)
                    {
                        string final = l[i].Substring(j+1);
                        double seconds = TimeSpan.Parse(final).TotalSeconds;
                        secList.Add((int)seconds);
                        break;
                    }
                }

            }
        }

        static void Allocatingfiles(int[] allocation)
        {
            for (int i = 0; i < allocation.Length; i++)
            {
                int num = i + 1;
                string fileName = num.ToString() + ".amr";
                string sPath = sourcePath;
                string tPath = targetPath;
                tPath += allocation[i] + 1;
                if (!System.IO.Directory.Exists(tPath))
                {
                    System.IO.Directory.CreateDirectory(tPath);
                }

                string sourceFile = System.IO.Path.Combine(sPath, fileName);
                string destFile = System.IO.Path.Combine(tPath, fileName);
                System.IO.File.Copy(sourceFile, destFile, true);
            }
        }
        private void btnOpendirsource_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                sourcePath = fbd.SelectedPath;
            }
        }
        private void btnOpendirtarget_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                targetPath = fbd.SelectedPath;
                targetPath += targetPath[2];
            }
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            maxSec = int.Parse(secBox.Text);
            ReadAudioFileInfo();
            System.Windows.MessageBox.Show(sourcePath.ToString());
            System.Windows.MessageBox.Show(targetPath.ToString());
            Thread t = new Thread(delegate () { folderFilling(); });
          //  Thread t1 = new Thread(() => WorstFit());
            t.Start();
        }

        static void WorstFit()
        {
            List<int> Folders = new List<int>();
            int[] allocation = new int[secList.Count];
            for (int i = 0; i < secList.Count; i++)
            {
                allocation[i] = -1;
            }
            for (int i = 0; i < secList.Count; i++)
            {
                int wstIdx = -1;
                for (int j = 0; j < Folders.Count; j++)
                {
                    if (Folders[j] >= secList[i])
                    {
                        if (wstIdx == -1)
                            wstIdx = j;
                        else if (Folders[wstIdx] <= Folders[j])
                            wstIdx = j;
                    }

                }
                if (wstIdx != -1)
                {
                    allocation[i] = wstIdx;
                    Folders[wstIdx] -= secList[i];
                }
                else if (wstIdx == -1)
                {
                    Folders.Add(maxSec);
                    Folders[Folders.Count - 1] -= secList[i];
                    allocation[i] = Folders.Count - 1;
                }

            }
            Allocatingfiles( allocation);
        }
        static void WorstFitPQ()
        {
            int folderindex = 0;
            PriorityQueue<Tuple<int, int>> Folders = new PriorityQueue<Tuple<int, int>>();
            int[] allocation = new int[secList.Count];
            for (int i = 0; i < secList.Count; i++)
            {
                allocation[i] = -1;
            }

            for (int i = 0; i < secList.Count; i++)
            {
                if (Folders.Count > 0)
                {
                    Tuple<int, int> tempp = Folders.Peek();
                    if (tempp.Item1 > secList[i])
                    {
                        var top = Folders.list[0];
                        Folders.Dequeue();
                        int rem = top.Item1, ind = top.Item2;
                        rem -= secList[i];
                        Tuple<int, int> after = new Tuple<int, int>(rem, ind);
                        Folders.Enqueue(after);
                        allocation[i] = ind;

                    }
                }

                if (allocation[i] == -1)
                {
                    int rem = maxSec - secList[i], ind = folderindex++;
                    Tuple<int, int> tmp = new Tuple<int, int>(rem, ind);
                    Folders.Enqueue(tmp);
                    allocation[i] = ind;
                }

            }
            Allocatingfiles(allocation);
        }
        static void WorstFitDec()
        {
            secList.Sort();
            secList.Reverse();
            List<int> Folders = new List<int>();
            int[] allocation = new int[secList.Count];
            for (int i = 0; i < secList.Count; i++)
            {
                allocation[i] = -1;
            }
            for (int i = 0; i < secList.Count; i++)
            {
                int wstIdx = -1;
                for (int j = 0; j < Folders.Count; j++)
                {
                    if (Folders[j] >= secList[i])
                    {
                        if (wstIdx == -1)
                            wstIdx = j;
                        else if (Folders[wstIdx] <= Folders[j])
                            wstIdx = j;
                    }

                }
                if (wstIdx != -1)
                {
                    allocation[i] = wstIdx;
                    Folders[wstIdx] -= secList[i];
                }
                else if (wstIdx == -1)
                {
                    Folders.Add(maxSec);
                    Folders[Folders.Count - 1] -= secList[i];
                    allocation[i] = Folders.Count - 1;
                }

            }
            Allocatingfiles(allocation);
        }
        static void WorstFitDecPQ()
        {
            secList.Sort();
            secList.Reverse();
            int folderindex = 0;
            PriorityQueue<Tuple<int, int>> Folders = new PriorityQueue<Tuple<int, int>>();
            int[] allocation = new int[secList.Count];
            for (int i = 0; i < secList.Count; i++)
            {
                allocation[i] = -1;
            }

            for (int i = 0; i < secList.Count; i++)
            {
                if (Folders.Count > 0)
                {
                    Tuple<int, int> tempp = Folders.Peek();
                    if (tempp.Item1 > secList[i])
                    {
                        var top = Folders.list[0];
                        Folders.Dequeue();
                        int rem = top.Item1, ind = top.Item2;
                        rem -= secList[i];
                        Tuple<int, int> after = new Tuple<int, int>(rem, ind);
                        Folders.Enqueue(after);
                        allocation[i] = ind;

                    }
                }

                if (allocation[i] == -1)
                {
                    int rem = maxSec - secList[i], ind = folderindex++;
                    Tuple<int, int> tmp = new Tuple<int, int>(rem, ind);
                    Folders.Enqueue(tmp);
                    allocation[i] = ind;
                }

            }
            Allocatingfiles(allocation);
        }
        static void firstFitDec()
        {
            secList.Sort();
            secList.Reverse();
            List<int> Folders = new List<int>();      //folder list, complexity: O(1) 
            int[] allocation = new int[secList.Count]; //array with the number of files, complexity: O(1) 
            for (int i = 0; i < secList.Count; i++)     //complexity: O(n), (n -> secList.Count)
            {
                allocation[i] = -1;                 //intialize all files by -1 as the file is still not allocated
            }
            for (int i = 0; i < secList.Count; i++)  //complexity: O(n), (n -> secList.Count)
            {
                int fstIdx = -1;                 // Complexity: O(1)   
                                                 // Initialize the variable of fisrt index as not found

                for (int j = 0; j < Folders.Count; j++)//complexity: O(m), (m -> Folders.Count)
                {
                    if (Folders[j] >= secList[i]) // Complexity: O(1)
                    {                             //Compare between Folders and Files to find the fisrt suitable Folder
                    

                        fstIdx = j;               // Complexity:O(1)
                                                  //put the current folder in the fisrt fit index
                        break;
                    }

                }
                if (fstIdx != -1)                 // Compexity: O(1)
                                                  // If there is a suitable place
                {
                    allocation[i] = fstIdx;                  //Complexity: O(1)
                                                             // assign folder i to be put in fstIdx folder

                    Folders[fstIdx] -= secList[i];           // Complexity: O(1)
                                                             //Reduce memory of the files 
                }
                else if (fstIdx == -1)             // Complexity :O(1)
                                                   //If there is not a place in folders
                {
                    Folders.Add(maxSec);           //Create new folder to add the file
                    Folders[Folders.Count - 1] -= secList[i];
                    allocation[i] = Folders.Count - 1;
                }

            }

                                           //Total Complexity of First-Fit : O(N*M)
            Allocatingfiles(allocation);
        }
        static void BestFit()
        {
            int[] allocation = new int[secList.Count];
            List<int> Folders = new List<int>();
            for (int i = 0; i < secList.Count; i++)
            {
                allocation[i] = -1;
            }
            for (int i = 0; i < secList.Count; i++)
            {
                int bstIdx = -1;
                for (int j = 0; j < Folders.Count; j++)
                {
                    if (Folders[j] >= secList[i])
                    {
                        if (bstIdx == -1)
                            bstIdx = j;
                        else if (Folders[bstIdx] > Folders[j])
                            bstIdx = j;
                    }

                }
                if (bstIdx != -1)
                {
                    allocation[i] = bstIdx;
                    Folders[bstIdx] -= secList[i];
                }
                else if (allocation[i] == -1)
                {

                    int toadd = maxSec - secList[i];
                    Folders.Add(toadd);
                    allocation[i] = Folders.Count - 1;
                }

            }
            Allocatingfiles(allocation);
        }
        static List<int> FolderFilling(ref List<Tuple<int, int>> v, ref List<Tuple<int, int>> w, int n, int W,int[,]V,bool[,]keep)
        {
            List<int> l = new List<int>();
            for (int i = 0; i < secList.Count+1; i++)
            {

            }
            for (int a = 0; a <= W; a++)
            {
                V[0, a] = 0;
            }
            for (int i = 0; i < n; i++)
            {
                V[i, 0] = 0;
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 0; j <= W; j++)
                {
                    if ((w[i].Item1 <= j) && (v[i].Item1 + V[i - 1, j - w[i].Item1]) > V[i - 1, j])
                    {
                        V[i, j] = v[i].Item1 + V[i - 1, j - w[i].Item1];
                        keep[i, j] = true;
                    }
                    else
                    {
                        V[i, j] = V[i - 1, j];
                        keep[i, j] = false;
                    }
                }

            }
            int K = W;
            for (int k = n; k >= 1; k--)
            {
                if (keep[k, K] == true)
                {
                    l.Add(w[k].Item2);
                    //Console.WriteLine(w[k].Item2.ToString());
                    K = K - w[k].Item1;
                    w.Remove(w[k]);
                    v.Remove(v[k]);
                }

            }
            return l;
        }
        static void folderFilling()
        {
             bool[,] keep = new bool[secList.Count+1, secList.Count+1];
         int[,] V = new int[secList.Count+1, secList.Count+1];
             List<Tuple<int, int>> w = new List<Tuple<int, int>>();
            List<int> indexes = new List<int>();
            w.Add(new Tuple<int, int>(0, 0));
            for (int i = 0; i < secList.Count(); i++)
            {
                w.Add(new Tuple<int, int>(secList[i], i + 1));
            }
            List<Tuple<int, int>> v = new List<Tuple<int, int>>();
            //new Tuple<int, int>(0, 0);
            v.Add(new Tuple<int, int>(0, 0));

            for (int i = 0; i < secList.Count(); i++)
            {

                v.Add(new Tuple<int, int>(secList[i], i + 1));
            }
            int countt = 1;
            while (v.Count() > 1)
            {
                Console.WriteLine("file " + countt.ToString() + ' ');
                List<int> folder_filling_Allocation = FolderFilling(ref v, ref w, v.Count() - 1, maxSec,V,keep);
                string sPath = sourcePath;                 string tPath = targetPath; 
                tPath += countt.ToString();
                if (!System.IO.Directory.Exists(tPath))
                {
                    System.IO.Directory.CreateDirectory(tPath);
                }
                for (int i = 0; i < folder_filling_Allocation.Count(); i++)
                {

                    string fileName = folder_filling_Allocation[i].ToString() + ".amr";
                    Console.WriteLine(folder_filling_Allocation[i].ToString());
                    string sourceFile = System.IO.Path.Combine(sPath, fileName);
                    string destFile = System.IO.Path.Combine(tPath, fileName);
                    System.IO.File.Copy(sourceFile, destFile, true);

                }


                countt++;
            }
        }

    }
}