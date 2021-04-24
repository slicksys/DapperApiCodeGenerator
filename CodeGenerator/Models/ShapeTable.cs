using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Models
{
    class ShapeTable
    {
            public string Name { get; set; }
            public string Type {get; set; }
            public string Length {get; set; }
            public bool IsNullable {get; set; }
    }
}
