using NCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTest
{
    public class Class1
    {
        private ClientUser a;

        public Class1(string aa) { A = new ClientUser(aa); }

        public ClientUser A
        {
            get
            {
                return a;
            }

            set
            {
                a = value;
            }
        }
    }
}
