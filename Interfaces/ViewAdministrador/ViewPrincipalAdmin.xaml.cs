namespace Interfaces.ViewAdministrador;

public partial class ViewPrincipalAdmin : ContentPage
{
    #region Constructor

    public ViewPrincipalAdmin()
    {
        InitializeComponent(); // Inicializa los componentes visuales definidos en el XAML
    }

    #endregion

    #region Métodos de UI - Navegación

    /// <summary>
    /// Evento que se ejecuta al hacer clic en el botón para eliminar usuarios.
    /// Navega a la vista de gestión con la interfaz de eliminar usuarios.
    /// </summary>
    private async void OnBorrarUsuariosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GestionViewsAdmin(new EliminarUsuarios()));
    }

    /// <summary>
    /// Evento que se ejecuta al hacer clic en el botón para consultar usuarios.
    /// Navega a la vista de gestión con la interfaz de consultar usuarios.
    /// </summary>
    private async void OnConsultarUsuariosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GestionViewsAdmin(new ConsultarUsuarios()));
    }

    #endregion

    // Inutilizado por el cambio de diseño
    #region Responsividad - Ajuste del layout según tamaño de pantalla

    /*
    /// <summary>
    /// Se ejecuta automáticamente cuando cambia el tamaño de la pantalla.
    /// Reorganiza el diseño de los botones dependiendo del ancho disponible.
    /// </summary>
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        // Verifica que el Grid y sus hijos estén disponibles
        if (BotonesGrid == null || BotonesGrid.Children.Count == 0)
            return;

        if (width < 600)
        {
            // Distribución en columna (modo vertical) para pantallas pequeñas
            BotonesGrid.ColumnDefinitions.Clear();
            BotonesGrid.RowDefinitions.Clear();

            BotonesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            BotonesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            BotonesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int i = 0; i < 3; i++)
            {
                View view = (View)BotonesGrid.Children[i];
                Grid.SetRow(view, i);
                Grid.SetColumn(view, 0);
            }
        }
        else
        {
            // Distribución en fila (modo horizontal) para pantallas amplias
            BotonesGrid.RowDefinitions.Clear();
            BotonesGrid.ColumnDefinitions.Clear();

            BotonesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            BotonesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            BotonesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            for (int i = 0; i < 3; i++)
            {
                View view = (View)BotonesGrid.Children[i];
                Grid.SetColumn(view, i);
                Grid.SetRow(view, 0);
            }
        }
    }
    */
    #endregion

    private async void OnActivarDesactivarClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GestionViewsAdmin(new ActivarDesactivarUsuarios()));

    }

    private async void OnLogoutClicked(object sender, TappedEventArgs e)
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
}
