using System.Diagnostics;


namespace TaskManager
{
    public partial class Form1 : Form
    {
        public Task task { get; set; }
        public Action action { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listViewTaskManager.SelectedItems.Count > 0)
            {
                var selectedProcessName = listViewTaskManager.SelectedItems[0].Text;
                var processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    if (process.ProcessName == selectedProcessName)
                    {
                        process.Kill();
                    }
                }
            }
        }

        private async void RefreshProcessList()
        {
            while (true)
            {
                listViewTaskManager.Invoke(new Action(() =>
                {

                    var topItem = listViewTaskManager.TopItem;
                    int topIndex;
                    if (topItem != null)
                    {
                        topIndex = topItem.Index;
                    }
                    else topIndex = 0;
                    listViewTaskManager.Items.Clear();
                        var processes = Process.GetProcesses();
                        foreach (var process in processes)
                        {
                            try
                            {
                                ListViewItem item = new ListViewItem(process.ProcessName);
                                item.SubItems.Add(process.Id.ToString());
                                item.SubItems.Add(process.PagedMemorySize.ToString());
                                item.SubItems.Add(process.VirtualMemorySize.ToString());
                                listViewTaskManager.Items.Add(item);
                            }
                            catch (Exception ex) { }

                        if (topIndex < listViewTaskManager.Items.Count)
                        {
                            listViewTaskManager.TopItem = listViewTaskManager.Items[topIndex];
                        }
                        else if (listViewTaskManager.Items.Count > 0)
                        {
                            listViewTaskManager.TopItem = listViewTaskManager.Items[listViewTaskManager.Items.Count - 1];
                        }
                    }
                }));

                await Task.Delay(2000);
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            action = RefreshProcessList;

            task = new Task(action);

            task.Start();
        }
    }
}