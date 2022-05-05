using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Exceptions
{
    public class CharMapAlreadyDefinedException : Exception
    {
        public string MapName { get; private set; }

        public CharMapAlreadyDefinedException(string mapName, string message)
            : base(message)
        {
            MapName = mapName;
        }
    }
}
