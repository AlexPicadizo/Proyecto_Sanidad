using ModeloBdClases.BaseDatos;
using ModeloBdClases.Clases;
using System.Collections.ObjectModel;

namespace Interfaces.ViewAdministrador;

public partial class ConsultarUsuarios : ContentView
{
    #region Campos

    // Instancia para el acceso a los métodos de la base de datos
    private readonly MetodosBD metodosBD = new MetodosBD();

    // Colección observable para vincular los usuarios a la UI y actualizarla automáticamente
    public ObservableCollection<Usuario> Usuarios { get; set; }

    private List<Usuario> _todosLosUsuarios;   // Copia completa para filtrar

    #endregion

    #region Constructor

    public ConsultarUsuarios()
    {
        InitializeComponent(); // Inicializa los componentes visuales definidos en el XAML

        Usuarios = new ObservableCollection<Usuario>(); // Inicializa la colección observable

        CargarUsuarios(); // Carga inicial de usuarios desde la base de datos

        BindingContext = this; // Establece el contexto de enlace de datos
    }

    #endregion

    #region Métodos

    /// <summary>
    /// Carga la lista de usuarios desde la base de datos y los agrega a la colección observable.
    /// Esto actualiza automáticamente la interfaz de usuario si está enlazada a la colección.
    /// </summary>
    private async void CargarUsuarios()
    {
        try
        {
            // 1. Obtener todos los usuarios de la base de datos. Si no hay usuarios, se asigna una lista vacía.
            _todosLosUsuarios = metodosBD.ObtenerUsuarios();

            // Verificamos que la lista de usuarios no sea nula
            if (_todosLosUsuarios != null)
            {
                // Limpiamos la colección observable para evitar duplicados
                Usuarios.Clear();

                // 2. Refrescamos la colección observable que está ligada al CollectionView de la interfaz de usuario
                foreach (var usuario in _todosLosUsuarios)
                    Usuarios.Add(usuario);
            }
        }
        catch (Exception error)
        {
            // Si ocurre un error durante la carga de los usuarios, mostramos un mensaje de alerta con el error.
            await Application.Current.MainPage.DisplayAlert("Error", $"Ocurrió un error: {error.Message}", "Aceptar");
        }
    }

    /// <summary>
    /// Este método se llama cuando se pulsa el botón "Buscar" del teclado.
    /// Filtra los usuarios de acuerdo con el texto introducido en el cuadro de búsqueda.
    /// </summary>
    private void OnBuscarUsuario(object sender, EventArgs e)
    {
        // Llama a FiltrarUsuarios con el texto ingresado en el search bar.
        FiltrarUsuarios(searchBarUsuarios.Text ?? "");
    }

    /// <summary>
    /// Este método se ejecuta cada vez que cambia el texto en el cuadro de búsqueda.
    /// Permite realizar un filtrado en vivo mientras el usuario escribe.
    /// </summary>
    /// <param name="sender">El objeto que dispara el evento.</param>
    /// <param name="e">Contiene el texto ingresado por el usuario.</param>
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        // Llama a FiltrarUsuarios con el nuevo texto ingresado, realizando el filtrado en vivo.
        FiltrarUsuarios(e.NewTextValue ?? "");
    }

    /// <summary>
    /// Filtra la lista de usuarios según el texto ingresado.
    /// Solo se muestran los usuarios cuyos nombres empiezan con el texto ingresado.
    /// </summary>
    /// <param name="texto">Texto utilizado para filtrar los usuarios. Solo se muestran los que comienzan con este texto.</param>
    private void FiltrarUsuarios(string texto)
    {
        // Filtra los usuarios que tienen un nombre que comienza con el texto ingresado, sin diferenciar entre mayúsculas y minúsculas
        var resultados = _todosLosUsuarios
            .Where(u => u.Nombre.StartsWith(texto, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Limpiamos la colección observable antes de agregar los resultados filtrados
        Usuarios.Clear();

        // Agregamos los resultados filtrados a la colección observable
        foreach (var u in resultados)
            Usuarios.Add(u);
    }

    #endregion
}
