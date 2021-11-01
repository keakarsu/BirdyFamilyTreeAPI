using BirdyModel.Database.Birdy;
using System;
using System.Collections.Generic;
using System.Text;

namespace BirdyModel
{
    public class BirdDto : Bird
    {
        public string mother_identity { get; set; }
        public string mother_nickname { get; set; }
        public string father_identity { get; set; }
        public string father_nickname { get; set; }
        public List<BirdDto> parent{ get; set; }
        public List<BirdDto> children{ get; set; }
    }
}
