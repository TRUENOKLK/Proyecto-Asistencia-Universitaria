USE AsistenciaUniversitaria;
GO
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Auditoria')
BEGIN
    CREATE TABLE Auditoria (
        IdAuditoria INT PRIMARY KEY IDENTITY(1,1),
        Email VARCHAR(150) NOT NULL,
        Accion VARCHAR(20) NOT NULL,
        Modulo VARCHAR(50) NOT NULL,
        Procedimiento VARCHAR(100) NULL,
        Detalle VARCHAR(300) NULL,
        FechaHora DATETIME NOT NULL DEFAULT GETDATE()
    );
    CREATE NONCLUSTERED INDEX IX_Auditoria_Accion ON Auditoria(Accion);
    CREATE NONCLUSTERED INDEX IX_Auditoria_FechaHora ON Auditoria(FechaHora DESC);
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ContadorSesiones')
BEGIN
    CREATE TABLE ContadorSesiones (
        Id INT PRIMARY KEY CHECK (Id = 1),
        SesionesActivas INT NOT NULL DEFAULT 0
    );
    INSERT INTO ContadorSesiones (Id, SesionesActivas) VALUES (1, 0);
END
GO
CREATE OR ALTER PROC SP_RegistrarAuditoria
(
    @Email VARCHAR(150),
    @Accion VARCHAR(20),
    @Modulo VARCHAR(50),
    @Procedimiento VARCHAR(100) = NULL,
    @Detalle VARCHAR(300) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRAN;
        INSERT INTO Auditoria (Email, Accion, Modulo, Procedimiento, Detalle, FechaHora)
        VALUES (@Email, @Accion, @Modulo, @Procedimiento, @Detalle, GETDATE());
        IF @Accion = 'LOGIN'
        BEGIN
            UPDATE ContadorSesiones SET SesionesActivas = SesionesActivas + 1 WHERE Id = 1;
        END
        ELSE IF @Accion = 'LOGOUT'
        BEGIN
            UPDATE ContadorSesiones SET SesionesActivas = CASE WHEN SesionesActivas > 0 THEN SesionesActivas - 1 ELSE 0 END WHERE Id = 1;
        END
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0) ROLLBACK TRAN;
    END CATCH
END
GO
CREATE OR ALTER PROC SP_ListarAuditoria
(
    @Accion VARCHAR(20) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1000
        IdAuditoria, Email, Accion, Modulo, Procedimiento, Detalle, FechaHora
    FROM Auditoria
    WHERE (@Accion IS NULL OR Accion = @Accion)
    ORDER BY FechaHora DESC;
END
GO
CREATE OR ALTER PROC SP_ObtenerContadorSesiones
AS
BEGIN
    SET NOCOUNT ON;
    SELECT SesionesActivas FROM ContadorSesiones WHERE Id = 1;
END
GO
SELECT 'Auditoria' AS Tabla, COUNT(*) AS Filas FROM Auditoria
UNION ALL
SELECT 'ContadorSesiones', COUNT(*) FROM ContadorSesiones;
GO
