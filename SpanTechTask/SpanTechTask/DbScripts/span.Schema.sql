/****** Object:  Schema [span]    Script Date: 07-03-2025 21:41:42 ******/
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'span')
EXEC sys.sp_executesql N'CREATE SCHEMA [span]'
GO
