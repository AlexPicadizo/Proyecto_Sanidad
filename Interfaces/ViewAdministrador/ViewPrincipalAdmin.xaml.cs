namespace Interfaces.ViewAdministrador;

public partial class ViewPrincipalAdmin : ContentPage
{
    #region Constructor

    public ViewPrincipalAdmin()
    {
        InitializeComponent(); // Inicializa los componentes visuales definidos en el XAML
    }

    #endregion

    #region M�todos de UI - Navegaci�n

    /// <summary>
    /// Evento que se ejecuta al hacer clic en el bot�n para eliminar usuarios.
    /// Navega a la vista de gesti�n con la interfaz de eliminar usuarios.
    /// </summary>
    private async void OnBorrarUsuariosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GestionViewsAdmin(new EliminarUsuarios()));
    }

    /// <summary>
    /// Evento que se ejecuta al hacer clic en el bot�n para consultar usuarios.
    /// Navega a la vista de gesti�n con la interfaz de consultar usuarios.
    /// </summary>
    private async void OnConsultarUsuariosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GestionViewsAdmin(new ConsultarUsuarios()));
    }

    #endregion

    // Inutilizado por el cambio de dise�o
    #region Responsividad - Ajuste del layout seg�n tama�o de pantalla

    /*
    /// <summary>
    /// Se ejecuta autom�ticamente cuando cambia el tama�o de la pantalla.
    /// Reorganiza el dise�o de los botones dependiendo del ancho disponible.
    /// </summary>
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        // Verifica que el Grid y sus hijos est�n disponibles
        if (BotonesGrid == null || BotonesGrid.Children.Count == 0)
            return;

        if (width < 600)
        {
            // Distribuci�n en columna (modo vertical) para pantallas peque�as
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
            // Distribuci�n en fila (modo horizontal) para pantallas amplias
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
        bool confirm = await DisplayAlert("Cerrar sesi�n", "�Est�s seguro de que deseas cerrar sesi�n?", "S�", "Cancelar");

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
