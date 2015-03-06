using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CommonLibrary
{
    public interface IData
    {
        XElement Element { get; }
    }
}
