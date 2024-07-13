using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Network
    {
        private static Network network = new Network();
        public static Network getNetwork() { return network; } 


        private Network() { 
        
        }

        public void init()
        {

        }


    }
}
