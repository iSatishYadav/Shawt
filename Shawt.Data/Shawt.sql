CREATE DATABASE [Links]
GO

USE [Links]
GO
/****** Object:  Table [dbo].[ApplicationUsers]    Script Date: 21-12-2019 19:28:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationUsers](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ApplicationUsers] PRIMARY KEY CLUSTERED 
(
	[ApplicationId] ASC,
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Link]    Script Date: 21-12-2019 19:28:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Link](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LinkId] [uniqueidentifier] NOT NULL,
	[OriginalLink] [nvarchar](max) NOT NULL,
	[CreatedBy] [nvarchar](200) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[Stats] [nvarchar](max) NULL,
 CONSTRAINT [PK_Link] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Log]    Script Date: 21-12-2019 19:28:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
	[Id] [uniqueidentifier] NOT NULL,
	[LinkId] [int] NOT NULL,
	[IpAddress] [nvarchar](50) NULL,
	[UserAgent] [nvarchar](max) NULL,
	[Timestamp] [datetime2](7) NULL,
	[Browser] [nvarchar](100) NULL,
	[Os] [nvarchar](50) NULL,
	[Device] [nvarchar](100) NULL,
 CONSTRAINT [PK__Log__3214EC071A94412F] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Log]  WITH CHECK ADD  CONSTRAINT [FK_Log_Link] FOREIGN KEY([LinkId])
REFERENCES [dbo].[Link] ([Id])
GO
ALTER TABLE [dbo].[Log] CHECK CONSTRAINT [FK_Log_Link]
GO
/****** Object:  StoredProcedure [dbo].[UpdateStats]    Script Date: 21-12-2019 19:28:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateStats] @Id int, @IpAddress NVARCHAR(50), @TimeStamp DATETIME2, @UserAgent NVARCHAR(MAX), @Browser NVARCHAR(100), @Os NVARCHAR(50), @Device NVARCHAR(100)
AS			
	UPDATE [dbo].Link SET Stats = JSON_MODIFY(JSON_MODIFY(Stats, '$.clicks', JSON_VALUE(Stats, '$.clicks') + 1), 'append $.log', JSON_MODIFY(JSON_MODIFY(JSON_MODIFY(JSON_MODIFY(JSON_MODIFY(JSON_MODIFY(JSON_MODIFY('{}', '$.id', CAST(NEWID() AS NVARCHAR(64))), '$.timestamp', CAST(@Timestamp AS NVARCHAR)), '$.ip', @IpAddress), '$.userAgent', @UserAgent), '$.browser', @Browser), '$.os', @Os), '$.device', @Device)) WHERE Id = @Id
	INSERT INTO [dbo].[Log] ([Id], [LinkId], [IpAddress], [UserAgent], [Timestamp], [Browser], [Os], [Device]) VALUES (NEWID(), @Id, @IpAddress, @UserAgent, @TimeStamp, @Browser, @Os, @Device)
RETURN 0

GO