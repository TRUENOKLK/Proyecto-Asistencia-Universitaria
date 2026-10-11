using Aplication.DTos;
using Aplication.Interface;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Reportes
{
   
    public class ReportePdfService : IReportePdfService
    {
        private static readonly string ColorPrimario = "#3457D5";
        private static readonly string ColorTextoSuave = "#6b7280";
        private static readonly string ColorBorde = "#e3e8f2";

        public byte[] GenerarReporteAsistencia(GrupoDto grupo, IEnumerable<ReporteAsistenciaDto> registros)
        {
            var lista = registros?.ToList() ?? new List<ReporteAsistenciaDto>();

            var sesiones = lista
                .GroupBy(r => new SesionKey(r.FechaSesion, r.Tema))
                .OrderBy(g => g.Key.FechaSesion)
                .ToList();

            var porEstudiante = lista
                .GroupBy(r => new { r.Carnet, r.Estudiante })
                .Select(g => new ResumenEstudiante
                {
                    Carnet = g.Key.Carnet ?? "-",
                    Nombre = g.Key.Estudiante ?? "-",
                    Presentes = g.Count(x => x.EstadoAsistencia == "Presente"),
                    Tardanzas = g.Count(x => x.EstadoAsistencia == "Tardanza"),
                    Justificadas = g.Count(x => x.EstadoAsistencia == "Justificada"),
                    Ausencias = g.Count(x => x.EstadoAsistencia == "Ausente"),
                    TotalSesiones = g.Count()
                })
                .OrderBy(e => e.Nombre)
                .ToList();

            var totalRegistros = lista.Count;
            var totalPresentes = lista.Count(r => r.EstadoAsistencia == "Presente");
            var porcentajeGeneral = totalRegistros > 0 ? Math.Round(totalPresentes * 100.0 / totalRegistros, 1) : 0;

            var documento = QuestPDF.Fluent.Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.6f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontColor("#1f2430"));

                    page.Header().Element(c => ComponerEncabezado(c, grupo));

                    page.Content().PaddingTop(10).Column(col =>
                    {
                        col.Spacing(14);
                        col.Item().Element(c => ComponerResumenGeneral(c, sesiones.Count, totalRegistros, porcentajeGeneral));
                        col.Item().Element(c => ComponerTablaPorEstudiante(c, porEstudiante));
                        col.Item().Element(c => ComponerDetallePorSesion(c, sesiones));
                    });

                    page.Footer().Row(row =>
                    {
                        row.RelativeItem().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(7.5f).FontColor(ColorTextoSuave);
                        row.RelativeItem().AlignRight().Text(t =>
                        {
                            t.DefaultTextStyle(s => s.FontSize(7.5f).FontColor(ColorTextoSuave));
                            t.Span("Página ");
                            t.CurrentPageNumber();
                            t.Span(" de ");
                            t.TotalPages();
                        });
                    });
                });
            });

            return documento.GeneratePdf();
        }

        private static void ComponerEncabezado(IContainer container, GrupoDto grupo)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Reporte de Asistencia").FontSize(18).Bold().FontColor(ColorPrimario);
                        c.Item().Text("Sistema de Asistencia Universitaria").FontSize(9).FontColor(ColorTextoSuave);
                    });
                });

                col.Item().PaddingTop(10).Background("#f8f9fd").Padding(10).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Grupo").FontSize(7.5f).FontColor(ColorTextoSuave);
                        c.Item().Text(grupo.NombreGrupo ?? "-").Bold();
                    });
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Asignatura").FontSize(7.5f).FontColor(ColorTextoSuave);
                        c.Item().Text(grupo.NombreAsignatura ?? "-").Bold();
                    });
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Docente").FontSize(7.5f).FontColor(ColorTextoSuave);
                        c.Item().Text(grupo.Docente ?? "-").Bold();
                    });
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Periodo").FontSize(7.5f).FontColor(ColorTextoSuave);
                        c.Item().Text(grupo.NombrePeriodo ?? "-").Bold();
                    });
                });

                col.Item().PaddingTop(6).LineHorizontal(1).LineColor(ColorBorde);
            });
        }

        private static void ComponerResumenGeneral(IContainer container, int totalSesiones, int totalRegistros, double porcentaje)
        {
            var colorPorcentaje = porcentaje >= 75 ? "#0f6b53" : porcentaje >= 50 ? "#b8860b" : "#c0392b";

            container.Row(row =>
            {
                row.Spacing(10);
                row.RelativeItem().Border(1).BorderColor(ColorBorde).CornerRadius(6).Padding(10).Column(c =>
                {
                    c.Item().AlignCenter().Text(totalSesiones.ToString()).FontSize(20).Bold().FontColor(ColorPrimario);
                    c.Item().AlignCenter().Text("Sesiones registradas").FontSize(8).FontColor(ColorTextoSuave);
                });
                row.RelativeItem().Border(1).BorderColor(ColorBorde).CornerRadius(6).Padding(10).Column(c =>
                {
                    c.Item().AlignCenter().Text(totalRegistros.ToString()).FontSize(20).Bold().FontColor(ColorPrimario);
                    c.Item().AlignCenter().Text("Registros de asistencia").FontSize(8).FontColor(ColorTextoSuave);
                });
                row.RelativeItem().Border(1).BorderColor(ColorBorde).CornerRadius(6).Padding(10).Column(c =>
                {
                    c.Item().AlignCenter().Text($"{porcentaje}%").FontSize(20).Bold().FontColor(colorPorcentaje);
                    c.Item().AlignCenter().Text("Asistencia general").FontSize(8).FontColor(ColorTextoSuave);
                });
            });
        }

        private static void ComponerTablaPorEstudiante(IContainer container, List<ResumenEstudiante> resumen)
        {
            container.Column(col =>
            {
                col.Item().Text("Resumen por estudiante").FontSize(12).Bold().FontColor("#1f2430");
                col.Item().PaddingTop(4).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(2);
                        c.RelativeColumn(4);
                        c.RelativeColumn(1.6f);
                        c.RelativeColumn(1.6f);
                        c.RelativeColumn(1.8f);
                        c.RelativeColumn(1.6f);
                        c.RelativeColumn(1.8f);
                    });

                    table.Header(header =>
                    {
                        EncabezadoCelda(header.Cell(), "Carnet");
                        EncabezadoCelda(header.Cell(), "Estudiante");
                        EncabezadoCelda(header.Cell(), "Presente");
                        EncabezadoCelda(header.Cell(), "Tardanza");
                        EncabezadoCelda(header.Cell(), "Justificada");
                        EncabezadoCelda(header.Cell(), "Ausente");
                        EncabezadoCelda(header.Cell(), "% Asist.");
                    });

                    if (resumen.Count == 0)
                    {
                        table.Cell().ColumnSpan(7).Padding(8).AlignCenter()
                            .Text("Todavía no hay asistencia registrada para este grupo.")
                            .FontColor(ColorTextoSuave).Italic();
                    }

                    foreach (var e in resumen)
                    {
                        var porcentajeInd = e.TotalSesiones > 0 ? Math.Round(e.Presentes * 100.0 / e.TotalSesiones, 1) : 0;

                        CeldaDato(table.Cell(), e.Carnet);
                        CeldaDato(table.Cell(), e.Nombre);
                        CeldaDato(table.Cell(), e.Presentes.ToString(), "#0f6b53");
                        CeldaDato(table.Cell(), e.Tardanzas.ToString(), "#b8860b");
                        CeldaDato(table.Cell(), e.Justificadas.ToString(), "#1f6fb2");
                        CeldaDato(table.Cell(), e.Ausencias.ToString(), "#c0392b");
                        CeldaDato(table.Cell(), $"{porcentajeInd}%");
                    }
                });
            });
        }

        private static void ComponerDetallePorSesion(IContainer container, List<IGrouping<SesionKey, ReporteAsistenciaDto>> sesiones)
        {
            container.Column(col =>
            {
                col.Item().Text("Detalle por sesión").FontSize(12).Bold().FontColor("#1f2430");

                if (sesiones.Count == 0)
                {
                    col.Item().PaddingTop(4).Text("Sin sesiones registradas.").FontColor(ColorTextoSuave).Italic();
                    return;
                }

                foreach (var sesion in sesiones)
                {
                    col.Item().PaddingTop(8).Column(sc =>
                    {
                        sc.Item().Text($"{sesion.Key.FechaSesion:dd/MM/yyyy} — {sesion.Key.Tema}")
                            .Bold().FontSize(10).FontColor(ColorPrimario);

                        sc.Item().PaddingTop(3).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);
                                c.RelativeColumn(4);
                                c.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                EncabezadoCelda(header.Cell(), "Carnet");
                                EncabezadoCelda(header.Cell(), "Estudiante");
                                EncabezadoCelda(header.Cell(), "Estado");
                            });

                            foreach (var reg in sesion.OrderBy(r => r.Estudiante))
                            {
                                var color = reg.EstadoAsistencia switch
                                {
                                    "Presente" => "#0f6b53",
                                    "Tardanza" => "#b8860b",
                                    "Justificada" => "#1f6fb2",
                                    _ => "#c0392b"
                                };
                                CeldaDato(table.Cell(), reg.Carnet ?? "-");
                                CeldaDato(table.Cell(), reg.Estudiante ?? "-");
                                CeldaDato(table.Cell(), reg.EstadoAsistencia ?? "-", color);
                            }
                        });
                    });
                }
            });
        }

        private static void EncabezadoCelda(IContainer cell, string texto)
        {
            cell.Background("#f8f9fd").BorderBottom(1).BorderColor(ColorBorde).Padding(5)
                .Text(texto).Bold().FontSize(8).FontColor("#1f2430");
        }

        private static void CeldaDato(IContainer cell, string texto, string? color = null)
        {
            cell.BorderBottom(1).BorderColor(ColorBorde).Padding(5)
                .Text(texto).FontSize(8.5f).FontColor(color ?? "#1f2430");
        }

        private class ResumenEstudiante
        {
            public string Carnet { get; set; } = "-";
            public string Nombre { get; set; } = "-";
            public int Presentes { get; set; }
            public int Tardanzas { get; set; }
            public int Justificadas { get; set; }
            public int Ausencias { get; set; }
            public int TotalSesiones { get; set; }
        }

        private sealed record SesionKey(DateTime FechaSesion, string? Tema);
    }
}
