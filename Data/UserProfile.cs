using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UAPTCBot.Bots;

namespace UAPTCBot.Data
{
    public class UserProfile
    {
        
        public string LastReportedSymptom { get; set; } = CovidSymptomChoices.NONE;
    }
}
