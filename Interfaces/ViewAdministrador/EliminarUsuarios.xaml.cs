using ModeloBdClases.BaseDatos;
using ModeloBdClases.Clases;
using System.Collections.ObjectModel;

namespace Interfaces.ViewAdministrador;

public partial class EliminarUsuarios : ContentView
{
    #region Campos

    // Instancia para acceder a la base de datos
    private readonly MetodosBD metodosBD = new MetodosBD();

    // Colección observable que permite actualizar la UI automáticamente
    public ObservableCollection<Usuario> Usuarios { get; set; }

    // Lista con todos los usuarios
    private List<Usuario> _todosLosUsuarios = new();

    #endregion

    #region Constructor

    public EliminarUsuarios()
    {
        InitializeComponent(); // Inicializa componentes visuales

        Usuarios = new ObservableCollection<Usuario>(); // Inicializa la lista observable

        CargarUsuarios(); // Carga inicial de usuarios

        BindingContext = this; // Enlace de datos con la vista
    }

    #endregion

    #region Métodos

    /// <summary>
    /// Carga los usuarios desde la base de datos y los agrega a la colección observable.
    /// Este método es llamado al inicializar la vista para mostrar todos los usuarios en el CollectionView.
    /// </summary>
    private async void CargarUsuarios()
    {
        try
        {
            // Obtiene la lista completa de usuarios desde la base de datos
            _todosLosUsuarios = metodosBD.ObtenerUsuarios();

            // Si la lista de usuarios no es nula (es decir, contiene datos)
            if (_todosLosUsuarios != null)
            {
                // Limpiamos la colección observable para evitar duplicados
                Usuarios.Clear();

                // Añadimos todos los usuarios de la lista a la colección observable
                foreach (var usuario in _todosLosUsuarios)
                    Usuarios.Add(usuario);
            }
        }
        catch (Exception error)
        {
            // Si ocurre un error al intentar cargar los usuarios, mostramos un mensaje de error
            await Application.Current.MainPage.DisplayAlert("Error", $"Ocurrió un error: {error.Message}", "Aceptar");
        }
    }

    /// <summary>
    /// Elimina un usuario tras confirmación, actualizando la base de datos y la colección.
    /// Este método se llama cuando se pulsa el botón "Eliminar" para un usuario.
    /// </summary>
    private async void EliminarUsuarioClick(object sender, EventArgs e)
    {
        try
        {
            // Comprobamos si el botón es el correcto y si su contexto es un usuario
            if (sender is Button button && button.BindingContext is Usuario usuario)
            {
                // Pedimos confirmación para eliminar al usuario
                bool confirmacion = await Application.Current.MainPage.DisplayAlert(
                    "Eliminar Usuario",
                    $"¿Estás seguro de que deseas eliminar a {usuario.Nombre}?",
                    "Sí", "No");

                // Si el usuario no confirma, terminamos el método
                if (!confirmacion) return;

                // Realizamos el proceso de eliminación en segundo plano para no bloquear la UI
                bool eliminado = await Task.Run(() =>
                {
                    // Obtenemos el ID del usuario a partir del email
                    int idUsuario = metodosBD.ObtenerIdUsuario(usuario.Email);

                    // Si el ID es mayor que 0, procedemos a borrar el usuario
                    return idUsuario > 0 && metodosBD.BorrarUsuario(idUsuario);
                });

                // De vuelta al hilo principal (UI), actualizamos la colección y mostramos mensaje
                if (eliminado)
                {
                    // Utilizamos InvokeOnMainThreadAsync para asegurarnos de que se ejecute en el hilo principal
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        // 1. Eliminamos el usuario de la lista maestra (_todosLosUsuarios)
                        _todosLosUsuarios.Remove(usuario);

                        // 2. Eliminamos el usuario de la colección observable (Usuarios) que se muestra en la UI
                        Usuarios.Remove(usuario);

                        // Mostramos un mensaje de éxito
                        await Application.Current.MainPage.DisplayAlert(
                            "Éxito",
                            $"Usuario {usuario.Nombre} eliminado correctamente",
                            "Aceptar");
                    });
                }
                else
                {
                    // Si no se pudo eliminar, mostramos un mensaje de error
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "No se pudo eliminar el usuario.",
                        "OK");
                }
            }
        }
        catch (Exception ex)
        {
            // Si ocurre algún error durante el proceso, mostramos un mensaje de error
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                $"Ocurrió un error: {ex.Message}",
                "Aceptar");
        }
    }

    /// <summary>
    /// Este método se ejecuta cuando se hace clic en la lupa de búsqueda o en el enter del teclado.
    /// Filtra los usuarios según el texto introducido en el cuadro de búsqueda.
    /// </summary>
    private void OnBuscarUsuario(object sender, EventArgs e)
    {
        // Llama a FiltrarUsuarios para actualizar la lista según el texto ingresado en el search bar
        FiltrarUsuarios(searchBarUsuarios.Text ?? "");
    }

    /// <summary>
    /// Este método se ejecuta mientras el texto del cuadro de búsqueda cambia (es decir, mientras se escribe).
    /// Filtra los usuarios en vivo conforme el usuario va escribiendo.
    /// </summary>
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        // Llama a FiltrarUsuarios con el nuevo texto de búsqueda
        FiltrarUsuarios(e.NewTextValue ?? "");
    }

    /// <summary>
    /// Filtra los usuarios que comienzan con el texto proporcionado en el cuadro de búsqueda.
    /// Solo se muestran los usuarios cuyos nombres empiezan por el texto ingresado.
    /// </summary>
    /// <param name="texto">Texto con el cual se filtran los usuarios. Solo se mostrarán aquellos cuyo nombre empiece con este texto.</param>
    private void FiltrarUsuarios(string texto)
    {
        // Filtramos los usuarios cuya propiedad 'Nombre' comienza con el texto ingresado, sin distinguir mayúsculas de minúsculas
        var resultados = _todosLosUsuarios
            .Where(u => u.Nombre.StartsWith(texto, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Limpiamos la colección observable antes de agregar los resultados filtrados
        Usuarios.Clear();

        // Añadimos los resultados filtrados a la colección observable
        foreach (var u in resultados)
            Usuarios.Add(u);
    }

    #endregion
}
