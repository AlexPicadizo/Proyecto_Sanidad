using ModeloBdClases.BaseDatos;
using ModeloBdClases.Clases;

namespace Interfaces.ViewLogin;

public partial class Login : ContentPage
{
    #region Campos

    // Instancia de la clase de conexión a base de datos
    apiBD apiBaseDatos;

    #endregion

    #region Constructor
    public Login()
    {
        InitializeComponent(); // Inicializa los componentes de la interfaz (definidos en el XAML)

        // Inicializa la conexión a la base de datos
        apiBaseDatos = new apiBD();
        apiBaseDatos.ConexionBd();

        // Limpia cualquier dato previo del usuario almacenado en las preferencias
        Preferences.Remove("UsuarioId");
    }

    #endregion

    #region Métodos de UI

    /// <summary>
    /// Evento que se dispara cuando el usuario hace clic en el botón de "Iniciar sesión".
    /// Valida el usuario, obtiene su rol y navega a la vista correspondiente.
    /// </summary>
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            // Instancia de la clase de métodos para operaciones de base de datos
            MetodosBD metodosBD = new MetodosBD();

            // Obtiene el usuario desde la base de datos usando correo y contraseña
            Usuario user = metodosBD.ObtenerUsuario(CorreoEntry.Text, ContraseniaEntry.Text);

            // Verificar si el usuario está activo
            if (user == null || !user.IsActiva)
            {
                // Si no se encuentra o está desactivado
                await DisplayAlert("ERROR", "La cuenta está desactivada", "OK");
                return;
            }

            // Obtiene el ID del usuario desde la base de datos
            int usuarioId = metodosBD.ObtenerIdUsuario(CorreoEntry.Text);

            // Verifica que el usuario exista (ID válido)
            if (usuarioId > 0)
            {
                // Guarda el ID del usuario en preferencias para su uso global en la app
                Preferences.Set("UsuarioId", usuarioId);
                Preferences.Set("NombreUsuario", user.Nombre); // Guarda el nombre para mostrarlo luego


                if (user.IsActiva == true)
                {
                    // Navega a la vista correspondiente según si el usuario es administrador o no
                    if (user.IsAdmin)
                    {
                        await Shell.Current.GoToAsync("//ViewPrincipalAdmin"); // Vista para administrador
                        limpiarLogin();
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("//AgregarPacientes");   // Vista para usuario normal
                        limpiarLogin();
                    }
                }
                else
                {
                    await DisplayAlert("ERROR", "Error: Esta cuenta está desctivada", "Ok");
                }
            }
        }
        catch (Exception error)
        {
            // Muestra un mensaje de error si ocurre alguna excepción
            await DisplayAlert("Error", error.Message, "OK");
        }
    }

    private void limpiarLogin()
    {
        CorreoEntry.Text = "";
        ContraseniaEntry.Text = "";
    }

    /// <summary>
    /// Pagina para el registro
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//RegistroUsuario"); // Vista para el registro
    }


    #endregion

}
