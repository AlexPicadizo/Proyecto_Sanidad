using ModeloBdClases.BaseDatos;
using ModeloBdClases.Clases;
using System.Collections.ObjectModel;

namespace Interfaces.ViewAdministrador;

public partial class ActivarDesactivarUsuarios : ContentView
{
    #region Campos
    // Instancia para el acceso a los métodos de la base de datos
    private readonly MetodosBD metodosBD = new MetodosBD();

    // Colección observable para vincular los usuarios a la UI y actualizarla automáticamente
    public ObservableCollection<Usuario> Usuarios { get; set; }

    private List<Usuario> _todosLosUsuarios;   // Copia completa para filtrar

    #endregion


    public ActivarDesactivarUsuarios()
	{
		InitializeComponent();

        // Elimina la barra de navegación superior para una vista más limpia
        NavigationPage.SetHasNavigationBar(this, false);

        Usuarios = new ObservableCollection<Usuario>();
        BindingContext = this; // Primero el binding
        CargarUsuarios(); // Luego la carga de datos

    }

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
            _todosLosUsuarios = metodosBD.ObtenerUsuarios() ?? new List<Usuario>();

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
    /// Manejador del evento SearchButtonPressed del SearchBar para buscar usuarios
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnBuscarUsuarios(object sender, EventArgs e)
    {
        // Llama a FiltrarUsuarios para actualizar la lista según el texto ingresado en el search bar
        FiltrarUsuario(searchBarUsuarios.Text ?? "");
    }

    /// <summary>
    /// Manejador del evento TextChanged del SearchBar para filtrar la lista en tiempo real
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        // Llama a FiltrarUsuarios con el nuevo texto ingresado, realizando el filtrado en vivo.
        FiltrarUsuario(e.NewTextValue ?? "");
    }

    /// <summary>
    /// Filtra la lista de usuarios según el texto ingresado.
    /// Solo se muestran los usuarios cuyos nombres empiezan con el texto ingresado.
    /// </summary>
    /// <param name="texto">Texto utilizado para filtrar los usuarios. Solo se muestran los que comienzan con este texto.</param>
    private void FiltrarUsuario(string texto)
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

    private async void OnToggleCuenta(object sender, ToggledEventArgs e)
    {
        var toggle = sender as Switch;
        if (toggle?.BindingContext is Usuario usuario)
        {
            // Cambiar el valor del modelo
            usuario.IsActiva = e.Value;

            // Actualizar el color del Switch
            toggle.OnColor = e.Value ? Colors.Green : Colors.Red;
            toggle.ThumbColor = Colors.White;

            // Cambiar el texto a Activar/Desactivar
            if (toggle.Parent is HorizontalStackLayout layout)
            {
                foreach (var view in layout.Children)
                {
                    if (view is Label label && label.Text != "⚙️")
                    {
                        label.Text = e.Value ? "Desactivar" : "Activar";
                        break;
                    }
                }
            }

            // Guardar el cambio en la base de datos
            var metodosBD = new MetodosBD();
            await metodosBD.ActualizarEstadoCuenta(usuario);  // Método que actualizará 'isActiva' en la BD
        }
    }

}