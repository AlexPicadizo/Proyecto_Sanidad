using ModeloBdClases.BaseDatos;
using ModeloBdClases.Clases;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Interfaces.ViewPaciente;

public partial class DiagnosticoPaciente : ContentView
{
    #region Atributos Privados

    private Paciente _paciente;                        // Almacena la información del paciente actual.
    private Diagnostico _diagnosticoActual;            // Diagnóstico actualmente seleccionado.
    MetodosBD metodosBD = new MetodosBD();             // Instancia de la clase de acceso a la base de datos.

    #endregion

    #region Constructor

    public DiagnosticoPaciente(Paciente paciente)
    {
        InitializeComponent();
        _paciente = paciente;

        // Inicializa la lista de diagnósticos si es null.
        if (_paciente.Diagnosticos == null)
            _paciente.Diagnosticos = new List<Diagnostico>();

        // Obtiene los diagnósticos del paciente desde la base de datos.
        int idPaciente = metodosBD.ObtenerIdPaciente(_paciente.Dni);
        var diagnosticosDesdeBD = metodosBD.ObtenerDiagnosticosPaciente(idPaciente);
        _paciente.Diagnosticos = diagnosticosDesdeBD ?? new List<Diagnostico>();

        // Asigna relaciones y tratamientos a cada diagnóstico.
        foreach (var diagnostico in _paciente.Diagnosticos)
        {
            diagnostico.Paciente = _paciente;
            diagnostico.Tratamientos = metodosBD.ObtenerTratamientosPorDiagnostico(diagnostico.Id);
        }

        // Asigna la fuente de datos al Picker.
        pickerDiagnosticos.ItemsSource = _paciente.Diagnosticos;

        // Establece los estilos iniciales de los botones deshabilitados.
        btnModificar.Style = (Style)this.Resources["DisabledButton"];
        btnAgregarTratamiento.Style = (Style)this.Resources["DisabledButton"];
    }

    #endregion

    #region Eventos

    /// <summary>
    /// Agrega un nuevo diagnóstico a la base de datos y lo muestra en la interfaz.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnAgregarDiagnostico(object sender, EventArgs e)
    {
        try
        {
            string nombre = entryNombreDiagnostico.Text?.Trim();
            string descripcion = editorDescripcionDiagnostico.Text?.Trim();
            DateTime fecha = datePickerFechaDiagnostico.Date;

            // Validamos los campos usando la clase Diagnostico
            Diagnostico diagnostico = null;
            try
            {
                // Si los datos son válidos, creamos el diagnóstico
                diagnostico = new Diagnostico(nombre, descripcion, fecha);
            }
            catch (Exception ex)
            {
                // Si ocurre una excepción, mostramos el mensaje de error al usuario
                await MostrarError(ex.Message);
                return;
            }

            // Recupero el ID del paciente mediante su DNI
            int idPaciente = metodosBD.ObtenerIdPaciente(_paciente.Dni);

            // Ahora que hemos validado los datos, agregamos el diagnóstico a la base de datos
            bool diagnosticoAgregado = metodosBD.AgregarDiagnostico(diagnostico.Nombre, diagnostico.Descripcion, diagnostico.FechaDiagnostico, idPaciente);

            // Si el diagnóstico no se pudo agregar, mostramos un mensaje de error
            if (!diagnosticoAgregado)
            {
                await MostrarError("Hubo un problema al agregar el diagnóstico a la base de datos.");
                return;
            }

            // 1. Obtiene diagnósticos actualizados de la base de datos
            var diagnosticosDesdeBD = metodosBD.ObtenerDiagnosticosPaciente(idPaciente);

            // 2. Actualiza la lista local (usa lista vacía si es null)
            _paciente.Diagnosticos = diagnosticosDesdeBD ?? new List<Diagnostico>();

            // 3. Obtiene el diagnóstico más reciente (último de la lista)
            var nuevoDiagnostico = _paciente.Diagnosticos.LastOrDefault();

            // 4. Si existe un nuevo diagnóstico:
            if (nuevoDiagnostico != null)
            {
                // Asigna referencia al paciente
                nuevoDiagnostico.Paciente = _paciente;

                // Carga sus tratamientos asociados
                nuevoDiagnostico.Tratamientos = metodosBD.ObtenerTratamientosPorDiagnostico(nuevoDiagnostico.Id);
            }

            // Actualiza la fuente de datos del Picker
            pickerDiagnosticos.ItemsSource = null;
            pickerDiagnosticos.ItemsSource = _paciente.Diagnosticos;
            pickerDiagnosticos.SelectedItem = nuevoDiagnostico;

            // Muestra los detalles del diagnóstico
            MostrarDetallesDiagnostico(nuevoDiagnostico);

            // Muestra un mensaje de confirmación
            await Application.Current.MainPage.DisplayAlert("Éxito", "Diagnóstico agregado correctamente.", "OK");

            // Limpia los campos del formulario.
            entryNombreDiagnostico.Text = string.Empty;
            editorDescripcionDiagnostico.Text = string.Empty;
            datePickerFechaDiagnostico.Date = DateTime.Now;
        }
        catch (Exception ex)
        {
            // Si ocurre algún error, mostramos un mensaje de error al usuario
            await MostrarError(ex.Message);
        }
    }

    /// <summary>
    /// Modifica los datos del diagnóstico actual y lo actualiza en la base de datos.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnModificarDiagnostico(object sender, EventArgs e)
    {
        // Verifica si hay un diagnóstico seleccionado
        if (_diagnosticoActual == null)
        {
            await MostrarError("Por favor, seleccione un diagnóstico.");
            return;
        }

        try
        {
            // Actualiza los datos del diagnóstico localmente
            _diagnosticoActual.Nombre = entryNombreDiagnostico.Text;
            _diagnosticoActual.Descripcion = editorDescripcionDiagnostico.Text;
            _diagnosticoActual.FechaDiagnostico = datePickerFechaDiagnostico.Date;

            // Validar el diagnóstico usando las validaciones internas de la clase Diagnóstico
            // Si alguna propiedad no es válida, lanzará una excepción
            // Así que aquí es donde se validan los datos
            try
            {
                // Se lanzarán excepciones si los valores no son válidos
                var nombreValido = _diagnosticoActual.Nombre; // Esto disparará la validación en la propiedad
                var descripcionValida = _diagnosticoActual.Descripcion; // Esto también disparará la validación
                var fechaValida = _diagnosticoActual.FechaDiagnostico; // Esto disparará la validación de la fecha
            }
            catch (Exception ex)
            {
                // Si alguna validación falla, capturamos el error y mostramos el mensaje al usuario
                await MostrarError(ex.Message);
                return;
            }

            // Llama al método que actualizará el diagnóstico en la base de datos
            bool diagnosticoModificado = metodosBD.ActualizarDiagnostico(_diagnosticoActual.Id, _diagnosticoActual.Nombre, _diagnosticoActual.Descripcion, _diagnosticoActual.FechaDiagnostico);

            // Si el diagnóstico no se pudo actualizar, muestra un mensaje de error
            if (!diagnosticoModificado)
            {
                await MostrarError("Hubo un problema al actualizar el diagnóstico en la base de datos.");
                return;
            }

            // Recarga los diagnósticos del paciente desde la base de datos
            var diagnosticosDesdeBD = metodosBD.ObtenerDiagnosticosPaciente(metodosBD.ObtenerIdPaciente(_paciente.Dni));
            _paciente.Diagnosticos = diagnosticosDesdeBD ?? new List<Diagnostico>();

            // Actualiza el Picker con los diagnósticos recargados
            pickerDiagnosticos.ItemsSource = null;
            pickerDiagnosticos.ItemsSource = _paciente.Diagnosticos;

            // Vuelve a seleccionar el diagnóstico modificado en el Picker
            pickerDiagnosticos.SelectedItem = _diagnosticoActual;

            // Muestra los detalles del diagnóstico actualizado
            MostrarDetallesDiagnostico(_diagnosticoActual);

            // Confirmación de la actualización
            await Application.Current.MainPage.DisplayAlert("Modificado", "Diagnóstico actualizado.", "OK");
        }
        catch (Exception ex)
        {
            // En caso de un error inesperado, muestra el mensaje de error
            await MostrarError(ex.Message);
        }
    }

    /// <summary>
    /// Abre una vista para añadir tratamientos al diagnóstico actual.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnAgregarTratamiento(object sender, EventArgs e)
    {
        if (_diagnosticoActual == null) return;

        int idPaciente = metodosBD.ObtenerIdPaciente(_paciente.Dni);
        var tratamientoView = new TratamientosPaciente(_diagnosticoActual, idPaciente);
        var page = new ContentPage { Content = tratamientoView };

        page.Disappearing += async (s, args) =>
        {
            // Actualiza la lista de tratamientos desde la base de datos
            _diagnosticoActual.Tratamientos = metodosBD.ObtenerTratamientosPorDiagnostico(_diagnosticoActual.Id);

            // Para cada tratamiento, carga sus medicamentos asociados
            foreach (var tratamiento in _diagnosticoActual.Tratamientos)
            {
                tratamiento.Medicamentos = metodosBD.ObtenerTratamientoMedicamentos(tratamiento.Id);
            }

            // Refresca la vista del diagnóstico para mostrar cambios
            MostrarDetallesDiagnostico(_diagnosticoActual);
        };

        await Navigation.PushAsync(page);
    }
    /// <summary>
    /// Muestra los detalles del diagnóstico seleccionado.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDiagnosticoSeleccionado(object sender, EventArgs e)
    {
        // Verifica si el ítem seleccionado es un objeto Diagnostico
        if (pickerDiagnosticos.SelectedItem is Diagnostico diagnostico)
        {
            // 1. Asigna/relaciona el paciente al diagnóstico seleccionado
            diagnostico.Paciente = _paciente;

            // 2. Obtiene todos los tratamientos asociados a este diagnóstico
            diagnostico.Tratamientos = metodosBD.ObtenerTratamientosPorDiagnostico(diagnostico.Id);

            // 3. Para cada tratamiento, carga sus medicamentos asociados
            foreach (var t in diagnostico.Tratamientos)
                t.Medicamentos = metodosBD.ObtenerTratamientoMedicamentos(t.Id);

            // 4. Muestra los detalles completos del diagnóstico en la UI
            MostrarDetallesDiagnostico(diagnostico);
        }
    }

    /// <summary>
    /// Elimina el diagnóstico seleccionado tras confirmar con el usuario.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnEliminarDiagnostico(object sender, EventArgs e)
    {
        // Verifica si el ítem seleccionado es un objeto Diagnostico
        if (pickerDiagnosticos.SelectedItem is Diagnostico diagnosticoSeleccionado)
        {
            // 1. Pide confirmación al usuario mediante un alert dialog
            bool confirmar = await Application.Current.MainPage.DisplayAlert(
                "Eliminar diagnóstico",
                $"¿Estás seguro de eliminar el diagnóstico \"{diagnosticoSeleccionado.Nombre}\"?",
                "Sí", "Cancelar");

            // Si el usuario cancela, sale del método
            if (!confirmar) return;

            // 2. Intenta eliminar el diagnóstico de la base de datos
            bool eliminado = metodosBD.EliminarDiagnostico(diagnosticoSeleccionado.Id);

            // 3. Si la eliminación fue exitosa
            if (eliminado)
            {
                // Actualiza la lista local eliminando el diagnóstico
                _paciente.Diagnosticos.Remove(diagnosticoSeleccionado);

                // Refresca el Picker (selector) de diagnósticos
                pickerDiagnosticos.ItemsSource = null;
                pickerDiagnosticos.ItemsSource = _paciente.Diagnosticos;
                pickerDiagnosticos.SelectedIndex = -1; // Deselecciona cualquier ítem

                // Limpia y oculta el panel de detalles
                layoutDetalleDiagnostico.IsVisible = false;
                entryNombreDiagnostico.Text = string.Empty;
                editorDescripcionDiagnostico.Text = string.Empty;
                datePickerFechaDiagnostico.Date = DateTime.Now;

                // Muestra confirmación de éxito
                await Application.Current.MainPage.DisplayAlert("Éxito", "Diagnóstico eliminado correctamente.", "OK");
            }
            else // Si falla la eliminación
            {
                await MostrarError("No se pudo eliminar el diagnóstico.");
            }
        }
    }

    #endregion

    #region Métodos Auxiliares

    /// <summary>
    /// Muestra los datos del diagnóstico en la interfaz.
    /// </summary>
    /// <param name="diagnostico">Diagnostico del que mostrar los detalles</param>
    private void MostrarDetallesDiagnostico(Diagnostico diagnostico)
    {
        // Guarda el diagnóstico actual, pasado como parámetro
        _diagnosticoActual = diagnostico;

        // Habilita y deshabilita botones y elementos de la interfaz
        btnModificar.IsEnabled = true;
        btnModificar.Style = (Style)this.Resources["SecondaryButton"];
        btnAgregarTratamiento.IsEnabled = true;
        btnAgregarTratamiento.Style = (Style)this.Resources["PrimaryButton"];

        // Muestra los detalles del diagnóstico
        entryNombreDiagnostico.Text = diagnostico.Nombre;
        editorDescripcionDiagnostico.Text = diagnostico.Descripcion;
        datePickerFechaDiagnostico.Date = diagnostico.FechaDiagnostico;

        // Muestra los tratamientos
        labelNombreDiagnostico.Text = diagnostico.Nombre;
        labelDescripcionDiagnostico.Text = diagnostico.Descripcion;
        labelFechaDiagnostico.Text = $"Fecha: {diagnostico.FechaDiagnostico:dd/MM/yyyy}";

        // Muestra los tratamientos
        layoutDetalleDiagnostico.IsVisible = true;
        layoutTratamientos.Children.Clear(); // Limpiar los tratamientos previos

        // En caso de que existan tratamientos
        if (diagnostico.Tratamientos.Count > 0)
        {
            // Añado los tratamientos al layout
            foreach (var tratamiento in diagnostico.Tratamientos)
                layoutTratamientos.Children.Add(CrearFrameTratamiento(tratamiento));
        }
        else
        {
            // Si no existen tratamientos
            layoutTratamientos.Children.Add(new Label
            {
                // Muestro mensaje de que no hay tratamientos
                Text = "Este diagnóstico no tiene tratamientos.",
                TextColor = Colors.Gray,
                FontAttributes = FontAttributes.Italic
            });
        }
    }

    /// <summary>
    /// Crea un Frame visual para mostrar la información de un tratamiento.
    /// </summary>
    /// <param name="tratamiento"> Tratamiento a mostrar</param>
    /// <returns> Frame con la información del tratamiento</returns>
    private Frame CrearFrameTratamiento(Tratamiento tratamiento)
    {
        // Creo un layout para los medicamentos del tratamiento
        var medicamentosLayout = new StackLayout
        {
            Spacing = 4,
            Margin = new Thickness(10, 0, 0, 0)
        };

        // En caso de que existan medicamentos
        if (tratamiento.Medicamentos?.Any() == true)
        {
            // Recorro los medicamentos del tratamiento
            foreach (var tm in tratamiento.Medicamentos)
            {
                // Agrego los medicamentos al layout
                medicamentosLayout.Children.Add(new Label
                {
                    // Muestro la información del medicamento y sus instrucciones, formateado
                    FormattedText = new FormattedString
                    {
                        // Spans para formatear el texto
                        // Crea un texto con múltiples partes con formatos distintos (Spans)
                        Spans =
                        {
                            // Primer Span: Nombre del medicamento
                            new Span
                            {
                                Text = $"- {tm.Medicamento.Nombre}",  // Ejemplo: "- Paracetamol"
                                FontAttributes = FontAttributes.Bold,   // Texto en negrita
                                FontSize = 14,                          // Tamaño 14
                                TextColor = Colors.Black                // Color negro
                            },
        
                            // Segundo Span: Instrucciones de uso
                            new Span
                            {
                                Text = $"\n  Instrucciones: {tm.Instrucciones}", // Ejemplo: "\n  Instrucciones: Tomar cada 8 horas"
                                FontSize = 13,                          // Tamaño ligeramente más pequeño
                                TextColor = Colors.DarkSlateGray        // Color gris oscuro
                            }
                        }
                    }
                });
            }
        }
        else
        {
            // Si no hay medicamentos muestra el mensaje correspondiente para indicar que no existen
            medicamentosLayout.Children.Add(new Label
            {
                Text = "No hay medicamentos asociados.",
                FontAttributes = FontAttributes.Italic,
                FontSize = 13,
                TextColor = Colors.Gray
            });
        }

        // Devuelvo el Frame con la información del tratamiento y sus medicamentos asociados formateado
        return new Frame
        {
            Padding = 15,
            Margin = new Thickness(0, 10),
            CornerRadius = 12,
            BackgroundColor = Color.FromArgb("#F0F4F8"),
            BorderColor = Color.FromArgb("#B0BEC5"),
            HasShadow = true,
            Content = new StackLayout                // Contenedor del contenido
            {
                Spacing = 8,                         // Espacio entre elementos
                Children =                           // Elementos hijos:
            {
                // 1. Nombre del tratamiento (destacado)
                new Label
                {
                    Text = tratamiento.Nombre,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16,
                    TextColor = Colors.DarkSlateBlue
                },
            
                // 2. Dosis diaria
                new Label
                {
                    Text = tratamiento.DosisDiaria,
                    FontSize = 14,
                    TextColor = Colors.DarkSlateGray
                },
            
                // 3. Fecha de inicio (formateada)
                new Label
                {
                    Text = $"Inicio: {tratamiento.FechaInicio?.ToShortDateString()}",
                    FontSize = 13,
                    TextColor = Colors.Gray
                },
            
                // 4. Fecha de fin (formateada)
                new Label
                {
                    Text = $"Fin: {tratamiento.FechaFin?.ToShortDateString()}",
                    FontSize = 13,
                    TextColor = Colors.Gray
                },
            
                // 5. Título sección medicamentos
                new Label
                {
                    Text = "Medicamentos:",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 14,
                    Margin = new Thickness(0, 10, 0, 0),  // Margen superior
                    TextColor = Colors.Teal
                },
            
                // 6. Layout con la lista de medicamentos (previamente creado)
                medicamentosLayout            
                }
            }
        };
    }

    // Muestra una alerta de error en pantalla.
    private Task MostrarError(string mensaje)
    {
        return Application.Current.MainPage.DisplayAlert("Error", mensaje, "OK");
    }

    #endregion
}
