using ModeloBdClases.BaseDatos;
using ModeloBdClases.Clases;
using System.Diagnostics;

namespace Interfaces.ViewRegistro;

public partial class RegistroUsuario : ContentPage
{
    #region Campos

    // Instancia de la clase que contiene los métodos de acceso a la base de datos
    private readonly MetodosBD _metodosBD = new MetodosBD();

    #endregion

    #region Constructor

    public RegistroUsuario()
    {
        InitializeComponent(); // Inicializa los componentes visuales definidos en el XAML
    }

    #endregion

    #region Métodos de UI

    /// <summary>
    /// Evento que se dispara cuando el usuario hace clic en el botón "Registrar".
    /// Realiza validación de campos, crea el objeto Usuario y lo registra en la base de datos.
    /// </summary>
    private async void RegistrarUsuarioClicked(object sender, EventArgs e)
    {
        try
        {

            // Crear el objeto Usuario con los datos ingresados
            Usuario nuevoUsuario;
            try
            {

                nuevoUsuario = new Usuario(
                    EntryNombre.Text,
                    EntryApellidos.Text,
                    EntryEmail.Text,
                    EntryContrasenia.Text, 
                    false);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
                return;
            }

            // Confirmación previa al registro
            bool confirmar = await Application.Current.MainPage.DisplayAlert(
                "Confirmar registro",
                $"¿Desea registrarte como {EntryNombre.Text} {EntryApellidos.Text}?",
                "Sí, registrar",
                "Cancelar");

            if (!confirmar) return;

            try
            {
                string hash = Seguridad.GenerarHash(nuevoUsuario.Contrasenia);

                bool resultado = await Task.Run(() =>
                    _metodosBD.AgregarUsuario(
                        nuevoUsuario.Nombre,
                        nuevoUsuario.Apellidos,
                        nuevoUsuario.Email,
                        hash,
                        nuevoUsuario.IsAdmin));

                if (resultado)
                {
                    ResetearFormulario(); // Limpia el formulario tras éxito
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Usuario registrado correctamente", "Aceptar");

                    // Redirección automática al login
                    await Shell.Current.GoToAsync("//Login");
                }
                

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar");
                return;
            }
            // Registro del usuario en segundo plano

        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "Aceptar");
            Debug.WriteLine($"Error en registro: {ex}");
        }
    }

    /// <summary>
    /// Muestra/oculta la contraseña y cambia el icono
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TogglePasswordVisibility(object sender, EventArgs e)
    {
        EntryContrasenia.IsPassword = !EntryContrasenia.IsPassword;

        // Opcional: alternar icono ojo‑abierto / cerrado
        var imgBtn = (ImageButton)sender;
        imgBtn.Source = EntryContrasenia.IsPassword ? "visible.png" : "novisible.png"; 
    }

    private async void VolverLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Login");
        ResetearFormulario();

    }
    #endregion

    #region Métodos Auxiliares

    /// <summary>
    /// Limpia todos los campos del formulario de registro.
    /// </summary>
    private void ResetearFormulario()
    {
        EntryNombre.Text = string.Empty;
        EntryApellidos.Text = string.Empty;
        EntryEmail.Text = string.Empty;
        EntryContrasenia.Text = string.Empty;
    }

    #endregion
}