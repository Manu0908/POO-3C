using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NexoLog.DTOs;
using NexoLog.Informes;

namespace NexoLog.Exportadores;

// Exportador de informes consolidados a formato XML con codificacion UTF-8.
public class ExportadorInformesXml : IExportadorInforme
{
    public void Exportar(InformeOperativo operativo, InformeFinanciero financiero, InformeInconsistencias inconsistencias, string rutaDestino)
    {
        string directorio = Path.GetDirectoryName(rutaDestino);
        if (!string.IsNullOrWhiteSpace(directorio) && !Directory.Exists(directorio))
        {
            Directory.CreateDirectory(directorio);
        }

        XElement raiz = new XElement("informesNexoLog",
            new XAttribute("fechaGeneracion", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
            new XAttribute("sistema", "NexoLog S.A.S.")
        );

        // Seccion Operativa
        if (operativo != null)
        {
            XElement elOperativo = new XElement("informeOperativo",
                new XElement("metricasGenerales",
                    new XElement("totalDespachos", operativo.TotalDespachos),
                    new XElement("totalEntregados", operativo.TotalEntregados),
                    new XElement("totalDevueltos", operativo.TotalDevueltos),
                    new XElement("totalPendientes", operativo.TotalPendientes),
                    new XElement("totalEnDistribucion", operativo.TotalEnDistribucion),
                    new XElement("totalPaquetes", operativo.TotalPaquetes),
                    new XElement("pesoTotalKg", operativo.PesoTotalKg.ToString("F2", CultureInfo.InvariantCulture)),
                    new XElement("cumplidosSLA", operativo.CumplidosSLA),
                    new XElement("incumplidosSLA", operativo.IncumplidosSLA),
                    new XElement("porcentajeCumplimiento", operativo.PorcentajeCumplimientoSobreEntregados.ToString("F2", CultureInfo.InvariantCulture))
                )
            );

            XElement elDesgloseComercios = new XElement("desgloseComercios");
            foreach (ResumenComercioOperativo com in operativo.DesgloseComercios)
            {
                elDesgloseComercios.Add(new XElement("comercio",
                    new XAttribute("codigo", com.CodigoComercio),
                    new XAttribute("nombre", com.NombreComercio),
                    new XElement("totalDespachos", com.TotalDespachos),
                    new XElement("entregados", com.Entregados),
                    new XElement("devueltos", com.Devueltos),
                    new XElement("pendientes", com.Pendientes),
                    new XElement("enDistribucion", com.EnDistribucion),
                    new XElement("cumplidosSLA", com.CumplidosSLA),
                    new XElement("incumplidosSLA", com.IncumplidosSLA),
                    new XElement("porcentajeCumplimiento", com.PorcentajeCumplimiento.ToString("F2", CultureInfo.InvariantCulture))
                ));
            }
            elOperativo.Add(elDesgloseComercios);

            XElement elDesgloseModalidades = new XElement("desgloseModalidades");
            foreach (ResumenModalidadOperativo mod in operativo.DesgloseModalidades)
            {
                elDesgloseModalidades.Add(new XElement("modalidad",
                    new XAttribute("nombre", mod.Modalidad),
                    new XElement("totalDespachos", mod.TotalDespachos),
                    new XElement("entregados", mod.Entregados),
                    new XElement("cumplidosSLA", mod.CumplidosSLA),
                    new XElement("incumplidosSLA", mod.IncumplidosSLA),
                    new XElement("porcentajeCumplimiento", mod.PorcentajeCumplimiento.ToString("F2", CultureInfo.InvariantCulture))
                ));
            }
            elOperativo.Add(elDesgloseModalidades);

            raiz.Add(elOperativo);
        }

        // Seccion Financiera
        if (financiero != null)
        {
            XElement elFinanciero = new XElement("informeFinanciero",
                new XElement("totales",
                    new XElement("granTotalFacturado", financiero.GranTotalFacturado.ToString("F2", CultureInfo.InvariantCulture)),
                    new XElement("despachosEntregadosNoValorados", financiero.DespachosEntregadosNoValorados)
                )
            );

            XElement elComerciosFin = new XElement("liquidacionesPorComercio");
            foreach (ItemFinancieroComercio item in financiero.DesgloseComercios)
            {
                elComerciosFin.Add(new XElement("comercio",
                    new XAttribute("codigo", item.CodigoComercio),
                    new XAttribute("nombre", item.NombreComercio),
                    new XElement("modalidadTarifa", item.ModalidadTarifa),
                    new XElement("tarifaBase", item.TarifaBase.ToString("F2", CultureInfo.InvariantCulture)),
                    new XElement("despachosTotales", item.DespachosTotales),
                    new XElement("despachosFacturables", item.DespachosFacturables),
                    new XElement("totalFacturado", item.TotalFacturado.ToString("F2", CultureInfo.InvariantCulture))
                ));
            }
            elFinanciero.Add(elComerciosFin);

            XElement elModalidadesTarifa = new XElement("cobrosPorModalidadTarifa");
            foreach (KeyValuePair<string, decimal> par in financiero.CobrosPorModalidadTarifa)
            {
                elModalidadesTarifa.Add(new XElement("modalidad",
                    new XAttribute("nombre", par.Key),
                    new XAttribute("totalFacturado", par.Value.ToString("F2", CultureInfo.InvariantCulture))
                ));
            }
            elFinanciero.Add(elModalidadesTarifa);

            raiz.Add(elFinanciero);
        }

        // Seccion Inconsistencias
        if (inconsistencias != null)
        {
            XElement elInconsistencias = new XElement("informeInconsistencias",
                new XElement("totalInconsistencias", inconsistencias.TotalInconsistencias)
            );

            XElement elPorTipo = new XElement("porTipo");
            foreach (KeyValuePair<string, int> par in inconsistencias.InconsistenciasPorTipo)
            {
                elPorTipo.Add(new XElement("inconsistencia",
                    new XAttribute("tipo", par.Key),
                    new XAttribute("cantidad", par.Value)
                ));
            }
            elInconsistencias.Add(elPorTipo);

            XElement elPorArchivo = new XElement("porArchivo");
            foreach (KeyValuePair<string, int> par in inconsistencias.InconsistenciasPorArchivo)
            {
                elPorArchivo.Add(new XElement("archivo",
                    new XAttribute("nombre", par.Key),
                    new XAttribute("cantidad", par.Value)
                ));
            }
            elInconsistencias.Add(elPorArchivo);

            XElement elDetalle = new XElement("listadoDetallado");
            foreach (InconsistenciaDto reg in inconsistencias.Listado)
            {
                elDetalle.Add(new XElement("registro",
                    new XAttribute("archivo", reg.ArchivoOrigen),
                    new XAttribute("referencia", reg.IdentificadorReferencia),
                    new XAttribute("tipo", reg.Tipo),
                    new XAttribute("fecha", reg.FechaDeteccion.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("motivo", reg.Motivo)
                ));
            }
            elInconsistencias.Add(elDetalle);

            raiz.Add(elInconsistencias);
        }

        XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), raiz);

        XmlWriterSettings configuracionXml = new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8
        };

        using (XmlWriter escritor = XmlWriter.Create(rutaDestino, configuracionXml))
        {
            doc.Save(escritor);
        }
    }
}
