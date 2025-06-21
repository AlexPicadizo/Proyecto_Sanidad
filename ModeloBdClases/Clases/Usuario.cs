using ModeloBdClases.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModeloBdClases.Clases
{
    public class Usuario
    {
        // Atributos
        private string nombre, apellidos, email, contrasenia;
        private bool isAdmin, isActiva;
        public event PropertyChangedEventHandler PropertyChanged;

        #region RELACIONES
        // Relación de uno a muchos con Pacientes para poder guardar los pacientes asociados
        public List<Paciente> Paciente { get; set; }
        #endregion

        #region CONSTRUCTORES

        public Usuario() { }

        /// <summary>
        /// Constructor con los parámetros necesarios para el login
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="contrasenia">Contrasenia del usuario</param>
        public Usuario(string email, string contrasenia)
        {
            Email = email;
            Contrasenia = contrasenia;
        }

        /// <summary>
        /// Constructor con todos los parámetros para agregar a la base de datos
        /// </summary>
        /// <param name="nombre">Nombre del usuario</param>
        /// <param name="apellidos">Apellidos del usuario</param>
        /// <param name="email">Email del usuario</param>
        /// <param name="contrasenia">Contrasenia del usuario</param>
        /// <param name="isAdmin">Es admin o no</param>
        public Usuario(string nombre, string apellidos, string email, string contrasenia, bool isAdmin)
        {
            Nombre = nombre;
            Apellidos = apellidos;
            Email = email;
            Contrasenia = contrasenia;
            IsAdmin = isAdmin;
        }

        /// <summary>
        /// Constructor con el parámetro añadido para activar o desactivar la cuenta
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="apellidos"></param>
        /// <param name="email"></param>
        /// <param name="contrasenia"></param>
        /// <param name="isAdmin"></param>
        /// <param name="isActiva"></param>
        public Usuario(string nombre, string apellidos, string email, string contrasenia, bool isAdmin, bool isActiva)
        {
            Nombre = nombre;
            Apellidos = apellidos;
            Email = email;
            Contrasenia = contrasenia;
            IsAdmin = isAdmin;
            IsActiva = isActiva;
        }

        public static Usuario DesdeBaseDatos(string nombre, string apellidos, string email, string hash, bool isAdmin, bool isActiva)
        {
            return new Usuario
            {
                nombre = nombre,
                apellidos = apellidos,
                email = email,
                contrasenia = hash, // ⚠️ se asigna directamente al campo privado sin pasar por la propiedad
                isAdmin = isAdmin,
                isActiva = isActiva
            };
        }
        #endregion

        #region PROPIEDADES
        public string Nombre
        {
            get => nombre;
            set
            {
                nombre = ValidadorDatos.ValidarNombre(value); // Llamar a la validación
            }
        }

        public string Apellidos
        {
            get => apellidos;
            set
            {
                apellidos = ValidadorDatos.ValidarApellidos(value); // Llamar a la validación
            }
        }

        public string Email
        {
            get => email;
            set
            {
                email = ValidadorDatos.ValidarEmail(value); // Llamar a la validación
            }
        }

        public string Contrasenia
        {
            get => contrasenia;
            set
            {
                contrasenia = ValidadorDatos.ValidarContrasenia(value); // Llamar a la validación
            }
        }

        public bool IsAdmin
        {
            get => isAdmin;
            set => isAdmin = value;
        }

        public bool IsActiva
        {
            get => isActiva;
            set
            {
                if (isActiva != value)
                {
                    isActiva = value;
                    OnPropertyChanged(nameof(IsActiva));
                    OnPropertyChanged(nameof(TextoActivacion));
                    OnPropertyChanged(nameof(ColorActivacion));
                }
            }
        }

        // Propiedad calculada para el texto del label
        public string TextoActivacion => IsActiva ? "Desactivar" : "Activar";

        // Propiedad calculada para el color del switch
        public Color ColorActivacion => IsActiva ? Colors.Green : Colors.Red;
        #endregion

        #region METODOS

        /// <summary>
        /// Metodo para notificar a la UI que una propiedad ha cambiado
        /// </summary>
        /// <param name="nombre"></param>
        protected void OnPropertyChanged(string nombre)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
        #endregion

    }
}
