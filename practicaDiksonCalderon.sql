USE [PracticaInitium]
GO
/****** Object:  Table [dbo].[Cliente]    Script Date: 05/02/2021 01:32:37 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cliente](
	[idCliente] [varchar](20) NOT NULL,
	[nombreCliente] [varchar](50) NOT NULL,
	[AsignadoCola] [varchar](1) NOT NULL,
 CONSTRAINT [PK_Cliente] PRIMARY KEY CLUSTERED 
(
	[idCliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cliente_Cola]    Script Date: 05/02/2021 01:32:37 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cliente_Cola](
	[idCliente] [varchar](20) NOT NULL,
	[idCola] [int] NOT NULL,
	[TiempoInicio] [datetime] NULL,
	[TiempoFin] [datetime] NULL,
	[Ticket] [int] NOT NULL,
	[EstadoCliente] [varchar](2) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cola]    Script Date: 05/02/2021 01:32:37 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cola](
	[idCola] [int] IDENTITY(1,1) NOT NULL,
	[nombreCola] [varchar](20) NOT NULL,
	[tiempoCola] [int] NOT NULL,
	[estadoCola] [varchar](1) NOT NULL,
 CONSTRAINT [PK_Cola] PRIMARY KEY CLUSTERED 
(
	[idCola] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Cliente] ADD  CONSTRAINT [DF_Cliente_AsignadoCola]  DEFAULT ('N') FOR [AsignadoCola]
GO
/****** Object:  StoredProcedure [dbo].[ProcesoAtencionCliente]    Script Date: 05/02/2021 01:32:37 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ProcesoAtencionCliente] 
@idCliente varchar(25), 
@mensaje_control  varchar(400) OUTPUT 

AS 
 declare
 @contador int,
 @idCola int,
 @tiempoInicio DateTime,
 @tiempoFin DateTime
BEGIN 

	SET @idCola =0;
	set nocount on;                
	declare @trancount int;                
	set @trancount = @@trancount;                
	begin try                
	if @trancount = 0                
		begin transaction                
	else                
		save transaction ProcesoAtencionCliente;

		

		IF((SELECT count(*) FROM Cola Where tiempoCola=2)=1)
			BEGIN
				SET @idCola= 2
			END
		IF (@idCola = 0)
		BEGIN
			IF((SELECT count(*) FROM Cola Where tiempoCola=3)=1)
				BEGIN
					SET @idCola= 3
				END
		END

		if (@idCola != 0)

		BEGIN
			UPDATE Cola
			SET estadoCola = 'O'
			WHERE idCola= @idCola 
		
			--INSERT A Cliente_Cola
			--Siempre y cuando este una cola libre

			SET @tiempoInicio=  GETDATE()
			IF @idCola = 1
			SET @tiempoFin =  DateAdd(MINUTE,2,@tiempoInicio)
			IF @idCola = 2
			SET @tiempoFin =  DateAdd(MINUTE,3,@tiempoInicio)

			INSERT INTO Cliente_Cola (idCliente, idCola, TiempoInicio, TiempoFin, Ticket, EstadoCliente)
			VALUES (@idCliente, @idCola,@tiempoInicio,@tiempoFin, 1,'CV')

			UPDATE Cliente
			SET AsignadoCola= 'S'
			WHERE idCliente = @idCliente

			SET @mensaje_control= 'Inserción realizado Satisfactoriamente'
			select @mensaje_control, 1 

			COMMIT
		END
		else
		SET @mensaje_control= 'No hay cola libre'

	end try 

	 BEGIN CATCH                    
	  BEGIN                    
  
	   rollback transaction ProcesoAtencionCliente;                
                                              
	   select @mensaje_control, 2, '0'                
	  END                    
 END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[VerificarColaCliente]    Script Date: 05/02/2021 01:32:37 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[VerificarColaCliente] 
@idCliente varchar(25), 
@idCola int,
@mensaje_control  varchar(400) OUTPUT 

AS 
 declare
 @contador int,
 @tiempoInicio DateTime,
 @tiempoFin DateTime
BEGIN 

	
	set nocount on;                
	declare @trancount int;                
	set @trancount = @@trancount;                
	begin try                
	if @trancount = 0                
		begin transaction                
	else                
		save transaction VerificarColaCliente;


		UPDATE Cliente_Cola
		SET EstadoCliente= 'CA'
		WHERE idCola = @idCola and idCliente= @idCliente

		UPDATE Cola
		SET estadoCola= 'L'
		WHERE idCola = @idCola

		UPDATE Cliente
		SET AsignadoCola= 'N'
		Where idCliente = @idCliente
		
		SET @mensaje_control= 'Cambio realizado Satisfactoriamente'
		COMMIT
	end try 

	 BEGIN CATCH                    
	  BEGIN                    
  
	   rollback transaction VerificarColaCliente;                
       SET @mensaje_control= 'No se pudo realizar el cambio'
	   select @mensaje_control, 2, '0'

	  END                    
 END CATCH
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Tiene dos valores S y N' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Cliente', @level2type=N'COLUMN',@level2name=N'AsignadoCola'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'EstadoCliente valores CE Cliente en espera, CV Cliente en Ventanilla
CP Cliente Procesado' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Cliente_Cola', @level2type=N'COLUMN',@level2name=N'EstadoCliente'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'CE= Cliente en Espera
CA= Cliente Atendido
CV= Cliente Ventanilla' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Cliente_Cola'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'tiene dos valores O= Ocupado
L= Libre' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Cola', @level2type=N'COLUMN',@level2name=N'estadoCola'
GO
