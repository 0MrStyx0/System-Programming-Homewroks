using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CarCatalog
{
    internal class CarCatalogViewModel : ViewModelBase
    {
        private ObservableCollection<Car> cars = new ObservableCollection<Car>();
        public ObservableCollection<Car> Cars
        {
            get { return cars; }
            set { Set(ref cars, value); }
        }

        private bool isSingle = false;
        public bool IsSingle
        {
            get { return isSingle; }
            set { Set(ref isSingle, value); }
        }

        private bool isMulti = true;
        public bool IsMulti
        {
            get { return isMulti; }
            set { Set(ref isMulti, value); }
        }
        private string time;
        public string Time
        {
            get { return time; }
            set { Set(ref time, value); }
        }
        private DispatcherTimer timer;

        private TimeSpan timeSpan;

        private RelayCommand singleMode;
        public RelayCommand SingleMode
        {
            get
            {
                return singleMode ?? new RelayCommand(() =>
                {
                    IsSingle = false;
                    IsMulti = true;
                });
            }
        }

        private RelayCommand multiMode;
        public RelayCommand MultiMode
        {
            get
            {
                return multiMode ?? new RelayCommand(() =>
                {
                    IsSingle = true;
                    IsMulti = false;
                });
            }
        }

        private RelayCommand start;
        public RelayCommand Start
        {
            get
            {
                return start ?? new RelayCommand(() =>
                {
                    timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += Timer_Tick;
                    timeSpan = TimeSpan.Zero;
                    timer.Start();
                    string path1 = "Data\\Cars1.json";
                    string path2 = "Data\\Cars2.json";
                    string path3 = "Data\\Cars3.json";
                    string path4 = "Data\\Cars4.json";
                    string path5 = "Data\\Cars5.json";

                    if (isSingle == false)
                    {
                        Thread thread = new Thread(() =>
                        {
                            using (FileStream stream = new FileStream(path1, FileMode.OpenOrCreate))
                            {
                                Cars = JsonSerializer.Deserialize<ObservableCollection<Car>>(stream);
                            }
                            using (FileStream stream = new FileStream(path2, FileMode.OpenOrCreate))
                            {
                                Cars.ToList().AddRange(JsonSerializer.Deserialize<List<Car>>(stream));
                            }
                            using (FileStream stream = new FileStream(path3, FileMode.OpenOrCreate))
                            {
                                Cars.ToList().AddRange(JsonSerializer.Deserialize<List<Car>>(stream));
                            }
                            using (FileStream stream = new FileStream(path4, FileMode.OpenOrCreate))
                            {
                                Cars.ToList().AddRange(JsonSerializer.Deserialize<List<Car>>(stream));
                            }
                            using (FileStream stream = new FileStream(path5, FileMode.OpenOrCreate))
                            {
                                Cars.ToList().AddRange(JsonSerializer.Deserialize<List<Car>>(stream));
                            }
                            timer.Stop();
                            MessageBox.Show("Data Load!");
                        });
                        thread.Start();
                    }
                    else if (isMulti == false)
                    {
                        ThreadPool.QueueUserWorkItem((param) =>
                        {
                            using (FileStream stream = new FileStream(path1, FileMode.OpenOrCreate))
                            {
                                Cars = JsonSerializer.Deserialize<ObservableCollection<Car>>(stream);
                            }
                        });

                        ThreadPool.QueueUserWorkItem((param) =>
                        {
                            using (FileStream stream = new FileStream(path2, FileMode.OpenOrCreate))
                            {
                                Cars.ToList().AddRange(JsonSerializer.Deserialize<List<Car>>(stream));
                            }
                        });

                        ThreadPool.QueueUserWorkItem((param) =>
                        {
                            using (FileStream stream = new FileStream(path3, FileMode.OpenOrCreate))
                            {
                                Cars.ToList().AddRange(JsonSerializer.Deserialize<List<Car>>(stream));
                            }
                        });

                        ThreadPool.QueueUserWorkItem((param) =>
                        {
                            using (FileStream stream = new FileStream(path4, FileMode.OpenOrCreate))
                            {
                                Cars.ToList().AddRange(JsonSerializer.Deserialize<List<Car>>(stream));
                            }
                        });

                        ThreadPool.QueueUserWorkItem((param) =>
                        {
                            using (FileStream stream = new FileStream(path5, FileMode.OpenOrCreate))
                            {
                                Cars.ToList().AddRange(JsonSerializer.Deserialize<List<Car>>(stream));
                            }
                            timer.Stop();
                            MessageBox.Show("Data Load!");
                        });
                    }

                });
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds(1));
            Time = timeSpan.ToString(@"hh\:mm\:ss");
        }
    }
}
