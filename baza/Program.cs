using System;
using System.Data;
using System.Data.SqlClient;

namespace baza
{
    class Program
    {
        static string connectionString = "";  //Połączenie z bazą danych 
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Wyświetlanie pracowników");
                Console.WriteLine("2. Wyświetlanie pojedynczego pracownika");
                Console.WriteLine("3. Wyświetlanie stanowisk w firmie oraz liczbę osób pracujących na tym stanowisku");
                Console.WriteLine("4. Dodawanie nowego pracownika");
                Console.WriteLine("5. Usuwanie danego pracownika");
                Console.WriteLine("6. Aktualizacja hasła użytkownika");
                Console.WriteLine("7. Notatki o danym użytkowniku");
                Console.WriteLine("8. Wyjście");
                Console.WriteLine("Wybierz opcję:");
                int option;
                if (!int.TryParse(Console.ReadLine(), out option))
                {
                    Console.WriteLine("Niepoprawna opcja. Spróbuj ponownie.");
                    continue;
                }
                switch (option)
                {
                    case 1:
                        WyswietlPracownikow();
                        break;
                    case 2:
                        WyswietlPojedynczegoPracownika();
                        break;
                    case 3:
                        WyswietlStanowiskaILiczbePracownikow();
                        break;
                    case 4:
                        DodajPracownika();
                        break;
                    case 5:
                        UsunPracownika();
                        break;
                    case 6:
                        AktualizujHaslo();
                        break;
                    case 7:
                        ZarzadzajNotatkami();
                        break;
                    case 8:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Niepoprawna opcja. Spróbuj ponownie.");
                        break;
                }
            }
        }
        // Funkcja do wyświetlania pracowników
        static void WyswietlPracownikow() 
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM workers";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["id_worker"]}");
                    Console.WriteLine($"{reader["name"]} {reader["surname"]} ({reader["age"]} lat)");
                    Console.WriteLine($"Login: {reader["login"]}");
                    Console.WriteLine($"Zatrudniony: {reader["hire_date"]} na stanowisku {GetRoleName((int)reader["id_role"])}");
                    Console.WriteLine();
                }
                reader.Close();
            }
        }
        static void WyswietlPojedynczegoPracownika()
        {
            Console.WriteLine("Podaj ID pracownika:");
            int idPracownika;
            if (!int.TryParse(Console.ReadLine(), out idPracownika))
            {
                Console.WriteLine("Niepoprawne ID. Spróbuj ponownie.");
                return;
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT * FROM workers WHERE id_worker = {idPracownika}";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["id_worker"]}");
                    Console.WriteLine($"{reader["name"]} {reader["surname"]} ({reader["age"]} lat)");
                    Console.WriteLine($"Login: {reader["login"]}");
                    Console.WriteLine($"Zatrudniony: {reader["hire_date"]} na stanowisku {GetRoleName((int)reader["id_role"])}");
                }
                else
                {
                    Console.WriteLine("Brak pracownika o podanym ID.");
                }
                reader.Close();
            }}
      static void WyswietlStanowiskaILiczbePracownikow()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT role_name, COUNT(*) AS liczba_pracownikow FROM workers JOIN role ON workers.id_role = role.id_role GROUP BY role_name";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"{reader["role_name"]} - {reader["liczba_pracownikow"]} pracowników");
                }
                reader.Close();
            }
        }

        static void DodajPracownika()
        {
            Console.WriteLine("Podaj imię:");
            string imie = Console.ReadLine();
            Console.WriteLine("Podaj nazwisko:");
            string nazwisko = Console.ReadLine();
            Console.WriteLine("Podaj login:");
            string login = Console.ReadLine();
            Console.WriteLine("Podaj hasło:");
            string haslo = Console.ReadLine();
            Console.WriteLine("Podaj stanowisko (1. Administrator, 2. Rekruter, 3. Programista, 4. HR):");
            int idStanowiska;
            if (!int.TryParse(Console.ReadLine(), out idStanowiska) || idStanowiska < 1 || idStanowiska > 4)
            {
                Console.WriteLine("Niepoprawne ID stanowiska.");
                return;
            }
            Console.WriteLine("Podaj wiek:");
            int wiek;
            if (!int.TryParse(Console.ReadLine(), out wiek) || wiek <= 0)
            {
                Console.WriteLine("Niepoprawny wiek. ");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"INSERT INTO workers (name, surname, login, password, id_role, age) VALUES ('{imie}', '{nazwisko}', '{login}', '{haslo}', {idStanowiska}, {wiek})";
                SqlCommand command = new SqlCommand(query, connection);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Pracownik dodany pomyślnie.");
                }
                else
                {
                    Console.WriteLine("Dodanie pracownika nie powiodło się.");
                }
            }
        }
        static void UsunPracownika()
        {
            Console.WriteLine("Podaj ID pracownika do usunięcia:");
            int idPracownika;
            if (!int.TryParse(Console.ReadLine(), out idPracownika))
            {
                Console.WriteLine("Niepoprawne ID. Operacja anulowana.");
                return;
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"DELETE FROM workers WHERE id_worker = {idPracownika}";
                SqlCommand command = new SqlCommand(query, connection);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine( "Usunięto  pomyślnie.");
                }
                else
                {
                    Console.WriteLine("Niepoprawne ID pracownika do usunięcia.");
                }
            }
        }

        static void AktualizujHaslo()
        {
            Console.WriteLine("Podaj ID pracownika, któremu chcesz zaktualizować hasło:");
            int idPracownika;
            if (!int.TryParse(Console.ReadLine(), out idPracownika))
            {
                Console.WriteLine("Niepoprawne ID.");
                return;
            }
            Console.WriteLine("Podaj nowe hasło:");
            string noweHaslo = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"UPDATE workers SET password = '{noweHaslo}' WHERE id_worker = {idPracownika}";
                SqlCommand command = new SqlCommand(query, connection);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Hasło zaktualizowano.");
                }
                else
                {
                    Console.WriteLine("Aktualizacja hasła nie powiodła się. Brak pracownika o podanym ID.");
                }
            }
        }
        //Funkcja do zarządzania notatkami pracownika
        static void ZarzadzajNotatkami()
        {
            Console.WriteLine("Podaj ID pracownika, dla którego chcesz zarządzać notatkami:");
            int idPracownika;
            if (!int.TryParse(Console.ReadLine(), out idPracownika))
            {
                Console.WriteLine("Niepoprawne ID. Operacja anulowana.");
                return;
            }
            Console.WriteLine("Podaj treść notatki:");
            string trescNotatki = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"INSERT INTO note (id_worker, content) VALUES ({idPracownika}, '{trescNotatki}')";
                SqlCommand command = new SqlCommand(query, connection);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Notatka dodana pomyślnie.");
                }
                else
                {
                    Console.WriteLine("Dodanie notatki nie powiodło się. Spróbuj ponownie.");
                }
            }
            static string GetRoleName(int roleId)  //Funkcja do pobierania nazwy stanowiska na podstawie jego ID
            {
                switch (roleId)
                {
                    case 1:
                        return "Administrator";
                    case 2:
                        return "Rekruter";
                    case 3:
                        return "Programista";
                    case 4:
                        return "HR";
                    default:
                        return "Nieznane";
                }
            }
        } 
    }}

//Środowisko pracy: Visual Studio2022, język: C#, system operacyjny: Windows
