﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler
{
    public class Error
    {
        public int Code { get; set; }
        public int Position { get; set; }
        public string Message { get; set; }
        public int Number { get; set; }
    }
}
