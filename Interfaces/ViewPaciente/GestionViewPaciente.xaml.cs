using Microsoft.Maui.Controls;
using ModeloBdClases.BaseDatos;
using ModeloBdClases.Clases;
using System;
using System.ComponentModel;

namespace Interfaces.ViewPaciente;

public partial class GestionViewPaciente : ContentPage
{
    #region Campos y Propiedades

    // Instancia de MetodosBD para interactuar con la base de datos
    private readonly MetodosBD metodosBD = new();

    // Paciente sobre el que se trabaja
    private readonly Paciente _paciente;

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor principal de la clase, inicializa la página y el paciente, 
    /// y carga los datos iniciales al crear la vista.
    /// </summary>
    public GestionViewPaciente(Paciente paciente)
    {
        InitializeComponent();
        _paciente = paciente;

        // Se suscribe al evento PropertyChanged para actualizar información cuando cambie la ubicación del paciente
        _paciente.PropertyChanged += Paciente_PropertyChanged;

        // Cargar datos iniciales al crear la página
        CargarDatosIniciales();
    }

    #endregion

    #region Eventos

    /// <summary>
    /// Evento que maneja los cambios en las propiedades del paciente.
    /// Si la propiedad que cambia es la ubicación, se actualiza la vista del paciente.
    /// </summary>
    private void Paciente_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        // Si la propiedad que cambió es la ubicación, se actualiza la información del paciente
        if (e.PropertyName == nameof(Paciente.Ubicacion))
        {
            ActualizarInformacionPaciente();
        }
    }

    #endregion

    #region Métodos de Carga de Datos

    /// <summary>
    /// Método asíncrono para cargar los datos iniciales del paciente,
    /// incluyendo la ubicación y las vistas correspondientes.
    /// </summary>
    private async void CargarDatosIniciales()
    {
        try
        {
            // Se obtiene la ubicación del paciente a partir de la base de datos usando el DNI
            var ubicacion = await Task.Run(() => metodosBD.ObtenerUbicacionPorDni(_paciente.Dni));

            // Asignar la ubicación al paciente
            _paciente.Ubicacion = ubicacion;

            // Actualizar la UI con la información cargada
            ActualizarHeader();
            ActualizarInformacionPaciente();
            CrearBotonera();

            // Mostrar la vista principal del paciente (Presentación)
            MostrarVista(new ViewPresentacionPaciente(_paciente));
        }
        catch (Exception ex)
        {
            // Mostrar mensaje de error en caso de fallo al cargar los datos
            await DisplayAlert("Error", $"Error al cargar datos: {ex.Message}", "OK");
        }
    }

    #endregion

    #region Métodos de Actualización de Vista

    /// <summary>
    /// Actualiza el encabezado de la página con el nombre y los datos del paciente.
    /// </summary>
    private void ActualizarHeader()
    {
        // Se actualizan las etiquetas de nombre y datos del paciente en la interfaz
        NombrePacienteLabel.Text = $"{_paciente.Nombre} {_paciente.Apellidos}".ToUpper();
        DatosPacienteLabel.Text = $"{_paciente.Edad} años  |  {_paciente.GrupoSanguineo}  |  DNI: {_paciente.Dni}";
    }

    /// <summary>
    /// Actualiza la información del paciente en la interfaz cuando cambia la ubicación.
    /// </summary>
    private void ActualizarInformacionPaciente()
    {
        // Se actualiza el contenido del _infoFrame con una nueva tarjeta de información
        _infoFrame.Content = CrearTarjetaInformacion();
    }

    /// <summary>
    /// Crea una tarjeta con la información relevante del paciente (ubicación y fecha de ingreso).
    /// </summary>
    /// <returns>Un objeto View que contiene la tarjeta con la información.</returns>
    private View CrearTarjetaInformacion()
    {
        // Crear un Grid que contiene dos columnas para mostrar la información del paciente
        var gridLayout = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            },
            ColumnSpacing = 20,
            Padding = new Thickness(10, 5),
            VerticalOptions = LayoutOptions.Center
        };

        // Etiqueta para la ubicación
        var ubicacionLabel = new Label
        {
            Text = _paciente.Ubicacion != null
                ? $"🛏️ Planta → {_paciente.Ubicacion.Planta}   ||   Habitación → {_paciente.Ubicacion.Habitacion}   ||   Cama → {_paciente.Ubicacion.Cama}"
                : "📍 Ubicación → **No asignada**",
            TextColor = Color.FromArgb("#1A237E"), // Color azul oscuro
            FontSize = 15,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center
        };

        // Etiqueta para la fecha de ingreso
        var fechaIngresoLabel = new Label
        {
            Text = $"📅 Ingreso → **{_paciente.FechaIngreso:dd/MM/yyyy}**",
            TextColor = Color.FromArgb("#1A237E"),
            FontSize = 15,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center
        };

        // Añadir las etiquetas a las columnas correspondientes en el Grid
        Grid.SetColumn(ubicacionLabel, 0);
        Grid.SetRow(ubicacionLabel, 0);
        gridLayout.Children.Add(ubicacionLabel);

        Grid.SetColumn(fechaIngresoLabel, 1);
        Grid.SetRow(fechaIngresoLabel, 0);
        gridLayout.Children.Add(fechaIngresoLabel);

        return gridLayout;
    }

    #endregion

    #region Métodos de Creación de Controles

    /// <summary>
    /// Crea la botonera con botones que permiten navegar a diferentes vistas relacionadas con el paciente.
    /// </summary>
    private void CrearBotonera()
    {
        BotoneraLayout.Children.Clear();

        BotoneraLayout.Children.Add(CrearBoton("🩺 Diagnósticos", "#3A9D9F", () =>
            MostrarVista(new DiagnosticoPaciente(_paciente))));

        BotoneraLayout.Children.Add(CrearBoton("📝 Evolución", "#4FB0B2", () =>
            MostrarVista(new EvolucionPaciente(_paciente))));

        BotoneraLayout.Children.Add(CrearBoton("📍 Ubicación", "#56C2C4", () =>
            MostrarVista(new LocalizacionPaciente(_paciente))));

        BotoneraLayout.Children.Add(CrearBoton("⚠️ Alergias", "#D97D54", () =>
            MostrarVista(new AlergiasPaciente(_paciente))));
    }

    /// <summary>
    /// Crea un botón con un texto, color y acción asociados.
    /// </summary>
    /// <param name="texto">Texto que aparecerá en el botón.</param>
    /// <param name="colorHex">Color de fondo del botón en formato hexadecimal.</param>
    /// <param name="accion">Acción que se ejecutará al hacer clic en el botón.</param>
    /// <returns>Un botón configurado con los parámetros proporcionados.</returns>
    private Button CrearBoton(string texto, string colorHex, Action accion)
    {
        return new Button
        {
            Text = texto,
            BackgroundColor = Color.FromArgb(colorHex),
            TextColor = Colors.White,
            CornerRadius = 12,
            FontAttributes = FontAttributes.Bold,
            FontSize = 14,
            WidthRequest = 140,
            HeightRequest = 45,
            Margin = new Thickness(6),
            Shadow = new Shadow
            {
                Radius = 5,
                Offset = new Point(2, 2),
                Opacity = 0.4f
            },
            Command = new Command(accion) // La acción asociada al botón
        };
    }

    #endregion

    #region Métodos de Navegación

    /// <summary>
    /// Muestra una vista dinámica dentro del contenedor _contentContainer.
    /// </summary>
    /// <param name="vista">La vista que se mostrará en el contenedor.</param>
    private void MostrarVista(View vista)
    {
        _contentContainer.Content = vista;
    }

    #endregion
}
