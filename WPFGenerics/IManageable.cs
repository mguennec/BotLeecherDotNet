using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFGenerics
{
    public interface IManageable
    {
        Guid InstanceID { get; set; }
        void SaveSettings();
    }
}
