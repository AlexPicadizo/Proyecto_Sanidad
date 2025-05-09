using Microsoft.Maui.Controls;
using ModeloBdClases.BaseDatos;
using ModeloBdClases.Clases;
using System.Text.RegularExpressions;

namespace Interfaces.ViewUsuario;

public partial class AgregarPacientes : ContentPage
{
    #region Propiedades y Campos

    // No es necesario agregar propiedades específicas aquí, ya que no se necesita estado compartido
    // Las variables de interfaz como 'entryNombre', 'entryEdad', etc., están enlazadas automáticamente en el XAML.

    #endregion

    #region Constructor

    public AgregarPacientes()
    {
        InitializeComponent(); // Inicializa los componentes de la página (el XAML)

        // Obtener el nombre del doctor desde preferencias (debes haberlo guardado antes)
        string nombreDoctor = Preferences.Get("NombreUsuario", "Doctor/a");

        // Asignar el texto personalizado
        labelBienvenida.Text = $"Bienvenido/a, Dr(a). {nombreDoctor}";
    }

    #endregion

    #region Métodos de UI

    /// <summary>
    /// Evento que se dispara cuando cambia el texto del Entry del DNI.
    /// - Limpia caracteres no válidos.
    /// - Cuando el usuario escribe 8 dígitos, añade la letra del DNI automáticamente.
    /// - Si el usuario escribe la letra manualmente, la normaliza con guion.
    /// - Permite borrar sin que se reescriba automáticamente la letra.
    /// </summary>
    private void OnDniTextChanged(object sender, TextChangedEventArgs e)
    {
        // Quitamos el manejador temporalmente para evitar recursividad al modificar el texto manualmente
        entryDni.TextChanged -= OnDniTextChanged;

        // Recuperamos el texto antiguo y nuevo (asegurándonos de que no sean null)
        string rawOld = e.OldTextValue ?? "";
        string rawNew = e.NewTextValue ?? "";

        // Limpiamos el texto nuevo eliminando cualquier carácter que no sea número o letra (ni espacios ni guiones)
        string cleanOld = Regex.Replace(rawOld.ToUpper(), @"[^0-9A-Z]", "");
        string cleanNew = Regex.Replace(rawNew.ToUpper(), @"[^0-9A-Z]", "");

        // Comprobamos si el usuario está escribiendo (añadiendo caracteres) o borrando
        bool isInserting = rawNew.Length > rawOld.Length;

        // Capturamos el último carácter escrito (si se está insertando)
        char lastChar = isInserting ? rawNew.Last() : '\0';

        // Limitamos la longitud a máximo 9 caracteres (8 números + 1 letra)
        if (cleanNew.Length > 9)
            cleanNew = cleanNew.Substring(0, 9);

        // CASO 1: El usuario ha escrito justo 8 dígitos → añadimos automáticamente la letra
        if (isInserting
            && char.IsDigit(lastChar)                // Último carácter escrito es un número
            && cleanNew.All(char.IsDigit)            // Todos los caracteres actuales son dígitos
            && cleanNew.Length == 8)                 // Y tenemos exactamente 8 dígitos
        {
            // Calculamos la letra correspondiente y la añadimos con guion
            char letra = CalcularLetraDni(cleanNew);
            cleanNew = $"{cleanNew}-{letra}";
        }
        // CASO 2: El usuario ha escrito una letra manualmente (ya hay 9 caracteres)
        else if (isInserting
            && cleanNew.Length == 9
            && char.IsLetter(lastChar))              // El último carácter añadido es una letra
        {
            string numero = cleanNew.Substring(0, 8); // Tomamos los primeros 8 dígitos
            char provided = cleanNew[8];              // Letra que el usuario escribió
            char correct = CalcularLetraDni(numero);  // Letra correcta según el algoritmo

            // Si es correcta, la usamos tal cual; si no, la corregimos
            cleanNew = $"{numero}-{(provided == correct ? provided : correct)}";
        }

        // Aplicamos el texto formateado al Entry
        entryDni.Text = cleanNew;

        // Reconectamos el manejador del evento
        entryDni.TextChanged += OnDniTextChanged;
    }

    /// <summary>
    /// Calcula la letra del DNI según el algoritmo oficial.
    /// </summary>
    private char CalcularLetraDni(string numeroStr)
    {
        const string tablaLetras = "TRWAGMYFPDXBNJZSQVHLCKE";
        int numero = int.Parse(numeroStr);
        return tablaLetras[numero % 23];
    }

    /// <summary>
    /// Evento que se dispara cuando el usuario hace clic en el botón "Guardar".
    /// Este método recoge los datos del formulario, valida y guarda al paciente.
    /// </summary>
    private async void OnGuardarPacienteClicked(object sender, EventArgs e)
    {
        try
        {
            // Obtener el ID del usuario logueado desde las preferencias
            int usuarioId = Preferences.Get("UsuarioId", 0);

            // Validar que el usuario esté logueado
            if (usuarioId == 0)
            {
                throw new Exception("No se pudo obtener el ID del usuario.");
            }

            // Obtener los datos del formulario
            string nombre = entryNombre.Text;
            string apellidos = entryApellidos.Text;
            int edad = int.Parse(entryEdad.Text);
            string dni = entryDni.Text;
            string grupoSanguineo = pickerGrupoSanguineo.SelectedItem.ToString();
            DateTime fechaIngreso = datePickerIngreso.Date;

            // Crear una instancia del método de base de datos
            MetodosBD metodosBD = new MetodosBD();

            // Ejecutar la inserción del paciente en segundo plano
            bool exito = await Task.Run(() => metodosBD.AgregarPaciente(nombre, apellidos, edad, dni, grupoSanguineo, fechaIngreso, usuarioId));

            if (exito)
            {
                // Enviar un mensaje para actualizar la lista de pacientes
                MessagingCenter.Send(this, "ActualizarPacientes");

                // Mostrar un mensaje de éxito
                await DisplayAlert("Éxito", "Paciente agregado correctamente", "OK");

                // Limpiar los campos del formulario
                LimpiarFormulario();
            }
            else
            {
                // Mostrar mensaje de error si la inserción falla
                await DisplayAlert("Error", "No se pudo agregar el paciente", "OK");
            }
        }
        catch (Exception error)
        {
            // Mostrar un mensaje en caso de excepción
            await DisplayAlert("Error", $"Error: {error.Message}", "Ok");
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Cerrar sesión", "¿Estás seguro de que deseas cerrar sesión?", "Sí", "Cancelar");

        if (confirm)
        {
            // Limpiar preferencias
            Preferences.Remove("UsuarioId");
            Preferences.Remove("NombreUsuario");

            // Volver a la pantalla de login (root)
            await Shell.Current.GoToAsync("//Login");
        }
    }


    #endregion

    #region Métodos Auxiliares

    /// <summary>
    /// Método para limpiar los campos del formulario después de agregar un paciente.
    /// </summary>
    private void LimpiarFormulario()
    {
        entryNombre.Text = string.Empty;
        entryApellidos.Text = string.Empty;
        entryEdad.Text = string.Empty;
        entryDni.Text = string.Empty;
        pickerGrupoSanguineo.SelectedItem = null;
        datePickerIngreso.Date = DateTime.Now;
    }

    #endregion
}
