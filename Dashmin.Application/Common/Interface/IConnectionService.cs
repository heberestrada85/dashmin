/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System.Data;

namespace Dashmin.Application.Common.Interface
{
    /// <summary>
    /// Intefaz que especifica como se debe implemenetar el servicio que devuelve conexiones a la base de datos.
    /// </summary>
    public interface IConnectionService
    {
        /// <summary>
        /// Conexión de base de datos Oracle que se necesita dentro del sistema.
        /// <seealso cref="IDbConnection"/>
        /// </summary>
        public IDbConnection GetOracleDb();

        // <summary>
        /// Conexión de base de datos Postgres que se necesita dentro del sistema.
        /// <seealso cref="IDbConnection"/>
        /// </summary>
        public IDbConnection GetNpgsqlDb();

        public void CloseConnection(IDbConnection conn);
    }
}
