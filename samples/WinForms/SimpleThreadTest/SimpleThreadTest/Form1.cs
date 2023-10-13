using SimpleThreadTest.DataCollections;
using SimpleThreadTest.DataObjects;
using System.Collections.Specialized;
using System.ComponentModel;
using ThunderDesign.Net.Threading.HelperClasses;

namespace SimpleThreadTest
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            //lbThreadOutput.DataSource = _ThreadTestList;
            //lbThreadOutput.DisplayMember = nameof(ThreadTestObject.DisplayText);
            //lbThreadOutput.ValueMember = nameof(ThreadTestObject.Id);
            //lbThreadOutput.data
            //lbThreadOutput.DataBind();
            //lbThreadOutput.ValueMember = nameof(ThreadTestObject.Counter_AsString);
            _ThreadTestCollection.CollectionChanged += OnThreadTestCollection_CollectionChanged;
        }

        private void OnThreadTestCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        if (item is ThreadTestObject)
                        {
                            ((ThreadTestObject)item).PropertyChanged += OnThreadTestObjectPropertyChanged;
                        }
                    }
                    OnRefreshDisplay(sender, new EventArgs());
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        if (item is ThreadTestObject)
                        {
                            ((ThreadTestObject)item).PropertyChanged -= OnThreadTestObjectPropertyChanged;
                        }
                    }
                    OnRefreshDisplay(sender, new EventArgs());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }

        private void OnThreadTestObjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!(sender is ThreadTestObject))
                return;

            if (String.Equals(e.PropertyName, nameof(ThreadTestObject.DisplayText), StringComparison.OrdinalIgnoreCase))
                OnRefreshDisplay(sender, new EventArgs());
        }

        private void OnRefreshDisplay(object? sender, EventArgs e)
        {
            if (lbThreadOutput.InvokeRequired)
            {
                //return;
                lbThreadOutput.Invoke(new EventHandler(OnRefreshDisplay), sender, e);
                return;
            }

            lbThreadOutput.DataSource = null;
            lbThreadOutput.DataSource = _ThreadTestCollection;
            lbThreadOutput.DisplayMember = nameof(ThreadTestObject.DisplayText);
            lbThreadOutput.ValueMember = nameof(ThreadTestObject.Id);
        }

        private async void BtnTest_Click(object sender, EventArgs e)
        {
            ThreadTestObject threadTestObject = new ThreadTestObject();
            threadTestObject.Id = 1;
            threadTestObject.Name = "Test 1";
            _ThreadTestCollection.Add(threadTestObject);

            ThreadHelper.RunAndForget(() =>
            {
                Task.Delay(1000).GetAwaiter().GetResult();
                threadTestObject.Name = "Changed";
            });
            
            //Task.Delay(5000).GetAwaiter().GetResult();

            threadTestObject = new ThreadTestObject();
            threadTestObject.Id = 2;
            threadTestObject.Name = "Test 2";
            _ThreadTestCollection.Add(threadTestObject);
            
            ThreadHelper.RunAndForget(() =>
            //Task.Run(()=>
            {
                ThreadTestObject threadTestObject2 = new ThreadTestObject();
                threadTestObject2.Id = 3;
                threadTestObject2.Name = "Test 3";
                _ThreadTestCollection.Add(threadTestObject2);

                threadTestObject2 = new ThreadTestObject();
                threadTestObject2.Id = 4;
                threadTestObject2.Name = "Test 4";
                _ThreadTestCollection.Add(threadTestObject2);

                threadTestObject2 = new ThreadTestObject();
                threadTestObject2.Id = 5;
                threadTestObject2.Name = "Test 5";
                _ThreadTestCollection.Add(threadTestObject2);
            });
        }

        //private readonly ThreadTestDictionary _ThreadTestDictionary = new ThreadTestDictionary();
        //private readonly ThreadTestList _ThreadTestList = new ThreadTestList();
        private readonly ThreadTestCollection _ThreadTestCollection = new ThreadTestCollection();

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            lbThreadOutput.DataSource = null;
            lbThreadOutput.DataSource = _ThreadTestCollection;
            lbThreadOutput.DisplayMember = nameof(ThreadTestObject.DisplayText);
            lbThreadOutput.ValueMember = nameof(ThreadTestObject.Id);
        }
    }
}