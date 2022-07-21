using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Standalone
{
    public class EvalutationParameters
    {
        private int _offset;
        private int _fieldnumber;
        private Plane _dir;

        public int Offset
        {
            get { return _offset; }
        }
        public int Fieldnumber
        {
            get { return _fieldnumber; }
        }
        public Plane Dir
        {
            get { return _dir; }
        }

        public EvalutationParameters(string offset, int fieldnumber, Plane dir)
        {
            _offset = int.Parse(offset);
            _fieldnumber = fieldnumber;
            _dir = dir;
        }
    }
}
