using System;
using System.Collections.Generic;
using System.Globalization;
using Npgsql;

namespace PrzychodniaStomatologicznaDentaPro
{
    static class DatabaseOperations
    {
        static string connectionString = "Host=localhost;Username=postgres;Password=2004;Database=stomatologia";

        public static void CheckProcedures()
        {
            string[] procedures = { "dodajpacjenta", "aktualizujdanepacjenta", "usunpacjenta" };
            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    foreach (var procedure in procedures)
                    {
                        string query = $"SELECT EXISTS (SELECT 1 FROM pg_proc WHERE proname = '{procedure}')";
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            bool exists = Convert.ToBoolean(cmd.ExecuteScalar());
                            Console.WriteLine($"Procedura '{procedure}' {(exists ? "istnieje" : "nie istnieje")}.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void DisplayDatabasePatient()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id_pacjenta, imię, nazwisko, data_urodzenia, adres, telefon, email FROM stomatologia.pacjent ORDER BY id_pacjenta;";

                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    int idPacjenta = reader.GetInt32(0);
                                    string imie = reader.GetString(1);
                                    string nazwisko = reader.GetString(2);
                                    DateTime dataUrodzenia = reader.GetDateTime(3);
                                    string formattedDate = dataUrodzenia.ToString("d", CultureInfo.InvariantCulture);
                                    string adres = reader.GetString(4);
                                    string telefon = reader.GetString(5);
                                    string email = reader.GetString(6);
                                    Console.WriteLine($"{idPacjenta}, {imie}, {nazwisko}, {formattedDate}, {adres}, {telefon}, {email}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Baza danych jest pusta.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        public static void DisplayDentists()
        {
            Console.Clear();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id_dentysty, imię, nazwisko, specjalizacja, telefon, email FROM stomatologia.dentysta ORDER BY id_dentysty;";

                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    int idDentysty = reader.GetInt32(0);
                                    string imie = reader.GetString(1);
                                    string nazwisko = reader.GetString(2);
                                    string specjalizacja = reader.GetString(3);
                                    string telefon = reader.GetString(4);
                                    string email = reader.GetString(5);
                                    Console.WriteLine($"{idDentysty}, {imie}, {nazwisko}, {specjalizacja}, {telefon}, {email}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Baza danych dentystów jest pusta.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static int AddPatient(string name, string surname, DateTime dateOfBirth, string address, string phone, string email)
        {
            int lastInsertedId;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("stomatologia.dodajpacjenta", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("imie", NpgsqlTypes.NpgsqlDbType.Text, name);
                    cmd.Parameters.AddWithValue("nazwisko", NpgsqlTypes.NpgsqlDbType.Text, surname);
                    cmd.Parameters.AddWithValue("data_urodzenia", NpgsqlTypes.NpgsqlDbType.Date, dateOfBirth);
                    cmd.Parameters.AddWithValue("adres", NpgsqlTypes.NpgsqlDbType.Text, address);
                    cmd.Parameters.AddWithValue("telefon", NpgsqlTypes.NpgsqlDbType.Varchar, phone);
                    cmd.Parameters.AddWithValue("email", NpgsqlTypes.NpgsqlDbType.Text, email);

                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new NpgsqlCommand("SELECT id_pacjenta FROM stomatologia.pacjent ORDER BY id_pacjenta DESC LIMIT 1", conn))
                {
                    lastInsertedId = (int)cmd.ExecuteScalar();
                    Console.WriteLine($"Dodano pacjenta. Identyfikator pacjenta: {lastInsertedId}");
                }
            }
            return lastInsertedId;
        }

        public static void UpdatePatientData(int idPatient, string name, string surname, DateTime dateOfBirth, string address, string phone, string email)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("stomatologia.aktualizujdanepacjenta", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("new_id_pacjenta", NpgsqlTypes.NpgsqlDbType.Integer, idPatient);
                    cmd.Parameters.AddWithValue("new_imie", NpgsqlTypes.NpgsqlDbType.Text, name);
                    cmd.Parameters.AddWithValue("new_nazwisko", NpgsqlTypes.NpgsqlDbType.Text, surname);
                    cmd.Parameters.AddWithValue("new_data_urodzenia", NpgsqlTypes.NpgsqlDbType.Date, dateOfBirth);
                    cmd.Parameters.AddWithValue("new_adres", NpgsqlTypes.NpgsqlDbType.Text, address);
                    cmd.Parameters.AddWithValue("new_telefon", NpgsqlTypes.NpgsqlDbType.Varchar, phone);
                    cmd.Parameters.AddWithValue("new_email", NpgsqlTypes.NpgsqlDbType.Text, email);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool DeletePatient(int idPatient)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("stomatologia.usunpacjenta", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("pid", NpgsqlTypes.NpgsqlDbType.Integer, idPatient);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static int AddVisit(int patientId, int dentistId, DateTime visitDate, TimeSpan visitTime, string notes)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("INSERT INTO stomatologia.wizyta (data, godzina, id_pacjenta, id_dentysty, notatki) VALUES (@data, @godzina, @id_pacjenta, @id_dentysty, @notatki) RETURNING id_wizyty", conn);
            cmd.Parameters.AddWithValue("data", NpgsqlTypes.NpgsqlDbType.Date, visitDate);
            cmd.Parameters.AddWithValue("godzina", NpgsqlTypes.NpgsqlDbType.Time, visitTime);
            cmd.Parameters.AddWithValue("id_pacjenta", NpgsqlTypes.NpgsqlDbType.Integer, patientId);
            cmd.Parameters.AddWithValue("id_dentysty", NpgsqlTypes.NpgsqlDbType.Integer, dentistId);
            cmd.Parameters.AddWithValue("notatki", NpgsqlTypes.NpgsqlDbType.Text, notes);

            int visitId = (int)cmd.ExecuteScalar();
            Console.WriteLine($"Wizyta została dodana. ID wizyty: {visitId}");
            return visitId;
        }


        public static void DisplayVisits()
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT id_wizyty, data, godzina, id_pacjenta, id_dentysty, notatki FROM stomatologia.wizyta", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int visitId = (int)reader["id_wizyty"];
                DateTime visitDate = (DateTime)reader["data"];
                string formattedDate = visitDate.ToString("d", CultureInfo.InvariantCulture);
                TimeSpan visitTime = (TimeSpan)reader["godzina"];
                int patientId = (int)reader["id_pacjenta"];
                int dentistId = (int)reader["id_dentysty"];
                string notes = (string)reader["notatki"];

                Console.WriteLine($"ID wizyty: {visitId}, Data: {formattedDate}, Godzina: {visitTime}, ID pacjenta: {patientId}, ID dentysty: {dentistId}, Notatki: {notes}");
            }
        }


        public static void AddProcedure(string name, string description, decimal price)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("INSERT INTO stomatologia.zabieg (nazwa, opis, cena) VALUES (@nazwa, @opis, @cena) RETURNING id_zabiegu", conn);
            cmd.Parameters.AddWithValue("nazwa", NpgsqlTypes.NpgsqlDbType.Text, name);
            cmd.Parameters.AddWithValue("opis", NpgsqlTypes.NpgsqlDbType.Text, description);
            cmd.Parameters.AddWithValue("cena", NpgsqlTypes.NpgsqlDbType.Numeric, price);

            int procedureId = (int)cmd.ExecuteScalar();
            Console.WriteLine($"Zabieg został dodany. ID zabiegu: {procedureId}");
        }

        public static void DisplayProcedures()
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT id_zabiegu, nazwa, opis, cena FROM stomatologia.zabieg", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"ID zabiegu: {reader["id_zabiegu"]}, Nazwa: {reader["nazwa"]}, Opis: {reader["opis"]}, Cena: {reader["cena"]}");
            }
        }

        public static void AddProcedureToVisit(int visitId, int procedureId)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var transaction = conn.BeginTransaction();
            try
            {
                using (var cmdCheckVisit = new NpgsqlCommand("SELECT COUNT(*) FROM stomatologia.wizyta WHERE id_wizyty = @id_wizyty", conn))
                {
                    cmdCheckVisit.Parameters.AddWithValue("id_wizyty", NpgsqlTypes.NpgsqlDbType.Integer, visitId);
                    int visitCount = (int)cmdCheckVisit.ExecuteScalar();
                    if (visitCount == 0)
                    {
                        Console.WriteLine("Wizyta nie istnieje.");
                        return;
                    }
                }

                using (var cmdCheckProcedure = new NpgsqlCommand("SELECT COUNT(*) FROM stomatologia.zabieg WHERE id_zabiegu = @id_zabiegu", conn))
                {
                    cmdCheckProcedure.Parameters.AddWithValue("id_zabiegu", NpgsqlTypes.NpgsqlDbType.Integer, procedureId);
                    int procedureCount = (int)cmdCheckProcedure.ExecuteScalar();
                    if (procedureCount == 0)
                    {
                        Console.WriteLine("Zabieg nie istnieje.");
                        return;
                    }
                }

                using (var cmd = new NpgsqlCommand("INSERT INTO stomatologia.wizyta_zabieg (id_wizyty, id_zabiegu) VALUES (@id_wizyty, @id_zabiegu)", conn))
                {
                    cmd.Parameters.AddWithValue("id_wizyty", NpgsqlTypes.NpgsqlDbType.Integer, visitId);
                    cmd.Parameters.AddWithValue("id_zabiegu", NpgsqlTypes.NpgsqlDbType.Integer, procedureId);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                Console.WriteLine("Zabieg został dodany do wizyty.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Błąd: {ex.Message}");
            }
        }


        public static void DisplayVisitProcedures(int visitId)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT z.id_zabiegu, z.nazwa, z.opis, z.cena FROM stomatologia.wizyta_zabieg wz " +
                "JOIN stomatologia.zabieg z ON wz.id_zabiegu = z.id_zabiegu " +
                "WHERE wz.id_wizyty = @id_wizyty", conn);
            cmd.Parameters.AddWithValue("id_wizyty", NpgsqlTypes.NpgsqlDbType.Integer, visitId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"ID zabiegu: {reader["id_zabiegu"]}, Nazwa: {reader["nazwa"]}, Opis: {reader["opis"]}, Cena: {reader["cena"]}");
            }
        }

        public static void DeleteCompletedVisit(int visitId)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string checkQuery = "SELECT data, godzina FROM stomatologia.wizyta WHERE id_wizyty = @visitId";
                        using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("visitId", visitId);
                            using (var reader = checkCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    DateTime visitDate = reader.GetDateTime(0);
                                    TimeSpan visitTime = reader.GetTimeSpan(1);
                                    DateTime visitDateTime = visitDate + visitTime;

                                    if (visitDateTime > DateTime.Now)
                                    {
                                        Console.WriteLine("Wizyta jeszcze się nie zakończyła.");
                                        return;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Wizyta nie istnieje.");
                                    return;
                                }
                            }
                        }

                        string deleteQuery = "DELETE FROM stomatologia.wizyta WHERE id_wizyty = @visitId";
                        using (var deleteCmd = new NpgsqlCommand(deleteQuery, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("visitId", visitId);
                            deleteCmd.ExecuteNonQuery();
                        }

                        // Aktualizacja identyfikatorów odwiedzin (CTE - Ogólne wyrażenia tabelaryczne)
                        string updateQuery = @"
                    WITH CTE AS (
                        SELECT id_wizyty, ROW_NUMBER() OVER (ORDER BY id_wizyty) AS new_id
                        FROM stomatologia.wizyta
                    )
                    UPDATE stomatologia.wizyta
                    SET id_wizyty = CTE.new_id
                    FROM CTE
                    WHERE stomatologia.wizyta.id_wizyty = CTE.id_wizyty;
                ";

                        /* @"UPDATE stomatologia.wizyta
                 SET id_wizyty = sub.new_id
                 FROM (
                     SELECT id_wizyty, ROW_NUMBER() OVER (ORDER BY id_wizyty) AS new_id
                     FROM stomatologia.wizyta
                 ) AS sub (zapytania z podzapytaniem)
                 WHERE stomatologia.wizyta.id_wizyty = sub.id_wizyty;
                 ";*/
                        using (var updateCmd = new NpgsqlCommand(updateQuery, conn))
                        {
                            updateCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        Console.WriteLine("Wizyta została usunięta, a identyfikatory wizyta zostały zaktualizowane.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Błąd: {ex.Message}");
                    }
                }
            }
        }

    }
}
