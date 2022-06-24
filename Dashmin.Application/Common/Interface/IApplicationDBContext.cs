/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System.Threading;
using System.Threading.Tasks;
using Dashmin.Application.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dashmin.Application.Common.Interface
{
    /// <summary>
    /// Interface IApplicationDbContext
    /// </summary>
    public interface IApplicationDBContext
    {
        DbSet<ind_global> ind_global { get; set; }
        DbSet<ind_ingresos_facturados> ind_ingresos_facturados { get; set; }
        DbSet<ind_objetivo_cobranza> ind_objetivo_cobranza { get; set; }
        DbSet<ind_pacientes_egresados> ind_pacientes_egresados { get; set; }
        DbSet<ind_promedio_diario_urgencias> ind_promedio_diario_urgencias { get; set; }
        DbSet<ind_promediodiariocirugiasala> ind_promediodiariocirugiasala { get; set; }
        DbSet<var_antiguedad_saldos> var_antiguedad_saldos { get; set; }
        DbSet<var_camas_censables> var_camas_censables { get; set; }
        DbSet<var_cargos_diario> var_cargos_diario { get; set; }
        DbSet<var_cirugias_paciente> var_cirugias_paciente { get; set; }
        DbSet<var_cronologico_facturas> var_cronologico_facturas { get; set; }
        DbSet<var_cuenta_paciente> var_cuenta_paciente { get; set; }
        DbSet<var_facturacion_tipo_paciente> var_facturacion_tipo_paciente { get; set; }
        DbSet<var_limite_credito> var_limite_credito { get; set; }
        DbSet<var_ocupacion_hospitalaria> var_ocupacion_hospitalaria { get; set; }
        DbSet<var_pacientes_ingresos> var_pacientes_ingresos { get; set; }
        DbSet<var_paquete_cobranza> var_paquete_cobranza { get; set; }
        DbSet<var_saldos_bancos> var_saldos_bancos { get; set; }
        DbSet<var_total_consultas_urgencias> var_total_consultas_urgencias { get; set; }
        DbSet<var_total_diagnosticos_egreso> var_total_diagnosticos_egreso { get; set; }
        DbSet<var_total_dias_hospitalizacion> var_total_dias_hospitalizacion { get; set; }
        DbSet<var_total_egresos> var_total_egresos { get; set; }
        DbSet<var_total_ingresos_actuales> var_total_ingresos_actuales { get; set; }

        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
