using System;
using System.Collections.Generic;
using System.Text;

namespace ONX100.Models
{
    public class UnitPropertiesStatus
    {
        public bool IsPoweredOn { get; set; }
        public bool IsMuted { get; set; }
        public int VolumeLevel { get; set; }
        public int InputSource { get; set; }

    }
}
