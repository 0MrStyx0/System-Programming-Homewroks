using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SemaphoreApp
{
    internal class SemaphoreAppViewModel:ViewModelBase
    {
        private Semaphore semaphore = new Semaphore(3,5);

        private Thread selectedThread;
        public Thread SelectedThread
        {
            get { return selectedThread; }
            set { Set(ref  selectedThread, value); }
        }

        private ObservableCollection<Thread> createdThreads = new ObservableCollection<Thread>();
        public ObservableCollection<Thread> CreatedThreads
        {
            get { return createdThreads; }
            set { Set(ref createdThreads, value); }
        }

        private ObservableCollection<Thread> waitingThreads = new ObservableCollection<Thread>();
        public ObservableCollection<Thread> WaitingThreads
        {
            get { return waitingThreads; }
            set { Set(ref waitingThreads, value); }
        }

        private ObservableCollection<Thread> workingThreads = new ObservableCollection<Thread>();
        public ObservableCollection<Thread> WorkingThreads
        {
            get { return workingThreads; }
            set { Set(ref workingThreads, value); }
        }

        private int count = 1;

        public SemaphoreAppViewModel()
        {
            Action action = Checker;
            Task task = new Task(action);
            task.Start();
        }


        private RelayCommand doubleClick;
        public RelayCommand DoubleClick
        {
            get
            {
                return doubleClick ?? new RelayCommand(() =>
                {
                    if (semaphore.WaitOne(0))
                    {
                        SelectedThread.Start();
                        WorkingThreads.Add(SelectedThread);
                        CreatedThreads.Remove(SelectedThread);
                    }
                    else 
                    {
                        waitingThreads.Add(SelectedThread);
                        CreatedThreads.Remove(SelectedThread);
                    }
                });
            }
        }

        private RelayCommand createThread;
        public RelayCommand CreateThread
        {
            get
            {
                return createThread ?? new RelayCommand(() =>
                {
                    Thread newThread = new Thread(LocalProcess) { Name = $"Thread{count}" };
                    CreatedThreads.Add(newThread);
                    count++;
                });
            }
        }

        private void LocalProcess()
        {
            Thread.Sleep(new Random().Next(5000, 10000));
            semaphore.Release();
        }

        private void Checker()
        {
            while (true)
            {
                if (WorkingThreads.Count > 0)
                {
                    foreach (var thread in WorkingThreads.ToList())
                    {
                        if (!thread.IsAlive)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                WorkingThreads.Remove(thread);
                            });
                        }
                    }
                }

                if (WaitingThreads.Count > 0)
                {
                    foreach (var thread in WaitingThreads.ToList())
                    {
                        if (semaphore.WaitOne())
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                WorkingThreads.Add(thread);
                                WaitingThreads.Remove(thread);
                            });
                            thread.Start();
                        }
                    }
                }
            } 
        }
    }
}
