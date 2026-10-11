USE AsistenciaUniversitaria;
GO
CREATE OR ALTER PROC SP_ExisteEmail
(
    @Email VARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE WHEN EXISTS (SELECT 1 FROM Usuarios WHERE Email = @Email)
                THEN CAST(1 AS BIT)
                ELSE CAST(0 AS BIT)
           END AS Existe;
END
GO
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes WHERE name = 'IX_Usuarios_Email' AND object_id = OBJECT_ID('Usuarios')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Usuarios_Email ON Usuarios(Email);
END
GO
CREATE OR ALTER PROC SP_EliminarCarrera
(
    @IdCarrera INT
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        DELETE FROM Carreras WHERE IdCarrera = @IdCarrera;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW 51001, 'NO SE PUEDE ELIMINAR: ESTA CARRERA TIENE ASIGNATURAS O ESTUDIANTES ASOCIADOS', 1;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_ListarAsignaturas
AS
BEGIN
    SELECT
        a.IdAsignatura,
        a.Codigo,
        a.NombreAsignatura,
        a.Creditos,
        a.IdCarrera,
        c.NombreCarrera
    FROM Asignaturas a
    INNER JOIN Carreras c ON a.IdCarrera = c.IdCarrera
    ORDER BY a.NombreAsignatura ASC;
END
GO
CREATE OR ALTER PROC SP_EditarAsignatura
(
    @IdAsignatura INT,
    @Codigo VARCHAR(20),
    @NombreAsignatura VARCHAR(100),
    @Creditos INT,
    @IdCarrera INT
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF EXISTS (SELECT 1 FROM Asignaturas WHERE Codigo = @Codigo AND IdAsignatura <> @IdAsignatura)
            THROW 51002, 'EL CÓDIGO DE ASIGNATURA YA ESTÁ EN USO POR OTRA ASIGNATURA', 1;
        UPDATE Asignaturas
        SET Codigo = @Codigo, NombreAsignatura = @NombreAsignatura, Creditos = @Creditos, IdCarrera = @IdCarrera
        WHERE IdAsignatura = @IdAsignatura;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_EliminarAsignatura
(
    @IdAsignatura INT
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        DELETE FROM Asignaturas WHERE IdAsignatura = @IdAsignatura;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW 51003, 'NO SE PUEDE ELIMINAR: ESTA ASIGNATURA TIENE GRUPOS ASOCIADOS', 1;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_EditarGrupo
(
    @IdGrupo INT,
    @NombreGrupo VARCHAR(20),
    @IdAsignatura INT,
    @IdProfesor INT,
    @IdPeriodo INT
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        UPDATE Grupos
        SET NombreGrupo = @NombreGrupo, IdAsignatura = @IdAsignatura, IdProfesor = @IdProfesor, IdPeriodo = @IdPeriodo
        WHERE IdGrupo = @IdGrupo;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_EliminarGrupo
(
    @IdGrupo INT
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        DELETE FROM Grupos WHERE IdGrupo = @IdGrupo;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW 51004, 'NO SE PUEDE ELIMINAR: ESTE GRUPO YA TIENE SESIONES O ESTUDIANTES INSCRITOS', 1;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_ObtenerGrupoPorId
(
    @IdGrupo INT
)
AS
BEGIN
    SELECT g.IdGrupo, g.NombreGrupo, g.IdAsignatura, g.IdProfesor, g.IdPeriodo,
           a.NombreAsignatura, p.Nombres + ' ' + p.Apellidos AS Docente, per.NombrePeriodo
    FROM Grupos g
    INNER JOIN Asignaturas a ON g.IdAsignatura = a.IdAsignatura
    INNER JOIN Profesores p ON g.IdProfesor = p.IdProfesor
    INNER JOIN Periodos per ON g.IdPeriodo = per.IdPeriodo
    WHERE g.IdGrupo = @IdGrupo;
END
GO
CREATE OR ALTER PROC SP_EditarEstudiante
(
    @IdEstudiante INT,
    @Nombres VARCHAR(100),
    @Apellidos VARCHAR(100),
    @IdCarrera INT
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        UPDATE Estudiantes
        SET Nombres = @Nombres, Apellidos = @Apellidos, IdCarrera = @IdCarrera
        WHERE IdEstudiante = @IdEstudiante;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_ObtenerEstudiantePorId
(
    @IdEstudiante INT
)
AS
BEGIN
    SELECT e.IdEstudiante, e.Carnet, e.Nombres, e.Apellidos, e.IdCarrera,
           c.NombreCarrera, u.Email, u.Activo
    FROM Estudiantes e
    INNER JOIN Carreras c ON e.IdCarrera = c.IdCarrera
    INNER JOIN Usuarios u ON e.IdUsuario = u.IdUsuario
    WHERE e.IdEstudiante = @IdEstudiante;
END
GO
CREATE OR ALTER PROC SP_CambiarEstadoEstudiante
(
    @IdEstudiante INT,
    @Activo BIT
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        UPDATE u
        SET u.Activo = @Activo
        FROM Usuarios u
        INNER JOIN Estudiantes e ON e.IdUsuario = u.IdUsuario
        WHERE e.IdEstudiante = @IdEstudiante;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_ObtenerProfesorPorId
(
    @IdProfesor INT
)
AS
BEGIN
    SELECT p.IdProfesor, p.Nombres, p.Apellidos, p.Telefono, u.Email, u.Activo
    FROM Profesores p
    INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario
    WHERE p.IdProfesor = @IdProfesor;
END
GO
CREATE OR ALTER PROC SP_CambiarEstadoProfesor
(
    @IdProfesor INT,
    @Activo BIT
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        UPDATE u
        SET u.Activo = @Activo
        FROM Usuarios u
        INNER JOIN Profesores p ON p.IdUsuario = u.IdUsuario
        WHERE p.IdProfesor = @IdProfesor;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_ResumenGruposPorProfesor
(
    @IdProfesor INT
)
AS
BEGIN
    SELECT
        g.IdGrupo,
        g.NombreGrupo,
        a.NombreAsignatura,
        COUNT(DISTINCT s.IdSesion) AS TotalSesiones,
        COUNT(al.IdAsistencia) AS TotalRegistros,
        SUM(CASE WHEN al.EstadoAsistencia = 'Presente' THEN 1 ELSE 0 END) AS TotalPresentes
    FROM Grupos g
    INNER JOIN Asignaturas a ON g.IdAsignatura = a.IdAsignatura
    LEFT JOIN Sesiones s ON s.IdGrupo = g.IdGrupo
    LEFT JOIN Asistencias al ON al.IdSesion = s.IdSesion
    WHERE g.IdProfesor = @IdProfesor
    GROUP BY g.IdGrupo, g.NombreGrupo, a.NombreAsignatura
    ORDER BY g.NombreGrupo;
END
GO
SELECT name AS ProcedimientoCreado
FROM sys.procedures
WHERE name IN (
    'SP_ExisteEmail', 'SP_EliminarCarrera', 'SP_ListarAsignaturas',
    'SP_EditarAsignatura', 'SP_EliminarAsignatura', 'SP_EditarGrupo',
    'SP_EliminarGrupo', 'SP_ObtenerGrupoPorId', 'SP_EditarEstudiante',
    'SP_ObtenerEstudiantePorId', 'SP_CambiarEstadoEstudiante',
    'SP_ObtenerProfesorPorId', 'SP_CambiarEstadoProfesor',
    'SP_ResumenGruposPorProfesor'
)
ORDER BY name;
GO
