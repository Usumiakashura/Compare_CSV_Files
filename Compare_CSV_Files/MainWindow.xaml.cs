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

namespace Compare_CSV_Files
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string fileFilter = "Text files (*.csv)|*.csv|All files (*.*)|*.*";
        private const string format1 = "000000", format2 = "00";
        private List<string> allStrokes, codes_AfterCompare;
        private List<string[]> codes_ModelFile, codes_ReportFile;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnUploadModelFile_Click(object sender, RoutedEventArgs e)
        {
            codes_ModelFile = GetCodesFromFile(ModelFileName);
        }

        private void btnUploadReportFile_Click(object sender, RoutedEventArgs e)
        {
            codes_ReportFile = GetCodesFromFile(ReportFileName);
        }

        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            if (codes_ModelFile?.Count > 0 && codes_ReportFile?.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = fileFilter;
                if (saveFileDialog.ShowDialog() == true)
                {
                    string path = saveFileDialog.FileName;
                    codes_AfterCompare = new List<string>();
                    for (int i = 0; i < codes_ModelFile.Count; i++)
                    {
                        string singleCode, numbers;
                        numbers = $"{codes_ModelFile[i][0]};{codes_ModelFile[i][1]};";
                        for (int m = 2, r = 2; m + 1 < codes_ModelFile[i].Length; m += 2, r++)
                        {
                            if (codes_ModelFile[i][m + 1].Contains("GAA") &&
                                (r >= codes_ReportFile[i].Length || !codes_ReportFile[i][r].Contains(codes_ModelFile[i][m])))
                            {
                                singleCode = numbers + $"{(r - 1).ToString(format2)};{codes_ModelFile[i][m + 1]};{codes_ModelFile[i][m]};";
                                codes_AfterCompare.Add(singleCode);
                            }
                        }
                    }
                    using (StreamWriter streamWriter = new StreamWriter(path, true, Encoding.GetEncoding("UTF-8")))
                    {
                        //---- for записывает в порядке убывания -----
                        for (int i = codes_AfterCompare.Count - 1; i >= 0; i--)
                        {
                            streamWriter.WriteLine(codes_AfterCompare[i]);
                        }
                        //---- foreach записывает в порядке возрастания -----
                        //foreach (string stroke in codes_AfterCompare)
                        //{
                        //    streamWriter.WriteLine(stroke);
                        //}
                        MessageBox.Show($"Файл успешно сохранен.\nПуть: {path}",
                            "Успешно",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Необходимо загрузить оба файла",
                    "Невозможно произвести сравнение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        //-------------------------
        private List<string[]> GetCodesFromFile(TextBox textBox)
        {
            List<string[]> codes_FromFile = new List<string[]>();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = fileFilter;
            if (ofd.ShowDialog() == true)
            {
                textBox.Text = ofd.FileName;
                allStrokes = File.ReadAllLines(ofd.FileName).ToList();
                codes_FromFile = new List<string[]>();
                int value = 0;
                for (int i = 0; i < allStrokes.Count(); i++)
                {
                    int.TryParse(allStrokes[i].Substring(0, allStrokes[i].IndexOf(';')), out value);
                    if (value > 0)
                    {
                        codes_FromFile.Add(($"{(i + 1).ToString(format1)};" + allStrokes[i]).Split(';'));
                        value = 0;
                    }
                }
            }
            return codes_FromFile;
        }
    }
}
