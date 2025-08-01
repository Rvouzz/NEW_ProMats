USE [DB_PROMATS]
GO
/****** Object:  Table [dbo].[tbl_stock_pofinish]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_stock_pofinish](
	[id_det] [int] IDENTITY(1,1) NOT NULL,
	[id_part] [int] NULL,
	[part_code] [nvarchar](50) NULL,
	[total_stock] [float] NULL,
	[update_date] [datetime2](7) NULL,
 CONSTRAINT [PK_tbl_stock_pofinish] PRIMARY KEY CLUSTERED 
(
	[id_det] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tbl_stock_pofinish] ADD  CONSTRAINT [DF_tbl_stock_pofinish_total_stock]  DEFAULT ((0)) FOR [total_stock]
GO
ALTER TABLE [dbo].[tbl_stock_pofinish] ADD  CONSTRAINT [DF_tbl_stock_pofinish_update_date]  DEFAULT (getdate()) FOR [update_date]
GO
