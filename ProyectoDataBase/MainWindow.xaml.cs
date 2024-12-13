using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;

namespace ProyectoDataBase
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ProyectoDataBase.Properties.Settings.pc_dam2_02_02_GestionLibros_dbo"].ConnectionString;
        private SqlConnection sqlConnection;

        public MainWindow()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connectionString);
        }

        private void txtUsuarioId_GotFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderUsuarioId.Visibility = Visibility.Collapsed;
        }

        private void txtUsuarioId_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuarioId.Text))
            {
                PlaceholderUsuarioId.Visibility = Visibility.Visible;
            }
        }

        private void CargarLibros_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "SELECT * FROM Libros";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(consulta, sqlConnection);
                DataTable librosTable = new DataTable();
                sqlDataAdapter.Fill(librosTable);

                ListaLibros.DisplayMemberPath = "Titulo";
                ListaLibros.SelectedValuePath = "Id"; 
                ListaLibros.ItemsSource = librosTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar libros: {ex.Message}");
            }
        }

    
        private void MostrarPrestamos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "SELECT * FROM Prestamos WHERE UsuarioId = @UsuarioId";
                SqlCommand command = new SqlCommand(consulta, sqlConnection);
                command.Parameters.AddWithValue("@UsuarioId", txtUsuarioId.Text);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                DataTable prestamosTable = new DataTable();
                sqlDataAdapter.Fill(prestamosTable);

                ListaPrestamos.DisplayMemberPath = "FechaPrestamo";
                ListaPrestamos.SelectedValuePath = "Id"; 
                ListaPrestamos.ItemsSource = prestamosTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar préstamos: {ex.Message}");
            }
        }

        private void RealizarPrestamo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "INSERT INTO Prestamos (UsuarioId, LibroId, FechaPrestamo) VALUES (@UsuarioId, @LibroId, @FechaPrestamo)";
                SqlCommand command = new SqlCommand(consulta, sqlConnection);
                command.Parameters.AddWithValue("@UsuarioId", txtUsuarioId.Text);
                command.Parameters.AddWithValue("@LibroId", ListaLibros.SelectedValue);
                command.Parameters.AddWithValue("@FechaPrestamo", DateTime.Now);

                sqlConnection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show("Préstamo realizado con éxito.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar préstamo: {ex.Message}");
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void DevolverLibro_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "UPDATE Prestamos SET FechaDevolucion = @FechaDevolucion WHERE Id = @PrestamoId";
                SqlCommand command = new SqlCommand(consulta, sqlConnection);
                command.Parameters.AddWithValue("@PrestamoId", ListaPrestamos.SelectedValue);
                command.Parameters.AddWithValue("@FechaDevolucion", DateTime.Now);

                sqlConnection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show("Libro devuelto con éxito.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al devolver libro: {ex.Message}");
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}

