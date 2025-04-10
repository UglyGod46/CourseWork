using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.models
{
    public class Token
    {
        public int Code { get; set; }
        public string Type { get; set; }
        public string Lexeme { get; set; }
        public string Position { get; set; }
    }
}