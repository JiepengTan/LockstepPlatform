using System;
using System.Collections.Generic;

namespace Lockstep.AI
{
    public class BTWorkingData : TAny
    {
        //------------------------------------------------------
        internal Dictionary<int, BTActionContext> _context;
        internal Dictionary<int, BTActionContext> context 
        {
            get 
            {
                return _context;
            }
        }
        //------------------------------------------------------
        public BTWorkingData()
        {
            _context = new Dictionary<int, BTActionContext>();
        }
        ~BTWorkingData()
        {
            _context = null;
        }
    }
}
