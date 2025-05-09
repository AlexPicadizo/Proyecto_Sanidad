namespace Interfaces.ViewAdministrador;

/// <summary>
/// Página contenedora dinámica para vistas de administración.
/// Recibe una vista específica (ContentView) y la muestra como contenido principal,
/// ocultando la barra de navegación por defecto.
/// </summary>
public class GestionViewsAdmin : ContentPage
{

    /// <summary>
    /// Constructor que configura la página sin barra de navegación
    /// y coloca la vista pasada como contenido.
    /// </summary>
    /// <param name="opcionAdmin">
    /// Vista de administración a mostrar (por ejemplo, AgregarUsuarios, ConsultarUsuarios, etc.).
    /// </param>
    public GestionViewsAdmin(ContentView opcionAdmin)
    {
        // Elimina la barra de navegación superior para una vista más limpia
        NavigationPage.SetHasNavigationBar(this, false);

        // Asigna la vista de administración recibida como contenido de la página
        Content = opcionAdmin;
    }
}
