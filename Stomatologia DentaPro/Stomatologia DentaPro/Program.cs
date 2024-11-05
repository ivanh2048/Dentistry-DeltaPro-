using System;
using System.Globalization;

namespace PrzychodniaStomatologicznaDentaPro
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Wybierz opcję:");
                Console.WriteLine("1. Dodaj pacjenta");
                Console.WriteLine("2. Aktualizuj dane pacjenta");
                Console.WriteLine("3. Usuń pacjenta");
                Console.WriteLine("4. Wyświetl pacjentów");
                Console.WriteLine("5. Sprawdź procedury");
                Console.WriteLine("6. Wyświetl dentystów");
                Console.WriteLine("7. Zarządzaj wizytami");
                Console.WriteLine("8. Zarządzaj zabiegami");
                Console.WriteLine("9. Wyjdź");

                int option;
                while (!int.TryParse(Console.ReadLine(), out option) || option < 1 || option > 9)
                {
                    Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.");
                }

                switch (option)
                {
                    case 1:
                        AddPatient();
                        break;
                    case 2:
                        UpdatePatientData();
                        break;
                    case 3:
                        DeletePatient();
                        break;
                    case 4:
                        DisplayDatabasePatient();
                        break;
                    case 5:
                        CheckProcedures();
                        break;
                    case 6:
                        DisplayDataBaseDentist();
                        break;
                    case 7:
                        VisitMenu();
                        break;
                    case 8:
                        ProcedureMenu();
                        break;
                    case 9:
                        return;
                }
            }
        }

        static void VisitMenu()
        {
            while (true)
            {
                Console.WriteLine("Wybierz opcję dla wizyt:");
                Console.WriteLine("1. Dodaj wizytę");
                Console.WriteLine("2. Wyświetl wizyty");
                Console.WriteLine("3. Dodaj zabieg do wizyty");
                Console.WriteLine("4. Wyświetl zabiegi wizyty");
                Console.WriteLine("5. Usuń zakończoną wizytę");
                Console.WriteLine("6. Wróć do głównego menu");

                int option;
                while (!int.TryParse(Console.ReadLine(), out option) || option < 1 || option > 6)
                {
                    Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.");
                }

                switch (option)
                {
                    case 1:
                        AddVisit();
                        break;
                    case 2:
                        DisplayVisits();
                        break;
                    case 3:
                        AddProcedureToVisit();
                        break;
                    case 4:
                        DisplayVisitProcedures();
                        break;
                    case 5:
                        DeleteCompletedVisit();
                        break;
                    case 6:
                        return;
                }
            }
        }

        static void DeleteCompletedVisit()
        {
            Console.WriteLine("Podaj ID wizyty do usunięcia:");
            int visitId = Convert.ToInt32(Console.ReadLine());
            DatabaseOperations.DeleteCompletedVisit(visitId);
            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
        }


        static void ProcedureMenu()
        {
            while (true)
            {
                Console.WriteLine("Wybierz opcję dla zabiegów:");
                Console.WriteLine("1. Dodaj zabieg");
                Console.WriteLine("2. Wyświetl zabiegi");
                Console.WriteLine("3. Wróć do głównego menu");

                int option;
                while (!int.TryParse(Console.ReadLine(), out option) || option < 1 || option > 3)
                {
                    Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.");
                }

                switch (option)
                {
                    case 1:
                        AddProcedure();
                        break;
                    case 2:
                        DisplayProcedures();
                        break;
                    case 3:
                        return;
                }
            }
        }

        static void CheckProcedures()
        {
            DatabaseOperations.CheckProcedures();
        }

        static void DisplayDatabasePatient()
        {
            DatabaseOperations.DisplayDatabasePatient();
        }

        static void DisplayDataBaseDentist()
        {
            DatabaseOperations.DisplayDentists();
        }

        static void AddPatient()
        {
            Console.WriteLine("Podaj imię pacjenta:");
            string name = Console.ReadLine();
            Console.WriteLine("Podaj nazwisko pacjenta:");
            string surname = Console.ReadLine();
            Console.WriteLine("Podaj datę urodzenia pacjenta (RRRR-MM-DD):");
            DateTime dateOfBirth;
            while (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth))
            {
                Console.WriteLine("Nieprawidłowy format daty. Podaj datę w formacie RRRR-MM-DD:");
            }
            Console.WriteLine("Podaj adres pacjenta:");
            string address = Console.ReadLine();
            Console.WriteLine("Podaj telefon pacjenta:");

            string phoneInput = Console.ReadLine();
            if (!int.TryParse(phoneInput, out _))
            {
                Console.WriteLine("Nieprawidłowy format telefonu. Wprowadź tylko cyfry.");
                return;
            }
            string phone = phoneInput;

            Console.WriteLine("Podaj email pacjenta:");
            string email = Console.ReadLine();

            try
            {
                int patientId = DatabaseOperations.AddPatient(name, surname, dateOfBirth, address, phone, email);
                Console.WriteLine("Pacjent został dodany.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }




        static void UpdatePatientData()
        {
            Console.WriteLine("Podaj ID pacjenta do aktualizacji:");
            int idPatient;
            while (!int.TryParse(Console.ReadLine(), out idPatient))
            {
                Console.WriteLine("Nieprawidłowy format ID. Podaj ID pacjenta jako liczbę całkowitą:");
            }

            Console.WriteLine("Podaj nowe imię:");
            string name = Console.ReadLine();
            Console.WriteLine("Podaj nowe nazwisko:");
            string surname = Console.ReadLine();
            Console.WriteLine("Podaj nową datę urodzenia (RRRR-MM-DD):");
            DateTime dateOfBirth;
            while (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateOfBirth))
            {
                Console.WriteLine("Niepoprawny format daty. Podaj datę w formacie RRRR-MM-DD:");
            }
            Console.WriteLine("Podaj nowy adres:");
            string address = Console.ReadLine();
            Console.WriteLine("Podaj nowy telefon:");
            string phone = Console.ReadLine();
            Console.WriteLine("Podaj nowy email:");
            string email = Console.ReadLine();

            try
            {
                DatabaseOperations.UpdatePatientData(idPatient, name, surname, dateOfBirth, address, phone, email);
                Console.WriteLine("Dane pacjenta zostały zaktualizowane.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }
        }

        static void DeletePatient()
        {
            Console.WriteLine("Podaj ID pacjenta do usunięcia:");
            int idPatient;
            while (!int.TryParse(Console.ReadLine(), out idPatient))
            {
                Console.WriteLine("Nieprawidłowy format ID. Podaj ID pacjenta jako liczbę całkowitą:");
            }

            try
            {
                DatabaseOperations.DeletePatient(idPatient);
                Console.WriteLine("Pacjent został usunięty.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }
        }

        static void AddVisit()
        {
            Console.WriteLine("Podaj ID pacjenta:");
            int patientId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Podaj ID dentysty:");
            int dentistId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Podaj datę wizyty (RRRR-MM-DD):");
            DateTime visitDate;
            while (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out visitDate))
            {
                Console.WriteLine("Niepoprawny format daty. Podaj datę w formacie RRRR-MM-DD:");
            }
            Console.WriteLine("Podaj czas wizyty (HH:MM):");
            TimeSpan visitTime;
            while (!TimeSpan.TryParse(Console.ReadLine(), out visitTime))
            {
                Console.WriteLine("Niepoprawny format czasu. Podaj czas w formacie HH:MM:");
            }
            Console.WriteLine("Podaj opis wizyty:");
            string description = Console.ReadLine();

            int visitId = DatabaseOperations.AddVisit(patientId, dentistId, visitDate, visitTime, description);
            Console.WriteLine($"Wizyta została dodana. ID wizyty: {visitId}");
        }


        static void DisplayVisits()
        {
            DatabaseOperations.DisplayVisits();
        }

        static void AddProcedure()
        {
            Console.WriteLine("Podaj nazwę zabiegu:");
            string procedureName = Console.ReadLine();
            Console.WriteLine("Podaj opis zabiegu:");
            string description = Console.ReadLine();
            Console.WriteLine("Podaj cenę zabiegu:");
            decimal price;
            while (!decimal.TryParse(Console.ReadLine(), out price))
            {
                Console.WriteLine("Nieprawidłowy format ceny. Podaj cenę w formacie liczby zmiennoprzecinkowej:");
            }

            DatabaseOperations.AddProcedure(procedureName, description, price);
        }


        static void DisplayProcedures()
        {
            DatabaseOperations.DisplayProcedures();
        }

        static void AddProcedureToVisit()
        {
            Console.WriteLine("Podaj ID wizyty:");
            int visitId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Podaj ID zabiegu:");
            int procedureId = Convert.ToInt32(Console.ReadLine());

            DatabaseOperations.AddProcedureToVisit(visitId, procedureId);
            Console.WriteLine("Zabieg został dodany do wizyty.");
        }

        static void DisplayVisitProcedures()
        {
            Console.WriteLine("Podaj ID wizyty:");
            int visitId = Convert.ToInt32(Console.ReadLine());

            DatabaseOperations.DisplayVisitProcedures(visitId);
        }
    }
}