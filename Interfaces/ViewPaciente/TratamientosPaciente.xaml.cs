using ModeloBdClases.BaseDatos;
using ModeloBdClases.Clases;
using System.Collections.ObjectModel;

namespace Interfaces.ViewPaciente;

public partial class TratamientosPaciente : ContentView
{
    #region Campos y Propiedades

    // Objeto de Diagn�stico recibido (asociado al tratamiento)
    private Diagnostico _diagnostico;

    // Instancia del tratamiento que se va a guardar
    Tratamiento tratamiento;

    // Acceso a la base de datos
    MetodosBD metodosBd = new MetodosBD();

    // ID del paciente al que se le aplicar� este tratamiento
    private int pacienteId;

    // Lista de medicamentos disponibles (cargados desde BD)
    private List<Medicamento> medicamentosDisponibles = new List<Medicamento>();

    // Lista observable que contiene los medicamentos seleccionados para este tratamiento
    private ObservableCollection<TratamientoMedicamento> medicamentosSeleccionados = new ObservableCollection<TratamientoMedicamento>();

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor que inicializa la vista de tratamientos con el diagn�stico y el ID del paciente.
    /// Carga los medicamentos disponibles y muestra la informaci�n inicial.
    /// </summary>
    /// <param name="diagnostico">El diagn�stico asociado al tratamiento.</param>
    /// <param name="idPaciente">El ID del paciente al que se le aplicar� el tratamiento.</param>
    public TratamientosPaciente(Diagnostico diagnostico, int idPaciente)
    {
        InitializeComponent();

        // Limpieza de la lista de medicamentos seleccionados por si hab�a datos previos
        medicamentosSeleccionados.Clear();

        _diagnostico = diagnostico;
        pacienteId = idPaciente;

        // Mostrar el nombre del diagn�stico en la interfaz
        lNombreDiagnostico.Text = _diagnostico.Nombre;

        // Fecha de inicio por defecto es el d�a de hoy
        datePickerInicio.Date = DateTime.Today;

        // Cargar los medicamentos disponibles desde la base de datos
        medicamentosDisponibles = metodosBd.ObtenerMedicamentos();

        // Asignar los medicamentos disponibles a la lista que se mostrar�
        listaMedicamentosDisponibles.ItemsSource = medicamentosDisponibles;

        // Vincular la lista de medicamentos seleccionados a la UI
        listaMedicamentos.ItemsSource = medicamentosSeleccionados;
    }

    #endregion

    #region M�todos de Guardado

    /// <summary>
    /// Evento al pulsar el bot�n "Guardar Tratamiento". 
    /// Valida los datos y guarda el tratamiento y los medicamentos en la base de datos.
    /// </summary>
    private async void OnGuardarTratamiento(object sender, EventArgs e)
    {
        // Validaci�n de campos requeridos
        if (string.IsNullOrWhiteSpace(entryNombre.Text))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Debe especificar el nombre del tratamiento", "OK");
            return;
        }

        // Captura de datos de entrada del tratamiento
        var nombre = entryNombre.Text;
        var fechaInicio = datePickerInicio.Date;
        var fechaFin = datePickerFin.Date;

        try
        {
            tratamiento = new Tratamiento(nombre, fechaInicio, fechaFin);

            // Validar las fechas del tratamiento
            tratamiento.ValidarFechas();
        }
        catch (Exception ex)
        {
            // Mostrar mensaje de error en caso de fallos en la validaci�n
            await Application.Current.MainPage.DisplayAlert("Error de validaci�n", ex.Message, "OK");
            return;
        }

        // Validar que el diagn�stico tenga un ID v�lido
        if (_diagnostico.Id <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Este diagn�stico a�n no ha sido guardado en la base de datos.", "OK");
            return;
        }

        // Guardar tratamiento en la BD y recuperar su ID
        var idDiagnostico = _diagnostico.Id;
        int nuevoId = metodosBd.GuardarTratamiento(tratamiento, idDiagnostico);
        tratamiento.Id = nuevoId;

        // Asociar tratamiento al diagn�stico en memoria
        _diagnostico.Tratamientos.Add(tratamiento);

        // Guardar cada medicamento asociado al tratamiento
        foreach (var tm in medicamentosSeleccionados)
        {
            int idMedicamento = tm.Medicamento.Id;

            // Si el medicamento no tiene ID, buscarlo por su nombre
            if (idMedicamento <= 0)
            {
                idMedicamento = metodosBd.ObtenerIdMedicamentoPorNombre(tm.Medicamento.Nombre);
            }

            // Guardar la relaci�n tratamiento-medicamento en la base de datos
            metodosBd.GuardarTratamientoMedicamento(tratamiento.Id, idMedicamento, tm.Instrucciones);
        }

        // Confirmaci�n y regreso
        await Application.Current.MainPage.DisplayAlert("�xito", "Tratamiento y medicamentos guardados correctamente", "OK");
        await Navigation.PopAsync();
    }

    #endregion

    #region M�todos de Manejo de Medicamentos

    /// <summary>
    /// Evento para eliminar un medicamento de la lista de medicamentos seleccionados.
    /// </summary>
    private void OnEliminarMedicamento(object sender, EventArgs e)
    {
        var button = sender as Button;
        var tm = button?.CommandParameter as TratamientoMedicamento;

        if (tm != null)
        {
            // Buscar el medicamento en la lista de seleccionados
            var item = medicamentosSeleccionados.FirstOrDefault(x => x.Medicamento.Id == tm.Medicamento.Id);

            if (item != null)
            {
                // Eliminar el medicamento de la lista de seleccionados
                medicamentosSeleccionados.Remove(item);

                // Devolver el medicamento a la lista de disponibles
                medicamentosDisponibles.Add(item.Medicamento);

                // Refrescar la interfaz
                listaMedicamentosDisponibles.ItemsSource = null;
                listaMedicamentosDisponibles.ItemsSource = medicamentosDisponibles;
                listaMedicamentos.ItemsSource = medicamentosSeleccionados;
            }
        }
    }

    /// <summary>
    /// Evento para agregar un medicamento a la lista de medicamentos seleccionados.
    /// </summary>
    private void OnAgregarMedicamento(object sender, EventArgs e)
    {
        var medicamentoSeleccionado = listaMedicamentosDisponibles.SelectedItem as Medicamento;

        // Validar que un medicamento ha sido seleccionado
        if (medicamentoSeleccionado == null)
        {
            Application.Current.MainPage.DisplayAlert("Error", "Selecciona un medicamento de la lista", "OK");
            return;
        }

        // Evitar agregar medicamentos duplicados
        if (medicamentosSeleccionados.Any(m => m.Medicamento.Id == medicamentoSeleccionado.Id))
        {
            Application.Current.MainPage.DisplayAlert("Info", "Este medicamento ya est� agregado.", "OK");
            return;
        }

        // Agregar el medicamento a la lista de seleccionados con las instrucciones por defecto
        medicamentosSeleccionados.Add(new TratamientoMedicamento(medicamentoSeleccionado, medicamentoSeleccionado.Descripcion));

        // Eliminar el medicamento de la lista de disponibles
        var medicamentoAEliminar = medicamentosDisponibles.FirstOrDefault(m => m.Id == medicamentoSeleccionado.Id);
        if (medicamentoAEliminar != null)
        {
            medicamentosDisponibles.Remove(medicamentoAEliminar);
        }

        // Refrescar la interfaz de usuario
        listaMedicamentosDisponibles.ItemsSource = null;
        listaMedicamentosDisponibles.ItemsSource = medicamentosDisponibles;
        listaMedicamentos.ItemsSource = medicamentosSeleccionados;

        // Limpiar los campos
        listaMedicamentosDisponibles.SelectedItem = null;
        buscadorMedicamentos.Text = string.Empty;
        entryInstruccionesMedicamento.Text = string.Empty;
    }

    /// <summary>
    /// Evento para filtrar la lista de medicamentos disponibles por nombre.
    /// </summary>
    private void OnBuscarMedicamento(object sender, TextChangedEventArgs e)
    {
        var texto = e.NewTextValue ?? "";

        // Filtrar medicamentos sin importar may�sculas/min�sculas
        var filtrados = medicamentosDisponibles
            .Where(m => m.Nombre.StartsWith(texto, StringComparison.OrdinalIgnoreCase))
            .ToList();

        listaMedicamentosDisponibles.ItemsSource = filtrados;
    }

    /// <summary>
    /// Evento al seleccionar un medicamento en la lista.
    /// Muestra el nombre y las instrucciones del medicamento en los campos correspondientes.
    /// </summary>
    private void OnSeleccionarMedicamento(object sender, SelectionChangedEventArgs e)
    {
        var medicamentoSeleccionado = e.CurrentSelection.FirstOrDefault() as Medicamento;

        if (medicamentoSeleccionado != null)
        {
            // Mostrar el nombre del medicamento en el campo de b�squeda
            buscadorMedicamentos.Text = medicamentoSeleccionado.Nombre;

            // Rellenar el campo de instrucciones con la descripci�n del medicamento
            entryInstruccionesMedicamento.Text = medicamentoSeleccionado.Descripcion;

            // Deshabilitar la edici�n de instrucciones (opcional)
            entryInstruccionesMedicamento.IsEnabled = false;
            entryInstruccionesMedicamento.BackgroundColor = Colors.LightGray;
        }
    }

    #endregion
}
