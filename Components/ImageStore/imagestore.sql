USE [master]
GO
/****** 对象:  Database [imageserver]    脚本日期: 10/19/2012 18:40:42 ******/
CREATE DATABASE [imageserver] ON  PRIMARY 
( NAME = N'imageserver', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\imageserver.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'imageserver_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\imageserver_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
 COLLATE Chinese_PRC_CI_AS
GO
EXEC dbo.sp_dbcmptlevel @dbname=N'imageserver', @new_cmptlevel=90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [imageserver].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO
ALTER DATABASE [imageserver] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [imageserver] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [imageserver] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [imageserver] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [imageserver] SET ARITHABORT OFF 
GO
ALTER DATABASE [imageserver] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [imageserver] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [imageserver] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [imageserver] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [imageserver] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [imageserver] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [imageserver] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [imageserver] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [imageserver] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [imageserver] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [imageserver] SET  ENABLE_BROKER 
GO
ALTER DATABASE [imageserver] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [imageserver] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [imageserver] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [imageserver] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [imageserver] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [imageserver] SET  READ_WRITE 
GO
ALTER DATABASE [imageserver] SET RECOVERY FULL 
GO
ALTER DATABASE [imageserver] SET  MULTI_USER 
GO
ALTER DATABASE [imageserver] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [imageserver] SET DB_CHAINING OFF 


USE [imageserver]
GO
/****** 对象:  Table [dbo].[Images]    脚本日期: 10/19/2012 18:40:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Images](
	[Id] [int] NOT NULL,
	[SId] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[ImageName] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[ImagePath] [varchar](128) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[Remarks] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[FullImagePath] [varchar](128) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[PubDate] [datetime] NOT NULL,
	[Source] [varchar](64) COLLATE Chinese_PRC_CI_AS NOT NULL,
 CONSTRAINT [PK_Images] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'自增长标识列' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Images', @level2type=N'COLUMN', @level2name=N'Id'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'经常性检查内容记录表的Id' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Images', @level2type=N'COLUMN', @level2name=N'SId'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'图片名称' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Images', @level2type=N'COLUMN', @level2name=N'ImageName'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'保存地址' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Images', @level2type=N'COLUMN', @level2name=N'ImagePath'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'Images', @level2type=N'COLUMN', @level2name=N'Remarks'
