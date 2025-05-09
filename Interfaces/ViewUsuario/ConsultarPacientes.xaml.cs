using Interfaces.ViewPaciente;
using ModeloBdClases.BaseDatos;
using ModeloBdClases.Clases;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Interfaces.ViewUsuario;

public partial class ConsultarPacientes : ContentPage, INotifyPropertyChanged
{
    #region Propiedades y Campos

    // Instancia para manejar operaciones con la base de datos.
    private MetodosBD metodosBD = new MetodosBD();

    // Lista observable de pacientes, que está ligada a un CollectionView en la interfaz.
    // Esto permitirá que los cambios en la lista se reflejen automáticamente en la UI.
    public ObservableCollection<Paciente> PacientesCollection { get; private set; }

    // Propiedad que indica si la lista de pacientes se está actualizando (cuando se hace pull-to-refresh).
    // Se usa para mostrar o esconder un indicador de carga (refresh spinner).
    private bool isRefreshing;
    public bool IsRefreshing
    {
        get => isRefreshing;
        set
        {
            // Si el valor de isRefreshing cambia, se actualiza y notifica la vista.
            if (isRefreshing != value)
            {
                isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing)); // Notifica a la vista que el valor de IsRefreshing ha cambiado.
            }
        }
    }

    // Comando que se ejecuta cuando el usuario hace pull-to-refresh en la pantalla.
    // Este comando está asociado a un control de tipo RefreshView.
    public ICommand RefreshCommand { get; }

    #endregion

    #region Constructor
    public ConsultarPacientes()
    {
        InitializeComponent(); // Inicializa los componentes visuales (XAML).
        BindingContext = this; // Establece el contexto de enlace de datos a esta clase para que los bindings en XAML funcionen.

        PacientesCollection = new ObservableCollection<Paciente>(); // Inicializa la colección observable de pacientes.
        collectionViewPacientes.ItemsSource = PacientesCollection; // Asocia la colección al CollectionView para su visualización.

        // Inicializa el comando de refresco (pull-to-refresh).
        RefreshCommand = new Command(RefrescarPacientes);

        // Carga inicial de pacientes desde la base de datos.
        CargarPacientes();

        // Suscripción a un evento de MessagingCenter para actualizar la lista de pacientes cuando se agreguen nuevos pacientes.
        MessagingCenter.Subscribe<AgregarPacientes>(this, "ActualizarPacientes", (sender) =>
        {
            CargarPacientes(); // Recarga los pacientes cuando se recibe el mensaje "ActualizarPacientes".
        });
    }

    #endregion

    #region Actualización de Datos

    /// <summary>
    /// Este método se ejecuta cuando el usuario hace pull-to-refresh en la pantalla.
    /// </summary>
    private void RefrescarPacientes()
    {
        IsRefreshing = true; // Muestra el indicador de carga.

        // Realiza la actualización de datos en un hilo de fondo.
        Task.Run(() =>
        {
            CargarPacientes(); // Recarga los pacientes.

            // Una vez que se haya terminado de cargar, vuelve al hilo principal para actualizar la UI.
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsRefreshing = false; // Oculta el indicador de carga.
            });
        });
    }

    /// <summary>
    /// Carga o recarga la lista de pacientes desde la base de datos.
    /// Este método se usa tanto para la carga inicial como para la actualización con el pull-to-refresh.
    /// </summary>
    private void CargarPacientes()
    {
        try
        {
            // Obtiene el ID del usuario actual desde las preferencias del dispositivo.
            int usuarioId = Preferences.Get("UsuarioId", 0);

            if (usuarioId == 0)
            {
                // Si no se encuentra el ID del usuario, muestra un mensaje de error.
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", "No se pudo obtener el usuario logueado.", "OK");
                });
                return;
            }

            // Obtiene la lista de pacientes del usuario desde la base de datos.
            var pacientes = metodosBD.ObtenerPacientesUsuario(usuarioId) ?? new List<Paciente>();

            // Vuelve al hilo principal para actualizar la UI.
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PacientesCollection.Clear(); // Limpiar la lista observable antes de agregar los nuevos pacientes.

                // Agregar los pacientes a la colección observable.
                foreach (Paciente paciente in pacientes)
                {
                    PacientesCollection.Add(paciente);
                }
            });
        }
        catch (Exception ex)
        {
            // Si ocurre un error, muestra un mensaje de error en la UI.
            MainThread.BeginInvokeOnMainThread(() =>
            {
                DisplayAlert("Error", $"Error al cargar pacientes: {ex.Message}", "OK");
            });
        }
    }

    #endregion

    #region Ciclo de Vida de la Página

    /// <summary>
    /// Este método se ejecuta cuando la página aparece en pantalla. Aquí se vuelve a cargar la lista de pacientes.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing(); // Llama al método base de OnAppearing.
        CargarPacientes(); // Recarga los pacientes cada vez que la página aparece.
    }

    /// <summary>
    /// Este método se ejecuta cuando la página desaparece de la pantalla.
    /// Se usa para dejar de escuchar eventos de MessagingCenter.
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing(); // Llama al método base de OnDisappearing.
        MessagingCenter.Unsubscribe<AgregarPacientes>(this, "ActualizarPacientes"); // Deja de escuchar el evento de "ActualizarPacientes".
    }

    #endregion

    #region Acciones de UI

    /// <summary>
    /// Este evento se dispara cuando el usuario hace clic en el botón "Eliminar" para eliminar a un paciente.
    /// </summary>
    private async void OnEliminarPacienteClicked(object sender, EventArgs e)
    {
        // Si el botón tiene un contexto asociado con un paciente, lo obtenemos.
        if (sender is Button button && button.BindingContext is Paciente paciente)
        {
            // Muestra una confirmación antes de eliminar al paciente.
            bool confirmacion = await Application.Current.MainPage.DisplayAlert(
                "Eliminar Paciente",
                $"¿Estás seguro de que deseas eliminar a {paciente.Nombre}?",
                "Sí",
                "No"
            );

            // Si el usuario confirma, procedemos con la eliminación.
            if (confirmacion)
            {
                await Task.Run(() =>
                {
                    // Obtiene el ID del paciente a eliminar basado en su DNI.
                    int idPaciente = metodosBD.ObtenerIdPaciente(paciente.Dni);

                    if (idPaciente > 0)
                    {
                        // Elimina al paciente de la base de datos.
                        bool eliminado = metodosBD.BorrarPaciente(idPaciente);

                        // Vuelve al hilo principal para actualizar la UI.
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            if (eliminado)
                            {
                                // Elimina al paciente de la colección observable si fue eliminado correctamente.
                                PacientesCollection.Remove(paciente);
                            }
                            else
                            {
                                // Muestra un mensaje de error si no se pudo eliminar.
                                DisplayAlert("Error", "No se pudo eliminar el paciente.", "OK");
                            }
                        });
                    }
                    else
                    {
                        // Si no se encuentra el paciente, muestra un mensaje de error.
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert("Error", "No se encontró el paciente en la base de datos.", "OK");
                        });
                    }
                });
            }
        }
    }

    /// <summary>
    /// Evento que se ejecuta cuando el usuario hace clic en el botón "Acceder", para acceder a la vista de gestión de un paciente.
    /// </summary>
    private async void OnAccederPacienteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Paciente paciente)
        {
            // Abre una nueva página para gestionar los detalles del paciente.
            var paginaPaciente = new GestionViewPaciente(paciente);
            await Navigation.PushAsync(paginaPaciente);
        }
    }

    #endregion

    #region Notificación de Cambios (INotifyPropertyChanged)

    /// <summary>
    /// Implementa la interfaz INotifyPropertyChanged para notificar a la UI cuando una propiedad cambia.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Este método notifica a la vista que una propiedad ha cambiado. 
    /// Se usa principalmente cuando la propiedad 'IsRefreshing' cambia para que la UI se actualice.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad que ha cambiado.</param>
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    #endregion

}

