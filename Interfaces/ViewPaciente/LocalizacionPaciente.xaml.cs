using ModeloBdClases.BaseDatos;
using ModeloBdClases.Clases;

namespace Interfaces.ViewPaciente;

public partial class LocalizacionPaciente : ContentView
{
    #region Variables y Propiedades

    // Instancia de la clase que maneja las operaciones de base de datos
    private readonly MetodosBD metodosBd = new MetodosBD();

    // Objeto Paciente que contiene la información del paciente a mostrar
    private readonly Paciente _paciente;

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor de la clase LocalizacionPaciente
    /// Establece el BindingContext para que el XAML se conecte con el modelo de datos (Paciente)
    /// </summary>
    public LocalizacionPaciente(Paciente paciente)
    {
        InitializeComponent();

        // Se establece el BindingContext con el paciente para que las propiedades del paciente sean accesibles en el XAML
        _paciente = paciente;
        BindingContext = _paciente; // ⭐ Esto asegura que el BindingContext se establezca correctamente

        // Carga la ubicación actual del paciente desde la base de datos
        CargarUbicacion();
    }

    #endregion

    #region Métodos

    /// <summary>
    /// Carga la ubicación del paciente desde la base de datos usando su DNI
    /// </summary>
    private async void CargarUbicacion()
    {
        try
        {
            // Se realiza la carga de la ubicación en un hilo separado para no bloquear la UI
            var ubicacion = await Task.Run(() => metodosBd.ObtenerUbicacionPorDni(_paciente.Dni));

            // Si se encuentra la ubicación, se asigna al paciente y se desactiva la edición
            if (ubicacion != null)
            {
                _paciente.Ubicacion = ubicacion; // Actualiza la propiedad Ubicacion del paciente
                CambiarModoEdicion(false); // Desactiva el modo de edición
            }
        }
        catch (Exception ex)
        {
            // Si ocurre un error al cargar la ubicación, mostramos una alerta al usuario
            await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Método que se ejecuta cuando el usuario hace clic en el botón "Establecer"
    /// Guarda la ubicación ingresada del paciente en la base de datos
    /// </summary>
    private async void OnEstablecerClicked(object sender, EventArgs e)
    {
        // Intentamos convertir los valores de texto a enteros
        int planta, habitacion, cama;

        try
        {

            planta = int.Parse(PlantaEntry.Text);
            habitacion = int.Parse(HabitacionEntry.Text);
            cama = int.Parse(CamaEntry.Text);

            // Usamos la clase Ubicacion para que realice las validaciones automáticamente
            // Al asignar los valores, se ejecutan las validaciones dentro de las propiedades de Ubicacion
            Ubicacion nuevaUbicacion = new Ubicacion(planta, habitacion, cama);

            // Intentamos guardar la nueva ubicación del paciente en la base de datos
            bool exito = await Task.Run(() =>
                metodosBd.GuardarUbicacionPorDni(_paciente.Dni, nuevaUbicacion));

            // Si la operación fue exitosa, actualizamos la ubicación del paciente y deshabilitamos la edición
            if (exito)
            {
                _paciente.Ubicacion = nuevaUbicacion; // Actualiza la ubicación del paciente

                // Mostramos un mensaje de éxito
                await Application.Current.MainPage.DisplayAlert("Éxito", "Ubicación guardada", "OK");

                // Desactivamos el modo de edición después de guardar
                CambiarModoEdicion(false);
            }
            else
            {
                // Si no fue exitoso, mostramos un mensaje de error
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo guardar la ubicación.", "OK");
            }
        }
        catch (Exception ex)
        {
            // En caso de que se lance una excepción dentro de la clase Ubicacion o cualquier otro error
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    /// <summary>
    /// Método que se ejecuta cuando el usuario hace clic en el botón "Modificar"
    /// Cambia el modo a edición, habilitando los campos para que el usuario pueda modificarlos
    /// </summary>
    private void OnModificarClicked(object sender, EventArgs e)
    {
        CambiarModoEdicion(true); // Activa el modo de edición
    }

    /// <summary>
    /// Cambia el modo de edición de la UI. Si 'edicion' es verdadero, los campos se habilitan para edición.
    /// </summary>
    /// <param name="edicion">Indica si se debe activar el modo de edición (true) o desactivarlo (false)</param>
    private void CambiarModoEdicion(bool edicion)
    {
        // Habilita o deshabilita los campos de texto y el botón dependiendo del modo de edición
        PlantaEntry.IsEnabled = edicion;
        HabitacionEntry.IsEnabled = edicion;
        CamaEntry.IsEnabled = edicion;
        EstablecerButton.IsEnabled = edicion;

        // Definir el estilo de los controles dependiendo de si estamos en modo de edición o no
        var entryStyle = edicion ? "BaseEntry" : "DisabledEntry";
        var buttonStyle = edicion ? "PrimaryButton" : "DisabledButton";

        // Aplicamos los estilos apropiados a los controles de la interfaz de usuario
        PlantaEntry.Style = (Style)Resources[entryStyle];
        HabitacionEntry.Style = (Style)Resources[entryStyle];
        CamaEntry.Style = (Style)Resources[entryStyle];
        EstablecerButton.Style = (Style)Resources[buttonStyle];

        // Si estamos en modo de edición, ocultamos el botón de "Modificar"
        ModificarButton.IsVisible = !edicion;
    }

    #endregion
}
