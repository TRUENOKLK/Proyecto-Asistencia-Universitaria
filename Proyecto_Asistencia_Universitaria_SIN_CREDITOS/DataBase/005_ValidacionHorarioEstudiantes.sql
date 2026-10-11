USE AsistenciaUniversitaria;
GO
-- ============================================================
-- MEJORA: Evitar que un mismo estudiante quede inscrito en dos
-- grupos cuyo día y horario se crucen (antes solo se validaba
-- el choque de horario del profesor, no el de los estudiantes).
-- ============================================================
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

        -- Si el grupo ya tiene estudiantes inscritos, verificar que el nuevo
        -- horario no choque con alguna otra clase de esos mismos estudiantes.
        IF EXISTS (
            SELECT 1
            FROM Inscripciones iPropio
            INNER JOIN Inscripciones iOtro ON iOtro.IdEstudiante = iPropio.IdEstudiante AND iOtro.IdGrupo <> @IdGrupo
            INNER JOIN Grupos gOtro ON gOtro.IdGrupo = iOtro.IdGrupo
            WHERE iPropio.IdGrupo = @IdGrupo
              AND gOtro.DiaSemana = @DiaSemana
              AND gOtro.HoraInicio IS NOT NULL AND gOtro.HoraFin IS NOT NULL
              AND @HoraInicio < gOtro.HoraFin AND @HoraFin > gOtro.HoraInicio
        )
            THROW 51008, 'NO SE PUEDE CAMBIAR EL HORARIO: HAY ESTUDIANTES INSCRITOS EN ESTE GRUPO QUE YA TIENEN OTRA CLASE EN ESE HORARIO', 1;

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

SELECT name AS ProcedimientoActualizado FROM sys.procedures WHERE name = 'SP_EditarGrupo';
GO
