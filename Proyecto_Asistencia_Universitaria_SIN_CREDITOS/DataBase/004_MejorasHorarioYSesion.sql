USE AsistenciaUniversitaria;
GO
-- ============================================================
-- MEJORA 1: Horario de clases por grupo (día + hora inicio/fin)
-- Permite que un profesor tenga varias clases en un mismo día
-- con distintos grupos, cada una con su propio rango de horas.
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Grupos') AND name = 'DiaSemana')
BEGIN
    ALTER TABLE Grupos ADD DiaSemana VARCHAR(15) NULL;
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Grupos') AND name = 'HoraInicio')
BEGIN
    ALTER TABLE Grupos ADD HoraInicio TIME NULL;
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Grupos') AND name = 'HoraFin')
BEGIN
    ALTER TABLE Grupos ADD HoraFin TIME NULL;
END
GO

CREATE OR ALTER PROC SP_ListarTodosLosGrupos
AS
BEGIN
    SELECT
        g.IdGrupo,
        g.NombreGrupo,
        a.NombreAsignatura,
        p.Nombres + ' ' + p.Apellidos AS Docente,
        per.NombrePeriodo,
        g.DiaSemana,
        g.HoraInicio,
        g.HoraFin
    FROM Grupos g
    INNER JOIN Asignaturas a ON g.IdAsignatura = a.IdAsignatura
    INNER JOIN Profesores p ON g.IdProfesor = p.IdProfesor
    INNER JOIN Periodos per ON g.IdPeriodo = per.IdPeriodo
    ORDER BY g.NombreGrupo ASC;
END
GO

CREATE OR ALTER PROC SP_ListarGruposPorProfesor
(
    @IdProfesor INT
)
AS
BEGIN
    SELECT
        g.IdGrupo,
        g.NombreGrupo,
        a.NombreAsignatura,
        p.NombrePeriodo,
        g.DiaSemana,
        g.HoraInicio,
        g.HoraFin
    FROM Grupos g
    INNER JOIN Asignaturas a ON g.IdAsignatura = a.IdAsignatura
    INNER JOIN Periodos p ON g.IdPeriodo = p.IdPeriodo
    WHERE g.IdProfesor = @IdProfesor
    ORDER BY
        CASE g.DiaSemana
            WHEN 'Lunes' THEN 1 WHEN 'Martes' THEN 2 WHEN 'Miércoles' THEN 3
            WHEN 'Jueves' THEN 4 WHEN 'Viernes' THEN 5 WHEN 'Sábado' THEN 6
            WHEN 'Domingo' THEN 7 ELSE 8
        END,
        g.HoraInicio;
END
GO

CREATE OR ALTER PROC SP_ObtenerGrupoPorId
(
    @IdGrupo INT
)
AS
BEGIN
    SELECT g.IdGrupo, g.NombreGrupo, g.IdAsignatura, g.IdProfesor, g.IdPeriodo,
           a.NombreAsignatura, p.Nombres + ' ' + p.Apellidos AS Docente, per.NombrePeriodo,
           g.DiaSemana, g.HoraInicio, g.HoraFin
    FROM Grupos g
    INNER JOIN Asignaturas a ON g.IdAsignatura = a.IdAsignatura
    INNER JOIN Profesores p ON g.IdProfesor = p.IdProfesor
    INNER JOIN Periodos per ON g.IdPeriodo = per.IdPeriodo
    WHERE g.IdGrupo = @IdGrupo;
END
GO

CREATE OR ALTER PROC SP_InsertarGrupo
(
    @NombreGrupo VARCHAR(20),
    @IdAsignatura INT,
    @IdProfesor INT,
    @IdPeriodo INT,
    @DiaSemana VARCHAR(15),
    @HoraInicio TIME,
    @HoraFin TIME
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        IF @HoraFin <= @HoraInicio
            THROW 51005, 'LA HORA DE FIN DEBE SER POSTERIOR A LA HORA DE INICIO', 1;

        IF EXISTS (
            SELECT 1 FROM Grupos
            WHERE IdProfesor = @IdProfesor
              AND DiaSemana = @DiaSemana
              AND @HoraInicio < HoraFin AND @HoraFin > HoraInicio
        )
            THROW 51006, 'EL DOCENTE YA TIENE OTRA CLASE ASIGNADA QUE SE CRUZA CON ESTE HORARIO EL MISMO DÍA', 1;

        INSERT INTO Grupos (NombreGrupo, IdAsignatura, IdProfesor, IdPeriodo, DiaSemana, HoraInicio, HoraFin)
        VALUES (@NombreGrupo, @IdAsignatura, @IdProfesor, @IdPeriodo, @DiaSemana, @HoraInicio, @HoraFin);
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROC SP_EditarGrupo
(
    @IdGrupo INT,
    @NombreGrupo VARCHAR(20),
    @IdAsignatura INT,
    @IdProfesor INT,
    @IdPeriodo INT,
    @DiaSemana VARCHAR(15),
    @HoraInicio TIME,
    @HoraFin TIME
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        IF @HoraFin <= @HoraInicio
            THROW 51005, 'LA HORA DE FIN DEBE SER POSTERIOR A LA HORA DE INICIO', 1;

        IF EXISTS (
            SELECT 1 FROM Grupos
            WHERE IdProfesor = @IdProfesor
              AND DiaSemana = @DiaSemana
              AND IdGrupo <> @IdGrupo
              AND @HoraInicio < HoraFin AND @HoraFin > HoraInicio
        )
            THROW 51006, 'EL DOCENTE YA TIENE OTRA CLASE ASIGNADA QUE SE CRUZA CON ESTE HORARIO EL MISMO DÍA', 1;

        UPDATE Grupos
        SET NombreGrupo = @NombreGrupo, IdAsignatura = @IdAsignatura, IdProfesor = @IdProfesor,
            IdPeriodo = @IdPeriodo, DiaSemana = @DiaSemana, HoraInicio = @HoraInicio, HoraFin = @HoraFin
        WHERE IdGrupo = @IdGrupo;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO

-- ============================================================
-- MEJORA 2: No permitir registrar una sesión con fecha anterior
-- al día actual (bloqueo también a nivel de base de datos).
-- ============================================================
CREATE OR ALTER PROC SP_CrearSesion
(
    @IdGrupo INT,
    @FechaSesion DATE,
    @Tema VARCHAR(200)
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF @FechaSesion < CAST(GETDATE() AS DATE)
            THROW 50005, 'NO SE PUEDE REGISTRAR UNA SESION CON FECHA ANTERIOR AL DIA DE HOY', 1;
        IF EXISTS (SELECT 1 FROM Sesiones WHERE IdGrupo = @IdGrupo AND FechaSesion = @FechaSesion)
            THROW 50004, 'YA EXISTE UNA SESION REGISTRADA PARA ESTA FECHA EN ESTE GRUPO', 1;
        INSERT INTO Sesiones (IdGrupo, FechaSesion, Tema)
        VALUES (@IdGrupo, @FechaSesion, @Tema);
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0)
            ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO

SELECT name AS ProcedimientoActualizado
FROM sys.procedures
WHERE name IN ('SP_ListarTodosLosGrupos','SP_ListarGruposPorProfesor','SP_ObtenerGrupoPorId','SP_InsertarGrupo','SP_EditarGrupo','SP_CrearSesion')
ORDER BY name;
GO
