CREATE DATABASE AsistenciaUniversitaria;
GO
USE AsistenciaUniversitaria;
GO
CREATE TABLE Roles (
    IdRol INT PRIMARY KEY IDENTITY(1,1),
    NombreRol VARCHAR(50) NOT NULL UNIQUE
);
CREATE TABLE Usuarios (
    IdUsuario INT PRIMARY KEY IDENTITY(1,1),
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    IdRol INT NOT NULL,
    Activo BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IdRol) REFERENCES Roles(IdRol)
);
CREATE TABLE Periodos (
    IdPeriodo INT PRIMARY KEY IDENTITY(1,1),
    NombrePeriodo VARCHAR(50) NOT NULL UNIQUE,
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL
);
CREATE TABLE Facultades (
    IdFacultad INT PRIMARY KEY IDENTITY(1,1),
    NombreFacultad VARCHAR(100) NOT NULL UNIQUE
);
CREATE TABLE Carreras (
    IdCarrera INT PRIMARY KEY IDENTITY(1,1),
    NombreCarrera VARCHAR(100) NOT NULL,
    IdFacultad INT NOT NULL,
    FOREIGN KEY (IdFacultad) REFERENCES Facultades(IdFacultad)
);
CREATE TABLE Asignaturas (
    IdAsignatura INT PRIMARY KEY IDENTITY(1,1),
    Codigo VARCHAR(20) NOT NULL UNIQUE,
    NombreAsignatura VARCHAR(100) NOT NULL,
    Creditos INT NOT NULL,
    IdCarrera INT NOT NULL,
    FOREIGN KEY (IdCarrera) REFERENCES Carreras(IdCarrera)
);
CREATE TABLE Profesores (
    IdProfesor INT PRIMARY KEY IDENTITY(1,1),
    Nombres VARCHAR(100) NOT NULL,
    Apellidos VARCHAR(100) NOT NULL,
    Telefono VARCHAR(20),
    IdUsuario INT NOT NULL UNIQUE,
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario)
);
CREATE TABLE Estudiantes (
    IdEstudiante INT PRIMARY KEY IDENTITY(1,1),
    Carnet VARCHAR(20) NOT NULL UNIQUE,
    Nombres VARCHAR(100) NOT NULL,
    Apellidos VARCHAR(100) NOT NULL,
    IdCarrera INT NOT NULL,
    IdUsuario INT NOT NULL UNIQUE,
    FOREIGN KEY (IdCarrera) REFERENCES Carreras(IdCarrera),
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario)
);
CREATE TABLE Grupos (
    IdGrupo INT PRIMARY KEY IDENTITY(1,1),
    NombreGrupo VARCHAR(20) NOT NULL,
    IdAsignatura INT NOT NULL,
    IdProfesor INT NOT NULL,
    IdPeriodo INT NOT NULL,
    FOREIGN KEY (IdAsignatura) REFERENCES Asignaturas(IdAsignatura),
    FOREIGN KEY (IdProfesor) REFERENCES Profesores(IdProfesor),
    FOREIGN KEY (IdPeriodo) REFERENCES Periodos(IdPeriodo)
);
CREATE TABLE Inscripciones (
    IdInscripcion INT PRIMARY KEY IDENTITY(1,1),
    IdEstudiante INT NOT NULL,
    IdGrupo INT NOT NULL,
    FechaInscripcion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IdEstudiante) REFERENCES Estudiantes(IdEstudiante),
    FOREIGN KEY (IdGrupo) REFERENCES Grupos(IdGrupo),
    CONSTRAINT UQ_Estudiante_Grupo UNIQUE (IdEstudiante, IdGrupo)
);
CREATE TABLE Sesiones (
    IdSesion INT PRIMARY KEY IDENTITY(1,1),
    IdGrupo INT NOT NULL,
    FechaSesion DATE NOT NULL,
    Tema VARCHAR(200) NOT NULL,
    FOREIGN KEY (IdGrupo) REFERENCES Grupos(IdGrupo),
    CONSTRAINT UQ_Grupo_Fecha UNIQUE (IdGrupo, FechaSesion)
);
CREATE TABLE Asistencias (
    IdAsistencia INT PRIMARY KEY IDENTITY(1,1),
    IdSesion INT NOT NULL,
    IdEstudiante INT NOT NULL,
    EstadoAsistencia VARCHAR(20) CHECK (EstadoAsistencia IN ('Presente', 'Ausente', 'Tardanza', 'Justificada')) NOT NULL,
    FechaRegistro DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IdSesion) REFERENCES Sesiones(IdSesion),
    FOREIGN KEY (IdEstudiante) REFERENCES Estudiantes(IdEstudiante),
    CONSTRAINT UQ_Sesion_Estudiante UNIQUE (IdSesion, IdEstudiante)
);
GO
CREATE OR ALTER PROC SP_ListarEstudiantesPorGrupo
(
    @IdGrupo INT
)
AS
BEGIN
    SELECT
        e.IdEstudiante,
        e.Carnet,
        e.Nombres,
        e.Apellidos
    FROM Inscripciones i
    INNER JOIN Estudiantes e ON i.IdEstudiante = e.IdEstudiante
    WHERE i.IdGrupo = @IdGrupo
    ORDER BY e.Apellidos ASC;
END
GO
CREATE OR ALTER PROC SP_RegistrarAsistencia
(
    @IdSesion INT,
    @IdEstudiante INT,
    @EstadoAsistencia VARCHAR(20)
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF EXISTS (SELECT 1 FROM Asistencias WHERE IdSesion = @IdSesion AND IdEstudiante = @IdEstudiante)
            THROW 50001, 'LA ASISTENCIA YA FUE REGISTRADA PARA ESTE ESTUDIANTE', 1;
        INSERT INTO Asistencias (IdSesion, IdEstudiante, EstadoAsistencia)
        VALUES (@IdSesion, @IdEstudiante, @EstadoAsistencia);
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0)
            ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_ReporteAsistencia
(
    @IdGrupo INT
)
AS
BEGIN
    SELECT
        s.FechaSesion,
        s.Tema,
        e.Carnet,
        e.Nombres + ' ' + e.Apellidos AS Estudiante,
        a.EstadoAsistencia
    FROM Asistencias a
    INNER JOIN Sesiones s ON a.IdSesion = s.IdSesion
    INNER JOIN Estudiantes e ON a.IdEstudiante = e.IdEstudiante
    WHERE s.IdGrupo = @IdGrupo
    ORDER BY s.FechaSesion DESC, e.Apellidos ASC;
END
GO
INSERT INTO Roles (NombreRol) VALUES ('Administrador'), ('Profesor'), ('Estudiante');
INSERT INTO Usuarios (Email, PasswordHash, IdRol) VALUES
('admin@uni.edu.ni', '100000.uxt3utyy0ZpYUy0iMekL0g==.wVHb1fvI83D6iiq4tzgrbbWBX6GbXiX8Dd5PV4OU3po=', 1),
('jramirez@uni.edu.ni', '100000.tSF/8iRbThmrg7ngbPokoA==.rq2cUpd+OW5Fe1SYVuK9OCJmj0ZfQBcCwLYHSMU7cJE=', 2),
('alopez@est.uni.edu.ni', '100000.BTujd/8yatqaxJ/bzyOcTw==.heenLNqUTFmKnhHqYqJ3nsma1vFRRvM/dDmLuQXFQgA=', 3);
INSERT INTO Periodos (NombrePeriodo, FechaInicio, FechaFin) VALUES ('2026-I', '2026-02-01', '2026-06-30');
INSERT INTO Facultades (NombreFacultad) VALUES ('Facultad de Ingeniería');
INSERT INTO Carreras (NombreCarrera, IdFacultad) VALUES ('Ingeniería en Sistemas', 1);
INSERT INTO Asignaturas (Codigo, NombreAsignatura, Creditos, IdCarrera) VALUES ('SIS101', 'Programación III', 4, 1);
INSERT INTO Profesores (Nombres, Apellidos, Telefono, IdUsuario) VALUES ('José', 'Ramírez', '8888-8888', 2);
INSERT INTO Estudiantes (Carnet, Nombres, Apellidos, IdCarrera, IdUsuario) VALUES ('2026-0001U', 'Ana', 'López', 1, 3);
INSERT INTO Grupos (NombreGrupo, IdAsignatura, IdProfesor, IdPeriodo) VALUES ('3M1-IS', 1, 1, 1);
INSERT INTO Inscripciones (IdEstudiante, IdGrupo) VALUES (1, 1);
INSERT INTO Sesiones (IdGrupo, FechaSesion, Tema) VALUES (1, GETDATE(), 'Introducción a Clean Architecture MVC');
EXEC SP_RegistrarAsistencia @IdSesion = 1, @IdEstudiante = 1, @EstadoAsistencia = 'Presente';
SELECT 'Base de Datos Creada y Poblada con Éxito' AS Estatus;
EXEC SP_ReporteAsistencia @IdGrupo = 1;
GO
USE AsistenciaUniversitaria;
GO
CREATE OR ALTER PROC SP_ValidarLogin
(
    @Email VARCHAR(100),
    @PasswordHash VARCHAR(255)
)
AS
BEGIN
    SELECT
        u.IdUsuario,
        u.Email,
        r.NombreRol,
        u.Activo
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    WHERE u.Email = @Email AND u.PasswordHash = @PasswordHash AND u.Activo = 1;
END
GO
CREATE OR ALTER PROC SP_ObtenerUsuarioPorEmail
(
    @Email VARCHAR(100)
)
AS
BEGIN
    SELECT
        u.IdUsuario,
        u.Email,
        u.PasswordHash,
        r.NombreRol,
        u.Activo
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    WHERE u.Email = @Email;
END
GO
CREATE OR ALTER PROC SP_InsertarEstudiante
(
    @Email VARCHAR(100),
    @PasswordHash VARCHAR(255),
    @Carnet VARCHAR(20),
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
        IF EXISTS (SELECT 1 FROM Usuarios WHERE Email = @Email)
            THROW 50002, 'EL CORREO YA ESTÁ REGISTRADO', 1;
        IF EXISTS (SELECT 1 FROM Estudiantes WHERE Carnet = @Carnet)
            THROW 50003, 'EL CARNET YA EXISTE', 1;
        DECLARE @IdUsuarioInsertado INT;
        INSERT INTO Usuarios (Email, PasswordHash, IdRol)
        VALUES (@Email, @PasswordHash, 3);
        SET @IdUsuarioInsertado = SCOPE_IDENTITY();
        INSERT INTO Estudiantes (Carnet, Nombres, Apellidos, IdCarrera, IdUsuario)
        VALUES (@Carnet, @Nombres, @Apellidos, @IdCarrera, @IdUsuarioInsertado);
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0)
            ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_ListarEstudiantes
AS
BEGIN
    SELECT
        e.IdEstudiante,
        e.Carnet,
        e.Nombres,
        e.Apellidos,
        c.NombreCarrera,
        u.Email,
        u.Activo
    FROM Estudiantes e
    INNER JOIN Carreras c ON e.IdCarrera = c.IdCarrera
    INNER JOIN Usuarios u ON e.IdUsuario = u.IdUsuario
    ORDER BY e.Apellidos ASC;
END
GO
CREATE OR ALTER PROC SP_ObtenerAsistenciaPorEstudianteEmail
(
    @Email VARCHAR(100)
)
AS
BEGIN
    SELECT
        s.FechaSesion,
        s.Tema,
        asig.NombreAsignatura,
        g.NombreGrupo,
        a.EstadoAsistencia
    FROM Asistencias a
    INNER JOIN Sesiones s ON a.IdSesion = s.IdSesion
    INNER JOIN Grupos g ON s.IdGrupo = g.IdGrupo
    INNER JOIN Asignaturas asig ON g.IdAsignatura = asig.IdAsignatura
    INNER JOIN Estudiantes e ON a.IdEstudiante = e.IdEstudiante
    INNER JOIN Usuarios u ON e.IdUsuario = u.IdUsuario
    WHERE u.Email = @Email
    ORDER BY s.FechaSesion DESC;
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
        p.NombrePeriodo
    FROM Grupos g
    INNER JOIN Asignaturas a ON g.IdAsignatura = a.IdAsignatura
    INNER JOIN Periodos p ON g.IdPeriodo = p.IdPeriodo
    WHERE g.IdProfesor = @IdProfesor;
END
GO
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
CREATE OR ALTER PROC SP_ListarSesionesPorGrupo
(
    @IdGrupo INT
)
AS
BEGIN
    SELECT
        IdSesion,
        FechaSesion,
        Tema
    FROM Sesiones
    WHERE IdGrupo = @IdGrupo
    ORDER BY FechaSesion DESC;
END
GO
USE AsistenciaUniversitaria;
GO
CREATE OR ALTER PROC SP_ListarFacultades
AS
BEGIN
    SELECT IdFacultad, NombreFacultad FROM Facultades ORDER BY NombreFacultad ASC;
END
GO
CREATE OR ALTER PROC SP_InsertarFacultad
(
    @NombreFacultad VARCHAR(100)
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF EXISTS (SELECT 1 FROM Facultades WHERE NombreFacultad = @NombreFacultad)
            THROW 50005, 'LA FACULTAD YA EXISTE', 1;
        INSERT INTO Facultades (NombreFacultad) VALUES (@NombreFacultad);
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_EditarFacultad
(
    @IdFacultad INT,
    @NombreFacultad VARCHAR(100)
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        UPDATE Facultades SET NombreFacultad = @NombreFacultad WHERE IdFacultad = @IdFacultad;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_EliminarFacultad
(
    @IdFacultad INT
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        DELETE FROM Facultades WHERE IdFacultad = @IdFacultad;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW 50006, 'NO SE PUEDE ELIMINAR PORQUE TIENE CARRERAS ASIGNADAS', 1;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_ListarPeriodos
AS
BEGIN
    SELECT IdPeriodo, NombrePeriodo, FechaInicio, FechaFin FROM Periodos ORDER BY FechaInicio DESC;
END
GO
CREATE OR ALTER PROC SP_InsertarPeriodo
(
    @NombrePeriodo VARCHAR(50),
    @FechaInicio DATE,
    @FechaFin DATE
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF EXISTS (SELECT 1 FROM Periodos WHERE NombrePeriodo = @NombrePeriodo)
            THROW 50007, 'EL PERIODO YA EXISTE', 1;
        INSERT INTO Periodos (NombrePeriodo, FechaInicio, FechaFin)
        VALUES (@NombrePeriodo, @FechaInicio, @FechaFin);
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_EditarPeriodo
(
    @IdPeriodo INT,
    @NombrePeriodo VARCHAR(50),
    @FechaInicio DATE,
    @FechaFin DATE
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        UPDATE Periodos
        SET NombrePeriodo = @NombrePeriodo, FechaInicio = @FechaInicio, FechaFin = @FechaFin
        WHERE IdPeriodo = @IdPeriodo;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_EliminarPeriodo
(
    @IdPeriodo INT
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        DELETE FROM Periodos WHERE IdPeriodo = @IdPeriodo;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW 50008, 'NO SE PUEDE ELIMINAR PORQUE TIENE GRUPOS ASIGNADOS', 1;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_ListarCarreras
AS
BEGIN
    SELECT
        c.IdCarrera,
        c.NombreCarrera,
        f.IdFacultad,
        f.NombreFacultad
    FROM Carreras c
    INNER JOIN Facultades f ON c.IdFacultad = f.IdFacultad
    ORDER BY c.NombreCarrera ASC;
END
GO
CREATE OR ALTER PROC SP_InsertarCarrera
(
    @NombreCarrera VARCHAR(100),
    @IdFacultad INT
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        INSERT INTO Carreras (NombreCarrera, IdFacultad) VALUES (@NombreCarrera, @IdFacultad);
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_EditarCarrera
(
    @IdCarrera INT,
    @NombreCarrera VARCHAR(100),
    @IdFacultad INT
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        UPDATE Carreras
        SET NombreCarrera = @NombreCarrera, IdFacultad = @IdFacultad
        WHERE IdCarrera = @IdCarrera;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
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
        c.NombreCarrera
    FROM Asignaturas a
    INNER JOIN Carreras c ON a.IdCarrera = c.IdCarrera
    ORDER BY a.NombreAsignatura ASC;
END
GO
CREATE OR ALTER PROC SP_InsertarAsignatura
(
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
        IF EXISTS (SELECT 1 FROM Asignaturas WHERE Codigo = @Codigo)
            THROW 50009, 'EL CÓDIGO DE ASIGNATURA YA EXISTE', 1;
        INSERT INTO Asignaturas (Codigo, NombreAsignatura, Creditos, IdCarrera)
        VALUES (@Codigo, @NombreAsignatura, @Creditos, @IdCarrera);
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_ListarProfesores
AS
BEGIN
    SELECT
        p.IdProfesor,
        p.Nombres,
        p.Apellidos,
        p.Telefono,
        u.Email,
        u.Activo
    FROM Profesores p
    INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario
    ORDER BY p.Apellidos ASC;
END
GO
CREATE OR ALTER PROC SP_InsertarProfesor
(
    @Email VARCHAR(100),
    @PasswordHash VARCHAR(255),
    @Nombres VARCHAR(100),
    @Apellidos VARCHAR(100),
    @Telefono VARCHAR(20)
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF EXISTS (SELECT 1 FROM Usuarios WHERE Email = @Email)
            THROW 50010, 'EL CORREO YA ESTÁ REGISTRADO', 1;
        DECLARE @IdUsuarioInsertado INT;
        INSERT INTO Usuarios (Email, PasswordHash, IdRol) VALUES (@Email, @PasswordHash, 2);
        SET @IdUsuarioInsertado = SCOPE_IDENTITY();
        INSERT INTO Profesores (Nombres, Apellidos, Telefono, IdUsuario)
        VALUES (@Nombres, @Apellidos, @Telefono, @IdUsuarioInsertado);
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_EditarProfesor
(
    @IdProfesor INT,
    @Nombres VARCHAR(100),
    @Apellidos VARCHAR(100),
    @Telefono VARCHAR(20)
)
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        UPDATE Profesores
        SET Nombres = @Nombres, Apellidos = @Apellidos, Telefono = @Telefono
        WHERE IdProfesor = @IdProfesor;
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
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
        per.NombrePeriodo
    FROM Grupos g
    INNER JOIN Asignaturas a ON g.IdAsignatura = a.IdAsignatura
    INNER JOIN Profesores p ON g.IdProfesor = p.IdProfesor
    INNER JOIN Periodos per ON g.IdPeriodo = per.IdPeriodo
    ORDER BY g.NombreGrupo ASC;
END
GO
CREATE OR ALTER PROC SP_InsertarGrupo
(
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
        INSERT INTO Grupos (NombreGrupo, IdAsignatura, IdProfesor, IdPeriodo)
        VALUES (@NombreGrupo, @IdAsignatura, @IdProfesor, @IdPeriodo);
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0) ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
SELECT * FROM Facultades;
IF NOT EXISTS (SELECT 1 FROM Facultades WHERE NombreFacultad = 'Facultad de Ciencias Jurídicas')
    INSERT INTO Facultades (NombreFacultad) VALUES ('Facultad de Ciencias Jurídicas');
GO
IF NOT EXISTS (SELECT 1 FROM Roles WHERE NombreRol = 'Administrador')
    INSERT INTO Roles (NombreRol) VALUES ('Administrador');
IF NOT EXISTS (SELECT 1 FROM Roles WHERE NombreRol = 'Profesor')
    INSERT INTO Roles (NombreRol) VALUES ('Profesor');
IF NOT EXISTS (SELECT 1 FROM Roles WHERE NombreRol = 'Estudiante')
    INSERT INTO Roles (NombreRol) VALUES ('Estudiante');
GO
EXEC SP_ObtenerUsuarioPorEmail 'admin@uni.edu.ni';
GO
