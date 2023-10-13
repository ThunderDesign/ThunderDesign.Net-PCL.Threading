﻿using SimpleThreadTest.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Collections;

namespace SimpleThreadTest.DataCollections
{
    internal class ThreadTestCollection : ObservableCollectionThreadSafe<ThreadTestObject>
    {
    }
}
