using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;

namespace UserList
{

    [Serializable]
    public class Person
    {
        private const string DataFilepath = "database";
        private const string PersonFileTemplate = "p{0}.bin";

        public string Name { get; private set; }

        public string Surname { get; private set; }

        public DateTime Birthday { get; private set; }

        public string Email { get; private set; }


        public Person(string name, string surname, string email, DateTime birthday)
        {
            if (name.Length < 2)
            {
                throw new WrongNameException($"_name {name} is too small!");
            }

            if (surname.Length < 2)
            {
                throw new WrongSurnameException($"_surname {surname} is too small!");
            }

            var yearsDif = DateTime.Today.YearsPassedCnt(birthday);
            if (yearsDif < 0 || yearsDif > 135)
            {
                throw new WrongBirthdayException(birthday);
            }


            if (email.Length < 3 || email.Count(f => f == '@') != 1 ||
                (email.IndexOf("@", StringComparison.Ordinal) == email.Length - 1) ||
                (email.IndexOf("@", StringComparison.Ordinal) == 0))
            {
                throw new WrongEmailException(email);
            }

            Name = name;
            Surname = surname;
            Birthday = birthday;
            Email = email;
        }

        public Person(string name, string surname, string email) : this(name, surname, email, DateTime.Today)
        {
        }

        public Person(string name, string surname, DateTime birthday) : this(name, surname, "not specified", birthday)
        {
        }

        public bool IsAdult => DateTime.Today.YearsPassedCnt(Birthday) >= 18;
        public bool IsBirthday => DateTime.Today.DayOfYear == Birthday.DayOfYear;

        public string ChineseSign => ChineseZodiaсs[(Birthday.Year + 8) % 12];

        public void CopyFrom(Person person)
        {
            Name = person.Name;
            Surname = person.Surname;
            Birthday = person.Birthday;
            Email = person.Email;
        }

        public string SunSign
        {
            get
            {
                var birthday = Birthday.Day;
                int value;
                switch (Birthday.Month)
                {
                    case 1:
                        value = birthday <= 20 ? 9 : 10;
                        break;
                    case 2:
                        value = birthday <= 19 ? 10 : 11;
                        break;
                    case 3:
                        value = birthday <= 20 ? 11 : 0;
                        break;
                    case 4:
                        value = birthday <= 20 ? 0 : 1;
                        break;
                    case 5:
                        value = birthday <= 20 ? 1 : 2;
                        break;
                    case 6:
                        value = birthday <= 20 ? 2 : 3;
                        break;
                    case 7:
                        value = birthday <= 21 ? 3 : 4;
                        break;
                    case 8:
                        value = birthday <= 22 ? 4 : 5;
                        break;
                    case 9:
                        value = birthday <= 21 ? 5 : 6;
                        break;
                    case 10:
                        value = birthday <= 21 ? 6 : 7;
                        break;
                    case 11:
                        value = birthday <= 21 ? 7 : 8;
                        break;
                    case 12:
                        value = birthday <= 21 ? 8 : 9;
                        break;
                    default:
                        value = 0;
                        break;
                }

                return WesternZodiaсs[value];
            }
        }


        private static Person LoadFrom(string filename)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(filename,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);
                var person = (Person)formatter.Deserialize(stream);
                stream.Close();
                return person;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async void LoadAllInto(List<Person> persons, Action action)
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(DataFilepath))
                {
                    Directory.CreateDirectory(DataFilepath);
                    persons.AddRange(PersonSpawner.SpawnPersons(50));
                    SaveAll(persons);
                }
                else
                {
                    persons.AddRange(Directory.EnumerateFiles(DataFilepath).Select(LoadFrom));
                }
                action();
            });
        }


        private void SaveTo(string filename)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Directory.CreateDirectory(Path.GetDirectoryName(filename) ?? throw new InvalidOperationException());
                Stream stream = new FileStream(path: filename,
                    mode: FileMode.Create,
                    access: FileAccess.Write,
                    share: FileShare.None);
                formatter.Serialize(serializationStream: stream, graph: this);
                stream.Close();
            }
            catch (IOException e)
            {

            }
        }

        public static void SaveAll(List<Person> persons)
        {
            var i = 0;
            persons.ForEach(delegate (Person p)
            {
                p.SaveTo(Path.Combine(DataFilepath, string.Format(PersonFileTemplate, i++)));
            });
            string extraFile;
            while (File.Exists(extraFile = Path.Combine(DataFilepath, string.Format(PersonFileTemplate, i++))))
                File.Delete(extraFile);
        }

        private static class PersonSpawner
        {
            public static List<Person> SpawnPersons(int count)
            {
                var persons = new List<Person>();
                var random = new Random();
                for (var i = 0; i < count; ++i)
                {
                    var name = Names[random.Next(Names.Length)];
                    var surname = Surnames[random.Next(Surnames.Length)];

                    persons.Add(new Person(name, surname,
                        $"{name}.{surname}@{Emails[random.Next(Emails.Length)]}",
                        DateTime.Now.AddYears(-random.Next(10, 80)).AddDays(-random.Next(31))
                            .AddMonths(-random.Next(12))));
                }

                return persons;
            }

            private static readonly string[] Emails =
            {
                "gmail.com",
                "ukr.net",
            };

            private static readonly string[] Names =
            {
                "Leisha",
                "Rabinowitz",
                "Ernest",
                "Gunderman",
                "Arleen",
                "Bossi",
                "Tesha",
                "Likes",
                "Dyan",
                "Reep",
                "Margarite",
                "Oehler",
                "Ernie",
                "Bouknight",
                "Ned",
                "Manseau",
                "Thao",
                "Sylvestre",
                "Darcie",
                "Schorr",
                "Silvana",
                "Huggins",
                "Sandy",
                "Pyburn",
                "Kayla",
                "Bachman",
                "Song",
                "Pfeiffer",
                "Collen",
                "Melody",
                "Pasty",
                "Vanderpool",
                "Jeromy",
                "Guidry",
                "Joycelyn",
                "Diem",
                "Jack",
                "Milbourne",
                "Lasonya",
                "Levar",
                "Eusebio",
                "Hamernik",
                "Renae",
                "Bradt",
                "Leatha",
                "Lo",
                "Asha",
                "Towner",
                "Angelena",
                "Creighton"
            };

            private static readonly string[] Surnames =
            {
                "Nichole",
                "Wegener",
                "Cleopatra",
                "Tannenbaum",
                "Thaddeus",
                "Sparling",
                "Genesis",
                "Sommer",
                "Assunta",
                "Stratford",
                "Nannie",
                "Southern",
                "Celestina",
                "Siller",
                "Janessa",
                "Curtin",
                "Chantell",
                "Waiters",
                "Malika",
                "Remington",
                "Nichol",
                "Cosey",
                "David",
                "Gagnier",
                "Sherie",
                "Dipaola",
                "Delta",
                "Feucht",
                "Kenyatta",
                "Minns",
                "Elane",
                "Zartman",
                "Mechelle",
                "Hinshaw",
                "Deja",
                "Parkman",
                "Ivelisse",
                "Blatt",
                "Kimi",
                "Lamon",
                "Kizzy",
                "Snell",
                "Keturah",
                "Carrera",
                "Kraig",
                "Dominick",
                "Tomoko",
                "Gans",
                "Brittani",
                "Dubiel"
            };

        }

        private static readonly string[] ChineseZodiaсs =
        {
            "Rat",
            "Ox",
            "Tiger",
            "Rabbit",
            "Dragon",
            "Snake",
            "Horse",
            "Goat",
            "Monkey",
            "Rooster",
            "Dog",
            "Pig"
        };

        private static readonly string[] WesternZodiaсs =
        {
            "Ram",
            "Bull",
            "Twins",
            "Crab",
            "Lion",
            "Virgin",
            "Scales",
            "Scorpion",
            "Archer",
            "Mountain Sea-Goat",
            "Water Bearer",
            "Fish"
        };
    }

    public class PersonCreationException : Exception
    {
        public PersonCreationException(string message)
            : base(message)
        {
        }
    }

    public class WrongNameException : PersonCreationException
    {
        public WrongNameException(string message)
            : base(message)
        {
        }
    }

    public class WrongSurnameException : PersonCreationException
    {
        public WrongSurnameException(string message)
            : base(message)
        {
        }
    }

    public class WrongEmailException : PersonCreationException
    {
        public WrongEmailException(string givenEmail)
            : base($"Email {givenEmail} is not valid!")
        {
        }
    }

    public class WrongBirthdayException : PersonCreationException
    {
        public WrongBirthdayException(DateTime birthday)
            : base($"Birthday {birthday.ToShortDateString()} is not valid!")
        {
        }
    }
}
