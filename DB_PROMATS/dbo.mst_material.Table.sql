USE [DB_PROMATS]
GO
/****** Object:  Table [dbo].[mst_material]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_material](
	[id_mtl] [int] IDENTITY(1,1) NOT NULL,
	[material_id] [nvarchar](100) NULL,
	[part_code] [nvarchar](100) NULL,
	[material_name] [nvarchar](max) NULL,
	[opening_stock] [int] NULL,
	[incoming] [int] NULL,
	[record_date] [datetime2](7) NULL,
 CONSTRAINT [PK_mst_material] PRIMARY KEY CLUSTERED 
(
	[id_mtl] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[mst_material] ADD  CONSTRAINT [DF_mst_material_record_date]  DEFAULT (getdate()) FOR [record_date]
GO
