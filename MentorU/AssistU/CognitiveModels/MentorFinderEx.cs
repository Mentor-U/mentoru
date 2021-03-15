using System;
using System.Linq;

namespace CoreBot.CognitiveModels
{
    public partial class MentorFinder
    {
        public string ExtractField
        {
            get
            {
                return Entities?._instance?.Field?.FirstOrDefault()?.Text;
            }
        }

        public string ExtractSkill
        {
            get
            {
                return Entities?._instance?.Field?.FirstOrDefault()?.Text;
            }
        }
    }
}
