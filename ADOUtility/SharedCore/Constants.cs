using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedCore
{
    public static class Constants
    {
        public static readonly Dictionary<string, (DateTime start, DateTime end)> Sprints = new()
        {
            {"2025_S01_Jan01-Jan14", (new DateTime(2025, 1, 1), new DateTime(2025, 1, 14))},
            {"2025_S02_Jan15-Jan28", (new DateTime(2025, 1, 15), new DateTime(2025, 1, 28))},
            {"2025_S03_Jan29-Feb11", (new DateTime(2025, 1, 29), new DateTime(2025, 2, 11))},
            {"2025_S04_Feb12-Feb25", (new DateTime(2025, 2, 12), new DateTime(2025, 2, 25))},
            {"2025_S05_Feb26-Mar11", (new DateTime(2025, 2, 26), new DateTime(2025, 3, 11))},
            {"2025_S06_Mar12-Mar25", (new DateTime(2025, 3, 12), new DateTime(2025, 3, 25))},
            {"2025_S07_Mar26-Apr08", (new DateTime(2025, 3, 26), new DateTime(2025, 4, 8))},
            {"2025_S08_Apr09-Apr22", (new DateTime(2025, 4, 9), new DateTime(2025, 4, 22))},
            {"2025_S09_Apr23-May06", (new DateTime(2025, 4, 23), new DateTime(2025, 5, 6))},
            {"2025_S10_May07-May20", (new DateTime(2025, 5, 7), new DateTime(2025, 5, 20))},
            {"2025_S11_May21-Jun03", (new DateTime(2025, 5, 21), new DateTime(2025, 6, 3))},
            {"2025_S12_Jun04-Jun17", (new DateTime(2025, 6, 4), new DateTime(2025, 6, 17))},
            {"2025_S13_Jun18-Jul01", (new DateTime(2025, 6, 18), new DateTime(2025, 7, 1))},
            {"2025_S14_Jul02-Jul15", (new DateTime(2025, 7, 2), new DateTime(2025, 7, 15))},
            {"2025_S15_Jul16-Jul29", (new DateTime(2025, 7, 16), new DateTime(2025, 7, 29))},
            {"2025_S16_Jul30-Aug12", (new DateTime(2025, 7, 30), new DateTime(2025, 8, 12))},
            {"2025_S17_Aug13-Aug26", (new DateTime(2025, 8, 13), new DateTime(2025, 8, 26))},
            {"2025_S18_Aug27-Sep09", (new DateTime(2025, 8, 27), new DateTime(2025, 9, 9))}
        };

        public static readonly List<(string name, string surname)> AQAsFirefly = new()
        {
            ("Aksana","Murashka"),
            ("Abdugofur","Turayev"),
            ("Bakhtiyar","Kussainov"),
            ("Davit","Simonyan"),
            ("Pavana","Balusseri"),
            ("Suraj","Sonawane")
        };

        public static readonly List<(string name, string surname)> AQAsLTDev = new()
        {
            ("Bhavani","K v"),
            ("Gomathi","Pachiappan"),
            ("Goutham","Pole"),
            ("Khusniddin","Nusurmatov"),
            ("Lusine","Hovhannisyan"),
            ("Sreelakshmi","Sajeev"),
            ("Vyacheslav","Kim"),
            ("Hleb","Salauyou")
        };

        public static readonly List<(string name, string surname)> AQAsFGCO = new()
        {
            ("Amit","Kumar"),
            ("Laura","Hambardzumyan"),
            ("Mohamed","Faisal"),
            ("Neha","Bhuptani"),
            ("Vitali","Pasavets")
        };

        public static readonly List<(string name, string surname)> AQAsLTAll = AQAsLTDev.Concat(AQAsFirefly).ToList();
    }
}
