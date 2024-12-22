using PWD_DSWD_SPC.Models.Registered;
using System.Numerics;
using System;
namespace PWD_DSWD_SPC.Barangays
{
 
   
        public static class Brgy
        {
            public static List<Account> GetAll()
            {
                return new List<Account>
                {
    
                    new Account() { AD = "aa", Barangay = "I-A (Sambat)" },
                    new Account() { AD = "bb", Barangay = "I-B (City+Riverside)" },
                    new Account() { AD = "cc", Barangay = "I-C (Bagong Bayan)" },
                    new Account() { AD = "dd", Barangay = "II-A (Triangulo)" },
                    new Account() { AD = "ee", Barangay = "II-B (Guadalupe)" },
                    new Account() { AD = "ff", Barangay = "II-C (Unson)" },
                    new Account() { AD = "gg", Barangay = "II-D (Bulante)" },
                    new Account() { AD = "hh", Barangay = "II-E (San Anton)" },
                    new Account() { AD = "ii", Barangay = "II-F (Villa Rey)" },
                    new Account() { AD = "jj", Barangay = "III-A (Hermanos Belen)" },
                    new Account() { AD = "kk", Barangay = "III-B" },
                    new Account() { AD = "ll", Barangay = "III-C (Labak/De Roma)" },
                    new Account() { AD = "mm", Barangay = "III-D (Villongco)" },
                    new Account() { AD = "nn", Barangay = "III-E" },
                    new Account() { AD = "oo", Barangay = "III-F (Balagtas)" },
                    new Account() { AD = "pp", Barangay = "IV-A" },
                    new Account() { AD = "qq", Barangay = "IV-B" },
                    new Account() { AD = "rr", Barangay = "IV-C" },
                    new Account() { AD = "ss", Barangay = "V-A" },
                    new Account() { AD = "tt", Barangay = "V-B" },
                    new Account() { AD = "uu", Barangay = "V-C" },
                    new Account() { AD = "vv", Barangay = "V-D" },
                    new Account() { AD = "ww", Barangay = "VI-A (Mavenida)" },
                    new Account() { AD = "xx", Barangay = "VI-B" },
                    new Account() { AD = "yy", Barangay = "VI-C (Bagong Pook)" },
                    new Account() { AD = "zz", Barangay = "VI-D (Lparkers)" },
                    new Account() { AD = "aaa", Barangay = "VI-E (YMCA)" },
                    new Account() { AD = "bbb", Barangay = "VII-A (P.Alcantara)" },
                    new Account() { AD = "ccc", Barangay = "VII-B" },
                    new Account() { AD = "ddd", Barangay = "VII-C" },
                    new Account() { AD = "eee", Barangay = "VII-D" },
                    new Account() { AD = "fff", Barangay = "VII-E" },
                    new Account() { AD = "ggg", Barangay = "Atisan" },
                    new Account() { AD = "hhh", Barangay = "Bautista" },
                    new Account() { AD = "iii", Barangay = "Concepcion (Bunot)" },
                    new Account() { AD = "jjj", Barangay = "Del Remedio (Wawa)" },
                    new Account() { AD = "kkk", Barangay = "Dolores" },
                    new Account() { AD = "lll", Barangay = "San Antonio 1 (Balanga)" },
                    new Account() { AD = "mmm", Barangay = "San Antonio 2 (Sapa)" },
                    new Account() { AD = "nnn", Barangay = "San Bartolome (Matang-ag)" },
                    new Account() { AD = "ooo", Barangay = "San Buenaventura (Palakpakin)" },
                    new Account() { AD = "ppp", Barangay = "San Crispin (Lumbangan)" },
                    new Account() { AD = "qqq", Barangay = "San Cristobal" },
                    new Account() { AD = "rrr", Barangay = "San Diego (Tiim)" },
                    new Account() { AD = "sss", Barangay = "San Francisco (Calihan)" },
                    new Account() { AD = "ttt", Barangay = "San Gabriel (Butucan)" },
                    new Account() { AD = "uuu", Barangay = "San Gregorio" },
                    new Account() { AD = "vvv", Barangay = "San Ignacio" },
                    new Account() { AD = "www", Barangay = "San Isidro (Balagbag)" },
                    new Account() { AD = "xxx", Barangay = "San Joaquin" },
                    new Account() { AD = "yyy", Barangay = "San Jose (Malamig)" },
                    new Account() { AD = "zzz", Barangay = "San Juan" },
                    new Account() { AD = "aaaa", Barangay = "San Lorenzo (Saluyan)" },
                    new Account() { AD = "bbbb", Barangay = "San Lucas 1 (Malinaw)" },
                    new Account() { AD = "cccc", Barangay = "San Lucas 2" },
                    new Account() { AD = "dddd", Barangay = "San Marcos (Tikew)" },
                    new Account() { AD = "eeee", Barangay = "San Mateo" },
                    new Account() { AD = "ffff", Barangay = "San Miguel" },
                    new Account() { AD = "gggg", Barangay = "San Nicolas" },
                    new Account() { AD = "hhhh", Barangay = "San Pedro" },
                    new Account() { AD = "iiii", Barangay = "San Rafael (Magampon)" },
                    new Account() { AD = "jjjj", Barangay = "San Roque (Buluburan)" },
                    new Account() { AD = "kkkk", Barangay = "San Vicente" },
                    new Account() { AD = "llll", Barangay = "Santa Ana" },
                    new Account() { AD = "mmmm", Barangay = "Santa Catalina (Sandig)" },
                    new Account() { AD = "nnnn", Barangay = "Santa Cruz (Putol)" },
                    new Account() { AD = "oooo", Barangay = "Santa Elena" },
                    new Account() { AD = "pppp", Barangay = "Santa Filomena (Banlagin)" },
                    new Account() { AD = "qqqq", Barangay = "Santa Isabel" },
                    new Account() { AD = "rrrr", Barangay = "Santa Maria" },
                    new Account() { AD = "ssss", Barangay = "Santa Maria Magdalena (Boe)" },
                    new Account() { AD = "tttt", Barangay = "Santa Monica" },
                    new Account() { AD = "uuuu", Barangay = "Santa Veronica (Bae)" },
                    new Account() { AD = "vvvv", Barangay = "Santiago I (Bulaho)" },
                    new Account() { AD = "wwww", Barangay = "Santiago II" },
                    new Account() { AD = "xxxx", Barangay = "Santisimo Rosario" },
                    new Account() { AD = "yyyy", Barangay = "Santo Angel (Ilog)" },
                    new Account() { AD = "zzzz", Barangay = "Santo Cristo" },
                    new Account() { AD = "aaaaa", Barangay = "Santo Niño (Arsum)" },
                    new Account() { AD = "bbbbb", Barangay = "Soledad (Macopa)" },
                    



            };
            }
        }
    }
