using ModeloBdClases.BaseDatos;
using ModeloBdClases.Clases;
using System.Collections.ObjectModel;

namespace Interfaces.ViewAdministrador;

public partial class ConsultarUsuarios : ContentView
{
    #region Campos

    // Instancia para el acceso a los m�todos de la base de datos
    private readonly MetodosBD metodosBD = new MetodosBD();

    // Colecci�n observable para vincular los usuarios a la UI y actualizarla autom�ticamente
    public ObservableCollection<Usuario> Usuarios { get; set; }

    private List<Usuario> _todosLosUsuarios;   // Copia completa para filtrar

    #endregion

    #region Constructor

    public ConsultarUsuarios()
    {
        InitializeComponent(); // Inicializa los componentes visuales definidos en el XAML

        Usuarios = new ObservableCollection<Usuario>(); // Inicializa la colecci�n observable

        CargarUsuarios(); // Carga inicial de usuarios desde la base de datos

        BindingContext = this; // Establece el contexto de enlace de datos
    }

    #endregion

    #region M�todos

    /// <summary>
    /// Carga la lista de usuarios desde la base de datos y los agrega a la colecci�n observable.
    /// Esto actualiza autom�ticamente la interfaz de usuario si est� enlazada a la colecci�n.
    /// </summary>
    private async void CargarUsuarios()
    {
        try
        {
            // 1. Obtener todos los usuarios de la base de datos. Si no hay usuarios, se asigna una lista vac�a.
            _todosLosUsuarios = metodosBD.ObtenerUsuarios();

            // Verificamos que la lista de usuarios no sea nula
            if (_todosLosUsuarios != null)
            {
                // Limpiamos la colecci�n observable para evitar duplicados
                Usuarios.Clear();

                // 2. Refrescamos la colecci�n observable que est� ligada al CollectionView de la interfaz de usuario
                foreach (var usuario in _todosLosUsuarios)
                    Usuarios.Add(usuario);
            }
        }
        catch (Exception error)
        {
            // Si ocurre un error durante la carga de los usuarios, mostramos un mensaje de alerta con el error.
            await Application.Current.MainPage.DisplayAlert("Error", $"Ocurri� un error: {error.Message}", "Aceptar");
        }
    }

    /// <summary>
    /// Este m�todo se llama cuando se pulsa el bot�n "Buscar" del teclado.
    /// Filtra los usuarios de acuerdo con el texto introducido en el cuadro de b�squeda.
    /// </summary>
    private void OnBuscarUsuario(object sender, EventArgs e)
    {
        // Llama a FiltrarUsuarios con el texto ingresado en el search bar.
        FiltrarUsuarios(searchBarUsuarios.Text ?? "");
    }

    /// <summary>
    /// Este m�todo se ejecuta cada vez que cambia el texto en el cuadro de b�squeda.
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
    /// Filtra la lista de usuarios seg�n el texto ingresado.
    /// Solo se muestran los usuarios cuyos nombres empiezan con el texto ingresado.
    /// </summary>
    /// <param name="texto">Texto utilizado para filtrar los usuarios. Solo se muestran los que comienzan con este texto.</param>
    private void FiltrarUsuarios(string texto)
    {
        // Filtra los usuarios que tienen un nombre que comienza con el texto ingresado, sin diferenciar entre may�sculas y min�sculas
        var resultados = _todosLosUsuarios
            .Where(u => u.Nombre.StartsWith(texto, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Limpiamos la colecci�n observable antes de agregar los resultados filtrados
        Usuarios.Clear();

        // Agregamos los resultados filtrados a la colecci�n observable
        foreach (var u in resultados)
            Usuarios.Add(u);
    }

    #endregion
}
