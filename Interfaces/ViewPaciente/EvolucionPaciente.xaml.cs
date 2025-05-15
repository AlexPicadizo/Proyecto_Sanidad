using ModeloBdClases.BaseDatos;
using ModeloBdClases.Clases;
using System;
using System.Collections.ObjectModel;

namespace Interfaces.ViewPaciente;

public partial class EvolucionPaciente : ContentView
{
    #region Campos y Propiedades

    // Paciente al que se le mostrarán las evoluciones
    private readonly Paciente _paciente;

    // Instancia de la clase MetodosBD que maneja las interacciones con la base de datos
    private readonly MetodosBD metodosBd = new MetodosBD();

    // Colección de notas de evolución que se va a mostrar en la interfaz
    private ObservableCollection<NotaEvolucion> _notasEvolucion = new();

    // Propiedad pública que expone la colección para enlazar con la vista (Binding)
    public ObservableCollection<NotaEvolucion> NotasEvolucion => _notasEvolucion;

    #endregion

    #region Constructor

    // Constructor que recibe el paciente y lo asigna a la propiedad _paciente
    // También inicia el proceso de carga de notas de evolución
    public EvolucionPaciente(Paciente paciente)
    {
        InitializeComponent();  // Inicializa la vista (XAML)
        _paciente = paciente;  // Asigna el paciente pasado al campo _paciente
        BindingContext = this;  // Establece el contexto de enlace de datos para la vista
        CargarNotasDeEvolucion();  // Llama al método para cargar las notas de evolución
    }

    #endregion

    #region Métodos de Carga y Creación de Notas

    /// <summary>
    /// Método asíncrono para cargar las notas de evolución desde la base de datos
    /// </summary>
    private async void CargarNotasDeEvolucion()
    {
        // Limpiar las notas previas en la interfaz
        layoutEvoluciones.Children.Clear();
        _notasEvolucion.Clear();  // Limpiar la colección interna de notas

        // Obtener el ID del paciente a partir de su DNI
        int pacienteId = metodosBd.ObtenerIdPaciente(_paciente.Dni);

        // Obtener las notas de evolución asociadas al paciente
        var notas = await metodosBd.ObtenerNotasEvolucionPaciente(pacienteId);

        // Si se obtuvieron notas, procesarlas
        if (notas != null && notas.Any())
        {
            // Ordenar las notas por fecha de manera descendente y agregarlas a la colección
            foreach (var nota in notas.OrderByDescending(n => n.Fecha))
            {
                _notasEvolucion.Add(nota);  // Añadir la nota a la colección observable
                // Crear la tarjeta visual para cada nota y añadirla a la interfaz
                layoutEvoluciones.Children.Add(await CrearTarjetaEvolucionAsync(nota));
            }
        }
    }

    /// <summary>
    /// Crea una tarjeta visual para mostrar una nota de evolución
    /// </summary>
    /// <param name="nota">Nota de evolución a mostrar</param>
    /// <returns>Una vista (Frame) que contiene la tarjeta</returns>
    private async Task<View> CrearTarjetaEvolucionAsync(NotaEvolucion nota)
    {
        Frame tarjeta = null;  // Referencia para poder eliminar el frame más adelante

        // Crear el botón para eliminar la nota
        var botonEliminar = new Button
        {
            Text = "Eliminar",
            BackgroundColor = Color.FromArgb("#EF4444"),  // Rojo para eliminar
            TextColor = Colors.White,
            CornerRadius = 8,
            FontSize = 12,
            Padding = new Thickness(10, 5),
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center
        };

        // Asociar el evento de clic para eliminar la nota
        botonEliminar.Clicked += async (s, e) =>
        {
            bool confirmacion = await Application.Current.MainPage.DisplayAlert(
                "Eliminar nota", "¿Estás seguro de que deseas eliminar esta nota de evolución?", "Sí", "Cancelar");

            if (!confirmacion) return;

            bool exito = await metodosBd.EliminarNotaEvolucion(nota.Id);

            if (exito)
            {
                _notasEvolucion.Remove(nota);

                // Eliminar visualmente el frame si existe
                if (tarjeta != null && layoutEvoluciones.Children.Contains(tarjeta))
                    layoutEvoluciones.Children.Remove(tarjeta);

                await Application.Current.MainPage.DisplayAlert("Eliminada", "Nota eliminada correctamente.", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar la nota.", "OK");
            }
        };

        // Crear un grid para organizar la fecha y el botón de eliminar
        var grid = new Grid
        {
            ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Star },  // Columna para la fecha
            new ColumnDefinition { Width = GridLength.Auto }   // Columna para el botón eliminar
        }
        };

        // Crear el label para mostrar la fecha de la nota
        var labelFecha = new Label
        {
            Text = "📅 " + nota.Fecha.ToString("dd/MM/yyyy"),  // Mostrar fecha con formato
            FontAttributes = FontAttributes.Bold,
            FontSize = 14,
            TextColor = Color.FromArgb("#1F2937"),  // Color oscuro
            VerticalOptions = LayoutOptions.Center
        };

        // Posicionar el botón de eliminar en la segunda columna
        Grid.SetColumn(botonEliminar, 1);
        Grid.SetRow(botonEliminar, 0);

        // Añadir controles al grid
        grid.Children.Add(labelFecha);
        grid.Children.Add(botonEliminar);

        // Crear el Frame (guardado en la variable tarjeta)
        tarjeta = new Frame
        {
            BorderColor = Color.FromArgb("#E5E7EB"),  // Color de borde gris
            CornerRadius = 12,
            Padding = new Thickness(15),
            Margin = new Thickness(10, 5),
            BackgroundColor = Color.FromArgb("#F9FAFB"),  // Fondo gris claro
            HasShadow = true,  // Sombra en el frame
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children =
            {
                grid,
                new BoxView
                {
                    HeightRequest = 1,  // Línea divisoria entre la fecha y el contenido
                    Color = Color.FromArgb("#E5E7EB"),
                    Margin = new Thickness(0, 5)
                },
                new Label
                {
                    Text = "📝 Contenido evolución:",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 14,
                    TextColor = Color.FromArgb("#4B5563")
                },
                new Label
                {
                    Text = nota.Contenido,  // Mostrar el contenido de la evolución
                    FontSize = 14,
                    TextColor = Color.FromArgb("#374151")
                }
            }
            }
        };

        return tarjeta;
    }

    #endregion

    #region Métodos de Eliminación y Guardado

    /// <summary>
    /// Método para eliminar la nota de evolución al hacer clic en el botón
    /// </summary>
    /// <param name="nota"></param>
    /// <returns></returns>
    private async Task EliminarNotaClicked(NotaEvolucion nota)
    {
        // Confirmar que el usuario desea eliminar la nota
        bool confirmacion = await Application.Current.MainPage.DisplayAlert(
            "Eliminar nota", "¿Estás seguro de que deseas eliminar esta nota de evolución?", "Sí", "Cancelar");

        // Si no confirma la eliminación, no hacer nada
        if (!confirmacion) return;

        // Intentar eliminar la nota desde la base de datos
        bool exito = await metodosBd.EliminarNotaEvolucion(nota.Id);

        // Si la eliminación fue exitosa, actualizar la interfaz
        if (exito)
        {
            _notasEvolucion.Remove(nota);  // Eliminar de la colección observable
            CargarNotasDeEvolucion();  // Recargar las notas
            await Application.Current.MainPage.DisplayAlert("Eliminada", "Nota eliminada correctamente.", "OK");
        }
        else
        {
            // Mostrar mensaje de error si la eliminación falló
            await Application.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar la nota.", "OK");
        }
    }

    /// <summary>
    /// Método para guardar una nueva evolución
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void guardarEvolucionClicked(object sender, EventArgs e)
    {
        try
        {
            // Crear la nueva nota de evolución con la fecha seleccionada y el contenido del editor
            var nuevaNota = new NotaEvolucion(fechaPicker.Date, notaEditor.Text)
            {
                Paciente = _paciente  // Asignar el paciente a la nota
            };

            // Obtener el ID del paciente
            int pacienteId = metodosBd.ObtenerIdPaciente(_paciente.Dni);

            // Intentar guardar la nota en la base de datos
            int nuevoId = await metodosBd.GuardarNotaEvolucionPorId(pacienteId, nuevaNota);
            if (nuevoId > 0)
            {
                nuevaNota.Id = nuevoId;
                _notasEvolucion.Add(nuevaNota);
                var nuevaTarjeta = await CrearTarjetaEvolucionAsync(nuevaNota);
                layoutEvoluciones.Children.Insert(0, nuevaTarjeta);

                await Application.Current.MainPage.DisplayAlert("Guardada", "Evolución guardada correctamente.", "OK");

                // Limpiar el contenido del editor
                notaEditor.Text = string.Empty;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo guardar la evolución.", "OK");
            }

        }
        catch (Exception error)
        {
            await Application.Current.MainPage.DisplayAlert("Error", error.Message, "Ok");
        }
    }

    #endregion
}

