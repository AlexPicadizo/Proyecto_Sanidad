namespace Interfaces.ViewAdministrador;

/// <summary>
/// P�gina contenedora din�mica para vistas de administraci�n.
/// Recibe una vista espec�fica (ContentView) y la muestra como contenido principal,
/// ocultando la barra de navegaci�n por defecto.
/// </summary>
public class GestionViewsAdmin : ContentPage
{

    /// <summary>
    /// Constructor que configura la p�gina sin barra de navegaci�n
    /// y coloca la vista pasada como contenido.
    /// </summary>
    /// <param name="opcionAdmin">
    /// Vista de administraci�n a mostrar (por ejemplo, AgregarUsuarios, ConsultarUsuarios, etc.).
    /// </param>
    public GestionViewsAdmin(ContentView opcionAdmin)
    {
        // Elimina la barra de navegaci�n superior para una vista m�s limpia
        NavigationPage.SetHasNavigationBar(this, false);

        // Asigna la vista de administraci�n recibida como contenido de la p�gina
        Content = opcionAdmin;
    }
}
