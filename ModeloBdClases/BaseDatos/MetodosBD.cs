using ModeloBdClases.Clases;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ModeloBdClases.BaseDatos
{
    public class MetodosBD
    {
        // Instancicación de la Api de Base de Datos, necesario para la conexion
        apiBD BdApi = new apiBD();

        #region ALERGIAS

        #region OBTENER TODAS LAS ALERGIAS
        public List<Alergia> ObtenerTodasLasAlergias()
        {
            List<Alergia> lista = new List<Alergia>(); // Creamos una lista vacía de alergias.

            using (var conexion = BdApi.ConexionBd()) // Establecemos la conexión con la base de datos.
            {
                conexion.Open(); // Abrimos la conexión.
                string query = "SELECT id, nombre FROM alergias"; // Consulta SQL para obtener todas las alergias.

                using (MySqlCommand cmd = new MySqlCommand(query, conexion)) // Preparamos el comando con la consulta.
                using (MySqlDataReader reader = cmd.ExecuteReader()) // Ejecutamos la consulta y leemos los resultados.
                {
                    while (reader.Read()) // Iteramos sobre cada fila devuelta por la consulta.
                    {
                        // Añadimos una nueva instancia de la clase Alergia con los datos de la fila.
                        lista.Add(new Alergia(
                            reader.GetInt32("id"), // Obtiene el ID de la alergia.
                            reader.GetString("nombre") // Obtiene el nombre de la alergia.
                        ));
                    }
                }

                conexion.Close(); // Cerramos la conexión.
            }

            return lista; // Devolvemos la lista de alergias.
        }
        #endregion

        #region AGREGAR ALERGIA A PACIENTE
        public bool AgregarAlergiaAPaciente(int pacienteId, int alergiaId)
        {
            using (var conexion = BdApi.ConexionBd()) // Establecemos la conexión.
            {
                conexion.Open(); // Abrimos la conexión.

                // Verifica si la relación ya existe para evitar duplicados.
                string checkQuery = "SELECT COUNT(*) FROM paciente_alergias WHERE paciente_id = @pacienteId AND alergia_id = @alergiaId";
                using (var checkCmd = new MySqlCommand(checkQuery, conexion))
                {
                    checkCmd.Parameters.AddWithValue("@pacienteId", pacienteId);
                    checkCmd.Parameters.AddWithValue("@alergiaId", alergiaId);

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar()); // Ejecuta la consulta y obtiene el conteo.
                    if (count > 0)
                        return false; // Si ya existe, devolvemos false.
                }

                // Si no existe la relación, la insertamos.
                string insertQuery = "INSERT INTO paciente_alergias (paciente_id, alergia_id) VALUES (@pacienteId, @alergiaId)";
                using (var insertCmd = new MySqlCommand(insertQuery, conexion))
                {
                    insertCmd.Parameters.AddWithValue("@pacienteId", pacienteId);
                    insertCmd.Parameters.AddWithValue("@alergiaId", alergiaId);

                    int filas = insertCmd.ExecuteNonQuery(); // Ejecutamos la inserción.
                    return filas > 0; // Si se insertaron filas, devolvemos true.
                }
            }
        }
        #endregion

        #region ELIMINAR ALERGIA DE PACIENTE
        public bool EliminarAlergiaDePaciente(int pacienteId, int alergiaId)
        {
            using (var conexion = BdApi.ConexionBd()) // Establecemos la conexión.
            {
                conexion.Open(); // Abrimos la conexión.

                string deleteQuery = "DELETE FROM paciente_alergias WHERE paciente_id = @pacienteId AND alergia_id = @alergiaId";
                using (var cmd = new MySqlCommand(deleteQuery, conexion)) // Preparamos el comando de eliminación.
                {
                    cmd.Parameters.AddWithValue("@pacienteId", pacienteId);
                    cmd.Parameters.AddWithValue("@alergiaId", alergiaId);

                    int filas = cmd.ExecuteNonQuery(); // Ejecutamos la eliminación.
                    return filas > 0; // Si se eliminaron filas, devolvemos true.
                }
            }
        }
        #endregion

        #region OBTENER ALERGIAS DE UN PACIENTE
        public List<Alergia> ObtenerAlergiasDePaciente(int pacienteId)
        {
            List<Alergia> lista = new List<Alergia>(); // Creamos una lista vacía para almacenar las alergias.

            using (var conexion = BdApi.ConexionBd()) // Establecemos la conexión.
            {
                conexion.Open(); // Abrimos la conexión.

                string query = @"SELECT a.id, a.nombre
                         FROM Alergias a
                         JOIN paciente_alergias pa ON a.id = pa.alergia_id
                         WHERE pa.paciente_id = @pacienteId"; // Consulta SQL con JOIN para obtener las alergias del paciente.

                using (var cmd = new MySqlCommand(query, conexion)) // Preparamos el comando con la consulta.
                {
                    cmd.Parameters.AddWithValue("@pacienteId", pacienteId); // Añadimos el parámetro pacienteId.

                    using (var reader = cmd.ExecuteReader()) // Ejecutamos la consulta y leemos los resultados.
                    {
                        while (reader.Read()) // Iteramos sobre cada fila devuelta.
                        {
                            lista.Add(new Alergia( // Añadimos las alergias a la lista.
                                reader.GetInt32("id"), // Obtiene el ID de la alergia.
                                reader.GetString("nombre") // Obtiene el nombre de la alergia.
                            ));
                        }
                    }
                }

                conexion.Close(); // Cerramos la conexión.
            }

            return lista; // Devolvemos la lista de alergias.
        }
        #endregion

        #endregion

        #region DIAGNOSTICO

        #region AGREGAR DIAGNOSTICO
        /// <summary>
        /// Agrega un nuevo diagnóstico a la base de datos
        /// </summary>
        /// <param name="nombre">Nombre del diagnóstico</param>
        /// <param name="descripcion">Descripción del diagnóstico</param>
        /// <param name="fechaDiagnostico">Fecha del diagnóstico</param>
        /// <param name="idPaciente">ID del paciente</param>
        /// <returns>Devuelve true si se agrega correctamente</returns>
        public bool AgregarDiagnostico(string nombre, string descripcion, DateTime fechaDiagnostico, int idPaciente)
        {
            try
            {
                // Establecemos la conexión a la base de datos
                using (MySqlConnection conexion = BdApi.ConexionBd())
                {
                    conexion.Open(); // Abrimos la conexión.
                    string query = "INSERT INTO Diagnosticos (paciente_id, nombre, descripcion, fecha_diagnostico) " +
                                   "VALUES (@pacienteId, @nombre, @descripcion, @fechaDiagnostico)"; // Consulta SQL para insertar un nuevo diagnóstico.

                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        // Agregamos los parámetros para evitar inyecciones SQL
                        cmd.Parameters.AddWithValue("@pacienteId", idPaciente);
                        cmd.Parameters.AddWithValue("@nombre", nombre.Trim()); // Eliminamos espacios en blanco al principio y final
                        cmd.Parameters.AddWithValue("@descripcion", descripcion?.Trim()); // Descripción, se maneja posible null
                        cmd.Parameters.AddWithValue("@fechaDiagnostico", fechaDiagnostico.Date); // Se almacena solo la fecha (sin hora)

                        // Ejecutamos la consulta y verificamos si se insertó correctamente
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0; // Si se afectaron filas, significa que el diagnóstico se agregó con éxito
                    }
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error, lo capturamos y mostramos el mensaje
                Console.WriteLine("Error al agregar diagnóstico: " + ex.Message);
                return false; // Devolvemos false si hubo un error
            }
        }
        #endregion

        #region OBTENER DIAGNOSTICOS DE UN PACIENTE
        /// <summary>
        /// Obtiene los diagnósticos de un paciente
        /// </summary>
        /// <param name="idPaciente"></param>
        /// <returns>Lista de diagnósticos del paciente</returns>
        public List<Diagnostico> ObtenerDiagnosticosPaciente(int idPaciente)
        {
            List<Diagnostico> diagnosticos = new List<Diagnostico>(); // Creamos una lista vacía para los diagnósticos.

            using (MySqlConnection conexion = BdApi.ConexionBd()) // Establecemos la conexión con la base de datos.
            {
                conexion.Open(); // Abrimos la conexión.

                string query = "SELECT id, nombre, descripcion, fecha_diagnostico FROM Diagnosticos WHERE paciente_id = @idPaciente"; // Consulta SQL para obtener los diagnósticos.

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@idPaciente", idPaciente); // Añadimos el parámetro paciente_id.

                    using (MySqlDataReader lector = cmd.ExecuteReader()) // Ejecutamos la consulta y leemos los resultados.
                    {
                        while (lector.Read()) // Iteramos sobre cada fila devuelta por la consulta.
                        {
                            diagnosticos.Add(new Diagnostico( // Añadimos los diagnósticos a la lista.
                                lector.GetInt32("id"),
                                lector.GetString("nombre"),
                                lector.GetString("descripcion"),
                                lector.GetDateTime("fecha_diagnostico")
                            ));
                        }
                    }
                }
            }

            return diagnosticos; // Devolvemos la lista de diagnósticos.
        }
        #endregion

        #region ACTUALIZAR DIAGNOSTICO
        public bool ActualizarDiagnostico(int idDiagnostico, string nuevoNombre, string nuevaDescripcion, DateTime nuevaFecha)
        {
            try
            {
                using (MySqlConnection conexion = BdApi.ConexionBd()) // Establecemos la conexión con la base de datos.
                {
                    conexion.Open(); // Abrimos la conexión.

                    // Consulta SQL para actualizar un diagnóstico.
                    string query = "UPDATE Diagnosticos SET nombre = @nombre, descripcion = @descripcion, fecha_diagnostico = @fecha WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nuevoNombre); // Parametro nombre
                        cmd.Parameters.AddWithValue("@descripcion", nuevaDescripcion); // Parametro descripción
                        cmd.Parameters.AddWithValue("@fecha", nuevaFecha); // Parametro fecha del diagnóstico
                        cmd.Parameters.AddWithValue("@id", idDiagnostico); // Parametro id del diagnóstico a actualizar

                        int filasAfectadas = cmd.ExecuteNonQuery(); // Ejecutamos la actualización.
                        return filasAfectadas > 0; // Si se afectaron filas, devolvemos true.
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar diagnóstico: " + ex.Message); // Capturamos y mostramos el error.
                return false; // Devolvemos false si hubo un error.
            }
        }
        #endregion

        #region ELIMINAR DIAGNOSTICO
        /// <summary>
        /// Elimina un diagnóstico de la base de datos y todos sus tratamientos asociados (por CASCADE)
        /// </summary>
        /// <param name="idDiagnostico">ID del diagnóstico a eliminar</param>
        /// <returns>True si se eliminó correctamente</returns>
        public bool EliminarDiagnostico(int idDiagnostico)
        {
            try
            {
                using (MySqlConnection conexion = BdApi.ConexionBd()) // Establecemos la conexión con la base de datos.
                {
                    conexion.Open(); // Abrimos la conexión.

                    // Consulta SQL para eliminar un diagnóstico. Los tratamientos asociados se eliminan automáticamente por CASCADE.
                    string query = "DELETE FROM Diagnosticos WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", idDiagnostico); // Añadimos el parámetro id.

                        int filasAfectadas = cmd.ExecuteNonQuery(); // Ejecutamos la eliminación.
                        return filasAfectadas > 0; // Si se eliminó, devolvemos true.
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar diagnóstico (ID: {idDiagnostico}): {ex.Message}"); // Mostramos el error si ocurre.
                return false; // Devolvemos false si hubo un error.
            }
        }
        #endregion

        #endregion

        #region PACIENTE

        #region AGREGAR PACIENTE
        /// <summary>
        /// Agrega un nuevo paciente a la base de datos
        /// </summary>
        /// <param name="nombre">Nombre del paciente</param>
        /// <param name="apellidos">Apellidos del paciente</param>
        /// <param name="edad">Edad del paciente</param>
        /// <param name="dni">DNI del paciente</param>
        /// <param name="grupoSanguineo">Grupo sanguíneo del paciente</param>
        /// <param name="fechaIngreso">Fecha de ingreso del paciente</param>
        /// <param name="usuarioId">ID del usuario asociado</param>
        /// <returns>Devuelve true si se agrega correctamente</returns>
        public bool AgregarPaciente(string nombre, string apellidos, int edad, string dni, string grupoSanguineo, DateTime fechaIngreso, int usuarioId)
        {
            try
            {
                // Validar y normalizar con la clase Paciente
                var paciente = new Paciente(nombre, apellidos, edad,
                    // Limpiamos guión para pasarlo al constructor/validaciones
                    dni.Replace("-", ""),
                    grupoSanguineo, fechaIngreso);

                // Extraer valores ya validados
                string dniValidado = paciente.Dni; // será "00000000X"

                using (MySqlConnection conexion = BdApi.ConexionBd())
                {
                    conexion.Open();
                    const string query = 
                        @"INSERT INTO Pacientes 
                        (nombre, apellidos, edad, dni, grupo_sanguineo, fecha_ingreso, usuario_id)
                        VALUES 
                        (@nombre, @apellidos, @edad, @dni, @grupoSanguineo, @fechaIngreso, @usuarioId)";

                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@nombre", paciente.Nombre);
                        cmd.Parameters.AddWithValue("@apellidos", paciente.Apellidos);
                        cmd.Parameters.AddWithValue("@edad", paciente.Edad);
                        cmd.Parameters.AddWithValue("@dni", dniValidado);
                        cmd.Parameters.AddWithValue("@grupoSanguineo", paciente.GrupoSanguineo);
                        cmd.Parameters.AddWithValue("@fechaIngreso", paciente.FechaIngreso);
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar paciente: " + ex.Message);
                return false;
            }
        }
        #endregion

        #region OBTENER ID PACIENTE
        /// <summary>
        /// Obtiene el ID de un paciente según su DNI
        /// </summary>
        /// <param name="dni">DNI del paciente</param>
        /// <returns>Devuelve el ID del paciente</returns>
        public int ObtenerIdPaciente(string dni)
        {
            // Declaramos la variable para almacenar el ID
            int idPaciente = 0;

            // Establecemos la conexión a la base de datos
            using (MySqlConnection conexion = BdApi.ConexionBd())
            {
                conexion.Open(); // Abrimos la conexión.

                // Definimos la consulta SQL
                string query = "SELECT id FROM Pacientes WHERE dni = @dni";
                MySqlCommand cmd = new MySqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@dni", dni); // Añadimos el parámetro DNI.

                // Ejecutamos la consulta
                MySqlDataReader lector = cmd.ExecuteReader();

                if (lector.Read()) // Si hay resultados, obtenemos el ID
                {
                    idPaciente = lector.GetInt32(0); // Asignamos el ID del paciente.
                }

                lector.Close(); // Cerramos el lector.

            }
            return idPaciente; // Devolvemos el ID obtenido.
        }
        #endregion

        #region BORRAR PACIENTE
        /// <summary>
        /// Borra un paciente de la base de datos
        /// </summary>
        /// <param name="idPaciente">ID del paciente</param>
        /// <returns>Devuelve true si se borra correctamente</returns>
        public bool BorrarPaciente(int idPaciente)
        {
            try
            {
                // Establecemos la conexión a la base de datos
                using (MySqlConnection conexion = BdApi.ConexionBd())
                {
                    conexion.Open(); // Abrimos la conexión.

                    // Consulta SQL para eliminar un paciente
                    string query = "DELETE FROM Pacientes WHERE id = @idPaciente";

                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@idPaciente", idPaciente); // Añadimos el parámetro ID.

                        // Ejecutamos la consulta y verificamos si se eliminó correctamente
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0; // Si se afectaron filas, devolvemos true (se eliminó correctamente).
                    }
                }
            }
            catch (Exception ex)
            {
                // Capturamos y mostramos el error en consola
                Console.WriteLine("Error al borrar paciente: " + ex.Message);
                return false; // Devolvemos false si hubo un error.
            }
        }
        #endregion

        #region OBTENER PACIENTES DE USUARIO
        /// <summary>
        /// Obtiene una lista de pacientes asociados a un usuario
        /// </summary>
        /// <param name="idUsuario">ID del usuario</param>
        /// <returns>Lista de pacientes</returns>
        public List<Paciente> ObtenerPacientesUsuario(int idUsuario)
        {
            List<Paciente> pacientes = new List<Paciente>(); // Creamos una lista vacía para almacenar los pacientes.

            using (MySqlConnection conexion = BdApi.ConexionBd())
            {
                MySqlDataReader lectorBd = null; // Declaramos el lector para los datos.
                MySqlCommand consulta = new MySqlCommand("SELECT * FROM Pacientes WHERE usuario_id = @idUsuario", conexion); // Consulta SQL para obtener los pacientes.
                consulta.Parameters.AddWithValue("@idUsuario", idUsuario); // Añadimos el parámetro ID del usuario.

                conexion.Open(); // Abrimos la conexión.
                lectorBd = consulta.ExecuteReader(); // Ejecutamos la consulta y leemos los resultados.

                // Iteramos sobre los resultados obtenidos.
                while (lectorBd.Read())
                {
                    pacientes.Add(new Paciente( // Agregamos cada paciente a la lista.
                        lectorBd.GetString(1), // Nombre
                        lectorBd.GetString(2), // Apellidos
                        lectorBd.GetInt32(3),  // Edad
                        lectorBd.GetString(4), // DNI
                        lectorBd.GetString(5), // Grupo sanguíneo
                        lectorBd.GetDateTime(6) // Fecha de ingreso
                    ));
                }

                lectorBd.Close(); // Cerramos el lector.
            }

            return pacientes; // Devolvemos la lista de pacientes.
        }
        #endregion

        #endregion

        #region USUARIO

        #region OBTENER UN USUARIO
        /// <summary>
        /// Obtiene un usuario específico basado en sus credenciales de email y contraseña.
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="contrasenia">Contraseña del usuario</param>
        /// <returns>Objeto Usuario si las credenciales son correctas</returns>
        /// <exception cref="Exception">Se lanza si los campos están vacíos o las credenciales son inválidas</exception>
        public Usuario ObtenerUsuario(string email, string contrasenia)
        {
            // Validamos que el email no esté vacío
            if (string.IsNullOrWhiteSpace(email)) throw new Exception("El email no puede estar vacío.");

            // Validamos que la contraseña no esté vacía
            if (string.IsNullOrWhiteSpace(contrasenia)) throw new Exception("La contraseña no puede estar vacía.");

            Usuario user = null; // Inicializamos el objeto usuario

            // Abrimos la conexión a la base de datos
            using (MySqlConnection conexion = BdApi.ConexionBd())
            {
                conexion.Open(); // Abrimos conexión

                // Creamos la consulta SQL con parámetros
                MySqlCommand consulta = new MySqlCommand("SELECT * FROM Usuarios WHERE email = @Email AND contrasenia = @Contrasenia", conexion);
                consulta.Parameters.AddWithValue("@Email", email); // Asignamos el valor del email
                consulta.Parameters.AddWithValue("@Contrasenia", contrasenia); // Asignamos el valor de la contraseña

                // Ejecutamos la consulta y leemos los resultados
                using (MySqlDataReader lectorBd = consulta.ExecuteReader())
                {
                    // Si hay resultados, creamos el objeto usuario con los datos
                    if (lectorBd.Read())
                    {
                        user = new Usuario(
                            lectorBd.GetString(1),  // nombre
                            lectorBd.GetString(2),  // apellidos
                            lectorBd.GetString(3),  // email
                            lectorBd.GetString(4),  // contrasenia
                            lectorBd.GetBoolean(5),  // isAdmin
                            lectorBd.GetBoolean(6)

                        );
                    }
                }

                conexion.Close(); // Cerramos la conexión
            }

            // Si no se encontró un usuario, lanzamos una excepción
            if (user == null)
                throw new Exception("Usuario no encontrado o credenciales incorrectas.");

            return user; // Devolvemos el usuario
        }
        #endregion

        #region OBTENER TODOS USUARIOS
        /// <summary>
        /// Obtiene una lista de todos los usuarios no administradores del sistema.
        /// </summary>
        /// <returns>Lista de objetos Usuario</returns>
        public List<Usuario> ObtenerUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>(); // Creamos la lista que devolveremos

            // Abrimos la conexión a la base de datos
            using (MySqlConnection conexion = BdApi.ConexionBd())
            {
                conexion.Open(); // Abrimos conexión

                // Consulta para seleccionar solo usuarios que no son administradores
                MySqlCommand consulta = new MySqlCommand("SELECT * FROM Usuarios WHERE isAdmin = 0", conexion);

                // Ejecutamos la consulta y leemos los resultados
                using (MySqlDataReader lectorBd = consulta.ExecuteReader())
                {
                    while (lectorBd.Read())
                    {
                        // Por cada usuario encontrado, lo añadimos a la lista
                        usuarios.Add(new Usuario(
                            lectorBd.GetString(1),  // nombre
                            lectorBd.GetString(2),  // apellidos
                            lectorBd.GetString(3),  // email
                            lectorBd.GetString(4),  // contrasenia
                            lectorBd.GetBoolean(5),  // isAdmin
                            lectorBd.GetBoolean(6)

                        ));
                    }
                }

                // La conexión se cierra automáticamente por el using
            }

            return usuarios; // Devolvemos la lista de usuarios
        }
        #endregion

        #region OBTENER TODOS USUARIOS ADMIN
        /// <summary>
        /// Obtiene una lista de todos los usuarios no administradores del sistema.
        /// </summary>
        /// <returns>Lista de objetos Usuario</returns>
        public List<Usuario> ObtenerUsuariosAdmin()
        {
            List<Usuario> usuarios = new List<Usuario>(); // Creamos la lista que devolveremos

            // Abrimos la conexión a la base de datos
            using (MySqlConnection conexion = BdApi.ConexionBd())
            {
                conexion.Open(); // Abrimos conexión

                // Consulta para seleccionar solo usuarios que no son administradores
                MySqlCommand consulta = new MySqlCommand("SELECT * FROM Usuarios WHERE isAdmin = 1", conexion);

                // Ejecutamos la consulta y leemos los resultados
                using (MySqlDataReader lectorBd = consulta.ExecuteReader())
                {
                    while (lectorBd.Read())
                    {
                        // Por cada usuario encontrado, lo añadimos a la lista
                        usuarios.Add(new Usuario(
                            lectorBd.GetString(1),  // nombre
                            lectorBd.GetString(2),  // apellidos
                            lectorBd.GetString(3),  // email
                            lectorBd.GetString(4),  // contrasenia
                            lectorBd.GetBoolean(5),  // isAdmin
                            lectorBd.GetBoolean(6)


                        ));
                    }
                }

                // La conexión se cierra automáticamente por el using
            }

            return usuarios; // Devolvemos la lista de usuarios
        }
        #endregion

        #region OBTENER ID USUARIO
        /// <summary>
        /// Obtiene el ID de un usuario basado en su email.
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <returns>ID del usuario</returns>
        /// <exception cref="Exception">Se lanza si el email está vacío o no se encuentra el usuario</exception>
        public int ObtenerIdUsuario(string email)
        {
            // Validamos que el email no esté vacío
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("El email no puede estar vacío.");

            int idUsuario = 0; // Variable para almacenar el ID

            // Abrimos la conexión a la base de datos
            using (MySqlConnection conexion = BdApi.ConexionBd())
            {
                conexion.Open(); // Abrimos conexión

                // Consulta para obtener el ID usando el email
                string query = "SELECT id FROM Usuarios WHERE email = @correo";
                MySqlCommand cmd = new MySqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@correo", email); // Asignamos el parámetro del email

                // Ejecutamos y leemos el resultado
                using (MySqlDataReader lector = cmd.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        idUsuario = lector.GetInt32(0); // Obtenemos el ID
                    }
                }

                conexion.Close(); // Cerramos la conexión
            }

            // Si no se encontró el ID, lanzamos excepción
            if (idUsuario == 0)
                throw new Exception("No se encontró un usuario con ese email.");

            return idUsuario; // Devolvemos el ID
        }
        #endregion

        #region AGREGAR USUARIO
        /// <summary>
        /// Agrega un nuevo usuario a la base de datos.
        /// </summary>
        /// <param name="nombre">Nombre del usuario</param>
        /// <param name="apellidos">Apellidos del usuario</param>
        /// <param name="correo">Email del usuario</param>
        /// <param name="contrasenia">Contraseña del usuario</param>
        /// <param name="isAdmin">Si es administrador o no</param>
        /// <returns>True si se agregó correctamente</returns>
        /// <exception cref="Exception">Se lanza si hay campos vacíos o un error en la inserción</exception>
        public bool AgregarUsuario(string nombre, string apellidos, string correo, string contrasenia, bool isAdmin)
        {
            try
            {
                // Abrimos la conexión a la base de datos
                using (MySqlConnection conexion = BdApi.ConexionBd())
                {
                    conexion.Open(); // Abrimos conexión

                    // Consulta de inserción SQL
                    string query = "INSERT INTO Usuarios (nombre, apellidos, email, contrasenia, isAdmin) " +
                                   "VALUES (@nombre, @apellidos, @correo, @contrasenia, @isAdmin)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        // Asignamos valores a los parámetros de la consulta
                        cmd.Parameters.AddWithValue("@nombre", nombre.Trim());
                        cmd.Parameters.AddWithValue("@apellidos", apellidos.Trim());
                        cmd.Parameters.AddWithValue("@correo", correo.Trim().ToLower());
                        cmd.Parameters.AddWithValue("@contrasenia", contrasenia);
                        cmd.Parameters.AddWithValue("@isAdmin", isAdmin);

                        int filasAfectadas = cmd.ExecuteNonQuery(); // Ejecutamos la consulta

                        // Verificamos si se insertó correctamente
                        if (filasAfectadas == 0)
                            throw new Exception("No se pudo agregar el usuario.");

                        return true; // Operación exitosa
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // Error por entrada duplicada (correo ya existe)
                    throw new Exception("El correo ya está registrado. Usa otro diferente.");
                else
                    throw new Exception("Error en la base de datos: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Capturamos errores y lanzamos excepción con mensaje detallado
                throw new Exception("Error al agregar usuario: " + ex.Message);
            }
        }
        #endregion

        #region BORRAR UN USUARIO
        /// <summary>
        /// Elimina un usuario de la base de datos por su ID.
        /// </summary>
        /// <param name="idUsuario">ID del usuario</param>
        /// <returns>True si se eliminó correctamente</returns>
        /// <exception cref="Exception">Se lanza si el ID es inválido o hay errores en la eliminación</exception>
        public bool BorrarUsuario(int idUsuario)
        {
            // Validamos que el ID sea mayor que 0
            if (idUsuario <= 0)
                throw new Exception("ID de usuario inválido.");

            try
            {
                // Abrimos la conexión a la base de datos
                using (MySqlConnection conexion = BdApi.ConexionBd())
                {
                    conexion.Open(); // Abrimos conexión

                    // Consulta para eliminar el usuario por ID
                    string query = "DELETE FROM Usuarios WHERE id = @idUsuario";

                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@idUsuario", idUsuario); // Asignamos el parámetro

                        int filasAfectadas = cmd.ExecuteNonQuery(); // Ejecutamos la consulta

                        // Verificamos si se eliminó algún registro
                        if (filasAfectadas == 0)
                            throw new Exception("No se encontró un usuario con ese ID.");

                        return true; // Operación exitosa
                    }
                }
            }
            catch (Exception ex)
            {
                // Capturamos y lanzamos cualquier error ocurrido
                throw new Exception("Error al borrar usuario: " + ex.Message);
            }
        }
        #endregion

        #region ACTUALIZAR ESTADO
        /// <summary>
        /// Método para actualizar el estado 'isActiva' del usuario en la base de datos
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns>True si la actualización fue exitosa</returns>
        public async Task<bool> ActualizarEstadoCuenta(Usuario usuario)
        {
            // Validamos que el usuario no sea nulo y que tenga un valor válido para IsActiva
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Email))
                throw new Exception("El usuario o el email no son válidos.");

            try
            {
                // Abrimos la conexión a la base de datos
                using (MySqlConnection conexion = BdApi.ConexionBd())
                {
                    conexion.Open(); // Abrimos conexión

                    // Consulta de actualización SQL
                    string query = "UPDATE Usuarios SET isActiva = @isActiva WHERE email = @correo";

                    // Creamos el comando y agregamos parámetros
                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@isActiva", usuario.IsActiva);  // El valor de isActiva
                        cmd.Parameters.AddWithValue("@correo", usuario.Email.Trim().ToLower());  // El email del usuario

                        int filasAfectadas = cmd.ExecuteNonQuery(); // Ejecutamos la consulta

                        // Verificamos si se actualizó correctamente
                        if (filasAfectadas == 0)
                            throw new Exception("No se encontró el usuario o no se pudo actualizar el estado.");

                        return true; // Actualización exitosa
                    }
                }
            }
            catch (Exception ex)
            {
                // Capturamos cualquier error y lanzamos una excepción
                throw new Exception("Error al actualizar el estado de la cuenta: " + ex.Message);
            }
        }
        #endregion

        #endregion

        #region TRATAMIENTO

        #region GUARDAR UN TRATAMIENTO
        /// <summary>
        /// Guarda un nuevo tratamiento asociado a un diagnóstico.
        /// </summary>
        /// <param name="tratamiento">Objeto Tratamiento con los datos del nuevo tratamiento</param>
        /// <param name="diagnosticoId">ID del diagnóstico al cual se vinculará el tratamiento</param>
        /// <returns>ID del nuevo tratamiento insertado en la base de datos</returns>
        /// <remarks>
        /// Se utiliza una consulta SQL para insertar los datos y se obtiene el último ID insertado.
        /// El método encapsula la lógica dentro de un bloque using para asegurar el cierre de la conexión.
        /// </remarks>
        public int GuardarTratamiento(Tratamiento tratamiento, int diagnosticoId)
        {
            int nuevoId = -1;

            using (var connection = BdApi.ConexionBd())
            {
                connection.Open();

                string query = "INSERT INTO tratamientos (nombre, fecha_inicio, fecha_fin, diagnostico_id) " +
                               "VALUES (@nombre, @inicio, @fin, @diagId); SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(query, connection))
                {
                    // Se agregan los parámetros a la consulta
                    command.Parameters.AddWithValue("@nombre", tratamiento.Nombre);
                    command.Parameters.AddWithValue("@inicio", tratamiento.FechaInicio);
                    command.Parameters.AddWithValue("@fin", tratamiento.FechaFin);
                    command.Parameters.AddWithValue("@diagId", diagnosticoId);

                    // Ejecuta la consulta y recupera el ID generado
                    nuevoId = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            return nuevoId;
        }
        #endregion

        #region OBTENER TRATAMIENTOS POR DIAGNÓSTICO
        /// <summary>
        /// Obtiene todos los tratamientos asociados a un diagnóstico específico.
        /// </summary>
        /// <param name="idDiagnostico">ID del diagnóstico del cual se desean obtener los tratamientos</param>
        /// <returns>Lista de objetos Tratamiento correspondientes al diagnóstico</returns>
        /// <remarks>
        /// El método realiza una consulta SQL y convierte cada fila del resultado en un objeto Tratamiento.
        /// Se maneja el valor nulo de fecha_fin correctamente.
        /// </remarks>
        public List<Tratamiento> ObtenerTratamientosPorDiagnostico(int idDiagnostico)
        {
            List<Tratamiento> tratamientos = new List<Tratamiento>();

            using (var connection = BdApi.ConexionBd())
            {
                connection.Open();

                string query = "SELECT id, nombre, fecha_inicio, fecha_fin FROM tratamientos WHERE diagnostico_id = @idDiagnostico";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idDiagnostico", idDiagnostico);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idTratamiento = reader.GetInt32("id");
                            string nombre = reader.GetString("nombre");
                            DateTime fechaInicio = reader.GetDateTime("fecha_inicio");
                            DateTime? fechaFin = reader.IsDBNull(reader.GetOrdinal("fecha_fin"))
                                ? (DateTime?)null
                                : reader.GetDateTime("fecha_fin");

                            // Se crea un nuevo objeto Tratamiento y se añade a la lista
                            tratamientos.Add(new Tratamiento(idTratamiento, nombre, fechaInicio, fechaFin));
                        }
                    }
                }
            }

            return tratamientos;
        }
        #endregion

        #region ELIMINAR TRATAMIENTO
        /// <summary>
        /// Elimina un tratamiento de la base de datos basado en su ID.
        /// </summary>
        /// <param name="idTratamiento">ID del tratamiento a eliminar</param>
        /// <returns>True si el tratamiento fue eliminado exitosamente, false si no se encontró o no se pudo eliminar</returns>
        /// <remarks>
        /// Primero se verifica si el tratamiento existe, y si es así, se procede a eliminarlo.
        /// Se manejan errores mediante try-catch y se escriben en consola en caso de fallo.
        /// </remarks>
        public bool EliminarTratamientoPorId(int idTratamiento)
        {
            try
            {
                using (MySqlConnection conexion = BdApi.ConexionBd())
                {
                    conexion.Open();

                    // Verificamos si el tratamiento existe antes de eliminar
                    string checkQuery = "SELECT COUNT(*) FROM tratamientos WHERE id = @idTratamiento";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conexion))
                    {
                        checkCmd.Parameters.AddWithValue("@idTratamiento", idTratamiento);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count == 0)
                        {
                            Console.WriteLine("Tratamiento no encontrado.");
                            return false;
                        }
                    }

                    // Eliminamos el tratamiento si existe
                    string deleteQuery = "DELETE FROM tratamientos WHERE id = @idTratamiento";
                    using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conexion))
                    {
                        cmd.Parameters.AddWithValue("@idTratamiento", idTratamiento);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar tratamiento: {ex.Message}");
                return false;
            }
        }
        #endregion

        #endregion

        #region TRATAMIENTO/MEDICAMENTO

        #region OBTENER TRATAMIENTO MEDICAMENTOS
        /// <summary>
        /// Obtener los medicamentos asociados a un tratamiento específico
        /// </summary>
        /// <param name="idTratamiento">ID del tratamiento</param>
        /// <returns>Lista de medicamentos asociados al tratamiento con sus instrucciones</returns>
        public List<TratamientoMedicamento> ObtenerTratamientoMedicamentos(int idTratamiento)
        {
            List<TratamientoMedicamento> lista = new List<TratamientoMedicamento>();

            using (MySqlConnection conexion = BdApi.ConexionBd())
            {
                conexion.Open();

                // Consulta con JOIN para traer medicamentos e instrucciones por tratamiento
                string query = @"SELECT tm.instrucciones, 
                                m.id AS medicamento_id, 
                                m.nombre AS medicamento_nombre, 
                                m.descripcion AS medicamento_descripcion
                         FROM tratamiento_medicamentos tm
                         JOIN Medicamentos m ON tm.medicamento_id = m.id
                         WHERE tm.tratamiento_id = @idTratamiento";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@idTratamiento", idTratamiento);

                    using (MySqlDataReader lector = cmd.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            // Crear objeto Medicamento desde los datos leídos
                            Medicamento medicamento = new Medicamento(
                                lector.GetInt32("medicamento_id"),
                                lector.GetString("medicamento_nombre"),
                                lector.GetString("medicamento_descripcion")
                            );

                            // Crear objeto TratamientoMedicamento con el medicamento y las instrucciones
                            TratamientoMedicamento tm = new TratamientoMedicamento(
                                medicamento,
                                lector.GetString("instrucciones")
                            );

                            // Añadir a la lista
                            lista.Add(tm);
                        }
                    }
                }
            }

            return lista;
        }
        #endregion

        #endregion

        #region EVOLUCIÓN

        #region GUARDAR UNA NOTA DE EVOLUCION
        /// <summary>
        /// Guarda una nueva nota de evolución asociada a un paciente
        /// </summary>
        /// <param name="id">ID del paciente</param>
        /// <param name="nota">Objeto NotaEvolucion con fecha y contenido</param>
        /// <returns>True si se guardó correctamente, false si no</returns>
        public async Task<int> GuardarNotaEvolucionPorId(int id, NotaEvolucion nota)
        {
            if (id <= 0) return 0;

            using (MySqlConnection conexion = BdApi.ConexionBd())
            {
                await conexion.OpenAsync();

                var query = "INSERT INTO Notas_Evolucion (paciente_id, fecha, contenido) VALUES (@pacienteId, @fecha, @contenido)";
                using (var cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@pacienteId", id);
                    cmd.Parameters.AddWithValue("@fecha", nota.Fecha);
                    cmd.Parameters.AddWithValue("@contenido", nota.Contenido);

                    int filasAfectadas = await cmd.ExecuteNonQueryAsync();

                    if (filasAfectadas > 0)
                    {
                        return (int)cmd.LastInsertedId; // <<<<< devuelve el ID generado
                    }
                    return 0;
                }
            }
        }
        #endregion

        #region OBTENER NOTAS DE EVOLUCION DE UN PACIENTE
        /// <summary>
        /// Obtiene todas las notas de evolución registradas para un paciente
        /// </summary>
        /// <param name="idPaciente">ID del paciente</param>
        /// <returns>Lista de notas con su fecha y contenido</returns>
        public async Task<List<NotaEvolucion>> ObtenerNotasEvolucionPaciente(int idPaciente)
        {
            // Inicializamos una lista para almacenar las notas de evolución del paciente
            List<NotaEvolucion> notasEvolucion = new();

            // Establecemos la conexión con la base de datos
            using (MySqlConnection conexion = BdApi.ConexionBd())
            {
                // Abrimos la conexión de manera asíncrona
                await conexion.OpenAsync();

                // Definimos la consulta SQL para obtener las notas de evolución del paciente
                string query = "SELECT id, fecha, contenido FROM Notas_Evolucion WHERE paciente_id = @idPaciente";

                // Creamos el comando SQL con la consulta y la conexión
                using (var cmd = new MySqlCommand(query, conexion))
                {
                    // Agregamos el parámetro necesario (ID del paciente) para la consulta SQL
                    cmd.Parameters.AddWithValue("@idPaciente", idPaciente);

                    // Ejecutamos la consulta y obtenemos un lector para leer los resultados
                    using (var lector = await cmd.ExecuteReaderAsync())
                    {
                        // Mientras haya filas en los resultados, las leemos
                        while (await lector.ReadAsync())
                        {
                            // Creamos un objeto 'NotaEvolucion' con los datos obtenidos (ID, fecha y contenido)
                            var nota = new NotaEvolucion(
                                lector.GetInt32("id"), // ID de la nota
                                lector.GetDateTime("fecha"), // Fecha de la nota
                                lector.GetString("contenido") // Contenido de la nota
                            );

                            // Agregamos la nota a la lista
                            notasEvolucion.Add(nota);
                        }
                    }
                }
            }

            // Retornamos la lista de notas de evolución
            return notasEvolucion;
        }
        #endregion

        #region ELIMINAR NOTA DE EVOLUCION
        /// <summary>
        /// Elimina una nota de evolución por su ID
        /// </summary>
        /// <param name="idNota"> ID de la nota de evolución</param>
        /// <returns> True si se elimina correctamente, false si no</returns>
        public async Task<bool> EliminarNotaEvolucion(int idNota)
        {
            // Establecemos la conexión con la base de datos
            using (MySqlConnection conexion = BdApi.ConexionBd())
            {
                // Abrimos la conexión de manera asíncrona
                await conexion.OpenAsync();

                // Definimos la consulta SQL para eliminar la nota de evolución por su ID
                string query = "DELETE FROM notas_evolucion WHERE id = @idNota";

                // Creamos el comando SQL con la consulta y la conexión
                using (var cmd = new MySqlCommand(query, conexion))
                {
                    // Agregamos el parámetro necesario (ID de la nota) para la consulta SQL
                    cmd.Parameters.AddWithValue("@idNota", idNota);

                    // Ejecutamos la consulta de eliminación y obtenemos el número de filas afectadas
                    int filasAfectadas = await cmd.ExecuteNonQueryAsync();

                    // Si se eliminaron filas correctamente (filasAfectadas > 0), retornamos true, caso contrario, false
                    return filasAfectadas > 0;
                }
            }
        }
        #endregion

        #endregion

        #region MEDICAMENTO

        #region OBTENER MEDICAMENTOS
        /// <summary>
        /// Recupera todos los medicamentos de la base de datos
        /// </summary>
        /// <returns>Lista de medicamentos (con ID, nombre y descripción)</returns>
        public List<Medicamento> ObtenerMedicamentos()
        {
            List<Medicamento> lista = new List<Medicamento>();

            using (MySqlConnection conn = BdApi.ConexionBd())
            {
                conn.Open();
                string query = "SELECT id, nombre, descripcion FROM medicamentos";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id");
                            string nombre = reader.GetString("nombre");
                            string descripcion = reader.GetString("descripcion");

                            Medicamento medicamento = new Medicamento(nombre, descripcion);
                            medicamento.Id = id; // Asigna el ID leído
                            lista.Add(medicamento);
                        }
                    }
                }
            }

            return lista;
        }
        #endregion

        #region OBTENER ID MEDICAMENTO POR NOMBRE
        /// <summary>
        /// Obtiene el ID de un medicamento buscándolo por nombre
        /// </summary>
        /// <param name="nombre">Nombre del medicamento</param>
        /// <returns>ID del medicamento</returns>
        /// <exception cref="Exception">Si no se encuentra el medicamento</exception>
        public int ObtenerIdMedicamentoPorNombre(string nombre)
        {
            int idMedicamento = -1;
            string query = "SELECT Id FROM Medicamentos WHERE Nombre = @nombre LIMIT 1";

            using (MySqlConnection conn = BdApi.ConexionBd())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idMedicamento = reader.GetInt32("Id");
                        }
                    }
                }
            }

            // Si no se encuentra, lanza una excepción
            if (idMedicamento == -1)
                throw new Exception($"No se encontró el medicamento con nombre '{nombre}'.");

            return idMedicamento;
        }
        #endregion

        #region GUARDAR TRATAMIENTO MEDICAMENTO
        /// <summary>
        /// Asocia un medicamento a un tratamiento con instrucciones específicas
        /// </summary>
        /// <param name="idTratamiento">ID del tratamiento</param>
        /// <param name="idMedicamento">ID del medicamento</param>
        /// <param name="instrucciones">Instrucciones de uso</param>
        public void GuardarTratamientoMedicamento(int idTratamiento, int idMedicamento, string instrucciones)
        {
            string query = "INSERT INTO tratamiento_medicamentos (tratamiento_id, medicamento_id, instrucciones) VALUES (@idTratamiento, @idMedicamento, @instrucciones)";

            using (MySqlConnection conn = BdApi.ConexionBd())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idTratamiento", idTratamiento);
                    cmd.Parameters.AddWithValue("@idMedicamento", idMedicamento);
                    cmd.Parameters.AddWithValue("@instrucciones", instrucciones);

                    cmd.ExecuteNonQuery(); // Ejecuta sin necesidad de leer resultado
                }
            }
        }
        #endregion

        #endregion

        #region UBICACION

        #region OBTENER UBICACION DE PACIENTE
        /// <summary>
        /// Obtiene la ubicación (planta, habitación y cama) de un paciente a partir de su DNI.
        /// </summary>
        /// <param name="dni">DNI del paciente</param>
        /// <returns>Objeto Ubicacion con los datos si existe, o null si no se encuentra</returns>
        public Ubicacion ObtenerUbicacionPorDni(string dni)
        {
            using (MySqlConnection conexion = BdApi.ConexionBd())
            {
                conexion.Open();

                // Consulta con JOIN para obtener la ubicación vinculada al paciente por su DNI
                var query = @"SELECT u.planta, u.habitacion, u.cama 
                      FROM Ubicaciones u
                      JOIN Pacientes p ON u.paciente_id = p.id
                      WHERE p.dni = @dni";

                using (var cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@dni", dni);

                    using (var lector = cmd.ExecuteReader())
                    {
                        // Si hay resultado, se construye y devuelve la ubicación
                        if (lector.Read())
                        {
                            return new Ubicacion(
                                lector.GetInt32("planta"),
                                lector.GetInt32("habitacion"),
                                lector.GetInt32("cama")
                            );
                        }
                    }
                }
            }

            // Si no hay ubicación registrada, retorna null
            return null;
        }
        #endregion

        #region GUARDAR UBICACION DE PACIENTE
        /// <summary>
        /// Guarda o actualiza la ubicación de un paciente identificado por su DNI.
        /// </summary>
        /// <param name="dni">DNI del paciente</param>
        /// <param name="ubicacion">Objeto Ubicacion con planta, habitación y cama</param>
        /// <returns>True si se guardó o actualizó correctamente, false si no</returns>
        public bool GuardarUbicacionPorDni(string dni, Ubicacion ubicacion)
        {
            using (MySqlConnection conexion = BdApi.ConexionBd())
            {
                conexion.Open();

                // Obtener el ID del paciente a partir del DNI
                var pacienteId = new MySqlCommand("SELECT id FROM Pacientes WHERE dni = @dni", conexion)
                {
                    Parameters = { new MySqlParameter("@dni", dni) }
                }.ExecuteScalar();

                // Si no existe el paciente, no se puede guardar la ubicación
                if (pacienteId == null) return false;

                // Verificar si ya existe una ubicación para el paciente
                bool existe = Convert.ToInt32(
                    new MySqlCommand("SELECT COUNT(*) FROM Ubicaciones WHERE paciente_id = @pacienteId", conexion)
                    {
                        Parameters = { new MySqlParameter("@pacienteId", pacienteId) }
                    }.ExecuteScalar()) > 0;

                // Si ya existe, se actualiza. Si no, se inserta una nueva ubicación
                var query = existe
                    ? "UPDATE Ubicaciones SET planta = @planta, habitacion = @habitacion, cama = @cama WHERE paciente_id = @pacienteId"
                    : "INSERT INTO Ubicaciones (paciente_id, planta, habitacion, cama) VALUES (@pacienteId, @planta, @habitacion, @cama)";

                using (var cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@pacienteId", pacienteId);
                    cmd.Parameters.AddWithValue("@planta", ubicacion.Planta);
                    cmd.Parameters.AddWithValue("@habitacion", ubicacion.Habitacion);
                    cmd.Parameters.AddWithValue("@cama", ubicacion.Cama);

                    // Ejecuta la consulta y devuelve true si se afectó alguna fila
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        #endregion

        #endregion

    }
}

