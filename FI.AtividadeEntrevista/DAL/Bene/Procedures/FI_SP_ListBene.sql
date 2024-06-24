CREATE PROCEDURE FI_SP_ListBene
    @IdCliente BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, IdCliente, Nome, CPF
    FROM BENEFICIARIOS
    WHERE IdCliente = @IdCliente;
END