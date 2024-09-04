using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EncryptorApp
{
    internal class EncryptViewModel:ViewModelBase
    {
        private int progressBar = 0;
        public int ProgressBar
        {
            get { return progressBar; }
            set { Set(ref progressBar, value); }
        }
        private string path;
        public string Path
        {
            get { return path; } set { Set(ref path, value); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { Set(ref password, value); }
        }

        private string key = "EncryptKey";

        public bool IsEncrypt { get; set; } = false;
        public bool IsDecrypt { get; set; } = false;

        private CancellationTokenSource CancelTokenSource;
        private CancellationToken token;

        private RelayCommand selectFile;
        public RelayCommand SelectFile
        {
            get
            {
                return selectFile ?? new RelayCommand(() =>
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    bool? IsOpen = dialog.ShowDialog();
                    if (IsOpen == true)
                    {
                        Path = dialog.FileName;
                    }
                });
            }
        }

        private RelayCommand startProcess;
        public RelayCommand StartProcess
        {
            get
            {
                return startProcess ?? new RelayCommand(async () =>
                {
                    CancelTokenSource = new CancellationTokenSource();
                    token = CancelTokenSource.Token;
                    if (password == "Admin" && IsEncrypt == true) 
                    {
                        try
                        {
                            token.ThrowIfCancellationRequested();
                            byte[] fileBytes = File.ReadAllBytes(Path);
                            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                            byte[] encryptedBytes = new byte[fileBytes.Length];


                            for (int i = 0; i < fileBytes.Length; i++)
                            {
                                encryptedBytes[i] = (byte)(fileBytes[i] ^ keyBytes[i % keyBytes.Length]);
                                await Task.Run(() =>
                                {
                                    token.ThrowIfCancellationRequested();
                                    ProgressBar = (i + 1) * 100 / fileBytes.Length;
                                },token);
                            }

                            File.WriteAllBytes(Path, encryptedBytes);
                            MessageBox.Show("File Encrypted!");
                            ProgressBar = 0;
                        }
                        catch(OperationCanceledException ex)
                        {
                            MessageBox.Show("Operation Canceled!");
                            ProgressBar = 0;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error!");
                        }

                    }
                    else if(password == "Admin" && IsDecrypt == true)
                    {
                        try
                        {
                            byte[] encryptedBytes = File.ReadAllBytes(Path);
                            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                            byte[] decryptedBytes = new byte[encryptedBytes.Length];

                            for(int i = 0;i < encryptedBytes.Length; i++)
                            {
                                decryptedBytes[i] = (byte)(encryptedBytes[i] ^ keyBytes[i % keyBytes.Length]);
                                await Task.Run(() =>
                                {
                                    token.ThrowIfCancellationRequested();
                                    ProgressBar = (i + 1) * 100 / encryptedBytes.Length;
                                },token);
                            }

                            File.WriteAllBytes(Path, decryptedBytes);
                            MessageBox.Show("File Decrypted!");
                            ProgressBar = 0;
                        }
                        catch (OperationCanceledException ex)
                        {
                            MessageBox.Show("Operation Canceled!");
                            ProgressBar = 0;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error!");
                        }
                    }
                    else { MessageBox.Show("Wrong Password"); }
                });
            }
        }

        private RelayCommand cancelProcess;
        public RelayCommand CancelProcess
        {
            get
            {
                return cancelProcess ?? new RelayCommand(() =>
                {
                    CancelTokenSource.Cancel();
                });
            }
        }
    }
}
