using ModeloBdClases.BaseDatos;
using ModeloBdClases.Clases;
using System.Collections.ObjectModel;

namespace Interfaces.ViewPaciente;

public partial class AlergiasPaciente : ContentView
{
    #region Campos privados

    private Paciente _paciente;
    private MetodosBD _metodosBD = new MetodosBD();
    private List<Alergia> _todasAlergias;
    private Alergia _alergiaSeleccionadaActual;

    #endregion

    #region Propiedades Binding

    /// <summary>
    /// Lista de alergias disponibles para buscar y seleccionar.
    /// Esta colección se muestra en el CollectionView filtrado.
    /// </summary>
    public ObservableCollection<Alergia> AlergiasFiltradas { get; set; }

    /// <summary>
    /// Lista de alergias ya asociadas al paciente.
    /// Esta colección se muestra en el CollectionView inferior.
    /// </summary>
    public ObservableCollection<Alergia> AlergiasSeleccionadas { get; set; }

    #endregion

    #region Constructor

    public AlergiasPaciente(Paciente paciente)
    {
        InitializeComponent();
        _paciente = paciente;

        AlergiasFiltradas = new ObservableCollection<Alergia>();
        AlergiasSeleccionadas = new ObservableCollection<Alergia>();

        BindingContext = this;

        CargarDatosIniciales();
    }

    #endregion

    #region Carga inicial de datos

    /// <summary>
    /// Carga las alergias disponibles y las del paciente desde la base de datos.
    /// Inicializa ambas listas para mostrarlas en pantalla.
    /// </summary>
    private async void CargarDatosIniciales()
    {
        try
        {
            _todasAlergias = _metodosBD.ObtenerTodasLasAlergias();
            int pacienteId = _metodosBD.ObtenerIdPaciente(_paciente.Dni);
            var alergiasDelPaciente = _metodosBD.ObtenerAlergiasDePaciente(pacienteId);

            // Actualizamos las colecciones en el hilo de UI
            Dispatcher.Dispatch(() =>
            {
                AlergiasSeleccionadas.Clear();
                foreach (var alergia in alergiasDelPaciente)
                    AlergiasSeleccionadas.Add(alergia);

                // Filtramos para que no aparezcan en la búsqueda las ya seleccionadas
                AlergiasFiltradas.Clear();
                foreach (var alergia in _todasAlergias
                    .Where(a => !AlergiasSeleccionadas.Any(sel => sel.Id == a.Id)))
                {
                    AlergiasFiltradas.Add(alergia);
                }
            });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Error cargando alergias: " + ex.Message, "OK");
        }
    }

    #endregion

    #region Búsqueda y filtrado

    /// <summary>
    /// Se ejecuta al presionar el botón de búsqueda.
    /// Filtra las alergias cuyo nombre contenga el texto ingresado.
    /// </summary>
    private void OnSearchAlergia(object sender, EventArgs e)
    {
        // Obtener el texto en minúsculas, evitando nulos
        string texto = searchBar.Text?.ToLower() ?? "";

        // Filtrar alergias que contengan el texto
        var resultados = _todasAlergias
            .Where(a => a.Nombre.ToLower().Contains(texto))
            .ToList();

        // Actualizar la lista filtrada en pantalla
        AlergiasFiltradas.Clear();
        foreach (var alergia in resultados)
            AlergiasFiltradas.Add(alergia);
    }

    /// <summary>
    /// Se ejecuta cada vez que el texto en el SearchBar cambia.
    /// Llama al método que realiza el filtrado en tiempo real.
    /// </summary>
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        FiltrarAlergias(e.NewTextValue ?? "");
    }

    /// <summary>
    /// Filtra la lista de alergias sin mostrar las que ya están seleccionadas.
    /// </summary>
    private void FiltrarAlergias(string texto)
    {
        // Filtro combinado: texto + no seleccionadas
        var resultados = _todasAlergias
            .Where(a =>
                a.Nombre.ToLower().Contains(texto.ToLower()) &&
                !AlergiasSeleccionadas.Any(sel => sel.Id == a.Id))
            .ToList();

        AlergiasFiltradas.Clear();
        foreach (var alergia in resultados)
            AlergiasFiltradas.Add(alergia);
    }

    #endregion

    #region Selección y agregado

    /// <summary>
    /// Se ejecuta al seleccionar una alergia en la lista filtrada.
    /// Guarda la alergia para poder agregarla y actualiza el texto del buscador.
    /// </summary>
    private void OnAlergiaSeleccionada(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Alergia seleccionada)
        {
            searchBar.Text = seleccionada.Nombre;
            _alergiaSeleccionadaActual = seleccionada;
        }
    }

    /// <summary>
    /// Agrega la alergia seleccionada al paciente, actualizando BD y lista visual.
    /// </summary>
    private async void OnAgregarAlergia(object sender, EventArgs e)
    {
        if (_alergiaSeleccionadaActual != null)
        {
            var seleccionada = _alergiaSeleccionadaActual;

            // Validar si ya está agregada
            if (AlergiasSeleccionadas.Any(a => a.Id == seleccionada.Id))
            {
                await Shell.Current.DisplayAlert("Aviso", "Esta alergia ya está agregada", "OK");
                return;
            }

            int pacienteId = _metodosBD.ObtenerIdPaciente(_paciente.Dni);
            bool exito = _metodosBD.AgregarAlergiaAPaciente(pacienteId, seleccionada.Id);

            if (exito)
            {
                // Actualizar lista visual
                AlergiasSeleccionadas.Add(seleccionada);

                // Limpiar selección
                _alergiaSeleccionadaActual = null;
                alergiasListView.SelectedItem = null;
                searchBar.Text = string.Empty;
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo agregar la alergia", "OK");
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("Aviso", "Seleccione una alergia primero", "OK");
        }
    }

    #endregion

    #region Eliminación

    /// <summary>
    /// Elimina una alergia del paciente. Pide confirmación, elimina de BD y de la lista.
    /// </summary>
    private async void OnEliminarAlergia(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Alergia alergia)
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Confirmar",
                $"¿Eliminar la alergia {alergia.Nombre}?",
                "Sí", "No");

            if (!confirmar) return;

            int pacienteId = _metodosBD.ObtenerIdPaciente(_paciente.Dni);
            bool exito = _metodosBD.EliminarAlergiaDePaciente(pacienteId, alergia.Id);

            if (exito)
            {
                AlergiasSeleccionadas.Remove(alergia);
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo eliminar la alergia", "OK");
            }
        }
    }

    #endregion
}
