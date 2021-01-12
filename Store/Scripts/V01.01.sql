CREATE TABLE [dbo].[Position](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FromCurrency] [nvarchar](10) NOT NULL,
	[ToCurrency] [nvarchar](10) NOT NULL,
	[OpenAmount] [float] NOT NULL,
	[OpenStamp] [datetime] NOT NULL,
	[OpenRate] [float] NOT NULL,
	[CloseStamp] [datetime] NULL,
	[CloseRate] [float] NULL,
	[CloseAmount] [float] NULL,
	[Diff] [float] NULL,
	[Fee] [float] NULL,
 CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))

CREATE NONCLUSTERED INDEX [IX_CloseAmount] ON [dbo].[Position]
(
	[CloseAmount] ASC
)